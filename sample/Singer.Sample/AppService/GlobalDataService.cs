using Dapper;
using ESignature.Business;
using Singer.Core;
using Singer.Core.Helper.Json;

namespace Singer.Sample.AppService
{
    /// <summary> 全局配置 </summary>
    public class GlobalDataService : SqliteService<GlobalDataService>
    {
        public GlobalDataService() : base(Const.GlobalDb)
        {
        }

        /// <summary> 查询键 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Query<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);
            try
            {
                var type = typeof(T);
                const string sql = "SELECT [value] FROM [global_data] WHERE [key]=@key";
                return UseConn(conn =>
                {
                    var value = conn.QueryFirstOrDefault<string>(sql, new { key });
                    return type.IsSimpleType() ? value.CastTo<T>() : JsonHelper.Json<T>(value ?? string.Empty);
                });
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary> 是否存在键 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            const string existsSql = "SELECT 1 FROM [global_data] WHERE [key]=@key LIMIT 0,1";
            return UseConn(conn =>
            {
                var t = conn.QueryFirstOrDefault<int?>(existsSql, new { key });
                return t.HasValue && t.Value == 1;
            });
        }

        /// <summary> 添加或更新 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void InsertOrUpdate(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            const string existsSql = "SELECT 1 FROM [global_data] WHERE [key]=@key LIMIT 0,1";
            const string sql = "INSERT INTO [global_data] ([key],[value]) VALUES (@key,@value)";
            const string updateSql = "UPDATE [global_data] SET [value]=@value WHERE [key]=@key";
            var type = value.GetType();
            var str = type.IsSimpleType() ? value.ToString() : JsonHelper.ToJson(value);
            UseConn(conn =>
            {
                var t = conn.QueryFirstOrDefault<int?>(existsSql, new { key });
                if (t.HasValue && t.Value == 1)
                    conn.Execute(updateSql, new { key, value = str });
                else
                    conn.Execute(sql, new { key, value = str });
            });
        }

        /// <summary> 删除键 </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            const string sql = "DELETE FROM [global_data] WHERE [key]=@key";
            UseConn(conn =>
            {
                conn.Execute(sql, new { key });
            });
        }
    }
}
