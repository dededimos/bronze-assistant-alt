using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Comparers
{
    /// <summary>
    /// A String Comparer that Prioritizes Strings in the list by their order in the Priority list
    /// </summary>
    public class StringPriorityComparer : IComparer<string>
    {
        private readonly List<string> priorityList;

        /// <summary>
        /// A String Comparer that Prioritizes Strings in the list by their order in the Priority list
        /// </summary>
        /// <param name="priorityList">The List of priorities , lower indexed items come always first</param>
        public StringPriorityComparer(List<string> priorityList)
        {
            this.priorityList = priorityList;
        }

        public int Compare(string? x, string? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }
            else
            {
                int xPriority = priorityList.IndexOf(x);
                int yPriority = priorityList.IndexOf(y);

                if (xPriority >= 0 && yPriority >= 0)
                {
                    // Both x and y are in the priority list
                    return xPriority.CompareTo(yPriority);
                }
                else if (xPriority >= 0)
                {
                    // Only x is in the priority list
                    return -1;
                }
                else if (yPriority >= 0)
                {
                    // Only y is in the priority list
                    return 1;
                }
                else
                {
                    // Neither x nor y are in the priority list
                    return x.CompareTo(y);
                }
            }
        }
    }
}
