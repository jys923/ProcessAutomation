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

            view.DataContext = viewModel;
            view.Show();
        }

        public void ShowMainView()
        {
            ShowView<MainView,MainViewModel>();
        }
    }
}
