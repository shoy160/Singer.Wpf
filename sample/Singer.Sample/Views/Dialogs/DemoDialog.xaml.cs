using System.Threading;
using System.Windows.Input;
using Singer.Client;
using Singer.Client.Commands;
using Singer.Client.Controls;

namespace Singer.Sample.Views.Dialogs
{
    /// <summary>
    /// DemoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DemoDialog : KDialog
    {

        public class VDemoDialog : DViewModel
        {
            public ICommand LoadCommand { get; }

            public VDemoDialog()
            {
                LoadCommand = new RelayCommand(() =>
                {
                    KLoading.Show(() =>
                    {
                        Thread.Sleep(3000);
                    }, owner: Element as DemoDialog);
                    KDialog.CloseDialog<DemoDialog>(true);
                });
            }
        }
        public DemoDialog()
        {
            InitializeComponent();
            new VDemoDialog().Bind(this);
        }
    }
}
