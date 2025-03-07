using Serilog;
using System.IO.Ports;
using System.Management;

namespace SonoCap.MES.UI.Services
{
    public class MotorService
    {
        private readonly SerialPort _serialPort;
        
        //사용안함 추후 개선 아예 지우고  _serialPort.IsOpen 과 통합
        private MotorState _motorState = MotorState.Close; // 기본값: Close (포트 닫힘)

        public MotorState CurrentState
        {
            get
            {
                if (!_serialPort.IsOpen)
                    _motorState = MotorState.Close; // 포트가 닫혀 있으면 상태를 Close로 변경

                return _motorState;
            }
        }


        private RPM _currentRPM = RPM.RPM_1250;
        private PRF _currentPRF = PRF.PRF_20;

        public event Action<MotorState>? OnMotorStateChanged;

        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int recvSize = _serialPort.BytesToRead;

            if (recvSize >= 2)
            {
                byte[] buff = new byte[2];
                _serialPort.Read(buff, 0, 2);
                Log.Information($"Received : {BitConverter.ToString(buff)}");

                //ProcessReceivedData();
            }
        }

        private void ProcessReceivedData()
        {
            switch (_motorState)
            {
                case MotorState.IsOpen:
                    _motorState = MotorState.Start;
                    SendCommand(GetCommandBytes(CMD.CMD_MOTOR_ON));
                    break;

                case MotorState.Close:
                    _motorState = MotorState.IsOpen;
                    SendCommand(GetCommandBytes(CMD.CMD_FREQ_INFO));
                    break;

                default:
                    Log.Warning("Unhandled motor state: " + _motorState);
                    break;
            }

            OnMotorStateChanged?.Invoke(_motorState);
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

            // PRF만 변경된 경우, 아무 동작도 하지 않음
            if (_currentRPM == newRPM && _currentPRF != newPRF)
            {
                Log.Information("PRF changed but motor update skipped.");
                _currentPRF = newPRF; // 내부 변수는 업데이트
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

        public MotorService()
        {
            _serialPort = new SerialPort();
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

        public enum MotorState
        {
            Close = 0,
            IsOpen = 1, //IsOpen
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

        public event EventHandler? CloseViewRequested;

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

            // 조건: prf나 rpm이 하나만 있으면 3바이트, 둘 다 있으면 4바이트, 없으면 2바이트
            int byteSize = (prf.HasValue && rpm.HasValue) ? 4 : (prf.HasValue || rpm.HasValue ? 3 : 2);

            byte[] bytesToSend = BitConverter.GetBytes(commandValue);
            Array.Reverse(bytesToSend, 0, byteSize); // 필요한 바이트 크기만 역순 정렬

            Log.Information($"{nameof(GetCommandBytes)} : {BitConverter.ToString(bytesToSend)}");
            return bytesToSend.Take(byteSize).ToArray(); // 필요한 바이트 크기만 반환
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
                    _serialPort.Open();

                    _serialPort.ReadTimeout = 100;
                    _serialPort.WriteTimeout = 100;

                    SendACK();

                    byte[] response = ReadResponse();

                    if (response.Length >= 2) // 최소 2바이트 응답인지 확인
                    {
                        int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0); // 바이트 배열 → int 변환

                        if (responseValue == (int)CMD.CMD_ACK) // 변환된 값과 enum 비교
                        {
                            Log.Information("ACK received! Valid port found: {port}");
                            validPort = port;
                            break;
                        }
                        //else
                        //{
                        //    Log.Warning($"Unexpected response: {responseValue:X4} (Expected: {CMD.CMD_ACK:X4})");
                        //}
                    }
                    else
                    {
                        //Log.Warning("No valid response received.");
                    }

                    //byte[] response = ReadResponse();
                    
                    //if (response == 0xF055)
                    //if (response == CMD.CMD_ACK)
                    //{
                    //    Log.Information($"Valid port found: {port}");
                    //    validPort = port;
                    //    break;
                    //}
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
                //_motorState = MotorState.Connect;
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
                const int responseLength = 2; // 응답 크기 (예제 기준)
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
                return Array.Empty<byte>(); // 타임아웃 시 빈 응답 반환
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
                return Array.Empty<byte>(); // 빈 응답 반환
            }

            // 명령 전송
            _serialPort.Write(command, 0, command.Length);
            Log.Information($"Command sent: {BitConverter.ToString(command)}");

            // 응답 받기
            return ReadResponse();
        }


        public void StartMotor()
        {
            //SendCommand(GetCommandBytes(CMD.CMD_MOTOR_ON));
            if (_motorState == MotorState.Start)
            {
                Log.Information("Motor is already started.");
                return;
            }
            
            byte[] response = SendCommand(GetCommandBytes(CMD.CMD_MOTOR_ON));

            if (response.Length > 0)
            {
                int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0); // 바이트 배열 → int 변환

                if (responseValue == (int)CMD.CMD_ACK) // 변환된 값과 enum 비교
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
            //SendCommand(GetCommandBytes(CMD.CMD_MOTOR_OFF));
            if (_motorState == MotorState.Stop)
            {
                Log.Information("Motor is already started.");
                return;
            }

            byte[] response = SendCommand(GetCommandBytes(CMD.CMD_MOTOR_OFF));

            if (response.Length > 0)
            {
                int responseValue = BitConverter.ToUInt16(response.Reverse().ToArray(), 0); // 바이트 배열 → int 변환

                if (responseValue == (int)CMD.CMD_ACK) // 변환된 값과 enum 비교
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

        //public bool IsOpen => _serialPort.IsOpen;

        public void Open()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (_serialPort.IsOpen)
                _serialPort.Write(buffer, offset, count);
            else
                Log.Warning("SerialPort is not open. Cannot write data.");
        }

        public int BytesToRead => _serialPort.BytesToRead;

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_serialPort.IsOpen)
                return _serialPort.Read(buffer, offset, count);

            Log.Warning("SerialPort is not open. Cannot read data.");
            return 0;
        }

        //public event SerialDataReceivedEventHandler? DataReceived
        //{
        //    add => _serialPort.DataReceived += value;
        //    remove => _serialPort.DataReceived -= value;
        //}
        //
        //public void Configure(string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None)
        //{
        //    if (_serialPort.IsOpen)
        //        _serialPort.Close();

        //    _serialPort.PortName = portName;
        //    _serialPort.BaudRate = baudRate;
        //    _serialPort.DataBits = dataBits;
        //    _serialPort.StopBits = stopBits;
        //    _serialPort.Parity = parity;

        //    Log.Information($"SerialPort configured: {portName}, {baudRate} baud");
        //}
    }
}

