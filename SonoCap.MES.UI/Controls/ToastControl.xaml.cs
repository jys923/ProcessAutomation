using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SonoCap.MES.UI.Controls
{
    public partial class ToastControl : UserControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(string), typeof(ToastControl), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public ToastControl()
        {
            InitializeComponent();
            // 일정 시간 후에 자동으로 닫히게 하려면 Timer 등을 이용하여 구현할 수 있습니다.
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                //CloseToast();
                Visibility = System.Windows.Visibility.Collapsed;
            };
            timer.Start();
        }

        private void CloseToast()
        {
            var parent = Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }
    }
}