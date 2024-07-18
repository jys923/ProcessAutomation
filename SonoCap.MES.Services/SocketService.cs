﻿using SonoCap.MES.Services.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Serilog;

namespace SonoCap.MES.Services
{
    public class SocketService : ISocketService
    {
        private byte[]? receivedData;
        private TcpClient _client;
        private TaskCompletionSource<bool> _responseReceived = new TaskCompletionSource<bool>();

        public event EventHandler<byte[]>? DataReceived;

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
                Console.WriteLine($"연결 오류: {ex.Message}");
            }
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
                    //byte[] headerBuffer = new byte[4];
                    //await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);
                    //int dataSize = BitConverter.ToInt32(headerBuffer, 0);
                    int dataSize = 1048576;
                    // 데이터를 받을 버퍼를 초기화합니다.
                    byte[] buffer = new byte[dataSize];
                    int totalBytesRead = 0;

                    // 데이터를 전부 받을 때까지 반복해서 읽습니다.
                    while (totalBytesRead < dataSize)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, dataSize - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            // 연결이 끊겼거나 EOF
                            throw new IOException("Connection closed prematurely.");
                        }
                        totalBytesRead += bytesRead;
                    }

                    Log.Information($"Total bytes read: {totalBytesRead}");

                    // 응답을 기다리는 작업이 완료됨을 알립니다.
                    receivedData = buffer;
                    OnDataReceived(buffer);
                    OnResponseReceived();

                    // 데이터 수신 완료 후 이벤트를 통해 데이터 전달
                }
            }
            catch (Exception ex)
            {
                // 예외 처리 필요
                Log.Information($"수신 오류: {ex.Message}");
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
                Log.Information($"송신 오류: {ex.Message}");
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        public async Task<byte[]?> WaitForResponseAsync()
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
