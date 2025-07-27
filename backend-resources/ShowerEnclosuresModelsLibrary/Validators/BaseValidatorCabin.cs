using FluentValidation;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShowerEnclosuresModelsLibrary.Validators.CabinValidationErrorCodes;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    public class BaseValidatorCabin<T> : AbstractValidator<T> where T : Cabin
    {
        public BaseValidatorCabin(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll)
        {
            RuleFor(c => c.Model).NotNull().WithErrorCode(InvalidCabinModel);
            RuleFor(c => c.IsPartOfDraw)
                .Must((c,draw) => c.Constraints.ValidDrawNumbers.Contains(draw))
                .WithErrorCode(InvalidDrawNumber);

            if (validationType == ValidationTypeEnum.ValidateAll)
            {
                //Allowed Metal Finishes
                RuleFor(c => c.MetalFinish)
                    .Must((c, MetalFinish) => c.Constraints.ValidMetalFinishes.Any(validFinish=> validFinish == MetalFinish))
                    .WithErrorCode(InvalidCabinMetalFinish);
            }

            //Allowed Thicknesses
            RuleFor(c => c.Thicknesses)
                .Must((c, Thicknesses) => c.Constraints.ValidThicknesses.Any(validThickness => validThickness == Thicknesses))
                .WithErrorCode(InvalidCabinThickness);

            RuleFor(c => c.Thicknesses)
                .Must((c, Thicknesses) => ((int?)c.Thicknesses ?? 0) >= (int)c.Constraints.BreakPointMinThickness)
                .When(c => c.Height > c.Constraints.HeightBreakPointGlassThickness || c.NominalLength > c.Constraints.LengthBreakPointGlassThickness, ApplyConditionTo.CurrentValidator)
                .WithErrorCode(CurrentDimensionsNeedBiggerThickness);


            //Allowed Glass Finishes
            RuleFor(c => c.GlassFinish)
                .Must((c, GlassFinish) => c.Constraints.ValidGlassFinishes.Any(validGlassFinish => validGlassFinish == GlassFinish))
                .WithErrorCode(InvalidCabinGlassFinish);

            //MinLength Boundaries
            RuleFor(c => c.LengthMin).Must((c,LengthMin) => LengthMin >= c.Constraints.MinPossibleLength && LengthMin <= c.Constraints.MaxPossibleLength)
                .WithErrorCode(InvalidCabinLength);

            //Height Boundaries
            RuleFor(c => c.Height).Must((c,Height) => Height >= c.Constraints.MinPossibleHeight && Height <= c.Constraints.MaxPossibleHeight)
                .WithErrorCode(InvalidCabinHeight);

            //StepBoundaries
            When(c => c.HasStep, () =>
            {
                RuleFor(c => c).Cascade(CascadeMode.Stop) //Stops validation on first failure
                .Must(c => c.GetStepCut().StepHeight > 0 && c.GetStepCut().StepLength > 0)
                .WithErrorCode(MissingStepDimension)

                .Must(c => c.GetStepCut().StepHeight >= c.Constraints.MinPossibleStepHeight)
                .WithErrorCode(StepHeightLessThanMinimum)

                .Must(c => (c.GetStepCut().StepHeight < c.Height && c.GetStepCut().StepLength < c.LengthMin))
                .WithErrorCode(StepDimensionsExceedCabin);
            });

            //Cannot Have SafeKids while Glass is Satin or Frosted
            When(c => c.HasExtra(Enums.CabinExtraType.SafeKids), () =>
            {
                RuleFor(c => c.GlassFinish).Cascade(CascadeMode.Stop)
                .Must(gFinish => gFinish is not Enums.GlassFinishEnum.Satin and not Enums.GlassFinishEnum.Frosted)
                .WithErrorCode(SafeKidsCannotBeAppliedOnSatinGlass);
            });

        }
    }
}
/*Rules here can be Executed with a Call to Base Constructor :Base(validationType)
 * 
 */
