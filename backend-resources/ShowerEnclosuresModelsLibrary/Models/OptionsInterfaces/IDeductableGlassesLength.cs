using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces
{
    public interface IDeductableGlassesLength
    {
        /// <summary>
        /// Returns the Deductable length of this part from the Specified Model's Structure
        /// How much Length it deducts from the Total Glasses Length
        /// </summary>
        /// <returns></returns>
        double GetDeductableLength(CabinModelEnum model);
    }
}
