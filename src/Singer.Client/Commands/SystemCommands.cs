using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Singer.Client.Controls;

namespace Singer.Client.Commands
{
    #region 系统枚举
    internal enum SC
    {
        F_ISSECURE = 1,
        SIZE = 61440,
        SEPARATOR = 61455,
        MOVE = 61456,
        ICON = 61472,
        MINIMIZE = 61472,
        MAXIMIZE = 61488,
        ZOOM = 61488,
        NEXTWINDOW = 61504,
        PREVWINDOW = 61520,
        CLOSE = 61536,
        VSCROLL = 61552,
        HSCROLL = 61568,
        MOUSEMENU = 61584,
        KEYMENU = 61696,
        ARRANGE = 61712,
        RESTORE = 61728,
        TASKLIST = 61744,
        SCREENSAVE = 61760,
        HOTKEY = 61776,
        DEFAULT = 61792,
        MONITORPOWER = 61808,
        CONTEXTHELP = 61824,
    }
    internal enum WM
    {
        NULL = 0,
        CREATE = 1,
        DESTROY = 2,
        MOVE = 3,
        SIZE = 5,
        ACTIVATE = 6,
        SETFOCUS = 7,
        KILLFOCUS = 8,
        ENABLE = 10,
        SETREDRAW = 11,
        SETTEXT = 12,
        GETTEXT = 13,
        GETTEXTLENGTH = 14,
        PAINT = 15,
        CLOSE = 16,
        QUERYENDSESSION = 17,
        QUIT = 18,
        QUERYOPEN = 19,
        ERASEBKGND = 20,
        SYSCOLORCHANGE = 21,
        SHOWWINDOW = 24,
        CTLCOLOR = 25,
        SETTINGCHANGE = 26,
        WININICHANGE = 26,
        ACTIVATEAPP = 28,
        SETCURSOR = 32,
        MOUSEACTIVATE = 33,
        CHILDACTIVATE = 34,
        QUEUESYNC = 35,
        GETMINMAXINFO = 36,
        WINDOWPOSCHANGING = 70,
        WINDOWPOSCHANGED = 71,
        CONTEXTMENU = 123,
        STYLECHANGING = 124,
        STYLECHANGED = 125,
        DISPLAYCHANGE = 126,
        GETICON = 127,
        SETICON = 128,
        NCCREATE = 129,
        NCDESTROY = 130,
        NCCALCSIZE = 131,
        NCHITTEST = 132,
        NCPAINT = 133,
        NCACTIVATE = 134,
        GETDLGCODE = 135,
        SYNCPAINT = 136,
        NCMOUSEMOVE = 160,
        NCLBUTTONDOWN = 161,
        NCLBUTTONUP = 162,
        NCLBUTTONDBLCLK = 163,
        NCRBUTTONDOWN = 164,
        NCRBUTTONUP = 165,
        NCRBUTTONDBLCLK = 166,
        NCMBUTTONDOWN = 167,
        NCMBUTTONUP = 168,
        NCMBUTTONDBLCLK = 169,
        SYSKEYDOWN = 260,
        SYSKEYUP = 261,
        SYSCHAR = 262,
        SYSDEADCHAR = 263,
        COMMAND = 273,
        SYSCOMMAND = 274,
        MOUSEMOVE = 512,
        LBUTTONDOWN = 513,
        LBUTTONUP = 514,
        LBUTTONDBLCLK = 515,
        RBUTTONDOWN = 516,
        RBUTTONUP = 517,
        RBUTTONDBLCLK = 518,
        MBUTTONDOWN = 519,
        MBUTTONUP = 520,
        MBUTTONDBLCLK = 521,
        MOUSEWHEEL = 522,
        XBUTTONDOWN = 523,
        XBUTTONUP = 524,
        XBUTTONDBLCLK = 525,
        MOUSEHWHEEL = 526,
        PARENTNOTIFY = 528,
        CAPTURECHANGED = 533,
        POWERBROADCAST = 536,
        DEVICECHANGE = 537,
        ENTERSIZEMOVE = 561,
        EXITSIZEMOVE = 562,
        IME_SETCONTEXT = 641,
        IME_NOTIFY = 642,
        IME_CONTROL = 643,
        IME_COMPOSITIONFULL = 644,
        IME_SELECT = 645,
        IME_CHAR = 646,
        IME_REQUEST = 648,
        IME_KEYDOWN = 656,
        IME_KEYUP = 657,
        NCMOUSELEAVE = 674,
        TABLET_DEFBASE = 704,
        TABLET_ADDED = 712,
        TABLET_DELETED = 713,
        TABLET_FLICK = 715,
        TABLET_QUERYSYSTEMGESTURESTATUS = 716,
        CUT = 768,
        COPY = 769,
        PASTE = 770,
        CLEAR = 771,
        UNDO = 772,
        RENDERFORMAT = 773,
        RENDERALLFORMATS = 774,
        DESTROYCLIPBOARD = 775,
        DRAWCLIPBOARD = 776,
        PAINTCLIPBOARD = 777,
        VSCROLLCLIPBOARD = 778,
        SIZECLIPBOARD = 779,
        ASKCBFORMATNAME = 780,
        CHANGECBCHAIN = 781,
        HSCROLLCLIPBOARD = 782,
        QUERYNEWPALETTE = 783,
        PALETTEISCHANGING = 784,
        PALETTECHANGED = 785,
        HOTKEY = 786,
        PRINT = 791,
        PRINTCLIENT = 792,
        APPCOMMAND = 793,
        THEMECHANGED = 794,
        DWMCOMPOSITIONCHANGED = 798,
        DWMNCRENDERINGCHANGED = 799,
        DWMCOLORIZATIONCOLORCHANGED = 800,
        DWMWINDOWMAXIMIZEDCHANGE = 801,
        DWMSENDICONICTHUMBNAIL = 803,
        DWMSENDICONICLIVEPREVIEWBITMAP = 806,
        GETTITLEBARINFOEX = 831,
        USER = 1024,
        TRAYMOUSEMESSAGE = 2048,
        APP = 32768,
    }
    #endregion
    public static class SystemCommands
    {
        /// <summary> 关闭窗体命令 </summary>
        public static RoutedCommand CloseWindowCommand { get; }
        /// <summary> 最大化命令 </summary>
        public static RoutedCommand MaximizeWindowCommand { get; }
        /// <summary> 最小化命令 </summary>
        public static RoutedCommand MinimizeWindowCommand { get; }
        /// <summary> 还原窗体命令 </summary>
        public static RoutedCommand RestoreWindowCommand { get; }
        

