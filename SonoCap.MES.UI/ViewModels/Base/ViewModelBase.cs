using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using SonoCap.WpfCommons;
using System.ComponentModel;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels.Base
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected Window? Window;
        public ISnackbarMessageQueue MessageQueue { get; }
        protected ViewModelBase()
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        }

        // 메시지만 출력
        protected void ShowSnackbar(string message)
        {
            if (MessageQueue is SnackbarMessageQueue queue)
            {
                queue.Clear();
                queue.Enqueue(message);
            }
        }

        // 파일 경로 출력 + 열기 버튼
        protected void ShowSnackbarWithOpen(string path)
        {
            if (MessageQueue is SnackbarMessageQueue queue)
            {
                queue.Clear();
                queue.Enqueue(
                    $"Capture: {path}",
                    "열기",
                    () => Utilities.OpenFolder(path)
                );
            }
        }

        private void AddLifecycleHander()
        {
            Window!.Loaded += OnWindowLoaded;
            Window!.Closing += OnWindowClosing;
            Window!.Activated += OnWindowActivated;
        }

        protected virtual void OnWindowActivated(object? sender, EventArgs e)
        {
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
