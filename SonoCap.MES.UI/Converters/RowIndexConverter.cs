using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SonoCap.MES.UI.Converters
{
    public class RowIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DataGridRow row) || row.Item == null || !(row.DataContext is IList data))
                return Binding.DoNothing;

            int index = data.IndexOf(row.Item) + 1;
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
