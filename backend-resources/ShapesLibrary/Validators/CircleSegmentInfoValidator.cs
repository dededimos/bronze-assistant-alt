using FluentValidation;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// Circle Segment Info Validator
    /// </summary>
    public class CircleSegmentInfoValidator : AbstractValidator<CircleSegmentInfo>
    {
        public CircleSegmentInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s).Must(s => s.Length >= s.Height).When(s => s.Orientation == Enums.CircleSegmentOrientation.PointingTop || s.Orientation == Enums.CircleSegmentOrientation.PointingBottom, ApplyConditionTo.CurrentValidator)
                .WithErrorCode("Must Length >= Height in Current Orientation");
            RuleFor(s => s).Must(s => s.Height >= s.Length).When(s => s.Orientation == Enums.CircleSegmentOrientation.PointingRight || s.Orientation == Enums.CircleSegmentOrientation.PointingLeft, ApplyConditionTo.CurrentValidator)
                .WithErrorCode("Must Height >= Length in Current Orientation");

            RuleFor(s => s.ChordLength).GreaterThanOrEqualTo(0).WithErrorCode("ChordLength Must be >0");
            RuleFor(s => s.Sagitta).GreaterThan(0).WithErrorCode("Sagitta Must be >0");
            //RuleFor(s => s).Must(s => MathCalculations.CircleSegment.GetRadius(s.ChordLength,s.Sagitta) >= s.Sagitta / 2d).WithErrorCode("Segment Radius must be >= Sagitta/2");
        }
    }
}
