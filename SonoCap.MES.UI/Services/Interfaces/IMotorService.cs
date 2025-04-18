using static SonoCap.MES.UI.Services.MotorService;

namespace SonoCap.MES.UI.Services.Interfaces
{
    public interface IMotorService
    {
        bool InitializeMotor();
        bool InitPort();
        void OnMotorStateChanged(int prfHz, int density);
        void StartMotor();
        void StopMotor();
    }
}