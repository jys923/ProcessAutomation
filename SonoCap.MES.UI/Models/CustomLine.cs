using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SonoCap.MES.UI.Model
{
    public class CustomLine
    {
        public System.Windows.Shapes.Line Line { get; set; }
        public TextBlock InfoTextBlock { get; set; }
        public string InfoText { get; set; } = string.Empty;

        public double _top { get; set; }
        public double _left { get; set; }

        public CustomLine()
        {
            Line = new System.Windows.Shapes.Line
            {
                Stroke = Brushes.Tomato,
                StrokeThickness = 2,
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
