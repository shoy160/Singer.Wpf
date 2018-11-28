using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Singer.Core.Helper.Json
{
    /// <summary> Json辅助类，基于Json.Net </summary>
    public static class JsonHelper
    {
        private static JsonSerializerSettings LoadSetting(bool indented)
        {
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new JsonContractResolver()
            };
            if (indented)
                setting.Formatting = Formatting.Indented;
            setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            setting.DateFormatString = Constants.DateFormatString;
            setting.Converters.Add(new TimeStampDateTimeConverter());
            return setting;
        }

        private static JsonSerializerSettings LoadSetting(bool indented, string[] props,
            bool retain)
        {
            var setting = LoadSetting(indented);
            if (props != null && props.Length > 0)
                setting.ContractResolver = new JsonContractResolver(retain, props);
            return setting;
        }

        /// <summary> 序列化为json格式 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObj"></param>
        /// <param name="indented">是否缩进</param>
        /// <returns></returns>
        public static string ToJson<T>(T jsonObj, bool indented = false)
        {
            return JsonConvert.SerializeObject(jsonObj, LoadSetting(indented));
        }

        /// <summary> 序列化为json格式 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObj"></param>
        /// <param name="indented">是否缩进</param>
        /// <param name="retain">保留/排除</param>
        /// <param name="props">属性选择</param>
        /// <returns></returns>
        public static string ToJson<T>(T jsonObj, bool indented = false,
            bool retain = true, params string[] props)
        {
            return JsonConvert.SerializeObject(jsonObj, LoadSetting(indented, props, retain));
        }

        /// <summary> 将json发序列化为对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Json<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, LoadSetting(false));
        }

        /// <summary> 反序列化json为列表 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IEnumerable<T> JsonList<T>(string json)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StringReader(json))
            {
                using (var jsonReader = new JsonTextReader(sr))
                {
                    var obj = serializer.Deserialize(jsonReader, typeof(IEnumerable<T>));
                    return obj as IEnumerable<T>;
                }
            }
        }

        /// <summary> 反序列化到匿名对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T Json<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject, LoadSetting(false));
        }
    }
}
