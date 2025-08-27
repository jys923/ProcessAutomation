using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class AlertBoxViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = string.Empty;
        
        [ObservableProperty]
        private string _prompt = string.Empty;

        [ObservableProperty]
        private bool _result;

        [RelayCommand]
        private void Ok(Window window)
        {
            Result = true;
            window.Close();
        }

        public AlertBoxViewModel(string title, string prompt)
        {
            Title = title;
            Prompt = prompt;
        }
    }
}
