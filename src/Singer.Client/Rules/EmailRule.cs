using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    public class EmailRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var emailReg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (value == null || !emailReg.IsMatch(value.ToString()))
                return new ValidationResult(false, "邮箱地址不正确！");
            return new ValidationResult(true, null);
        }
    }
}
