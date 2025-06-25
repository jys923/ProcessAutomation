using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SonoCap.MES.PreviewUI.Views
{
    /// <summary>
    /// PreviewSaveView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreviewSaveView : Window
    {
        public PreviewSaveView()
        {
            InitializeComponent();
        }
        private void SrcImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SrcImg.Focus(); // 여기서 직접 포커스 줌
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 클릭된 요소가 Image가 아니면 포커스 제거
            //if (e.OriginalSource is not Image && Keyboard.FocusedElement is UIElement focused)
            //{
            //    focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //}

            if (e.OriginalSource is not FrameworkElement fe || fe.Name != "SrcImg")
            {
                if (Keyboard.FocusedElement is UIElement focused)
                    focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
