using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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
            //Window!.Initialized += OnWindowInitialized;
            Window!.Loaded += OnWindowLoaded;
            //Window!.ContentRendered += OnWindowContentRendered;
            Window!.Activated += OnWindowActivated;
            //Window!.Deactivated += OnWindowDeactivated;
            Window!.Closing += OnWindowClosing;
            //Window!.Closed += OnWindowClosed;
            //Window!.Unloaded += OnWindowUnloaded;
        }

        protected virtual void OnWindowActivated(object? sender, EventArgs e)
        {
        }

        protected virtual void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            RmMsg();
        }

        
        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            AddMsg();
        }

        internal void SetWindow(Window window)
        {
            Window = window;
            AddLifecycleHander();
        }

        protected void RegisterMessageHandler<TMessage>(
            Action<TMessage> handler
        ) where TMessage : class
        {
            WeakReferenceMessenger.Default.Register<TMessage>(this, (_, m) => handler(m));
        }

        // ViewModelBase 내부에 추가
        protected void UnregisterMessagesOfType<TMessage>()
            where TMessage : class
        {
            WeakReferenceMessenger.Default.Unregister<TMessage>(this);
        }

        protected void UnregisterAllMessages()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        protected virtual void AddMsg()
        {
        }

        protected virtual void RmMsg()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
