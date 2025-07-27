using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.DBValidators
{
    public class ValidatorCabinDB : BaseValidatorCabin<CabinDB>
    {
        public ValidatorCabinDB(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Step Boundaries
            RuleFor(c => c.HasStep)
                .Equal(false)
                .WithErrorCode(CabinValidationErrorCodes.CannotHaveStepError);

            ////Draw Cannot be Null
            //RuleFor(c => c.Draw)
            //    .NotNull().WithMessage($"Ο αριθμός Σχεδίου δεν μπορεί να είναι κενός");

            //Closure options
            //RuleFor(c => c.ClosureMagnetOptions).Cascade(CascadeMode.Stop)
            //    .NotNull().WithMessage($"Η επιλογή Κλεισίματος δεν μπορεί να είναι κενή")
            //    .NotEqual(MagnetClosureOptionsEnum.WithoutStrip).When(
            //    c => c.Draw == DrawDBEnum.Draw52 ||
            //    c.Draw == DrawDBEnum.Draw53 ||
            //    c.Draw == DrawDBEnum.Draw59 ||
            //    c.Draw == DrawDBEnum.Draw61
            //    , ApplyConditionTo.CurrentValidator).WithMessage($"Το Σχέδιο αυτό δεν μπορεί να γίνει Χωρίς Λάστιχο");

            //Door HeightAdjustment
            RuleFor(c => c.Constraints.DoorDistanceFromBottom)
                .NotEmpty().WithErrorCode(CabinValidationErrorCodes.DoorDistanceFromBottomGreaterThanZero);

            When(c => c.IsPartOfDraw != CabinDrawNumber.DrawDB51, () =>
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);
                RuleFor(cabin => cabin.Parts.CloseStrip).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutCloserError);
            });

            When(c => c.Parts.CloseProfile is not null, () => 
            {
                RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.CloseProfileWithoutHandle);
            });
        }
    }
}
