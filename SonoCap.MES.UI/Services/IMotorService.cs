using SonoCap.MES.UI.Services;

namespace SonoCap.MES.UI.Services
{
    public interface IMotorService
    {
        event Action<MotorService.MotorState> MotorStateChanged;
        bool IsConnected { get; }
        bool InitializePort();
        void StartMotor();
        void StopMotor();
        void SendACK();
        byte[] GetCommandBytes(MotorService.CMD cmd, MotorService.RPM? rpm = null, MotorService.PRF? prf = null);
    }
}