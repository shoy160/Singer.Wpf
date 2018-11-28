using ESignature.Business;
using Singer.Client.Commands;
using Singer.Sample.Commands;
using Singer.Sample.Helper;
using System;
using System.Windows.Input;

namespace Singer.Sample.ViewModels
{
    public class VSystemUpgrade : VBase
    {
        public string Version => Const.CurrentVersion.ToString();
        private bool _isLatest;
        public bool IsLatest
        {
            get { return _isLatest; }
            set
            {
                _isLatest = value;
                OnPropertyChanged(() => IsLatest);
            }
        }

        private DateTime _upgradeTime;

        public DateTime UpgradeTime
        {
            get { return _upgradeTime; }
            set
            {
                _upgradeTime = value;
                OnPropertyChanged(() => UpgradeTime);
            }
        }
        private string _upgradeDesc;

        public string UpgradeDesc
        {
            get { return _upgradeDesc; }
            set
            {
                _upgradeDesc = value;
                OnPropertyChanged(() => UpgradeDesc);
            }
        }

        public ICommand UpgradeCommand { get; }

        public VSystemUpgrade()
        {
            var dto = RestHelper.Instance.GetVersion();
            UpgradeDesc = (dto?.UpgradeInstructions ?? string.Empty).TrimStart('\n');
            UpgradeTime = dto?.UpgradeTime ?? DateTime.Now;
            IsLatest = dto == null || Const.CurrentVersion >= new Version(dto.Version);
            App.HasUpgrade = !IsLatest;

            UpgradeCommand = new RelayCommand(() =>
            {
                ClientCommands.CheckVersionCommand.Execute(null);
            }, () => !IsLatest);
        }
    }
}
