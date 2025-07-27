using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    public static class CommonConstants
    {
        public static class Currencies
        {
            public const string EURO = "\u20AC";
        }

        public static class Units
        {
            public const string SquareMeter = "m²";
        }

        public static class StringFormats
        {
            public const string ZeroDecimals = "F0";
            public const string TwoDecimalsSquareMeterXML = "0.00 \"m²\"";
            public const string TwoDecimalsMeterXML = "0.00\"m\"";
        }
    }
}
