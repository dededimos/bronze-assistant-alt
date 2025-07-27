using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.NPValidators
{
    public class ValidatorCabinNP : BaseValidatorCabin<CabinNP>
    {
        public ValidatorCabinNP(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);

            When(c => c.IsPartOfDraw is not CabinDrawNumber.DrawNP44
                                     and not CabinDrawNumber.DrawQP44
                                     and not CabinDrawNumber.DrawMV2 
                                     and not CabinDrawNumber.DrawNV2
                                     , () =>
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);
            });

            When(c => c.Parts.CloseProfile is not null, () =>
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.CloseProfileWithoutHandle);
            });

            RuleFor(cabin => cabin.Parts.MiddleHinge).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutMiddleHinge);
        }
    }
}
