using Singer.Client.Controls;
using Singer.Core;
using Singer.Core.Messaging;
using System;
using System.Windows;

namespace Singer.Client
{
    public abstract class DViewModel : DViewModel<FrameworkElement>
    {
        protected DViewModel(IMessenger messenger = null) : base(messenger)
        {
        }
    }

    public abstract class DViewModel<T> : KNotifyPropertyChanged where T : FrameworkElement
    {
        private IMessenger _messengerInstance;

        public T Element { get; set; }

        public IMessenger MessengerInstance
        {
            get => _messengerInstance ?? Messenger.Default;
            set => _messengerInstance = value;
        }

        protected DViewModel(IMessenger messenger = null)
        {
            if (messenger != null)
                MessengerInstance = messenger;
        }

        public void UiInvoke(Action action)
        {
            Element.Dispatcher.Invoke(action);
        }

        protected void ShowDialog<TD>()
            where TD : KDialog, new()
        {
            new TD().Show();
        }

        public virtual void Binded()
        {
        }

        public virtual void Cleanup()
        {
            MessengerInstance.Unregister(this);
        }
    }
}
