using System;
using System.Globalization;
using System.Windows.Data;

namespace Singer.Client.Converter
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().BitmapImageFromBase64();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
