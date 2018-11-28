using Singer.Client.Controls;
using Singer.Core;
using Singer.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace Singer.Client
{
    public abstract class DApplication : Application
    {
        private readonly ILogger _logger = LogManager.Logger<DApplication>();

        [DllImport("user32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public string CallExeName => Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

        protected bool Boostrap()
        {
            var currentProcess = Process.GetCurrentProcess();
            var process = Process.GetProcesses()
                .FirstOrDefault(t => t.ProcessName == CallExeName && t.Id != currentProcess.Id);
            if (process != null)
            {
                //选中当前的句柄窗口
                SetForegroundWindow(process.MainWindowHandle);
                Current.Shutdown();
                return false;
            }
            //全局异常处理
            DispatcherUnhandledException += (sender, ev) =>
            {
                var exception = ev.Exception;
                if (exception is BusiException)
                {
                    KMessageBox.Alert(exception.Message);
                    ev.Handled = true;
                    return;
                }
                _logger.Error(ev.Exception.Message, ev.Exception);
#if DEBUG
                KMessageBox.Alert(ev.Exception.Message);
#endif
                ev.Handled = true;
            };
            return true;
        }
    }
}
