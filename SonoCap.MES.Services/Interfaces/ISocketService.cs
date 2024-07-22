using SonoCap.MES.Models;

namespace SonoCap.MES.Services.Interfaces
{
    public interface ISocketService
    {
        event EventHandler<ImgAndMeta> DataReceived;

        Task ConnectAsync(string serverIP, int port);
        Task ReceiveDataAsync();
        Task SendDataAsync(string data);
        Task<ImgAndMeta?> WaitForResponseAsync();
    }
}
