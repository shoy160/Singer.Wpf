using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    public class MobileRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || !new Regex("^1[3-9][0-9]{9}$").IsMatch(value.ToString()))
                return new ValidationResult(false, "手机号码不正确");
            return new ValidationResult(true, null);
        }
    }
}
