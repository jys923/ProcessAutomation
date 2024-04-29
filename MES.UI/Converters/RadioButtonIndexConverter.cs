using System.Globalization;
using System.Windows.Data;

namespace MES.UI.Converters
{
    public class RadioButtonIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && int.TryParse(parameter?.ToString(), out int targetIndex))
            {
                return selectedIndex == targetIndex;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                return int.Parse(parameter?.ToString());
            }
            return Binding.DoNothing;
        }
    }
}
