using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// The Circle Info Validator
    /// </summary>
    public class CircleInfoValidator : AbstractValidator<CircleInfo>
    {
        public CircleInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s.Radius).GreaterThan(0).WithErrorCode($"{nameof(CircleInfo.Radius)} must be > 0");

        }
    }
}
