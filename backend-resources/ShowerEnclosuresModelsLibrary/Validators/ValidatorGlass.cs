using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    /// <summary>
    /// Validates the Glass
    /// </summary>
    public class ValidatorGlass : AbstractValidator<Glass>
    {
        public ValidatorGlass()
        {
            // Length Boundaries
            RuleFor(g => g.Length)
                .InclusiveBetween(Glass.MinGlassLength, Glass.MaxGlassLength)
                .WithErrorCode(GlassValidationErrorCodes.GlassLengthOutOfBoundsError);

            // Height Boundaries
            RuleFor(g => g.Height)
                .InclusiveBetween(Glass.MinGlassHeight, Glass.MaxGlassHeight)
                .WithErrorCode(GlassValidationErrorCodes.GlassHeightOutOfBoundsError);

            // Step Boundaries
            RuleFor(g => g).Cascade(CascadeMode.Stop)
                .Must(g => (g.StepLength == 0 && g.StepHeight == 0))
                .When(g => g.GlassType == GlassTypeEnum.DoorGlass, ApplyConditionTo.CurrentValidator)
                .WithErrorCode(GlassValidationErrorCodes.DoorGlassWithStepError)

                .Must(g => (g.StepHeight > 0 && g.StepLength > 0) || (g.StepLength == 0 && g.StepHeight == 0))
                .WithErrorCode(GlassValidationErrorCodes.GlassStepInvalidDimensionsError)

                .Must(g => (g.StepHeight < g.Height) && (g.StepLength < g.Length))
                .WithErrorCode(GlassValidationErrorCodes.StepGreaterThanGlassError);

            //Draw must be Set
            RuleFor(g => g.Draw).NotEmpty().WithErrorCode(GlassValidationErrorCodes.GlassDrawNotSetError);
            //Type must be Set
            RuleFor(g => g.GlassType).NotEmpty().WithErrorCode(GlassValidationErrorCodes.GlassTypeNotSetError);
            //Thickness Must be Set
            RuleFor(g => g.Thickness).NotNull().NotEqual(GlassThicknessEnum.GlassThicknessNotSet).WithErrorCode(GlassValidationErrorCodes.GlassThicknessNotSetError);
            //Finish Must be Set
            RuleFor(g => g.Finish).NotNull().NotEqual(GlassFinishEnum.GlassFinishNotSet).WithErrorCode(GlassValidationErrorCodes.GlassFinishNotSetError);

            //Corner Radius Cannot Exceed Glass Length
            RuleFor(g => g).Must(g => (g.CornerRadiusTopLeft + g.CornerRadiusTopRight) < g.Length).WithErrorCode(GlassValidationErrorCodes.CornerRadiusOutOfBoundsError);

        }
    }
}
