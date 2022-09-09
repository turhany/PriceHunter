using System.Globalization;
using System.Text.RegularExpressions;

namespace PriceHunter.Common.Extensions
{
    public static class StringExtensions
    {
        //https://stackoverflow.com/a/69205767
        public static decimal ConvertToDecimal(this string text)
        {
            // Decimal separator is ",".
            CultureInfo culture = new CultureInfo("tr-TR");
            // Decimal sepereator is ".".
            CultureInfo culture1 = new CultureInfo("en-US");
            // You can remove letters here by adding regex expression.
            text = Regex.Replace(text, @"\s+|[a-z|A-Z]", "");
            decimal result = 0;
            var success = decimal.TryParse(text, NumberStyles.AllowThousands |
                NumberStyles.AllowDecimalPoint, culture, out result);
            // No need NumberStyles.AllowThousands or
            // NumberStyles.AllowDecimalPoint but I used:
            decimal result1 = 0;
            var success1 = decimal.TryParse(text, NumberStyles.AllowThousands |
                NumberStyles.AllowDecimalPoint, culture1, out result1);
            if (success && success1)
            {
                if (result > result1)
                    return result1;
                else
                    return result;
            }
            if (success && !success1)
                return result;
            if (!success && success1)
                return result1;
            return 0;
        }
    }
}
