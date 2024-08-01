using System.IO.Ports;
using System.Management;

namespace SonoCap.MES.UI.Services
{
    public class MotorService : SerialPort//, IMotorService
    {
        public enum MotorState
        {
            disconnect = 0,
            connect = 1,
            stop = 2,
            start = 3,
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

        public IEnumerable<string> MyGetPortNames(string contain)
        {
            var Ports = new List<string>();

            // Query to get all serial ports
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");

            foreach (ManagementObject obj in searcher.Get())
            {
                // Get the description and port name
                var description = obj["Description"]?.ToString();
                var portName = obj["DeviceID"]?.ToString();

                // Check if the description contains "Silicon Labs"
                if (description != null && description.Contains(contain))
                {
                    // Add the port name (e.g., COM3) to the list
                    Ports.Add(portName);
                }
            }

            return Ports;
        }

        public byte[] GenerateCommand(bool isModeSel, RPM rpm, PRF prf)
        {
            List<byte> commandBytes = new List<byte>();

            // 모드 선택 여부에 따라 명령어 추가
            if (isModeSel)
                commandBytes.AddRange(BitConverter.GetBytes((ushort)CMD.CMD_MODE_SEL));
            else
                commandBytes.AddRange(BitConverter.GetBytes((ushort)CMD.CMD_MOTOR_ON));

            // RPM과 PRF 추가
            commandBytes.Add((byte)rpm);
            commandBytes.Add((byte)prf);

            return commandBytes.ToArray();
        }

        public byte[] GetCommandBytes(bool isModeSel, bool[] cmd, bool[] rpm, bool[] prf)
        {
            byte[] bytesToSend;

            int cmdHex;
            if (cmd[0])
                cmdHex = (int)CMD.CMD_MODE_SEL;
            else if (cmd[1])
                cmdHex = (int)CMD.CMD_MOTOR_ON;
            else if (cmd[2])
                cmdHex = (int)CMD.CMD_MOTOR_OFF;
            else if (cmd[3])
                cmdHex = (int)CMD.CMD_FREQ_INFO;
            else
                cmdHex = (int)0;

            int rpmHex;
            if (rpm[0])
                rpmHex = (int)RPM.RPM_1250;
            else if (rpm[1])
                rpmHex = (int)RPM.RPM_1500;
            else if (rpm[2])
                rpmHex = (int)RPM.RPM_1600;
            else
                rpmHex = (int)RPM.RPM_1875;

            int prfHex;
            if (prf[0])
                prfHex = (int)PRF.PRF_10;
            else if (prf[1])
                prfHex = (int)PRF.PRF_12;
            else if (prf[2])
                prfHex = (int)PRF.PRF_15;
            else if (prf[3])
                prfHex = (int)PRF.PRF_16;
            else
                prfHex = (int)PRF.PRF_20;

            if (isModeSel)
            {
                bytesToSend = BitConverter.GetBytes((cmdHex << 16) + (rpmHex << 8) + prfHex);
            }
            else
            {
                byte[] temp = BitConverter.GetBytes(cmdHex); // return 4bytes array
                bytesToSend = new byte[2];
                Array.Copy(temp, bytesToSend, 2);
            }

            Array.Reverse(bytesToSend); // BitConverer result reverse

            return bytesToSend;
        }

        public byte[] GetCommandBytes(int cmdHex)
        {
            byte[] bytesToSend;

            byte[] temp = BitConverter.GetBytes(cmdHex); // return 4bytes array
            bytesToSend = new byte[2];
            Array.Copy(temp, bytesToSend, 2);

            Array.Reverse(bytesToSend); // BitConverer result reverse

            return bytesToSend;
        }

        public void InitPort(String portname, int baudrate)
        {
            PortName = portname;
            BaudRate = baudrate;// int.Parse(Baud_Combox.SelectedItem.ToString());          //콤보 박스에서 Baud Rate 선택.
            DataBits = 8;
            StopBits = StopBits.One;
            Parity = Parity.None;
            Open();
        }

        public void InitPort()
        {
            string portname = MyGetPortNames("Silicon Labs CP210x").FirstOrDefault() ?? string.Empty;
            PortName = portname;
            BaudRate = 9600;
            DataBits = 8;
            StopBits = StopBits.One;
            Parity = Parity.None;
            Open();
        }

        public void SendACK()
        {
            byte[] bytesToSend = GetCommandBytes((int)CMD.CMD_ACK);
            Write(bytesToSend, 0, bytesToSend.Length);
        }

    }
}