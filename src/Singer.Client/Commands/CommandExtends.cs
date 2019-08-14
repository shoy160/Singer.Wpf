using System;
using System.Windows;
using System.Windows.Input;

namespace Singer.Client.Commands
{
    public static class CommandExtends
    {
        /// <summary>
        /// 绑定命令和命令事件到宿主UI
        /// </summary>
        public static void BindCommand(this UIElement ui, ICommand com, Action<object, ExecutedRoutedEventArgs> call)
        {
            var bind = new CommandBinding(com);
            bind.Executed += new ExecutedRoutedEventHandler(call);
            ui.CommandBindings.Add(bind);
        }

        public static void Bind<T>(this DViewModel<T> model, T element)
            where T : FrameworkElement
        {
            model.Element = element;
            element.DataContext = model;
            model.OnBinded();
        }
    }
}
