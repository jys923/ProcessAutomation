using Serilog;
using SonoCap.MES.UI.Services.Interfaces;
using System.IO.Ports;
using System.Management;

namespace SonoCap.MES.UI.Services
{
    public class MotorService : IMotorService
    {
        private readonly ISerialPortWrapper _serialPort;

        private MotorState _motorState = MotorState.Stop; // 기본값: Close (포트 닫힘)

        private RPM _currentRPM = RPM.RPM_1250;
        private PRF _currentPRF = PRF.PRF_20;

        public event EventHandler? CloseViewRequested;

        // 사용자가 SerialPort를 주입하도록 변경
        public MotorService(ISerialPortWrapper serialPort)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
        }

        public void UpdateSettings(int lineDensity, int viewDepth)
        {
            RPM newRPM = GetRPMFromDensity(lineDensity);
            PRF newPRF = GetPRFFromDepth(viewDepth);

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

            _currentRPM = newRPM;
            _currentPRF = newPRF;
            SendMotorSettings();
        }

        private void SendMotorSettings()
        {
            if (!_serialPort.IsOpen)
            {
                Log.Warning("Motor port is not open. Cannot send settings.");
                return;
            }

            byte[] command = GetCommandBytes(CMD.CMD_MODE_SEL, _currentRPM, _currentPRF);
            _serialPort.Write(command, 0, command.Length);
            Log.Information($"Motor settings updated: RPM={_currentRPM}, PRF={_currentPRF}");
        }

        public PRF GetPRFFromDepth(int depthInCm)
        {
            return depthInCm switch
            {
                7 => PRF.PRF_10,
                6 => PRF.PRF_12,
                5 => PRF.PRF_15,
                4 => PRF.PRF_16,
                3 => PRF.PRF_20,
                _ => PRF.PRF_20
            };
        }

        public RPM GetRPMFromDensity(int density)
        {
            return density switch
            {
                1 => RPM.RPM_1875,
                2 => RPM.RPM_1600,
                3 => RPM.RPM_1500,
                4 => RPM.RPM_1250,
                _ => RPM.RPM_1250
            };
        }

        public enum MotorState
        {
            //Close = 0,
            //IsOpen = 1,
            Stop = 2,
            Start = 3,
        }

        public enum PRF
        {
            PRF_10 = 0x01,
            PRF_12 = 0x02,
            PRF_15 = 0x03,
            PRF_16 = 0x04,
            PRF_20 = 0x05
        }

        public enum RPM
        {
            RPM_1250 = 0x0A,
            RPM_1500 = 0x0B,
            RPM_1600 = 0x0C,
            RPM_1875 = 0x0D
        }

        public enum CMD
        {
            CMD_MODE_SEL = 0xAC33,
            CMD_MOTOR_ON = 0xAB55,
            CMD_MOTOR_OFF = 0xFF03,
            CMD_FREQ_INFO = 0xFA55,
            CMD_ACK = 0xF055
        }

