using SonoCap.MES.Models;

namespace SonoCap.MES.Services.Interfaces
{
    public interface ISocketService : IDisposable
    {
        event EventHandler<ImgAndMeta> DataReceived;
        event EventHandler CloseViewRequested;

        Task ConnectAsync(string serverIP, int port);
        Task ReceiveDataAsync();
        Task SendDataAsync(string data);
        Task<ImgAndMeta?> WaitForResponseAsync();
    }
}
