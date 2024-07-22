using System.Globalization;
using System.Windows.Data;

namespace SonoCap.MES.UI.Converters
{
    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double gridHeight)
            {
                return gridHeight / 3; // Grid의 높이의 1/3을 반환
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
