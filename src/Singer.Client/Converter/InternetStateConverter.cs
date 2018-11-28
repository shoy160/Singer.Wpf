using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Singer.Core;

namespace Singer.Client.Converter
{
    /// <summary> 网络状态转换器 </summary>
    public class InternetStateConverter : IValueConverter
    {
        public Brush DefaultBrush { get; set; }
        public Brush UnConnectBrush { get; set; }
        public Brush ConnectedBrush { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DefaultBrush;
            return value.CastTo(false) ? ConnectedBrush : UnConnectBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
