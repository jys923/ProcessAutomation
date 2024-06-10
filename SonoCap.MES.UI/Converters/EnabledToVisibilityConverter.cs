using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SonoCap.MES.UI.Converters
{
    public class EnabledToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
