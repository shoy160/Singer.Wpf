using Singer.Client.Commands;
using System.Windows;
using System.Windows.Controls;

namespace Singer.Client.Controls
{
    /// <summary> Page页基础类 </summary>
    public class KPage : Page
    {
        protected DViewModel<FrameworkElement> Model { get; private set; }
        public KPage()
        {
            Unloaded += (sender, e) =>
            {
                Model?.Cleanup();
            };
            Loaded += (sender, e) => { Model = Model ?? DataContext as DViewModel<FrameworkElement>; };
        }

        public KPage(DViewModel<FrameworkElement> model) : this()
        {
            Model = model;
            Model.Bind(this);
        }
    }
}
