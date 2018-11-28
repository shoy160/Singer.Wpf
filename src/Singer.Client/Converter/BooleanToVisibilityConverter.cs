using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Singer.Client.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = (bool?)value ?? false;
            if (parameter == null)
                return flag ? Visibility.Visible : Visibility.Collapsed;
            return flag ? Visibility.Collapsed : Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vsb = value as Visibility?;
            if (parameter == null) return vsb == Visibility.Visible;
            return vsb == Visibility.Collapsed;
        }
    }
}
