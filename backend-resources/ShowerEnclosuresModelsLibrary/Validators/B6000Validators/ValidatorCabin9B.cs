using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.B6000Validators
{
    public class ValidatorCabin9B : BaseValidatorCabin<Cabin9B>
    {
        public ValidatorCabin9B(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            
            RuleFor(c => c.HasStep)
                //When we do not have a FixedPanel we cannot have step
                .Must((c, HasStep) => HasStep == false).When(c => c.LengthMin <= c.Constraints.AddedFixedGlassLengthBreakpoint ,ApplyConditionTo.CurrentValidator)
                .WithErrorCode(CabinValidationErrorCodes.BWithoutFixedCannotHaveStep);

            RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);

        }
    }
}
