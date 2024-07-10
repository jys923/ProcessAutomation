using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public class InputBoxMotor
    {
        public static MotorModule? Show(string title, string prompt, string defaultInputMessage = "")
        {
            InputBoxMotorViewModel viewModel = new InputBoxMotorViewModel(title, prompt);
            InputBoxMotorView view = new InputBoxMotorView
            {
                DataContext = viewModel
            };

            return view.ShowDialog() ?? false
              ? viewModel.Response
              : null;
        }
    }
}
