using ESignature.Business;
using Singer.Core;
using Singer.Core.Helper;
using Singer.Sample.AppService;
using System.Management;

namespace Singer.Sample.Helper
{
    public class MachineCodeHelper
    {
        private MachineCodeHelper() { }

        public static MachineCodeHelper Instance =
            Singleton<MachineCodeHelper>.Instance ??
            (Singleton<MachineCodeHelper>.Instance = new MachineCodeHelper());

        public string MachineCode
        {
            get
            {
                var code = GlobalDataService.Instance.Query<string>(GlobalKeys.MachineCode);
                if (!string.IsNullOrWhiteSpace(code))
                    return code;
                code = $"PC_{GetCpuInfo()}_{GetMacAddress().Replace(":", string.Empty)}";
                GlobalDataService.Instance.InsertOrUpdate(GlobalKeys.MachineCode, code);
                return code;
            }
        }

        /// <summary> 获取cpu序列号 </summary>
        public string GetCpuInfo()
        {
            using (var cimobject = new ManagementClass("Win32_Processor"))
            {
                var moc = cimobject.GetInstances();

                foreach (var o in moc)
                {
                    using (var mo = (ManagementObject)o)
                    {
                        return mo.Properties["ProcessorId"].Value.ToString();
                    }
                }
            }
            return CommonHelper.Guid16;
        }

        /// <summary> 获取硬盘ID </summary>
        public string GetHDid()
        {
            using (var cimobject1 = new ManagementClass("Win32_DiskDrive"))
            {
                var moc1 = cimobject1.GetInstances();
                foreach (var o in moc1)
                {
                    using (var mo = (ManagementObject)o)
                    {
                        return (string)mo.Properties["Model"].Value;
                    }
                }
            }
            return CommonHelper.Guid16;
        }

        /// <summary> 获取网卡硬件地址 </summary>
        public string GetMacAddress()
        {
            using (var mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                var moc2 = mc.GetInstances();
                foreach (var o in moc2)
                {

                    using (var mo = (ManagementObject)o)
                    {
                        if ((bool)mo["IPEnabled"])
                        {
                            return mo["MacAddress"].ToString();
                        }
                    }
                }
            }
            return CommonHelper.Guid32;
        }
    }
}
