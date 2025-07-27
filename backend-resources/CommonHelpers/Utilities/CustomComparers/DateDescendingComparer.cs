using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Utilities.CustomComparers
{
    public class DateDescendingComparer : IComparer<DateTime>
    {
        /// <summary>
        /// Compares Dates in Descending Order (From Newer Date to Older Date)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DateTime x, DateTime y)
        {
            // use the default comparer to do the original comparison for datetimes
            int ascendingResult = Comparer<DateTime>.Default.Compare(x, y);

            // turn the result around
            return 0 - ascendingResult;
        }
    }
}
