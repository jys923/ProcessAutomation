namespace SonoCap.MES.UI.Services.Interfaces
{
    public interface IMotorService
    {
        bool InitPort();
        void StartMotor();
        void StopMotor();
        void UpdateSettings(int lineDensity, int viewDepth);
    }
}