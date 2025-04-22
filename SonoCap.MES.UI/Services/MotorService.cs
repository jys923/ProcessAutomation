using Serilog;
using SonoCap.MES.UI.Services.Interfaces;
using System.IO.Ports;

namespace SonoCap.MES.UI.Services
{
    public class MotorService : IMotorService, IDisposable
    {
        private enum MotorState { Stop = 2, Start = 3 }
        private enum PRF { PRF_10 = 0x01, PRF_12 = 0x02, PRF_15 = 0x03, PRF_16 = 0x04, PRF_20 = 0x05 }
        private enum RPM { RPM_1250 = 0x0A, RPM_1500 = 0x0B, RPM_1600 = 0x0C, RPM_1875 = 0x0D }
        private enum CMD { CMD_MODE_SEL = 0xAC33, CMD_MOTOR_ON = 0xAB55, CMD_MOTOR_OFF = 0xFF03, CMD_FREQ_INFO = 0xFA55, CMD_ACK = 0xF055 }

        private readonly ISerialPortWrapper _serialPort;
        private MotorState _motorState = MotorState.Stop;
        private RPM _currentRPM = RPM.RPM_1250;
        private PRF _currentPRF = PRF.PRF_20;

        private bool disposedValue = false;

        public MotorService(ISerialPortWrapper serialPort)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
        }

        public bool InitializeMotor()
        {
            if (!InitPort()) return false;

            return SendAndCheckAck(CMD.CMD_MOTOR_ON)
                && SendAndCheckAck(CMD.CMD_MOTOR_OFF)
                && SendAndCheckAck(CMD.CMD_MODE_SEL, _currentRPM, _currentPRF);
        }

        public bool InitPort()
        {
            foreach (var port in SerialPort.GetPortNames())
            {
                if (TryOpenValidPort(port))
                {
                    ConfigurePort(port);
                    _serialPort.Open();
                    Log.Information($"Port {port} opened and ready.");
                    return true;
                }
            }

            Log.Warning("No valid serial port found.");
            return false;
        }

        private bool TryOpenValidPort(string port)
        {
            try
            {
                ConfigurePort(port);
                _serialPort.Open();

                if (SendAndCheckAck(CMD.CMD_ACK))
                {
                    Log.Information($"Valid port found: {port}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Port {port} check failed: {ex.Message}");
            }
            finally
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }

            return false;
        }

        private void ConfigurePort(string portName)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
            _serialPort.ReadTimeout = 100;
            _serialPort.WriteTimeout = 100;
        }

        public void StartMotor()
        {
            if (_motorState == MotorState.Start) return;
            if (SendAndCheckAck(CMD.CMD_MOTOR_ON)) _motorState = MotorState.Start;
        }

        public void StopMotor()
        {
            if (_motorState == MotorState.Stop) return;
            if (SendAndCheckAck(CMD.CMD_MOTOR_OFF)) _motorState = MotorState.Stop;
        }

        public void OnMotorStateChanged(int prfHz, int density)
        {
            PRF prf = GetPRFFromHz(prfHz);
            RPM rpm = GetRPMFromDensity(density);
            UpdateSettings(rpm, prf);
        }

        private void UpdateSettings(RPM newRPM, PRF newPRF)
        {
            if (_currentRPM == newRPM && _currentPRF == newPRF)
            {
                Log.Information("Motor settings unchanged, skipping update.");
                return;
            }

            if (_currentRPM == newRPM && _currentPRF != newPRF)
            {
                Log.Information("PRF changed but motor update skipped.");
                _currentPRF = newPRF;
                return;
            }

            // RPM이 변경되었을 때만 실행
            _currentRPM = newRPM;
            _currentPRF = newPRF;
            SendAndCheckAck(CMD.CMD_MODE_SEL, _currentRPM, _currentPRF);
        }

        private bool SendAndCheckAck(CMD cmd, RPM? rpm = null, PRF? prf = null)
        {
            var response = SendAndReceive(cmd, rpm, prf);
            return IsAck(response);
        }

        private byte[] SendAndReceive(CMD cmd, RPM? rpm = null, PRF? prf = null)
        {
            if (!_serialPort.IsOpen) return Array.Empty<byte>();

            var command = GetCommandBytes(cmd, rpm, prf);
            _serialPort.Write(command, 0, command.Length);
            Log.Information($"Command sent: {BitConverter.ToString(command)}");
            return ReadResponse();
        }

        private byte[] ReadResponse()
        {
            try
            {
                byte[] buffer = new byte[2];
                int bytesRead = 0;

                while (bytesRead < 2)
                {
                    int read = _serialPort.Read(buffer, bytesRead, 2 - bytesRead);
                    if (read <= 0) break;
                    bytesRead += read;
                }

                return buffer;
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        private bool IsAck(byte[] response)
        {
            if (response.Length < 2) return false;
            int val = BitConverter.ToUInt16(response.Reverse().ToArray(), 0);
            return val == (int)CMD.CMD_ACK;
        }

        private byte[] GetCommandBytes(CMD cmd, RPM? rpm = null, PRF? prf = null)
        {
            int commandValue = Convert.ToInt32(cmd);
            if (rpm.HasValue) commandValue = (commandValue << 8) | Convert.ToInt32(rpm.Value);
            if (prf.HasValue) commandValue = (commandValue << 8) | Convert.ToInt32(prf.Value);

            int size = (rpm.HasValue && prf.HasValue) ? 4 : (rpm.HasValue || prf.HasValue ? 3 : 2);
            byte[] bytes = BitConverter.GetBytes(commandValue);
            Array.Reverse(bytes);
            return bytes[^size..];
        }

        private PRF GetPRFFromHz(int hz) => hz switch
        {
            20000 => PRF.PRF_20,
            16000 => PRF.PRF_16,
            15000 => PRF.PRF_15,
            12000 => PRF.PRF_12,
            10000 => PRF.PRF_10,
            _ => PRF.PRF_20,
        };

        private RPM GetRPMFromDensity(int density) => density switch
        {
            1 => RPM.RPM_1875,
            2 => RPM.RPM_1600,
            3 => RPM.RPM_1500,
            4 => RPM.RPM_1250,
            _ => RPM.RPM_1250,
        };

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue && disposing)
            {
                StopMotor();
                disposedValue = true;
            }
        }

        ~MotorService() => Dispose(disposing: false);
    }
}
