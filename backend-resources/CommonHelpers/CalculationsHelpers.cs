using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    /// <summary>
    /// Contains Various Methods - Calculation/Aggragation/Geometry e.t.c.
    /// </summary>
    public static class CalculationsHelpers
    {
        /// <summary>
        /// Returns the Closest Number contained in the List to the Provided input
        /// </summary>
        /// <param name="numberToMatch">The number we want to match</param>
        /// <param name="listOfNumbers">The List of Numbers we have</param>
        /// <returns>The Closest number of the list to the Provided number</returns>
        /// <exception cref="ArgumentException">When list is Empty or Null</exception>
        public static int MatchToNearest(int numberToMatch, List<int> listOfNumbers)
        {
            if (listOfNumbers == null || listOfNumbers.Count == 0)
            {
                throw new ArgumentException($"Provided {nameof(listOfNumbers)} is null or Empty , Method:{nameof(MatchToNearest)}");
            }
            //x,y at the first Iteration are the first two numbers on the list ,
            //then in the following iterrations, x is the Returned number from the previous iteration and Y the next Number from the List
            //The Below LINQ call applies the if statement and returns according to each iteration whichever number is closest to our number 
            //After finishing all iterations we get back the closest number from the lsit to our Number.
            int nearest = listOfNumbers.Aggregate((x, y) => Math.Abs(x - numberToMatch) < Math.Abs(y - numberToMatch) ? x : y);

            return nearest;
        }
    }
}
