using System.Globalization;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    /// <summary> 密码规则 </summary>
    public class PasswordRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "密码不能为空");
            if (value.ToString().Length < 6)
                return new ValidationResult(false, "密码不能少于6位");
            return new ValidationResult(true, null);
        }
    }
}
