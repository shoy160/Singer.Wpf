using Singer.Core.Helper;
using Singer.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ESignature.Business
{
    public static class Const
    {
        public const string Version = "1.0.0";
        public const string AppName = "esignatue";

        /// <summary> 数据库版本 </summary>
        internal static int DbVersion = 1;
#if DEBUG
        internal static string DbPassword = string.Empty;
#else
        internal static string DbPassword = $"k98djnc__{AppName}";
#endif

        internal const string GlobalDb = AppName + "_global";

        internal static string ConnectionString =
            "Data Source={0};Pooling=true;FailIfMissing=false;Version=3;UTF8Encoding=True;Journal Mode=Off;";

        internal static string Host => ConfigHelper.Read<string>(GlobalKeys.Host);

        public static ILogger DefaultLogger => LogManager.Logger(AppName);

        private const string DbPathFormat = "Contents\\data\\{0}.db";


        internal static string DbPath(this string name)
        {
#if DEBUG
            name = $"{name}_debug";
#else
            name = $"{name}_pro";
#endif
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(DbPathFormat, name));
        }

        /// <summary> 获得当前应用软件的版本 </summary>
        public static Version CurrentVersion
        {
            get
            {
                var location = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location;
                var version = FileVersionInfo.GetVersionInfo(location).ProductVersion;
                return !string.IsNullOrWhiteSpace(version) ? new Version(version) : new Version();
            }
        }

        public static string UserAgent
        {
            get
            {
                var agent = new StringBuilder();
                agent.AppendFormat("ESignature.Client/{0}/{1}/{2}", CurrentVersion, Environment.OSVersion,
                    Environment.MachineName);
                return agent.ToString();
            }
        }
    }
}