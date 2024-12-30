using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SonoCap.MES.UI.Model
{
    public class CustomEllipse
    {
        public Ellipse Ellipse { get; set; }
        public TextBlock InfoTextBlock { get; set; }
        public string InfoText { get; set; } = string.Empty;

        public double _top { get; set; }
        public double _left { get; set; }

        public CustomEllipse()
        {
            Ellipse = new Ellipse
            {
                Stroke = Brushes.Tomato,
                StrokeThickness = 2
            };

            InfoTextBlock = new TextBlock
            {
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                FontSize = 12
            };

            //InfoText = 0;
        }

        public void UpdateText()
        {
            InfoTextBlock.Text = InfoText;
        }
    }
}
