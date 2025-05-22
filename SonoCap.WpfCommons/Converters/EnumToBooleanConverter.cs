using System;
using System.Globalization;
using System.Windows.Data;

namespace SonoCap.WpfCommons.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (value.ToString() == parameter.ToString())
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool isChecked) || !isChecked || parameter == null)
                return null;

            return Enum.Parse(targetType, parameter.ToString());
        }
    }
}
