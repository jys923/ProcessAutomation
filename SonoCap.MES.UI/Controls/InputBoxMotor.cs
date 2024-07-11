using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;

namespace SonoCap.MES.UI.Controls
{
    public class InputBoxMotor
    {
        public static MotorModule? Show(string title, string prompt, IMotorModuleRepository motorModuleRepository) // 추가 매개변수
        {
            InputBoxMotorViewModel viewModel = new InputBoxMotorViewModel(title, prompt, motorModuleRepository);
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
