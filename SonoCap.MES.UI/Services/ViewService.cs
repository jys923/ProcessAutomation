using Microsoft.Extensions.DependencyInjection;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.Services.Interfaces;
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
                windows.ElementAt(0).Show(); // 창 다시 보이기
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

        public void ShowFirstView()
        {
            if (!ActivateView<FirstView>())
                ShowView<FirstView, FirstViewModel>();
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

        public void ShowPreviewSaveView()
        {
            if (!ActivateView<PreviewSaveView>())
                ShowView<PreviewSaveView, PreviewSaveViewModel>();
        }

        public TResult? ShowDialogView<TView, TViewModel, TResult>(Func<TViewModel, TResult?> getResult, object? parameter = null)
            where TView : Window
            where TViewModel : ViewModelBase
        {
            var viewModel = (TViewModel)_serviceProvider.GetService(typeof(TViewModel))!;
            var view = (TView)_serviceProvider.GetService(typeof(TView))!;
            view.DataContext = viewModel;
            viewModel.SetWindow(view);

            if (parameter != null && viewModel is IParameterReceiver receiver)
            {
                receiver.ReceiveParameter(parameter);
            }

            bool? result = view.ShowDialog();
            return result == true ? getResult(viewModel) : default;
        }

        public MotorModule? ShowInputBoxMotorView(string title, string prompt)
        {
            var param = new LabelInput { Title = title, Prompt = prompt };
            return ShowDialogView<InputBoxMotorView, InputBoxMotorViewModel, MotorModule>(
                vm => vm.Response,
                param
            );
        }
        //public MotorModule? ShowInputBoxMotorView(string title, string prompt)
        //{
        //    var param = new InputBoxParameter { Title = title, Prompt = prompt };
        //    return ShowDialogView<InputBoxMotorView, InputBoxMotorViewModel, MotorModule>(
        //        vm => vm.Response,
        //        param
        //    );
        //}

        //public MotorModule? ShowInputBoxMotorView(string title, string prompt)
        //{
        //    InputBoxMotorViewModel viewModel = new InputBoxMotorViewModel(title, prompt, _serviceProvider.GetService<IMotorModuleRepository>());
        //    InputBoxMotorView view = new InputBoxMotorView
        //    {
        //        DataContext = viewModel
        //    };

        //    return view.ShowDialog() ?? false
        //        ? viewModel.Response
        //        : null;
        //}

    }
}