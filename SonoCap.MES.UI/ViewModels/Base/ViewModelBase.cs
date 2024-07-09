using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels.Base
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected Window? Window;

        private void AddLifecycleHander()
        {
            Window!.Loaded += OnWindowLoaded;
            Window!.Closing += OnWindowClosing;
        }

        protected virtual void OnWindowClosing(object? sender, CancelEventArgs e)
        {
        }

        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        internal void SetWindow(Window window)
        {
            Window = window;
            AddLifecycleHander();
        }
    }
}
