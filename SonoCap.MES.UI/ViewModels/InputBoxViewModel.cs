using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class InputBoxViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;
        [ObservableProperty]
        private string _response = string.Empty;

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public InputBoxViewModel(string title, string prompt, string defaultInputMessage)
        {
            Title = title;
            Prompt = prompt;
            Response = defaultInputMessage;
        }
    }
}
