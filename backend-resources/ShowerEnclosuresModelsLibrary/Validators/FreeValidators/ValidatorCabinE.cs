using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.FreeValidators
{
    public class ValidatorCabinE : BaseValidatorCabin<CabinE>
    {
        public ValidatorCabinE(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);

        }
    }
}
