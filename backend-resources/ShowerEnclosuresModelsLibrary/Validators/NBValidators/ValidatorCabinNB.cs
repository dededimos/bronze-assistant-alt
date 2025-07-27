using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.NBValidators
{
    public class ValidatorCabinNB : BaseValidatorCabin<CabinNB>
    {
        public ValidatorCabinNB(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);

            When(c => c.IsPartOfDraw is not CabinDrawNumber.DrawNB31 and not CabinDrawNumber.DrawQB31 and not CabinDrawNumber.DrawNV , () =>
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);
            });

            When(c => c.Parts.CloseProfile is not null, () =>
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.CloseProfileWithoutHandle);
            });
        }
    }
}
