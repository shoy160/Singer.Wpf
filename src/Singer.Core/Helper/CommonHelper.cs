using System;
using System.Linq;

namespace Singer.Core.Helper
{
    public static class CommonHelper
    {
        /// <summary> 获取Guid </summary>
        /// <returns></returns>
        public static string Guid32 => Guid.NewGuid().ToString();

        /// <summary>
        /// 16位Guid
        /// </summary>
        /// <returns></returns>
        public static string Guid16
        {
            get
            {
                var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
                return $"{i - DateTime.Now.Ticks:x}";
            }
        }

        /// <summary>
        /// 获取对象指定属性值
        /// </summary>
        /// <param name="info">对象</param>
        /// <param name="field">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(object info, string field)
        {
            if (info == null)
            {
                return null;
            }
            var t = info.GetType();
            var property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            return property.FirstOrDefault()?.GetValue(info, null);
        }
    }
}
