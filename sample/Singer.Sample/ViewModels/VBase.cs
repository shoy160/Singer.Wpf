using System.Windows;
using Singer.Client;
using Singer.Client.Controls;
using Singer.Core.Messaging;
using Singer.Sample.Commands;

namespace Singer.Sample.ViewModels
{
    public abstract class VBase : VBase<FrameworkElement>
    {
        protected VBase(IMessenger messenger = null) : base(messenger)
        {
        }
    }

    public abstract class VBase<T> : DViewModel<T> where T : FrameworkElement
    {
        protected VBase(IMessenger messenger = null) : base(messenger)
        {
        }

        protected void Jump(string uri, int menudId = 0)
        {
            ClientCommands.JumpPage(uri, menudId);
        }

        protected void Jump(KPage page, int menudId = 0)
        {
            ClientCommands.JumpPage(page, menudId);
        }

        protected void Refresh()
        {
            ClientCommands.Refresh();
        }
    }
}
