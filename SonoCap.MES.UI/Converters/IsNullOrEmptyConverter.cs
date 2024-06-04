using System.Globalization;
using System.Windows.Data;

namespace SonoCap.MES.UI.Converters
{
    public class IsNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value가 문자열이고 null 또는 빈 문자열인 경우 true를 반환합니다.
            return string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
