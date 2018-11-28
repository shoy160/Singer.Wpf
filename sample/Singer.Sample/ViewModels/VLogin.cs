using System;
using System.Windows.Controls;
using System.Windows.Input;
using Singer.Client.Commands;
using Singer.Client.Controls;
using Singer.Sample.Dtos;
using Singer.Sample.Helper;
using Singer.Sample.Views;

namespace Singer.Sample.ViewModels
{
    /// <summary> 登录 </summary>
    public class VLogin : VBase
    {
        public ICommand LoginCommand { get; }

        private string _account;

        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                OnPropertyChanged(() => Account);
            }
        }

        private string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(() => Password);
            }
        }

        public VLogin(Action closeAction)
        {
            LoginCommand = new RelayCommand(() =>
            {
                var result = RestHelper.Instance.Login(Account, Password);
                if (result.Status)
                {
                    App.CurrentUser = new UserInfoDto
                    {
                        Id = "123",
                        Name = "shay"
                    };
                    closeAction?.Invoke();
                }
                else
                {
                    KMessageBox.Alert(result.Message);
                }
            }, () =>
            {
                var ele = Element as LoginView;
                return !string.IsNullOrWhiteSpace(ele?.Account.Text) && !Validation.GetHasError(ele.Account) &&
                       !string.IsNullOrWhiteSpace(ele.Password.Password) && !Validation.GetHasError(ele.Password);
            });
        }
    }
}
