using Singer.Client.Commands;
using System;
using System.Windows;

namespace Singer.Client.Controls
{
    public class VLoading : DViewModel
    {
        private string _text = "正在加载...";
        private string _icon = "/Singer.Client;component/Resources/Images/loading.gif";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(() => Icon);
            }
        }
    }

    /// <summary> 加载数据插件 </summary>
    public partial class KLoading
    {
        private readonly Action _toDoAction;
        public VLoading Model { get; }

        private KLoading(Action toDoAction)
        {
            InitializeComponent();
            Model = new VLoading();
            Model.Bind(this);
            _toDoAction = toDoAction;
            Loaded += LoadingLoaded;
        }

        private void LoadingLoaded(object sender, RoutedEventArgs e)
        {
            _toDoAction.BeginInvoke(OnComplate, null);
        }

        private void OnComplate(IAsyncResult ar)
        {
            Dispatcher?.Invoke(new Action(Close));
        }

        /// <summary> 展示进度条 </summary>
        /// <param name="toDoAction"></param>
        /// <param name="msg"></param>
        /// <param name="icon"></param>
        /// <param name="customIcon"></param>
        /// <param name="owner"></param>
        public static void Show(Action toDoAction, string msg = null, int icon = 0, string customIcon = null, Window owner = null)
        {
            var loading = new KLoading(toDoAction);
            if (!string.IsNullOrWhiteSpace(msg))
                loading.Model.Text = msg;
            if (!string.IsNullOrWhiteSpace(customIcon))
                loading.Model.Icon = customIcon;
            else if (icon > 0)
            {
                var number = icon % 8;
                var suf = number > 0 ? $"_{number.ToString().PadLeft(2, '0')}" : string.Empty;
                loading.Model.Icon =
                    $"/Singer.Client;component/Resources/Images/loading{suf}.gif";
            }
            loading.Show(owner);
        }
    }
}
