using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Singer.Core
{
    public class KNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var exp = propertyExpression.Body as MemberExpression;
            if (exp == null)
                return;
            var propertyName = exp.Member.Name;
            OnPropertyChanged(propertyName);
        }
    }
}
