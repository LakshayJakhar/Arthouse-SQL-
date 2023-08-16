using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ArthouseUI.Converters
{
    /// <summary>
    /// Convert between Decimal and a string formatted as n2
    /// Note that in ConvertBack we strip out any characters that are not
    /// wanted such as $ or , characters.  We remove everything except digit characters
    /// and the decimal point.  If the users enters an extra decimal point then 0 is returned
    /// </summary>
    public class CurrencyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return string.Empty;

                return String.Format("{0:n2}", value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string val = value.ToString();
                //strip out any characters that are not a digit, decimal place character or minus sign
                string cleanVal = new string(val.Where(c => (char.IsDigit(c)) || c == '.' || c == '-').ToArray());
                if (decimal.TryParse(cleanVal, out decimal result))
                {
                    return result;
                }
                else
                {
                    return 0m;
                }
            }
            catch (Exception)
            {
                return 0m;
            }
        }
    }

}
