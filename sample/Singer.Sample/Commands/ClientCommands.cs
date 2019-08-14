using ESignature.Business;
using Singer.Client.Commands;
using Singer.Client.Controls;
using Singer.Core.Helper;
using Singer.Sample.AppService;
using Singer.Sample.Helper;
using Singer.Sample.ViewModels;
using Singer.Sample.Views;
using Singer.Update;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Singer.Sample.Commands
{
    public static class ClientCommands
    {
        /// <summary> 打开页面命令 </summary>
        public static RelayCommand<string> LoadPageCommand { get; }
        /// <summary> 打开url链接命令 </summary>
        public static RelayCommand<string> OpenUrlCommand { get; }
        /// <summary> 版本检测命令 </summary>
        public static RelayCommand CheckVersionCommand { get; }

        /// <summary> 复制命令 </summary>
        public static RelayCommand<string> CopyCommand { get; }

        //public static RoutedCommand ShowSystemMenuCommand { get; }

        static ClientCommands()
        {
            //ShowSystemMenuCommand = new RoutedCommand("ShowSystemMenu", typeof(SystemCommands));
            LoadPageCommand = new RelayCommand<string>(uri =>
            {
                JumpPage(uri);
            }, uri => Application.Current.MainWindow is MainView);
            OpenUrlCommand = new RelayCommand<string>(url =>
            {
                var token = new GlobalDataService().Query<TokenResult>(GlobalKeys.AccessToken);
                var host = ConfigHelper.Read<string>("web_host");
                var uri = $"{host}#/redirect?token={token?.access_token}&redirect={url}";
                Process.Start(uri);
            }, url => !string.IsNullOrWhiteSpace(url));

            CheckVersionCommand = new RelayCommand(() =>
            {
                try
                {
                    var dto = RestHelper.Instance.GetVersion();
                    if (dto == null || Const.CurrentVersion >= new Version(dto.Version))
                        return;
                    App.HasUpgrade = true;

                    Updater.Instance.StartUpdate(dto.DownloadUrl, dto.Version, dto.UpgradeInstructions, dto.Md5);
                    if (dto.Mandatory)
                    {
                        Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
                    }
                }
                catch (Exception ex)
                {
                    Const.DefaultLogger.Error(ex.Message, ex);
                }
            });

            CopyCommand = new RelayCommand<string>(text =>
            {
                Clipboard.SetDataObject(text);
                KMessageBox.Alert("复制成功");
            }, text => !string.IsNullOrWhiteSpace(text));
        }

        internal static void RestartApp()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        /// <summary> 跳转Page </summary>
        /// <param name="uri"></param>
        /// <param name="menudId"></param>
        internal static void JumpPage(string uri, int menudId = 0)
        {
            if (!(Application.Current.MainWindow is MainView main))
                return;
            if (menudId > 0)
            {
                var model = main.DataContext as VMain;
                model?.SelectMenu(menudId);
            }
            main.PageContext.Navigate(new Uri($"Views/Pages/{uri}", UriKind.Relative));
        }

        /// <summary> 跳转Page </summary>
        /// <param name="page"></param>
        /// <param name="menudId"></param>
        internal static void JumpPage(KPage page, int menudId = 0)
        {
            if (!(Application.Current.MainWindow is MainView main))
                return;
            if (menudId > 0)
            {
                var model = main.DataContext as VMain;
                model?.SelectMenu(menudId);
            }
            main.PageContext.Navigate(page);
        }

        internal static void Refresh()
        {
            if (!(Application.Current.MainWindow is MainView main))
                return;
            main.PageContext.Refresh();
        }
    }
}
