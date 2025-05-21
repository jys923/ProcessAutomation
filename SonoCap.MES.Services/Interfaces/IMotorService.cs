namespace SonoCap.MES.Services.Interfaces
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