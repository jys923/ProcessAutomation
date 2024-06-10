using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SonoCap.MES.UI.Converters
{
    public class ValidatingBorderBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isValidation = values[0] as bool?;
            bool? isEnabled = values[1] as bool?;
            Brush defaultBrush = (Brush)values[2];

            if (isEnabled.HasValue && isEnabled == false)
            {
                return defaultBrush;
            }

            return (isValidation.HasValue && isValidation == false) ? Brushes.Red : defaultBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
