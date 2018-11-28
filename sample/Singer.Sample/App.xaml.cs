using ESignature.Client.Views;
using Singer.Core.Helper;
using Singer.Sample.Commands;
using Singer.Sample.Dtos;
using Singer.Sample.Views;
using System;
using System.Threading;
using System.Windows;

namespace Singer.Sample
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        /// <summary> 当前用户 </summary>
        public static UserInfoDto CurrentUser;

        public static string UserId => CurrentUser?.Id;

        /// <summary> 是否有更新 </summary>
        public static bool HasUpgrade;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Boostrap())
                return;
#if DEBUG
            //CurrentUser = new UserInfoDto
            //{
            //    Id = "123",
            //    Name = "shay",
            //    LoginDate = DateTime.Now
            //};
#endif

            ConfigHelper.InitConfig(@"Contents/config/application.xml");
            //AutoCreateTableHelper.InitializeDb();
            ThreadPool.QueueUserWorkItem(CheckVersion);

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (CheckUser())
            {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                base.OnStartup(e);
                return;
            }
            var loginResult = new LoginView().ShowDialog();
            if (loginResult.HasValue && loginResult.Value)
            {
                if (!CheckUser())
                    return;
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                base.OnStartup(e);
            }
            else
            {
                Shutdown();
            }
        }

        private static void CheckVersion(object args)
        {
            ClientCommands.CheckVersionCommand.Execute(args);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigHelper.Dispose();
            base.OnExit(e);
        }

        private static bool CheckUser()
        {
            if (CurrentUser == null)
                return false;
            new MainView().Show();
            return true;
        }
    }
}
