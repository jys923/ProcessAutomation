using System.IO.Ports;

namespace SonoCap.MES.Services
{
    public interface ISerialPortWrapper
    {
        string PortName { get; set; }
        int BaudRate { get; set; }
        int DataBits { get; set; }
        StopBits StopBits { get; set; }
        Parity Parity { get; set; }
        bool IsOpen { get; }

        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        void Open();
        void Close();
        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
        void DiscardInBuffer();
        void DiscardOutBuffer();
    }

    public class SerialPortWrapper : ISerialPortWrapper
    {
        private SerialPort _serialPort;

        public string PortName
        {
            get => _serialPort.PortName;
            set => _serialPort.PortName = value;
        }

        public int BaudRate
        {
            get => _serialPort.BaudRate;
            set => _serialPort.BaudRate = value;
        }

        public int DataBits
        {
            get => _serialPort.DataBits;
            set => _serialPort.DataBits = value;
        }

        public StopBits StopBits
        {
            get => _serialPort.StopBits;
            set => _serialPort.StopBits = value;
        }

        public Parity Parity
        {
            get => _serialPort.Parity;
            set => _serialPort.Parity = value;
        }

        public bool IsOpen => _serialPort.IsOpen;

        // ReadTimeout과 WriteTimeout 속성 추가
        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _serialPort.WriteTimeout;
            set => _serialPort.WriteTimeout = value;
        }

        public SerialPortWrapper()
        {
            _serialPort = new SerialPort();
        }

        public void Open()
        {
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.Read(buffer, offset, count);
        }

        public void DiscardInBuffer()
        {
            _serialPort.DiscardInBuffer();
        }

        public void DiscardOutBuffer()
        {
            _serialPort.DiscardOutBuffer();
        }
    }
}
