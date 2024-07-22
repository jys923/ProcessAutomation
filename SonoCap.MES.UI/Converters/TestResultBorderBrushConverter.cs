using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SonoCap.MES.UI.Converters
{
    public class TestResultBorderBrushConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    int testResult = (int)value;
        //    if (testResult == -2)
        //    {
        //        return Brushes.Tomato;
        //    }
        //    return new SolidColorBrush(Color.FromArgb(0xFF, 0xD5, 0xDF, 0xE5));
        //}
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool testResult = (bool)value;
            if (testResult)
            {
                return Brushes.Tomato;
            }
            return new SolidColorBrush(Color.FromArgb(0xFF, 0xD5, 0xDF, 0xE5));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
