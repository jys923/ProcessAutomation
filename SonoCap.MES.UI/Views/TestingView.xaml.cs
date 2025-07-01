using NetTopologySuite.Utilities;
using SonoCap.MES.UI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace SonoCap.MES.UI.Views
{
    /// <summary>
    /// TestView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestingView : Window
    {
        //private TestingViewModel viewModel;

        public TestingView()
        {
            InitializeComponent();

            // DataContext가 설정될 때까지 대기
            //Loaded += TestingView_Loaded;
        }
        private void ImageFocusArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(ImageFocusArea);
        }

        //private void TestingView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // DataContext가 설정된 후에 ViewModel을 가져옴
        //    viewModel = (TestingViewModel)DataContext;
        //}
    }
}