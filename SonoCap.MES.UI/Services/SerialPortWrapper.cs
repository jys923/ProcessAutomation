using System.IO.Ports;

namespace SonoCap.MES.UI.Services
{
    public interface ISerialPortWrapper
    {
        bool IsOpen { get; }
        string PortName { get; set; }
        int BytesToRead { get; }
        void Open();
        void Close();
        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
        byte[] ReadResponse();
    }

    public class SerialPortWrapper : ISerialPortWrapper
    {
        private readonly SerialPort _serialPort;

        public bool IsOpen => _serialPort.IsOpen;
        public int BytesToRead => _serialPort.BytesToRead;

        public string PortName
        {
            get => _serialPort.PortName;
            set
            {
                if (_serialPort.IsOpen)
                    throw new InvalidOperationException("Cannot change port name while port is open.");
                _serialPort.PortName = value;
            }
        }

        public SerialPortWrapper(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _serialPort.ReadTimeout = 100;
            _serialPort.WriteTimeout = 100;
        }

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
            _serialPort.Write(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_serialPort.IsOpen)
                return _serialPort.Read(buffer, offset, count);

            Console.WriteLine("SerialPort is not open. Cannot read data.");
            return 0;
        }

        public byte[] ReadResponse()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = this.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                byte[] response = new byte[bytesRead];
                Array.Copy(buffer, response, bytesRead);
                return response;
            }
            return Array.Empty<byte>();
        }
    }
}
