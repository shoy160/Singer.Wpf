using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Singer.Core.Helper.Json
{
    public class TimeStampDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(0);
            }
            else
            {
                long num = 0;
                if (value is DateTime)
                    num = ((DateTime)value).TimeStamp();
                else if (value is DateTimeOffset)
                    num = ((DateTimeOffset)value).DateTime.TimeStamp();
                writer.WriteValue(num);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (!(objectType == typeof(DateTime)) && !(objectType == typeof(DateTimeOffset)) &&
                (!(objectType == typeof(DateTime?)) && !(objectType == typeof(DateTimeOffset?))))
                throw new JsonSerializationException("不是日期格式 .");
            if (reader.Value == null)
                return objectType.IsNullableType() ? null : (object)0;
            var timestamp = (long)reader.Value;
            return timestamp.ToDateTime();
        }
    }
}
