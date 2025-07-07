using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SonoCap.MES.UI.Messages
{
    public class ViewModelActionMessage : ValueChangedMessage<(string TargetViewModel, string Action)>
    {
        public string TargetViewModel => Value.TargetViewModel;
        public string Action => Value.Action;

        public ViewModelActionMessage(string targetViewModel, string action)
            : base((targetViewModel, action))
        {
        }
    }

    public class ViewModelPingMessage : RequestMessage<bool>
    {
        // 예시: 대상 ViewModel이 살아있는지 확인
    }

    public class ViewModelCloseMessage : ValueChangedMessage<string>
    {
        public ViewModelCloseMessage(string targetViewModel) : base(targetViewModel) { }
    }
}
