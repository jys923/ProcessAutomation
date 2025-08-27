using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;
using System.Windows;
using System.Windows.Threading;

namespace SonoCap.MES.UI.Controls
{
    public class AlertBox
    {
        public static bool Show2(string title, string prompt) // 추가 매개변수
        {
            Window ownerWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            AlertBoxViewModel viewModel = new AlertBoxViewModel(title, prompt);
            AlertBoxView view = new AlertBoxView
            {
                DataContext = viewModel,
                Owner = ownerWindow // Owner 속성 설정
            };

            return view.ShowDialog() ?? false;
        }

        public static bool Show3(string title, string prompt)
        {
            var owner = Application.Current.Windows.OfType<Window>()
                             .FirstOrDefault(w => w.IsActive)
                       ?? Application.Current.MainWindow;

            var vm = new AlertBoxViewModel(title, prompt);
            var dlg = new AlertBoxView
            {
                DataContext = vm,
                Owner = owner,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            // 1) 부모만 잠그기
            var wasEnabled = owner.IsEnabled;
            owner.IsEnabled = false;

            bool? result = null;
            var frame = new DispatcherFrame();

            // 2) 닫힐 때 프레임 종료 + 부모 복구
            dlg.Closed += (s, e) =>
            {
                result = dlg.DialogResult;
                frame.Continue = false;
            };

            // 3) 모델리스로 띄우고, 의사 모달 루프 진입
            dlg.Show();                 // ← ShowDialog() 아님 (중요)
            Dispatcher.PushFrame(frame);

            // 4) 복구
            owner.IsEnabled = wasEnabled;
            owner.Activate();

            return result ?? false;
        }

        public static bool Show(string title, string prompt)
        {
            var owner = Application.Current.Windows.OfType<Window>()
                             .FirstOrDefault(w => w.IsActive)
                       ?? Application.Current.MainWindow;

            var vm = new AlertBoxViewModel(title, prompt);
            var dlg = new AlertBoxView
            {
                DataContext = vm,
                Owner = owner,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var wasEnabled = owner.IsEnabled;
            owner.IsEnabled = false;

            bool result = false;
            var frame = new DispatcherFrame();

            dlg.Closed += (s, e) =>
            {
                // VM에 Result 프로퍼티를 하나 두고, 확인 누르면 true 세팅
                if (dlg.DataContext is AlertBoxViewModel avm)
                    result = avm.Result;

                frame.Continue = false;
            };

            try
            {
                dlg.Show();
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