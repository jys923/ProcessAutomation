using SonoCap.MES.UI.ViewModels;
using System.Windows;
using System.Windows.Threading;

namespace SonoCap.MES.UI.Helpers
{
    public static class DialogHelper
    {
        public static bool ShowModalToOwner(Window dialog, Window owner)
        {
            if (dialog == null) throw new ArgumentNullException(nameof(dialog));
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            dialog.Owner = owner;
            dialog.ShowInTaskbar = false;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var wasEnabled = owner.IsEnabled;
            owner.IsEnabled = false;

            bool result = false;
            var frame = new DispatcherFrame();

            dialog.Closed += (s, e) =>
            {
                if (dialog.DataContext is AlertBoxViewModel vm)
                    result = vm.Result;   // VM.Result에서 확인 여부 가져옴
                frame.Continue = false;
            };

            try
            {
                dialog.Show();
                Dispatcher.PushFrame(frame);
            }
            finally
            {
                owner.IsEnabled = wasEnabled;
                owner.Activate();
            }

            return result;
        }
    }

}
