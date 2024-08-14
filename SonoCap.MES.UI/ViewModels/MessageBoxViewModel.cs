using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class MessageBoxViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public MessageBoxViewModel(string title, string prompt)
        {
            Title = title;
            Prompt = prompt;
        }
    }
}
