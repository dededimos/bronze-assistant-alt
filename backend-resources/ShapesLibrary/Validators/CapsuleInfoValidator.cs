using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// The Capsule Info Validator
    /// </summary>
    public class CapsuleInfoValidator : AbstractValidator<CapsuleInfo>
    {
        public CapsuleInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s.MajorLength).GreaterThan(0).WithErrorCode($"{nameof(CapsuleInfo.MajorLength)} must be > 0");
            RuleFor(s => s.MinorLength).GreaterThan(0).WithErrorCode($"{nameof(CapsuleInfo.MinorLength)} must be > 0");
            RuleFor(s => s).Must(s => s.MajorLength >= s.MinorLength).WithErrorCode($"{nameof(CapsuleInfo.MajorLength)} must be >= {nameof(CapsuleInfo.MinorLength)}");
        }
    }
}
