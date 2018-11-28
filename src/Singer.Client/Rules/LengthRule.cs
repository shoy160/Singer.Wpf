using System.Globalization;
using System.Windows.Controls;

namespace Singer.Client.Rules
{
    public class LengthRule : ValidationRule
    {
        int min;
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        int max;
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        bool isNull;
        public bool isNUll {
            get { return isNull; }
            set { isNull = value; }
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        string field;
        public string Field
        {
            get { return field; }
            set { field = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
           string text = value.ToString();
            if (!isNull) {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return new ValidationResult(false, Field + "不能为空!");
                }

            }
            if (text.Length < Min) {
                return new ValidationResult(false, Field + "不能少于" + Min + "个字符!");
            }
            if (text.Length > Max) {
                return new ValidationResult(false, Field + "不能多于" + Max + "个字符!");
            }
            return new ValidationResult(true, null);
        }
    }
}
