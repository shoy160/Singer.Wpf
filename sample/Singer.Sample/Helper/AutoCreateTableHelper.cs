using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using ESignature.Business;
using Singer.Core;
using Singer.Core.Domain;
using Singer.Core.Helper;
using Singer.Sample.AppService;
using Singer.Sample.Dtos;

namespace Singer.Sample.Helper
{
    public class AutoCreateTableHelper
    {
        private static readonly CacheHelper Cache = new CacheHelper("sql");

        /// <summary> 初始化数据库 </summary>
        public static void InitializeDb()
        {
            InitializeGlobalDb();
            var globalService = new GlobalDataService();
            var dbVersion = globalService.Query<int>(GlobalKeys.DbVersion);
            if (dbVersion < Const.DbVersion)
            {
                //活动业务库
                InitializeBusiDb();
                globalService.InsertOrUpdate(GlobalKeys.DbVersion, Const.DbVersion);
            }
            VersionUpdateSql();
        }

        /// <summary> 初始化全局配置表 </summary>
        public static void InitializeGlobalDb()
        {
            //全局数据库
            var globalPath = Const.GlobalDb.DbPath();
            if (File.Exists(globalPath))
            {
                CheckGlobalData();
                return;
            }
            var dir = Path.GetDirectoryName(globalPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var conn = new SQLiteConnection(string.Format(Const.ConnectionString, globalPath)))
            {
                conn.Open();
                if (!string.IsNullOrWhiteSpace(Const.DbPassword))
                    conn.ChangePassword(Const.DbPassword);
                conn.Execute(GlobalTables());
            }
            InitDataSql();
        }

        /// <summary> 初始化业务数据库 </summary>
        public static void InitializeBusiDb()
        {
            //活动数据库
            var dbPath = Const.AppName.DbPath();
            if (File.Exists(dbPath)) //如果升级 存在文件,转移文件在初始化数据
            {
                var backupPath = $"__old_{Const.AppName}_{DateTime.Now:yyyyMMddHHmm}".DbPath();
                var fi = new FileInfo(dbPath);
                fi.MoveTo(backupPath);
            }
            else
            {
                var dir = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            var sql = TablesSql();
            using (var conn = new SQLiteConnection(string.Format(Const.ConnectionString, dbPath)))
            {
                conn.Open();
                if (!string.IsNullOrWhiteSpace(Const.DbPassword))
                    conn.ChangePassword(Const.DbPassword);
                conn.Execute(sql);
            }
        }

        /// <summary> 备份数据库 </summary>
        public static string Backup()
        {
            var dbPath = Const.AppName.DbPath();
            if (!File.Exists(dbPath))
                return null;
            var backupPath = CommonHelper.Guid16.DbPath();
            var fi = new FileInfo(dbPath);
            fi.CopyTo(backupPath);
            return backupPath;
        }

        /// <summary> 还原数据库 </summary>
        public static void Restore(string path)
        {
            if (!File.Exists(path))
                return;
            var dbPath = Const.AppName.DbPath();
            var fi = new FileInfo(path);
            fi.MoveTo(dbPath);
        }

        public static string GlobalTables()
        {
            var sql = new StringBuilder();
            sql.AppendLine("create table \"global_data\" (\"key\" TEXT NOT NULL PRIMARY KEY,\"value\" TEXT NOT NULL);");
            sql.AppendLine(
                "CREATE TABLE \"synch_data\" (\"id\" TEXT NOT NULL PRIMARY KEY, \"table\" TEXT NOT NULL,\"state\" INT NOT NULL,\"data_id\" TEXT NOT NULL, \"data\" TEXT NULL,\"create_date\" DATETIME NOT NULL);");
            return sql.ToString();
        }

        /// <summary> 组装sql语句 </summary>
        public static string TablesSql()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(EntityBase)));
            var sql = new StringBuilder();
            foreach (var type in types)
            {
                sql.AppendLine(type.GenerateTableSql());
            }
            return sql.ToString();
        }

        /// <summary> 生成insert语句 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string InsertSql<T>()
            where T : EntityBase
        {
            var type = typeof(T);
            var key = $"insert_{type.FullName}";
            var sql = Cache.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(sql))
                return sql;
            //字段
            var fields = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var sb = new StringBuilder();
            sb.Append($"INSERT INTO [{type.Name.ToLower()}]");
            var fieldSql = string.Join(",", fields.Select(t => $"[{t.Name.ToLower()}]"));
            var paramSql = string.Join(",", fields.Select(t => $"@{t.Name}"));
            sb.Append($" ({fieldSql}) VALUES ({paramSql})");
            sql = sb.ToString();
            Cache.Set(key, sql);
            return sql;
        }

        /// <summary> 生成修改SQL </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereField">根据哪个字段来修改</param>
        /// <returns></returns>
        public static string UpdateSql<T>(string whereField) where T : EntityBase
        {
            var type = typeof(T);
            var key = $"update_{type.FullName}";
            var sql = Cache.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(sql))
                return sql;
            var fields = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var sb = new StringBuilder();
            sb.Append($"UPDATE [{type.Name.ToLower()}] SET ");
            for (var i = 0; i < fields.Length; i++)
            {
                sb.Append(string.Format(("[{0}]=@{1},"), fields[i].Name.ToLower(), fields[i].Name.ToLower()));
            }
            sql = sb.ToString().Substring(0, sb.Length - 1);
            sql = sql + " where " + whereField + "=@" + whereField;
            Cache.Set(key, sql);
            return sql;
        }


        /// <summary> 初始化数据 </summary>
        private static void InitDataSql()
        {
            var service = GlobalDataService.Instance;
            //service.InsertOrUpdate(GlobalKeys.Host, ConfigHelper.Read<string>("api_host"));
            //service.InsertOrUpdate(GlobalKeys.DataMode, DataMode.Online);
            service.InsertOrUpdate(GlobalKeys.MachineCode, MachineCodeHelper.Instance.MachineCode);
            service.InsertOrUpdate(GlobalKeys.Menus, ConfigHelper.ReadList<MenuDto>("menus"));
        }

        private static void CheckGlobalData()
        {
            var service = GlobalDataService.Instance;
            service.InsertOrUpdate(GlobalKeys.Menus, ConfigHelper.ReadList<MenuDto>("menus"));
        }

        /// <summary> 版本更新 </summary>
        private static void VersionUpdateSql()
        {
            var service = GlobalDataService.Instance;
            var version = service.Query<string>(GlobalKeys.VersionUpdateSql);
            if (version == Const.Version) return;
            //版本更新Sql
            if (string.IsNullOrWhiteSpace(version))
            {
                service.InsertOrUpdate(GlobalKeys.Menus, ConfigHelper.ReadList<MenuDto>("menus"));
            }
            service.InsertOrUpdate(GlobalKeys.VersionUpdateSql, Const.Version);
        }
    }
}
