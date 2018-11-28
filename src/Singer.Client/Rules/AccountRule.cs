using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    public class AccountRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return new ValidationResult(false, "帐号不能为空");
            //var emailReg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            //var mobileReg = new Regex("^1[3-9][0-9]{9}$");
            //if (emailReg.IsMatch(value.ToString()) || mobileReg.IsMatch(value.ToString()))
            //    return new ValidationResult(true, null);
            //return new ValidationResult(false, "请输入手机号码或邮箱帐号");
            var accountReg = new Regex("^(1[3-9][0-9]{9})|([0-9a-z]{4,12})$", RegexOptions.IgnoreCase);
            return accountReg.IsMatch(value.ToString())
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "登录帐号格式不正确");
        }
    }
}
