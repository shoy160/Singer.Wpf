using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace Singer.Client.Controls
{
    public class KWebBrowser
    {
        private readonly Window _owner;
        private readonly FrameworkElement _placementTarget;
        private readonly Form _form;
        private DispatcherOperation _dispatcherOperation;

        public WebBrowser WebBrowser { get; }

        /// <summary> 自定义WebBrowser </summary>
        /// <param name="placementTarget"></param>
        public KWebBrowser(FrameworkElement placementTarget)
        {
            _placementTarget = placementTarget;
            _owner = Window.GetWindow(placementTarget) ?? Application.Current.MainWindow;
            if (_owner == null) return;
            _form = new Form();
            WebBrowser = new WebBrowser
            {
                ScrollBarsEnabled = false,
                ScriptErrorsSuppressed = false
            };
            _form.Opacity = _owner.Opacity;
            _form.ShowInTaskbar = false;
            _form.KeyPreview = true;
            _form.FormBorderStyle = FormBorderStyle.None;
            WebBrowser.Dock = DockStyle.Fill;
            _form.Width = (int)placementTarget.ActualWidth;
            _form.Height = (int)placementTarget.ActualHeight;
            _form.Controls.Add(WebBrowser);
            _owner.LocationChanged += (sender, e) => SizeChange();
            _placementTarget.SizeChanged += (sender, e) => SizeChange();
            WebBrowser.DocumentCompleted += DocumentCompleted;
            WebBrowser.IsWebBrowserContextMenuEnabled = false;
            if (_owner.IsVisible)
                SourceInit();
            else
                _owner.SourceInitialized += (sender, e) => SourceInit();
            DependencyPropertyDescriptor.FromProperty(UIElement.OpacityProperty, typeof(Window))
                .AddValueChanged(_owner, (param0, param1) => _form.Opacity = _owner.Opacity);
            _form.KeyDown += FormKeyDown;
            placementTarget.Unloaded += (sender, e) =>
            {
                Close();
            };
        }

        public void Close()
        {
            WebBrowser.Dispose();
            _form.Close();
        }

        private static void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F4 || e.Modifiers != Keys.Alt)
                return;
            e.Handled = true;
        }

        private void SizeChange()
        {
            if (_dispatcherOperation != null) return;
            _dispatcherOperation = _owner.Dispatcher.BeginInvoke(new Action(Reposition));
        }

        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (WebBrowser.Document == null || WebBrowser.Document.Window == null)
                return;
            WebBrowser.Document.Window.Error += DocumentError;
        }

        private static void DocumentError(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private void SourceInit()
        {
            var nativeWindow = new NativeWindow();
            var fromVisual = (HwndSource)PresentationSource.FromVisual(_owner);
            if (fromVisual != null)
                nativeWindow.AssignHandle(fromVisual.Handle);
            _form.Show(nativeWindow);
            nativeWindow.ReleaseHandle();
        }

        private void Reposition()
        {
            _dispatcherOperation = null;
            var offset = _placementTarget.TranslatePoint(new Point(), _owner);
            var size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            var hwndSource = (HwndSource)PresentationSource.FromVisual(_owner);
            CompositionTarget ct = hwndSource?.CompositionTarget;
            if (ct != null)
            {
                offset = ct.TransformToDevice.Transform(offset);
                size = ct.TransformToDevice.Transform(size);
            }

            var screenLocation = new Win32.POINT(offset);
            if (hwndSource != null)
                Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
            var screenSize = new Win32.POINT(size);
            Win32.MoveWindow(_form.Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
        }
    }

    internal static class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public POINT(Point pt)
            {
                X = Convert.ToInt32(pt.X);
                Y = Convert.ToInt32(pt.Y);
            }
        };

        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    }
}
