using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// The Circle Quadrant Validator
    /// </summary>
    public class CircleQuadrantInfoValidator : AbstractValidator<CircleQuadrantInfo>
    {
        public CircleQuadrantInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s.Radius).GreaterThan(0).WithErrorCode($"{nameof(CircleQuadrantInfo.Radius)} must be > 0");
        }
    }
}
