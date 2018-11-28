using Singer.Client.Commands;
using Singer.Sample.ViewModels;

namespace ESignature.Client.Views.Pages
{
    /// <summary>
    /// SystemUpgrade.xaml 的交互逻辑
    /// </summary>
    public partial class SystemUpgrade
    {
        public SystemUpgrade()
        {
            InitializeComponent();
            new VSystemUpgrade().Bind(this);
        }
    }
}
