using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Singer.Client.Controls
{
    /// <summary> 控件公共附加属性 </summary>
    public class AttachProperty : DependencyObject
    {
        #region 水印
        /// <summary> 水印 </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(AttachProperty), new FrameworkPropertyMetadata(string.Empty));
        public static string GetWatermark(DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        /// <summary> 水印字体大小 </summary>
        public static readonly DependencyProperty WatermarkSizeProperty = DependencyProperty.RegisterAttached(
            "WatermarkSize", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(14D));

        public static double GetWatermarkSize(DependencyObject d)
        {
            return (double)d.GetValue(WatermarkSizeProperty);
        }

        public static void SetWatermarkSize(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkSizeProperty, value);
        }

        #endregion

        #region 字体图标
        /// <summary> 字体图标 </summary>
        public static readonly DependencyProperty FontIconProperty = DependencyProperty.RegisterAttached(
            "FontIcon", typeof(string), typeof(AttachProperty), new FrameworkPropertyMetadata(string.Empty));

        public static string GetFontIcon(DependencyObject d)
        {
            return (string)d.GetValue(FontIconProperty);
        }

        public static void SetFontIcon(DependencyObject obj, string value)
        {
            obj.SetValue(FontIconProperty, value);
        }

        /// <summary> 字体图标大小 </summary>
        public static readonly DependencyProperty FIconSizeProperty = DependencyProperty.RegisterAttached(
            "FIconSize", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(12D));

        public static double GetFIconSize(DependencyObject d)
        {
            return (double)d.GetValue(FIconSizeProperty);
        }

        public static void SetFIconSize(DependencyObject obj, double value)
        {
            obj.SetValue(FIconSizeProperty, value);
        }

        /// <summary> 字体图标大小 </summary>
        public static readonly DependencyProperty FIconColorProperty = DependencyProperty.RegisterAttached(
            "FIconColor", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.White));

        public static Brush GetFIconColor(DependencyObject d)
        {
            return (Brush)d.GetValue(FIconSizeProperty);
        }

        public static void SetFIconColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(FIconSizeProperty, value);
        }
        /// <summary> 字体图标Margin </summary>
        public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.RegisterAttached(
            "FIconMargin", typeof(Thickness), typeof(AttachProperty), new FrameworkPropertyMetadata(null));

        public static Thickness GetFIconMargin(DependencyObject d)
        {
            return (Thickness)d.GetValue(FIconMarginProperty);
        }

        public static void SetFIconMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(FIconMarginProperty, value);
        }

        /// <summary> 图标 </summary>
        public static readonly DependencyProperty IconImageProperty = DependencyProperty.RegisterAttached(
            "IconImage", typeof(ImageSource), typeof(AttachProperty), new FrameworkPropertyMetadata(null));

        public static ImageSource GetIconImage(DependencyObject d)
        {
            return (ImageSource)d.GetValue(IconImageProperty);
        }

        public static void SetIconImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconImageProperty, value);
        }

        /// <summary> 图标宽度 </summary>
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.RegisterAttached(
            "IconWidth", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(22D));

        public static double GetIconWidth(DependencyObject d)
        {
            return (double)d.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        /// <summary> 图标宽度 </summary>
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.RegisterAttached(
            "IconHeight", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(22D));

        public static double GetIconHeight(DependencyObject d)
        {
            return (double)d.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        #endregion

        #region PasswordBar

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(AttachProperty), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            var password = (string)e.NewValue;

            if (passwordBox != null && passwordBox.Password != password)
            {
                passwordBox.Password = password;
            }
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        //public static readonly DependencyProperty IsMonitoringProperty =
        //    DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(AttachProperty),
        //        new UIPropertyMetadata(false, OnIsMonitoringChanged));

        //public static bool GetIsMonitoring(DependencyObject obj)
        //{
        //    return (bool)obj.GetValue(IsMonitoringProperty);
        //}

        //public static void SetIsMonitoring(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(IsMonitoringProperty, value);
        //}

        //public static readonly DependencyProperty PasswordLengthProperty =
        //    DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(AttachProperty), new UIPropertyMetadata(0));

        //public static int GetPasswordLength(DependencyObject obj)
        //{
        //    return (int)obj.GetValue(PasswordLengthProperty);
        //}

        //public static void SetPasswordLength(DependencyObject obj, int value)
        //{
        //    obj.SetValue(PasswordLengthProperty, value);
        //}
        //private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is PasswordBox)
        //    {
        //        var pb = d as PasswordBox;
        //        if ((bool)e.NewValue)
        //        {
        //            pb.PasswordChanged += PasswordChanged;
        //        }
        //        else
        //        {
        //            pb.PasswordChanged -= PasswordChanged;
        //        }
        //    }
        //    else if (d is TextBox) { }
        //}

        //private static void PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    var pb = sender as PasswordBox;
        //    if (pb == null)
        //    {
        //        return;
        //    }
        //    SetPasswordLength(pb, pb.Password.Length);
        //}
        #endregion

        #region AllowsAnimationProperty 启用旋转动画
        /// <summary>
        /// 启用旋转动画
        /// </summary>
        public static readonly DependencyProperty RotateAnimationProperty = DependencyProperty.RegisterAttached(
            "RotateAnimation", typeof(bool), typeof(AttachProperty), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));

        public static bool GetAllowsAnimation(DependencyObject d)
        {
            return (bool)d.GetValue(RotateAnimationProperty);
        }

        public static void SetAllowsAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(RotateAnimationProperty, value);
        }

        /// <summary>
        /// 旋转动画刻度
        /// </summary>
        private static readonly DoubleAnimation RotateAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));

        /// <summary>
        /// 绑定动画事件
        /// </summary>
        private static void AllowsAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as FrameworkElement;
            if (uc == null) return;
            if (uc.RenderTransformOrigin == new Point(0, 0))
            {
                uc.RenderTransformOrigin = new Point(0.5, 0.5);
                var trans = new RotateTransform(0);
                uc.RenderTransform = trans;
            }
            var value = (bool)e.NewValue;
            if (value)
            {
                RotateAnimation.To = 180;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
            else
            {
                RotateAnimation.To = 0;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
        }
        #endregion

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(AttachProperty), new FrameworkPropertyMetadata(null));

        public static CornerRadius GetCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region Hover
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.RegisterAttached(
            "HoverBackground", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBackgroundProperty, value);
        }

        public static Brush GetHoverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBackgroundProperty);
        }

        public static readonly DependencyProperty HoverBorderProperty = DependencyProperty.RegisterAttached(
            "HoverBorder", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBorder(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBorderProperty, value);
        }

        public static Brush GetHoverBorder(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBorderProperty);
        }

        public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.RegisterAttached(
            "HoverBrush", typeof(Brush), typeof(AttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static void SetHoverBrush(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBrushProperty, value);
        }

        public static Brush GetHoverBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBrushProperty);
        }

        public static readonly DependencyProperty HoverOpacityProperty = DependencyProperty.RegisterAttached(
            "HoverOpacity", typeof(double), typeof(AttachProperty), new FrameworkPropertyMetadata(1D));

        public static void SetHoverOpacity(DependencyObject element, double value)
        {
            element.SetValue(HoverOpacityProperty, value);
        }

        public static double GetHoverOpacity(DependencyObject element)
        {
            return (double)element.GetValue(HoverOpacityProperty);
        }
        #endregion

    }
}
