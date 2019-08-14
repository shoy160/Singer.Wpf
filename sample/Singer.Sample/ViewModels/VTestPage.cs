using System.Threading;
using System.Windows.Input;
using Singer.Client.Commands;
using Singer.Client.Controls;
using Singer.Sample.Views.Dialogs;

namespace Singer.Sample.ViewModels
{
    public class VTestPage : VBase
    {
        private int _total;

        public int Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged(() => Total);
            }
        }
        public ICommand LoadingCommand { get; }
        public ICommand DialogCommand { get; }
        private static int index = 0;
        public VTestPage()
        {
            Total = 200;
            LoadingCommand = new RelayCommand(() =>
            {
                KLoading.Show(() => { Thread.Sleep(2000); }, "努力加载中...", icon: index++);
            });
            DialogCommand = new RelayCommand(() =>
            {
                var result = new DemoDialog().Show();
                if (result.HasValue && result.Value)
                {
                    UiInvoke(() => KMessageBox.Alert("Success"));
                }
            });
        }
    }
}
