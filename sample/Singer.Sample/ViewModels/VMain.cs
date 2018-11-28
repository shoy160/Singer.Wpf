using ESignature.Business;
using Singer.Client.Commands;
using Singer.Client.Controls;
using Singer.Core;
using Singer.Core.Helper;
using Singer.Sample.AppService;
using Singer.Sample.Commands;
using Singer.Sample.Dtos;
using Singer.Sample.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Singer.Sample.ViewModels
{

    public class VMain : VBase
    {
        private ObservableCollection<MenuDto> _menus;

        public ObservableCollection<MenuDto> Menus
        {
            get { return _menus; }
            set
            {
                _menus = value;
                OnPropertyChanged(() => Menus);
            }
        }

        #region 命令
        /// <summary> 返回命令 </summary>
        public ICommand GoBackCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region 依赖属性

        private MenuDto _selectedItem;
        public MenuDto SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
                ClientCommands.LoadPageCommand.ExecuteCommand(value.Url);
            }
        }

        private DispatcherTimer _timer;

        private bool _isConnect;

        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                _isConnect = value;
                OnPropertyChanged(() => IsConnect);
                OnPropertyChanged(() => ConnectionState);
            }
        }

        private DateTime _now;

        public DateTime TimeNow
        {
            get { return _now; }
            set
            {
                _now = value;
                OnPropertyChanged(() => TimeNow);
                OnPropertyChanged(() => TooltipTime);
            }
        }

        #endregion

        public string TooltipTime => $"{_now:yyyy年MM月dd日 HH时mm分ss秒}";
        public string ConnectionState => _isConnect ? "网络质量好" : "网络已断开";

        public UserInfoDto User => App.CurrentUser;

        private bool _isGoback;

        /// <summary> 是否能返回列表 </summary>
        public bool IsGoback
        {
            get { return _isGoback; }
            set
            {
                _isGoback = value;
                OnPropertyChanged(() => IsGoback);
            }
        }

        public VMain()
        {
            IsConnect = Constants.IsConnected();


            InitMenusStatue();

            TimeNow = DateTime.Now;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (sender, e) =>
            {
                TimeNow = DateTime.Now;
                if (TimeNow.Second % 20 == 0 && IsConnect != Constants.IsConnected())
                {
                    IsConnect = Constants.IsConnected();
                    if (!IsConnect)
                    {
                        UiInvoke(() =>
                        {
                            KMessageBox.Alert("网络已断开，自动切换至离线模式");
                        });
                    }
                }
            };
            _timer.Start();

            GoBackCommand = new RelayCommand(() =>
            {
            });
            LogoutCommand = new RelayCommand(() =>
            {
                RestHelper.Instance.Logout();
                new GlobalDataService().Delete(GlobalKeys.AccessToken);
                ClientCommands.RestartApp();
            });
        }

        private void InitMenusStatue()
        {
            var list = ConfigHelper.ReadList<MenuDto>("menus"); // GlobalDataService.Instance.Query<List<MenuDto>>(GlobalKeys.Menus).ToList();
            list.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.Image) && !t.Image.StartsWith("/"))
                    t.Image = $"/Contents/Icons/{t.Image}";
                t.Enabled = true;
            });
            Menus = new ObservableCollection<MenuDto>(list);
        }

        public void SelectMenu(int id = 1)
        {
            if (Menus == null || !Menus.Any())
                return;
            SelectedItem = Menus.FirstOrDefault(t => t.Id == id) ?? Menus.FirstOrDefault();
        }

        public override void Cleanup()
        {
            base.Cleanup();
            _timer.Stop();
            _timer = null;
        }
    }
}
