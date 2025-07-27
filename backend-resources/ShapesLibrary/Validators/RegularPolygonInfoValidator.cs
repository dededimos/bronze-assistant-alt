using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    public class RegularPolygonInfoValidator : AbstractValidator<RegularPolygonInfo>
    {
        public RegularPolygonInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s => s.NumberOfSides).GreaterThan(2).WithErrorCode($"{nameof(RegularPolygonInfo.NumberOfSides)} must be > 2");
            RuleFor(s => s.GetTotalLength()).GreaterThan(0).WithErrorCode($"{nameof(RegularPolygonInfo.GetTotalLength)} must be > 0");
            RuleFor(s => s.GetTotalHeight()).GreaterThan(0).WithErrorCode($"{nameof(RegularPolygonInfo.GetTotalHeight)} must be > 0");
        }
    }
}