        //public static RoutedCommand ShowSystemMenuCommand { get; }

        static SystemCommands()
        {
            CloseWindowCommand = new RoutedCommand("CloseWindow", typeof(SystemCommands));
            MaximizeWindowCommand = new RoutedCommand("MaximizeWindow", typeof(SystemCommands));
            MinimizeWindowCommand = new RoutedCommand("MinimizeWindow", typeof(SystemCommands));
            RestoreWindowCommand = new RoutedCommand("RestoreWindow", typeof(SystemCommands));
        }
        internal static void RestartApp()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        private static void _PostSystemCommand(Window window, SC command)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero || !NativeHelper.IsWindow(handle))
                return;
            PostMessage(handle, WM.SYSCOMMAND, new IntPtr((int)command), IntPtr.Zero);
        }

        public static void CloseWindow(Window window)
        {
            _PostSystemCommand(window, SC.CLOSE);
        }

        public static void MaximizeWindow(Window window)
        {
            _PostSystemCommand(window, SC.MAXIMIZE);
        }

        public static void MinimizeWindow(Window window)
        {
            _PostSystemCommand(window, SC.MINIMIZE);
        }

        public static void RestoreWindow(Window window)
        {
            _PostSystemCommand(window, SC.RESTORE);
        }

        //public static void ShowSystemMenu(Window window, Point screenLocation)
        //{
        //    ShowSystemMenuPhysicalCoordinates(window, DpiHelper.LogicalPixelsToDevice(screenLocation));
        //}

        //internal static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        //{
        //    IntPtr handle = new WindowInteropHelper(window).Handle;
        //    if (handle == IntPtr.Zero || !NativeHelper.IsWindow(handle))
        //        return;
        //    uint num = NativeHelper.TrackPopupMenuEx(NativeHelper.GetSystemMenu(handle, false), 256U, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, handle, IntPtr.Zero);
        //    if ((int)num == 0)
        //        return;
        //    NativeHelper.PostMessage(handle, WM.SYSCOMMAND, new IntPtr((long)num), IntPtr.Zero);
        //}

        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        private static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
        {
            if (!_PostMessage(hWnd, Msg, wParam, lParam))
                throw new Win32Exception();
        }

        internal static void OpenWindow(Window win)
        {
            var t = Application.Current.MainWindow;
            win.WindowState = t.WindowState;
            Application.Current.MainWindow = win;
            win.Show();
            t.Close();
        }
    }
}
