using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class DecimalToPercentDiscountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalDiscount)
            {
                return decimalDiscount * 100;
            }
            else
            {
                throw new NotSupportedException($"{nameof(DecimalToPercentDiscountConverter)} Supports ONLY decimal Values for Conversion");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringDiscountPercent)
            {
                return decimal.TryParse(stringDiscountPercent, out decimal discountPercent) ? discountPercent * 0.01m : throw new Exception("Value was not in a supported Decimal Format");
            }
            else
            {
                throw new NotSupportedException("Conversion back to Decimal is Only Supported for string Values");
            }
        }
    }
}
