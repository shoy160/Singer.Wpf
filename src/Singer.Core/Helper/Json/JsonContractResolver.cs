using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Singer.Core.Helper.Json
{
    internal class JsonContractResolver : DefaultContractResolver
    {
        private readonly string[] _props;
        private readonly bool _retain;

        /// <summary> 构造函数 </summary>
        /// <param name="retain">保留/排除：true为保留</param>
        /// <param name="props"></param>
        public JsonContractResolver(bool retain = true,
            params string[] props)
        {
            _retain = retain;
            _props = props;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var propList = base.CreateProperties(type, memberSerialization);
            if (_props == null || _props.Length == 0)
                return propList;
            return
                propList.Where(
                    p => _retain
                        ? _props.Contains(p.PropertyName)
                        : !_props.Contains(p.PropertyName))
                    .ToList();
        }
    }
}
