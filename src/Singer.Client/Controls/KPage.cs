using System.Windows.Controls;

namespace Singer.Client.Controls
{
    /// <summary> Page页基础类 </summary>
    public class KPage : Page
    {
        public KPage()
        {
            Unloaded += (sender, e) =>
            {
                (DataContext as DViewModel)?.Cleanup();
            };
        }
    }
}
