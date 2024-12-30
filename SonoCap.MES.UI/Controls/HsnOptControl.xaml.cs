using SonoCap.MES.UI.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SonoCap.MES.UI.Controls
{
    /// <summary>
    /// HsnOptControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HsnOptControl : UserControl
    {
        //TextBox control 참고
        public HsnOptControl()
        {
            InitializeComponent();
        }


        public string Text1
        {
            get { return (string)GetValue(Text1Property); }
            set { SetValue(Text1Property, value); }
        }

        public string Text2
        {
            get { return (string)GetValue(Text2Property); }
            set { SetValue(Text2Property, value); }
        }

        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(HsnOptControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(string), typeof(HsnOptControl), new PropertyMetadata(string.Empty));

        //private readonly HsnOptViewModel _viewModel;

        //public HsnOptControl(HsnOptViewModel viewModel)
        //{
        //    InitializeComponent();
        //    _viewModel = viewModel;
        //    this.DataContext = _viewModel;
        //}

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
