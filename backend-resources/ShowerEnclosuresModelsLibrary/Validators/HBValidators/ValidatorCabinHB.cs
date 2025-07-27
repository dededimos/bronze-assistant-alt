using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.HBValidators
{
    public class ValidatorCabinHB : BaseValidatorCabin<CabinHB>
    {
        public ValidatorCabinHB(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Partial Length Boundaries and Nullness (Door must always leave length of 200mm , Fixed must always leave length of 450mm
            RuleFor(c => c).Cascade(CascadeMode.Stop)   //Stops on first Failure
                .Must(c => c.Constraints.PartialLength >= c.Constraints.MinDoorLength && c.Constraints.PartialLength <= c.Constraints.MaxDoorLength)
                //When we define by the Door Length
                .When(c => c.Constraints.LengthCalculation == LengthCalculationOption.ByDoorLength, ApplyConditionTo.CurrentValidator)
                .WithErrorCode(CabinValidationErrorCodes.InvalidPartialLengthDoor)

                //When we define by the Fixed Length
                .Must(c => c.Constraints.PartialLength >= c.Constraints.MinFixedLength && c.Constraints.PartialLength <= c.Constraints.MaxFixedLength)
                .When(c => c.Constraints.LengthCalculation == LengthCalculationOption.ByFixedLength, ApplyConditionTo.CurrentValidator)
                .WithErrorCode(CabinValidationErrorCodes.InvalidPartialLengthFixed);

            When(c => c.IsPartOfDraw != CabinDrawNumber.DrawHB34, () =>
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
