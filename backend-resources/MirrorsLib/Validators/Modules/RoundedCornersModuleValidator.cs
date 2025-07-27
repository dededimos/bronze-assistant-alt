using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class RoundedCornersModuleValidator : AbstractValidator<RoundedCornersModuleInfo>
    {
        public RoundedCornersModuleValidator()
        {
            RuleFor(c => c).Must(c => c.TopLeft > 0 || c.TopRight > 0 || c.BottomLeft > 0 || c.BottomRight > 0).WithErrorCode("AllRadiusesZero");
        }
    }
}
