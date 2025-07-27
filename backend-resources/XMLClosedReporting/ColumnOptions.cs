using XMLClosedReporting.StylesModels;

namespace XMLClosedReporting
{
    public class ColumnOptions
    {
        private const string euroUnicode = "\u20AC";

        /// <summary>
        /// The Styles of the Values of this Column
        /// </summary>
        public StyleOptions ValueCellsStyle { get; set; } = StyleOptions.ValueCellsDefaultStyles();

        /// <summary>
        /// a Specific Format if the Column is a number (ex. '0.00\u20AC')
        /// </summary>
        public string NumberFormat { get; set; } = string.Empty;

        /// <summary>
        /// Weather it should Sum the Values of this Column
        /// </summary>
        public bool ShouldSumColumnValues { get; set; }
        /// <summary>
        /// The Title of the SumValue Cell
        /// </summary>
        public string SumCellTitle { get; set; } = "Total";

        /// <summary>
        /// Weather a Hyperlink should be Created out of the Value
        /// </summary>
        public bool ShouldCreateHyperlinkInValue { get; set; }

        /// <summary>
        /// Sets the Number Format to Euro Currency
        /// </summary>
        /// <param name="decimalPlaces">The Decimal Places to use</param>
        public void SetEuroCurrencyFormat(int decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                NumberFormat = $"0{euroUnicode}";
                return;
            }

            string format = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                format = $"{format}{0}";
            }
            NumberFormat = $"{format}{euroUnicode}";
        }
        /// <summary>
        /// Sets the Number Format to percentage
        /// </summary>
        /// <param name="decimalPlaces">The Decimal places to use</param>
        public void SetPercentageFormat(int decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                NumberFormat = $"0%";
                return;
            }

            string format = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                format = $"{format}{0}";
            }
            NumberFormat = $"{format}%";
        }
    }

}
