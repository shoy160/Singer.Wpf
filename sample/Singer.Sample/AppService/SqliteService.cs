using ESignature.Business;
using Singer.Core.Domain;
using System.Data;
using System.Data.SQLite;

namespace Singer.Sample.AppService
{
    public class SqliteService<T> : ServiceBase<T> where T : ServiceBase, new()
    {
        public SqliteService(string currentDb) : base(string.IsNullOrWhiteSpace(currentDb) ? Const.AppName : currentDb)
        { }
        protected override IDbConnection Conn(string dbName)
        {
            dbName = string.IsNullOrWhiteSpace(dbName) ? CurrentDb : dbName;
            var path = dbName.DbPath();
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3,
                FailIfMissing = false,
                JournalMode = SQLiteJournalModeEnum.Off,
                Pooling = true,
                Password = Const.DbPassword
            };
            return new SQLiteConnection(builder.ConnectionString);
        }
    }
}
