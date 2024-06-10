using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SonoCap.MES.UI.Converters
{
    public class ValidatingBorderThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isValidation = values[0] as bool?;
            bool? isEnabled = values[1] as bool?;
            Thickness defaultBrush = (Thickness)values[2];

            if (isEnabled.HasValue && isEnabled == false)
            {
                return defaultBrush;
            }

            return (isValidation.HasValue && isValidation == false) ? new Thickness(2) : defaultBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
