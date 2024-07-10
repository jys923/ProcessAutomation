using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class InputBoxMotorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;
        [ObservableProperty]
        private MotorModule? _response;

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public InputBoxMotorViewModel(string title, string prompt)
        {
            Title = title;
            Prompt = prompt;
            Response = new MotorModule { Sn = "aaaaasdcasdsd" };
        }
    }
}
