namespace SonoCap.MES.Services.Interfaces
{
    public interface ISocketService
    {
        event EventHandler<byte[]> DataReceived;

        Task ConnectAsync(string serverIP, int port);
        Task ReceiveDataAsync();
        Task SendDataAsync(string data);
    }
}
