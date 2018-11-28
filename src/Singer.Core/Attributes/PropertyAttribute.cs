using System;

namespace Singer.Core.Attributes
{
    public class KeyAttribute : Attribute { }

    public class RequireAttribute : Attribute { }

    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value = null)
        {
            Value = value;
        }
    }
}
