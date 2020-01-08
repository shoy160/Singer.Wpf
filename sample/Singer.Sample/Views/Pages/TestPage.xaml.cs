using Singer.Client.Commands;
using Singer.Sample.ViewModels;

namespace Singer.Sample.Views.Pages
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage
    {
        public TestPage() : base(new VTestPage())
        {
            InitializeComponent();
            new VTestPage().Bind(this);
        }
    }
}
