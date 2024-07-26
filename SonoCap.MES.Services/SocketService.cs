using SonoCap.MES.Services.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Serilog;
using System.Runtime.InteropServices;
using SonoCap.MES.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SonoCap.MES.Services
{
    public class SocketService : ISocketService
    {
        //private byte[]? receivedData;
        private ImgAndMeta? receivedData;
        private TcpClient _client;
        private TaskCompletionSource<bool> _responseReceived = new TaskCompletionSource<bool>();

        public event EventHandler<ImgAndMeta>? DataReceived;

        public SocketService()
        {
            _client = new TcpClient();
        }

        public async Task ConnectAsync(string serverIP, int port)
        {
            try
            {
                await _client.ConnectAsync(IPAddress.Parse(serverIP), port);
            }
            catch (Exception ex)
            {
                // 예외 처리 필요
                Log.Error($"연결 오류: {ex.Message}");

                // 프로그램 종료
                Environment.Exit(1); // 또는 Application.Current.Shutdown()
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SocketHeader
        {
            public byte CMD;
            public int ImgSize;
            public int DataSize;
        }

        public async Task ReceiveDataAsync()
        {
            try
            {
                NetworkStream stream = _client.GetStream();
                while (_client.Connected)
                {
                    Log.Information("ReceiveDataAsync");
                    // 헤더를 읽어 데이터 크기를 가져옵니다.
                    byte[] headerBuffer = new byte[Marshal.SizeOf<SocketHeader>()];
                    await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);

                    // 헤더를 구조체로 변환
                    SocketHeader header;
                    using (MemoryStream headerStream = new MemoryStream(headerBuffer))
                    using (BinaryReader headerReader = new BinaryReader(headerStream))
                    {
                        header.CMD = headerReader.ReadByte();
                        header.ImgSize = headerReader.ReadInt32();
                        header.DataSize = headerReader.ReadInt32();
                    }

                    // 유효성 검사 예시: CMD 값이 특정 범위를 벗어나면 무시
                    if (header.CMD != (byte)'A' || header.ImgSize < 0 || header.DataSize < 0)
                    {
                        Log.Information($"유효하지 않은 CMD 값: {header.CMD}, 데이터 무시");
                        return;
                    }

                    // 데이터를 받을 버퍼를 초기화합니다.
                    byte[] buffer = new byte[header.ImgSize];
                    int totalBytesRead = 0;

                    // 데이터를 전부 받을 때까지 반복해서 읽습니다.
                    while (totalBytesRead < header.ImgSize)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, header.ImgSize - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            // 연결이 끊겼거나 EOF
                            throw new IOException("Connection closed prematurely.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    Log.Information($"Total Image bytes read: {totalBytesRead}");

                    byte[] bufferMeta = new byte[header.DataSize];
                    totalBytesRead = 0;

                    // 데이터를 전부 받을 때까지 반복해서 읽습니다.
                    while (totalBytesRead < header.DataSize)
                    {
                        int bytesRead = await stream.ReadAsync(bufferMeta, totalBytesRead, header.DataSize - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            // 연결이 끊겼거나 EOF
                            throw new IOException("Connection closed prematurely.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    string metaJson = System.Text.Encoding.UTF8.GetString(bufferMeta);
                    
                    Log.Information($"Total Meta bytes read: {totalBytesRead}");

                    // 응답을 기다리는 작업이 완료됨을 알립니다.
                    receivedData = new ImgAndMeta(buffer, metaJson);
                    OnDataReceived(receivedData);
                    OnResponseReceived();

                    // 데이터 수신 완료 후 이벤트를 통해 데이터 전달
                }
            }
            catch (Exception ex)
            {
                // 예외 처리 필요
                Log.Error($"수신 오류: {ex.Message}");
            }
        }


        public async Task SendDataAsync(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await _client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                // 예외 처리 필요
                Log.Error($"송신 오류: {ex.Message}");
            }
        }

        protected virtual void OnDataReceived(ImgAndMeta data)
        {
            DataReceived?.Invoke(this, data);
        }

        public async Task<ImgAndMeta?> WaitForResponseAsync()
        {
            await _responseReceived.Task;
            return receivedData;
        }

        // 응답을 받았을 때 호출하는 메서드
        private void OnResponseReceived()
        {
            _responseReceived.TrySetResult(true);
            _responseReceived = new TaskCompletionSource<bool>(); // 새로운 TaskCompletionSource 생성
        }
    }
}
