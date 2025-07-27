using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;

namespace ShowerEnclosuresModelsLibrary.Validators.FreeValidators
{
    public class ValidatorCabinWFlipper : BaseValidatorCabin<CabinWFlipper>
    {
        public ValidatorCabinWFlipper(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll):base(validationType)
        {            
            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);
        }
    }
}
