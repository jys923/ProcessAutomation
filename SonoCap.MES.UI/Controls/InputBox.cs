using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public class InputBox
    {
        public static string? Show(string title, string prompt, string defaultInputMessage = "")
        {
            InputBoxViewModel viewModel = new InputBoxViewModel(title, prompt, defaultInputMessage);
            InputBoxView view = new InputBoxView
            {
                DataContext = viewModel
            };

            return view.ShowDialog() ?? false
              ? viewModel.Response
              : null;
        }
    }
}
