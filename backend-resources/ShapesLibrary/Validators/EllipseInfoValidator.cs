using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// Ellipse Info Validator
    /// </summary>
    public class EllipseInfoValidator : AbstractValidator<EllipseInfo>
    {
        public EllipseInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s.Length).GreaterThan(0).WithErrorCode($"{nameof(EllipseInfo.Length)} must be > 0");
            RuleFor(s => s.Height).GreaterThan(0).WithErrorCode($"{nameof(EllipseInfo.Height)} must be > 0");
            RuleFor(s=> s).Must(s=> s.RadiusMajor >= s.RadiusMinor).WithErrorCode($"{nameof(EllipseInfo.RadiusMajor)} must be >= {nameof(EllipseInfo.RadiusMinor)}");
        }
    }
}
