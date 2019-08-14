using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Singer.Core
{
    /// <summary> 项目常量 </summary>
    public static class Constants
    {
        /// <summary> 版本号 </summary>
        public const string Version = "1.0.9";
        internal const string DateFormatString = "yyyy-MM-dd HH:mm:ss";

        internal static readonly Dictionary<string, string> ContentTypes = new Dictionary<string, string>
        {
            {"*", "application/octet-stream"},
            {".doc", "application/msword"},
            {".ico", "image/x-icon"},
            {".gif", "image/gif"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".png", "image/x-png"},
            {".mp3", "audio/mpeg"},
            {".mpeg", "audio/mpeg"},
            {".flv", "video/x-flv"},
            {".mp4", "application/octet-stream"},
        };

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

        /// <summary> 是否64位系统 </summary>
        public static bool Is64Bit => !Environment.Is64BitProcess;

        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary> 是否有网络链接 </summary>
        public static bool IsConnected()
        {
            try
            {
                if (!InternetGetConnectedState(out var connection, 0))
                    return false;
#if DEBUG
                const int INTERNET_CONNECTION_MODEM = 1;

                const int INTERNET_CONNECTION_LAN = 2;

                const int INTERNET_CONNECTION_PROXY = 4;

                const int INTERNET_CONNECTION_MODEM_BUSY = 8;

                var netstatus = string.Empty;
                if ((connection & INTERNET_CONNECTION_PROXY) != 0)
                    netstatus += " 采用代理上网  \n";
                if ((connection & INTERNET_CONNECTION_MODEM) != 0)
                    netstatus += " 采用调治解调器上网 \n";
                if ((connection & INTERNET_CONNECTION_LAN) != 0)
                    netstatus += " 采用网卡上网  \n";
                if ((connection & INTERNET_CONNECTION_MODEM_BUSY) != 0)
                    netstatus += " MODEM被其他非INTERNET连接占用  \n";
                Console.WriteLine(netstatus);
#endif
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}