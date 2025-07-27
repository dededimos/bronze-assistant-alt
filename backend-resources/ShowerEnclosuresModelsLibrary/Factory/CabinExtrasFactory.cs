using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Factory
{
    public static class CabinExtrasFactory
    {
        /// <summary>
        /// Creates a Cabin Extra
        /// </summary>
        /// <param name="type">The Type of Extra to Create</param>
        /// <returns>The Extra Type</returns>
        /// <exception cref="ArgumentException">When Type is Not Recognized</exception>
        public static CabinExtra CreateCabinExtra(CabinExtraType type)
        {
            return type switch
            {
                CabinExtraType.StepCut => new StepCut(),
                CabinExtraType.BronzeClean => new CabinExtra(CabinExtraType.BronzeClean),
                CabinExtraType.SafeKids => new CabinExtra(CabinExtraType.SafeKids),
                CabinExtraType.BronzeCleanNano => throw new NotImplementedException(),
                _ => throw new ArgumentException($"Not Recognized Extra Type : {type}"),
            };
        }
    }
}
