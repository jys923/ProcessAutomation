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
        private TestingViewModel viewModel;

        private Collection<Ellipse> ellipses = new();

        public TestingView()
        {
            InitializeComponent();

            // DataContext가 설정될 때까지 대기
            Loaded += TestingView_Loaded;
        }

        private void TestingView_Loaded(object sender, RoutedEventArgs e)
        {
            // DataContext가 설정된 후에 ViewModel을 가져옴
            viewModel = (TestingViewModel)DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Lines) && viewModel.Lines.Count > 0)
            {
                drawingCanvas.Children.Clear();
                drawingCanvas.Children.Add(ResImg);

                foreach (var line in viewModel.Lines)
                {
                    drawingCanvas.Children.Add(line.Line);
                    drawingCanvas.Children.Add(line.InfoTextBlock);
                }
            }
            else if (e.PropertyName == nameof(viewModel.Ellipses) && viewModel.Ellipses.Count > 0)
            {
                drawingCanvas.Children.Clear();
                drawingCanvas.Children.Add(ResImg);

                foreach (var ellipse in viewModel.Ellipses)
                {
                    drawingCanvas.Children.Add(ellipse.Ellipse);
                    drawingCanvas.Children.Add(ellipse.InfoTextBlock);
                }
            }
            else if (e.PropertyName == nameof(viewModel.ClearCanvas) && viewModel.ClearCanvas)
            {
                drawingCanvas.Children.Clear();
                drawingCanvas.Children.Add(ResImg);
                viewModel.ClearCanvas = false;
            }
        }
    }
}

//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Shapes;

//namespace SonoCap.MES.UI.Views
//{
//    public partial class TestingView : Window
//    {
//        private bool isDrawing;
//        private System.Windows.Shapes.Line currentLine;

//        public TestingView()
//        {
//            InitializeComponent();
//            SetImagePosition();
//        }

//        private void SetImagePosition()
//        {
//            double canvasWidth = drawingCanvas.Width;
//            double canvasHeight = drawingCanvas.Height;
//            double imageWidth = ResImg.Width;
//            double imageHeight = ResImg.Height;

//            Canvas.SetLeft(ResImg, (canvasWidth - imageWidth) / 2);
//            Canvas.SetTop(ResImg, (canvasHeight - imageHeight) / 2);
//        }

//        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                // 기존 선과 텍스트 블록을 지웁니다.
//                drawingCanvas.Children.Clear();
//                drawingCanvas.Children.Add(ResImg);
//                drawingCanvas.Children.Add(lengthTextBlock);

//                isDrawing = true;
//                Point startPoint = e.GetPosition(drawingCanvas);

//                currentLine = new System.Windows.Shapes.Line();
//                currentLine.Stroke = Brushes.Tomato;
//                currentLine.StrokeThickness = 2;
//                currentLine.X1 = startPoint.X;
//                currentLine.Y1 = startPoint.Y;
//                currentLine.X2 = startPoint.X;
//                currentLine.Y2 = startPoint.Y;

//                drawingCanvas.Children.Add(currentLine);
//                lengthTextBlock.Visibility = Visibility.Visible;
//            }
//        }

//        private void Canvas_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (isDrawing && currentLine != null)
//            {
//                Point currentPoint = e.GetPosition(drawingCanvas);
//                currentLine.X2 = currentPoint.X;
//                currentLine.Y2 = currentPoint.Y;

//                // 선의 길이 계산
//                double length = Math.Sqrt(Math.Pow(currentLine.X2 - currentLine.X1, 2) + Math.Pow(currentLine.Y2 - currentLine.Y1, 2));
//                lengthTextBlock.Text = $"{length:F2}";

//                // 텍스트 위치 설정 (선의 중간 위치)
//                double midX = (currentLine.X1 + currentLine.X2) / 2;
//                double midY = (currentLine.Y1 + currentLine.Y2) / 2;

//                Canvas.SetLeft(lengthTextBlock, midX);
//                Canvas.SetTop(lengthTextBlock, midY);
//            }
//        }

//        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Released)
//            {
//                isDrawing = false;
//            }
//        }
//    }
//}

