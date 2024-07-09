using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.Views;
using System.ComponentModel;
using System.Windows;

namespace SonoCap.MES.UI.Services
{
    public class ViewService : IViewService
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowView<TView, TViewModel>(object? parameter = null)
            where TView : Window
            where TViewModel : INotifyPropertyChanged
        {
            INotifyPropertyChanged viewModel = (INotifyPropertyChanged)_serviceProvider.GetService(typeof(TViewModel))!;
            Window view = (Window)_serviceProvider.GetService(typeof(TView))!;

            if(parameter != null && viewModel is IParameterReceiver parameterReceiver)
            {
                parameterReceiver.ReceiveParameter(parameter);
            }

            view.DataContext = viewModel;
            view.Show();
        }

        private bool ActivateView<TView>() where TView : Window
        {
            IEnumerable<Window> windows = Application.Current.Windows.OfType<TView>();

            if (windows.Any())
            {
                windows.ElementAt(0).Activate();
                return true;
            } 
            return false;
        }

        public void ShowMainView()
        {
            ShowView<MainView, MainViewModel>();
        }

        public void ShowTestView(SubData subData)
        {
            if (!ActivateView<TestView>())
            {
                ShowView<TestView, TestViewModel>(subData);
            }
        }
    }
}
