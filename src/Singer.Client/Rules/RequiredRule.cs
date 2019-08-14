using System.Globalization;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    /// <summary> 必填规则 </summary>
    public class RequiredRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "该字段不能为空值！");
            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "该字段不能为空！");
            return new ValidationResult(true, null);
        }
    }
}
