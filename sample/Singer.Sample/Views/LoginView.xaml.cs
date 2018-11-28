using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Singer.Client.Commands;
using Singer.Client.Controls;
using Singer.Sample.ViewModels;

namespace Singer.Sample.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
            var model = new VLogin(() =>
            {
                DialogResult = true;
                Close();
            });
            model.Bind(this);

            void EnterKey(KeyEventArgs e)
            {
                if (e.Key != Key.Enter) return;
                var hasError = Validation.GetHasError(Account) || Validation.GetHasError(Password);
                if (hasError) return;
                model.LoginCommand.Execute(null);
            }

            Account.KeyUp += (sender, e) => EnterKey(e);
            Password.KeyUp += (sender, e) => EnterKey(e);
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is PasswordBox box))
                return;
            var pwd = AttachProperty.GetPassword(box);
            if (pwd != box.Password)
                AttachProperty.SetPassword(box, box.Password);
        }
    }
}
