using System.Windows;
using System.Windows.Input;
using Singer.Client.Commands;
using Singer.Sample.Commands;
using Singer.Sample.ViewModels;

namespace ESignature.Client.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        public MainView(int id = 1, string uri = null)
        {
            InitializeComponent();
            var model = new VMain();
            model.Bind(this);

            Loaded += (sender, e) =>
            {
                model.SelectMenu(id);
                if (!string.IsNullOrWhiteSpace(uri))
                    ClientCommands.JumpPage(uri);
            };
            UserModal.MouseLeftButtonDown += (sender, e) => { e.Handled = true; };
            KeyDown += (sender, e) =>
            {
                //if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.T)
                //{
                //    SystemCommands.JumpPage(new TestPage());
                //}
            };
        }

        private void UserAvatarClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
                return;
            if (UserModal.Visibility != Visibility.Visible)
            {
                UserModal.Visibility = Visibility.Visible;
                MouseLeftButtonDown += MouseLeftDown;
            }
            else
            {
                UserModal.Visibility = Visibility.Collapsed;
            }
        }

        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (Equals(e.OriginalSource, Avatar))
                return;
            UserModal.Visibility = Visibility.Collapsed;
            MouseLeftButtonDown -= MouseLeftDown;
        }
    }
}
