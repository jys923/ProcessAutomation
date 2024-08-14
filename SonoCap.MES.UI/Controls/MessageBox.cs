using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public class MessageBox
    {
        public static bool Show(string title, string prompt) // 추가 매개변수
        {
            MessageBoxViewModel viewModel = new MessageBoxViewModel(title, prompt);
            MessageBoxView view = new MessageBoxView
            {
                DataContext = viewModel
            };

            return view.ShowDialog() ?? false;
        }
    }
}
