using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.WSValidator
{
    public class ValidatorCabinWS : BaseValidatorCabin<CabinWS>
    {
        public ValidatorCabinWS(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll):base(validationType)
        {

            //Boundary for Overlap
            RuleFor(c => c.Constraints.Overlap)
                .NotEmpty().WithErrorCode(CabinValidationErrorCodes.OverlapGreaterThanZero);

            When(c => c.Parts.Handle is not null, () =>
            {
                //HandleDistance not Zero
                RuleFor(c => c.Parts.Handle!.GetSlidingDoorAirDistance())
                    .NotEmpty().WithErrorCode(CabinValidationErrorCodes.HDGreaterThanZero);  //HandleDistanmce Cannot be Zero
            });

            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);

            RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);

        }
    }
}
