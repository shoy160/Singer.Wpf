using System;

namespace Singer.Sample.Dtos
{
    [Serializable]
    public class ManifestDto
    {
        /// <summary> 版本 </summary>
        public string Version { get; set; }
        /// <summary> 强制更新 </summary>
        public bool Mandatory { get; set; }
        /// <summary> 下载链接 </summary>
        public string DownloadUrl { get; set; }
        /// <summary> 更新时间 </summary>
        public DateTime UpgradeTime { get; set; }
        public Guid Md5 { get; set; }
        /// <summary> 更新说明 </summary>
        public string UpgradeInstructions { get; set; }
    }
}
