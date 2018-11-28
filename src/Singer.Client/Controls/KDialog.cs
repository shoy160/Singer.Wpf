using Singer.Core.Messaging;
using System;
using System.ComponentModel;
using System.Windows;

namespace Singer.Client.Controls
{
    public class KDialog : KWindow
    {
        private bool _dialogResult;
        public KDialog() : base(true, "K-Dialog")
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            MaxHeight = SystemParameters.WorkArea.Height - 10;
            Messenger.Default.Register<bool>(this, result =>
            {
                _dialogResult = result;
                Close();
            }, GlobalKeys.MtCloseDialog);
        }

        public new bool? Show(Window owner = null)
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

        public static void CloseDialog(bool result = false)
        {
            Messenger.Default.Notify<bool, KDialog>(result, GlobalKeys.MtCloseDialog);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsActive)
                DialogResult = _dialogResult;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            var win = Owner as KWindow;
            if (win != null)
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
