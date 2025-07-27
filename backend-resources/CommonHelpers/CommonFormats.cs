using static CommonHelpers.CommonConstants;

namespace CommonHelpers
{
    public static class CommonFormats
    {
        public static string EuroCurrencyFormat(int decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                return $"0{Currencies.EURO}";
            }
            string format = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                format = $"{format}{0}";
            }
            return $"{format}{Currencies.EURO}";
        }
        public static string PercentageFormat(int decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                return $"0%";
            }
            string format = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                format = $"{format}{0}";
            }
            return $"{format}%";
        }
    }
}
