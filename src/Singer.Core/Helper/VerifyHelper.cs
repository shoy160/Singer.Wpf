using System.Text.RegularExpressions;

namespace Singer.Core.Helper
{
    /// <summary>
    /// 后台校验帮助类
    /// </summary>
    public static class VerifyHelper
    {

        public static bool IsPhone(this string mobilePhone)
        {
            if (mobilePhone == null || !new Regex("^1[3-9][0-9]{9}$").IsMatch(mobilePhone))
                return false;
            return true;
        }

        public static bool IsEmail(this string email)
        {
            var emailReg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (email == null || !emailReg.IsMatch(email.ToString()))
                return false;
            return true;
        }

        public static bool IsNull(this string values)
        {
            return string.IsNullOrWhiteSpace(values);
        }

        public static bool IsLength(this string values, int minLength, int maxLength, out string msg, bool isNull = false)
        {
            msg = "";
            if (!isNull)
            {
                if (string.IsNullOrWhiteSpace(values))
                {
                    msg = "不能为空!";
                    return false;
                }
            }
            if (values.Length < minLength)
            {
                msg = "输入字符最少为" + minLength + "位字符!";
                return false;
            }
            if (values.Length > maxLength)
            {
                msg = "输入字符最多为" + maxLength + "位字符!";
                return false;
            }
            return true;
        }
    }
}
