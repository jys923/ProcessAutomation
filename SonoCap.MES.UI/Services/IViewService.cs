using SonoCap.MES.Models;
using SonoCap.MES.UI.ViewModels.Base;
using System.Windows;

namespace SonoCap.MES.UI.Services
{
    public interface IViewService
    {
        void ShowView<TView, TViewModel>(object? parameter = null)
            where TView : Window
            where TViewModel : ViewModelBase;

        void ShowMainView();

        void ShowTestingView(SubData subData);

        void ShowTestListView();
        
        void ShowProbeListView();
    }
}
