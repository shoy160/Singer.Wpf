using Singer.Core.Messaging;
using System;
using System.ComponentModel;
using System.Windows;

namespace Singer.Client.Controls
{
    public class KDialog : KWindow
    {
        private bool? _dialogResult;
        public KDialog() : base(true, "K-Dialog")
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            MaxHeight = SystemParameters.WorkArea.Height - 10;
            Messenger.Default.Register<bool>(this, result =>
            {
                _dialogResult = result;
                Dispatcher?.Invoke(new Action(Close));
            }, GlobalKeys.MtCloseDialog);
        }

        /// <summary> 展示对话框 </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool? Show(Window owner = null)
        {
            try
            {
                var win = (owner ?? Application.Current.MainWindow) as KWindow;
                if (win == null || Equals(win, this))
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    return ShowDialog();
                }
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                win.DialogCount++;
                Owner = win;
                win.ShowMask = true;
                return ShowDialog();
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 关闭对话框 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public static void CloseDialog<T>(bool result = false) where T : KDialog
        {
            Messenger.Default.Notify<bool, T>(result, GlobalKeys.MtCloseDialog);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_dialogResult.HasValue)
                DialogResult = _dialogResult;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Owner is KWindow win)
            {
                win.DialogCount--;
                if (win.DialogCount <= 0)
                    win.ShowMask = false;
            }
            Messenger.Default.Unregister(this);
            base.OnClosed(e);
        }
    }
}
