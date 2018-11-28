using Singer.Update.UI;
using System;
using System.Windows;

namespace Singer.Update
{
    internal class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
#if DEBUG
                var app = new App();
                var downUi = new DownFileProcess("Singer.Update",
                    "http://file.dayeasy.net/update/markingTool/MarkingTool_0.6.29.exe", "1.0.0",
                    "1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能；1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能。1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能。",
                    Guid.NewGuid().ToString("N"))
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                app.Run(downUi);
#endif
                return;
            }
            if (args[0] != "update" || args.Length != 6)
                return;
            try
            {
                for (var i = 1; i < args.Length; i++)
                {
                    args[i] = Updater.Base64(args[i]);
                }
                var app = new App();
                var downUi = new DownFileProcess(args[1], args[2], args[3], args[4], args[5])
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.Run(downUi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
