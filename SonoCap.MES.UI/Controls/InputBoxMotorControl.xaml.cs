using System.Windows;
using System.Windows.Controls;

namespace SonoCap.MES.UI.Controls
{
    /// <summary>
    /// InputBoxMotorControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputBoxMotorControl : UserControl
    {
        public InputBoxMotorControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtInput.Focus();
        }
    }
}
