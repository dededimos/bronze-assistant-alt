using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.BuilderExceptions
{
    public class GlassCalculationException : Exception
    {
        public GlassCalculationException(string message = "Glasses List Was Empty") : base(message) { }

        /// <summary>
        /// Thrown when attempting to Calculate Remaining Glasses but the Cabin providing the Glasses , has no Glasses to start with
        /// </summary>
        /// <param name="cabin">The Cabin for which the Exception is thrown</param>
        /// <exception cref="GlassCalculationException"></exception>
        public static void ThrowIfEmptyGlassListInCalculations(Cabin cabin)
        {
            if (!cabin.Glasses.Any())
            {
                throw new GlassCalculationException
                    ($"Remaining Glasses Cannot be Calculated when the Initial Glasses List is Empty , Cabin:{cabin.GetType().Name}");
            }
        }
        /// <summary>
        /// Throws if all Glasses are present
        /// </summary>
        /// <param name="cabin">The Cabin for which the Exception is thrown</param>
        /// <exception cref="GlassCalculationException"></exception>
        public static void ThrowIfAllGlassesPresent(Cabin cabin , int throwWhenMoreThan)
        {
            int presentGlasses = cabin.Glasses.Count;
            if (presentGlasses > throwWhenMoreThan)
            {
                throw new GlassCalculationException
                    ($"There are already {presentGlasses} Glasses Present , Cabin:{cabin.GetType().Name}");
            }
        }
    }
}
