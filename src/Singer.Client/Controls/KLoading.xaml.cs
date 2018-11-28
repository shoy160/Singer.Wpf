using System;
using System.Windows;

namespace Singer.Client.Controls
{
    /// <summary> 加载数据插件 </summary>
    public partial class KLoading
    {
        public string Text
        {
            get { return TxtMessage.Text; }
            set { TxtMessage.Text = value; }
        }

        private readonly Action _toDoAction;

        private KLoading(Action toDoAction)
        {
            InitializeComponent();
            _toDoAction = toDoAction;
            Loaded += LoadingLoaded;
        }

        private void LoadingLoaded(object sender, RoutedEventArgs e)
        {
            _toDoAction.BeginInvoke(OnComplate, null);
        }

        private void OnComplate(IAsyncResult ar)
        {
            Dispatcher.Invoke(new Action(Close));
        }

        public static void Show(Action toDoAction, string msg = "加载中...")
        {
            var loading = new KLoading(toDoAction) { Text = msg };
            loading.Show();
        }
    }
}
