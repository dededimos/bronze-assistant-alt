using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.B6000Validators
{
    public class ValidatorCabin9F : BaseValidatorCabin<Cabin9F>
    {
        public ValidatorCabin9F(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Empty For Now - Calls BaseConstructor            
        }
    }
}
