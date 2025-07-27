using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    /// <summary>
    /// Used to Call Validations of certain Criteria
    /// </summary>
    public enum ValidationTypeEnum
    {
        /// <summary>
        /// To Validate All Properties
        /// </summary>
        ValidateAll,
        /// <summary>
        /// To Vaidate Only Properties needed for Glass Calculations
        /// </summary>
        OnlyForCalculations
    }
}