        private void CloseView()
        {
            CloseViewRequested?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<string> MyGetPortNames(string contain)
        {
            var ports = new List<string>();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");

            foreach (ManagementObject obj in searcher.Get())
            {
                var description = obj["Description"]?.ToString();
                var portName = obj["DeviceID"]?.ToString();

                if (description != null && description.Contains(contain))
                {
                    ports.Add(portName);
                }
            }

            return ports;
        }

        public byte[] GetCommandBytes(CMD cmd, RPM? rpm = null, PRF? prf = null)
        {
            int commandValue = ConvertEnumToHex(cmd);

            if (rpm.HasValue)
                commandValue = (commandValue << 8) | ConvertEnumToHex(rpm.Value);

            if (prf.HasValue)
                commandValue = (commandValue << 8) | ConvertEnumToHex(prf.Value);

            int byteSize = (prf.HasValue && rpm.HasValue) ? 4 : (prf.HasValue || rpm.HasValue ? 3 : 2);

            byte[] bytesToSend = BitConverter.GetBytes(commandValue);
            Array.Reverse(bytesToSend, 0, byteSize);

            Log.Information($"{nameof(GetCommandBytes)} : {BitConverter.ToString(bytesToSend)}");
            return bytesToSend.Take(byteSize).ToArray();
        }

        private int ConvertEnumToHex<T>(T value) where T : Enum
        {
            return Convert.ToInt32(value);
        }

        public bool InitPort()
        {
            var ports = SerialPort.GetPortNames();
            string validPort = string.Empty;

            foreach (var port in ports)
            {
                try
                {
                    _serialPort.PortName = port;
                    _serialPort.BaudRate = 9600;
                    _serialPort.DataBits = 8;
                    _serialPort.StopBits = StopBits.One;
                    _serialPort.Parity = Parity.None;
                    _serialPort.ReadTimeout = 100;
                    _serialPort.WriteTimeout = 100;

                    _serialPort.Open();

                    //OpenPort(port);

                    SendACK();
                    byte[] response = ReadResponse();

                    if (response.Length >= 2)
                    {
                        int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0);

                        if (responseValue == (int)CMD.CMD_ACK)
                        {
                            Log.Information("ACK received! Valid port found: {port}");
                            validPort = port;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Error on port {port}: {e.Message}");
                }
                finally
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                }
            }

            if (!string.IsNullOrEmpty(validPort))
            {
                _serialPort.PortName = validPort;
                _serialPort.Open();
                Log.Information($"Port {validPort} successfully initialized.");
                return true;
            }

            Log.Information("No valid port found.");
            return false;
        }

        private byte[] ReadResponse()
        {
            try
            {
                const int responseLength = 2;
                byte[] responseBuffer = new byte[responseLength];

                int bytesRead = 0;
                while (bytesRead < responseLength)
                {
                    int read = _serialPort.Read(responseBuffer, bytesRead, responseLength - bytesRead);
                    if (read > 0)
                    {
                        bytesRead += read;
                    }
                }

                Log.Information($"Response received: {BitConverter.ToString(responseBuffer)}");
                return responseBuffer;
            }
            catch (TimeoutException)
            {
                Log.Warning("Serial response timeout.");
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                Log.Error($"Error reading response: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public void SendACK()
        {
            byte[] bytesToSend = GetCommandBytes(CMD.CMD_ACK);
            _serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public void OpenPort(string portName)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
            _serialPort.ReadTimeout = 100;
            _serialPort.WriteTimeout = 100;

            _serialPort.Open();
        }

        public void ClosePort()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                Log.Information("Serial port closed.");
            }
        }

        public byte[] SendCommand(byte[] command)
        {
            if (!_serialPort.IsOpen)
            {
                Log.Warning("Serial port is closed. Cannot send command.");
                return Array.Empty<byte>();
            }

            _serialPort.Write(command, 0, command.Length);
            Log.Information($"Command sent: {BitConverter.ToString(command)}");

            return ReadResponse();
        }

        public void StartMotor()
        {
            if (_motorState == MotorState.Start)
            {
                Log.Information("Motor is already started.");
                return;
            }

            byte[] response = SendCommand(GetCommandBytes(CMD.CMD_MOTOR_ON));

            if (response.Length > 0)
            {
                int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0);

                if (responseValue == (int)CMD.CMD_ACK)
                {
                    Log.Information($"Received response: {BitConverter.ToString(response)}");
                    _motorState = MotorState.Start;
                }
            }
            else
            {
                Log.Warning("No response received.");
            }
        }

        public void StopMotor()
        {
            if (_motorState == MotorState.Stop)
            {
                Log.Information("Motor is already stopped.");
                return;
            }

            byte[] response = SendCommand(GetCommandBytes(CMD.CMD_MOTOR_OFF));

            if (response.Length > 0)
            {
                int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0);

                if (responseValue == (int)CMD.CMD_ACK)
                {
                    Log.Information($"Received response: {BitConverter.ToString(response)}");
                    _motorState = MotorState.Stop;
                }
            }
            else
            {
                Log.Warning("No response received.");
            }
        }
    }
}
