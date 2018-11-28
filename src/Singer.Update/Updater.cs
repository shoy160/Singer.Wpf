using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Singer.Update
{
    public class Updater
    {
        private const string UpdateExe = "Singer.Update.exe";

        private static Updater _instance;
        public static Updater Instance => _instance ?? (_instance = new Updater());

        private void UpdateProcess(string appDir, params string[] paras)
        {
            if (string.IsNullOrWhiteSpace(appDir))
                return;
            var exePath = Path.Combine(appDir, UpdateExe);
            var arg = new StringBuilder();
            arg.Append("update ");
            foreach (var para in paras)
            {
                arg.Append(string.IsNullOrWhiteSpace(para) ? ToBase64("无") : ToBase64(para));
                arg.Append(" ");
            }

            var processStartInfo = new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                WorkingDirectory = appDir,
                Arguments = arg.ToString().TrimEnd(' ')
            };
            Process.Start(processStartInfo);
        }

        internal static string ToBase64(string arg)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(arg));
        }

        internal static string Base64(string arg)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(arg));
        }

        public void StartUpdate(string downloadUrl, string appVersion, string desc, Guid md5)
        {
            if (CurrentVersion >= new Version(appVersion))
            {
                //当前版本是最新的，不更新
                return;
            }

            //更新程序复制到缓存文件夹
            var appDir = AppDomain.CurrentDomain.BaseDirectory;

            UpdateProcess(appDir, CallExeName, downloadUrl, appVersion,
                desc, md5.ToString("N"));
        }

        public bool UpdateFinished = false;

        private string _callExeName;
        public string CallExeName
        {
            get
            {
                if (string.IsNullOrEmpty(_callExeName))
                {
                    _callExeName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
                }
                return _callExeName;
            }
        }

        /// <summary>
        /// 获得当前应用软件的版本
        /// </summary>
        public virtual Version CurrentVersion
        {
            get
            {
                var entryAss = Assembly.GetEntryAssembly();
                if (entryAss == null)
                    return null;
                return new Version(FileVersionInfo.GetVersionInfo(entryAss.Location).ProductVersion);
            }
        }

        /// <summary>
        /// 获得当前应用程序的根目录
        /// </summary>
        public virtual string CurrentApplicationDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }
    }
}
