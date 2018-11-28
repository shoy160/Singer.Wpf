using System;

namespace Singer.Client.Controls
{
    /// <summary> 进度条插件 </summary>
    public partial class KProcess
    {
        private string _message;
        private KProcess(Action<KProcess> action, string msg = null)
        {
            InitializeComponent();
            _message = msg;
            Loaded += (sender, e) =>
            {
                TxtProcess.Text = msg;
                action.BeginInvoke(this, OnComplate, null);
            };
        }
        private void OnComplate(IAsyncResult ar)
        {
            Dispatcher.Invoke(new Action(Close));
        }

        public void Change(int percent, string msg = null)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    _message = msg;
                }

                TxtProcess.Text = _message;
                RectProcess.Width = ((float)percent / 100) * BProcess.ActualWidth;
            }));
        }

        public static void Start(Action<KProcess> action, string msg = null)
        {
            var dialog = new KProcess(action, msg);
            dialog.Show();
        }
    }
}
