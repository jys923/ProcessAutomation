using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels;
using SonoCap.MES.UI.ViewModels.Base;
using SonoCap.MES.UI.Views;
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
            where TViewModel : ViewModelBase
        {
            ViewModelBase viewModel = (ViewModelBase)_serviceProvider.GetService(typeof(TViewModel))!;
            Window view = (Window)_serviceProvider.GetService(typeof(TView))!;

            viewModel.SetWindow(view);
            if (parameter != null && viewModel is IParameterReceiver parameterReceiver)
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
            if (!ActivateView<MainView>())
                ShowView<MainView, MainViewModel>();
        }

        public void ShowTestingView(SubData subData)
        {
            if (!ActivateView<TestingView>())
                ShowView<TestingView, TestingViewModel>(subData);
        }

        public void ShowTestListView()
        {
            if (!ActivateView<TestListView>())
                ShowView<TestListView, TestListViewModel>();
        }

        public void ShowProbeListView()
        {
            if (!ActivateView<ProbeListView>())
                ShowView<ProbeListView, ProbeListViewModel>();
        }
    }
}
