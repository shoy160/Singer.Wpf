using Singer.Client.Commands;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Singer.Client.Controls
{
    public class KWindow : Window
    {
        #region 依赖属性
        //点击左侧菜单跳转page
        public static readonly DependencyProperty OpenMenuUrlCommandProperty = DependencyProperty.Register(
             "OpenMenuUrl", typeof(RoutedCommand), typeof(KWindow), new PropertyMetadata(null));
        public RoutedCommand OpenMenuUrl
        {
            get { return (RoutedCommand)GetValue(OpenMenuUrlCommandProperty); }
            set { SetValue(OpenMenuUrlCommandProperty, value); }
        }

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(KWindow),
                new PropertyMetadata(true));

        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        public static readonly DependencyProperty HeaderBrushProperty = DependencyProperty.Register(
            "HeaderBrush", typeof(Brush), typeof(KWindow), new PropertyMetadata(Brushes.Black));

        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderForegroundBrushProperty = DependencyProperty.Register(
            "HeaderForegroundBrush", typeof(Brush), typeof(KWindow), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// 头 字体颜色
        /// </summary>
        public Brush HeaderForegroundBrush
        {
            get { return (Brush)GetValue(HeaderForegroundBrushProperty); }
            set { SetValue(HeaderForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(
            "ShowHeader", typeof(bool), typeof(KWindow), new PropertyMetadata(true));

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundOpacityProperty = DependencyProperty.Register(
            "HeaderBackgroundOpacity", typeof(double), typeof(KWindow), new PropertyMetadata(0.3D));

        public double HeaderBackgroundOpacity
        {
            get { return (double)GetValue(HeaderBackgroundOpacityProperty); }
            set { SetValue(HeaderBackgroundOpacityProperty, value); }
        }

        public static readonly DependencyProperty ShowMaskProperty = DependencyProperty.Register(
            "ShowMask", typeof(bool), typeof(KWindow), new PropertyMetadata(false));

        public bool ShowMask
        {
            get { return (bool)GetValue(ShowMaskProperty); }
            set { SetValue(ShowMaskProperty, value); }
        }

        public static readonly DependencyProperty MaskOpacityProperty = DependencyProperty.Register(
            "MaskOpacity", typeof(double), typeof(KWindow), new PropertyMetadata(0.4D));

        public double MaskOpacity
        {
            get { return (double)GetValue(MaskOpacityProperty); }
            set { SetValue(MaskOpacityProperty, value); }
        }

        #endregion

        /// <summary> 窗口改变大小消息 </summary>
        private const int WM_SIZEING = 0x0214;
        private const int WM_MAX = 0x0024;

        /// <summary> 是否可拖动 </summary>
        private readonly bool _dragable;

        private readonly string _defaultStyle;

        /// <summary> 弹窗数量 </summary>
        public int DialogCount { get; set; }

        /// <summary> 构造函数 </summary>
        /// <param name="dragable">是否可拖动</param>
        /// <param name="style">样式</param>
        /// <param name="hook"></param>
        /// <param name="enableShortKey">启用快捷键</param>
        public KWindow(bool dragable = true, string style = "K-Window", bool hook = true, bool enableShortKey = true)
        {
            _dragable = dragable;
            _defaultStyle = style;

            InitializeTheme();
            InitializeStyle();

            this.BindCommand(SystemCommands.CloseWindowCommand, (s, e) => { SystemCommands.CloseWindow(this); });
            this.BindCommand(ApplicationCommands.Close, (s, e) => { SystemCommands.CloseWindow(this); });
            this.BindCommand(SystemCommands.MinimizeWindowCommand, (s, e) =>
            {
                WindowState = WindowState.Minimized;
                e.Handled = true;
            });
            var mg = Margin;
            var corner = AttachProperty.GetCornerRadius(this);

            this.BindCommand(SystemCommands.MaximizeWindowCommand, (s, e) =>
            {
                if (WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(this);
                    Margin = mg;
                    AttachProperty.SetCornerRadius(this, corner);
                    WindowState = WindowState.Normal;
                }
                else
                {
                    SystemCommands.MaximizeWindow(this);
                    Margin = new Thickness(0);
                    AttachProperty.SetCornerRadius(this, new CornerRadius(0));
                    WindowState = WindowState.Maximized;
                }
                e.Handled = true;
            });

            //窗体加载完成事件
            Loaded += (sender, args) =>
            {
            };

            if (enableShortKey)
            {
                //exc退出全屏
                KeyUp += (s, e) =>
                {
                    if (WindowState == WindowState.Maximized && (e.Key == Key.F11 || e.Key == Key.Escape))
                    {
                        SystemCommands.RestoreWindow(this);
                        Margin = mg;
                        AttachProperty.SetCornerRadius(this, corner);
                        WindowState = WindowState.Normal;
                    }
                    else if (WindowState == WindowState.Normal && e.Key == Key.F11)
                    {
                        SystemCommands.MaximizeWindow(this);
                        Margin = new Thickness(0);
                        AttachProperty.SetCornerRadius(this, new CornerRadius(0));
                        WindowState = WindowState.Maximized;
                    }
                };
            }

            Unloaded += (sender, args) =>
            {
                (DataContext as DViewModel)?.Cleanup();
            };
            //资源初始化事件
            SourceInitialized += (sender, args) =>
            {
                if (!hook) return;
                //重绘窗体大小
                var handle = new WindowInteropHelper(this).Handle;
                var source = HwndSource.FromHwnd(handle);
                source?.AddHook(WindowProc);
            };
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SIZEING:
                    //窗口大小改变
                    //MessageBox.Show(msg.ToString());
                    break;
                case WM_MAX:
                    WmGetMinMaxInfo(hwnd, lparam);
                    handled = true;
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary> 初始化样式 </summary>
        private void InitializeStyle()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!string.IsNullOrWhiteSpace(_defaultStyle))
            {
                Style = FindResource(_defaultStyle) as Style;
            }
        }

        /// <summary> 初始化模版 </summary>
        private void InitializeTheme()
        {
            //var resource = Application.LoadComponent(new Uri("Singer.Client;component/Resources/Singer.xaml",
            //    UriKind.Relative)) as ResourceDictionary;
            //Application.Current.Resources.MergedDictionaries.Add(resource);
            if (_dragable)
            {
                MouseLeftButtonDown += (sender, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                    }
                };
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            const int monitorDefaulttonearest = 0x00000002;
            var monitor = MonitorFromWindow(hwnd, monitorDefaulttonearest);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new Monitorinfo();
                GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcMonitorArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcMonitorArea.bottom - rcWorkArea.top);
                //最大化不覆盖菜单栏
                //mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                //mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, Monitorinfo lpmi);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #region Nested type: MINMAXINFO

        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
        #endregion

        #region Nested type: MONITORINFO
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class Monitorinfo
        {
            public int cbSize = Marshal.SizeOf(typeof(Monitorinfo));
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }
        #endregion

        #region Nested type: POINT
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        #endregion

        #region Nested type: RECT
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static readonly RECT Empty;

            public int Width => Math.Abs(right - left);

            public int Height => bottom - top;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty => left >= right || top >= bottom;

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }
        #endregion
    }
}
