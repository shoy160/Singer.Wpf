using System;
using System.Data;

namespace Singer.Core.Domain
{
    public abstract class ServiceBase
    {
        protected readonly string CurrentDb;

        protected ServiceBase(string currentDb = null)
        {
            CurrentDb = currentDb;
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        protected abstract IDbConnection Conn(string dbName);

        /// <summary> 使用数据库 </summary>
        /// <param name="connAction"></param>
        /// <param name="name"></param>
        protected T UseConn<T>(Func<IDbConnection, T> connAction, string name = null)
        {
            using (var conn = Conn(name))
            {
                return connAction.Invoke(conn);
            }
        }

        /// <summary> 使用数据库 </summary>
        /// <param name="connAction"></param>
        /// <param name="name"></param>
        protected void UseConn(Action<IDbConnection> connAction, string name = null)
        {
            using (var conn = Conn(name))
            {
                connAction.Invoke(conn);
            }
        }

        protected T UseConn<T>(Func<IDbConnection, IDbTransaction, T> connAction, string name = null)
        {
            using (var conn = Conn(name))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    var result = connAction.Invoke(conn, trans);
                    trans.Commit();
                    return result;
                }
                catch
                {
                    trans.Rollback();
                    return default(T);
                }
            }
        }
    }

    public abstract class ServiceBase<T> : ServiceBase where T : ServiceBase, new()
    {
        protected ServiceBase(string currentDb = null)
            : base(currentDb)
        {
        }

        public static T Instance => Singleton<T>.Instance ?? (Singleton<T>.Instance = new T());
    }
}
