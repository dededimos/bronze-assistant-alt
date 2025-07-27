using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class MagnifyerModuleValidator : AbstractValidator<MagnifierModuleInfo>
    {
        public MagnifyerModuleValidator()
        {
            RuleFor(m => m.MagnifierDimensions.Radius).NotEmpty().WithErrorCode($"MagnifyerRadiusZero");
            RuleFor(m => m.VisibleMagnifierDimensions.Radius).NotEmpty().WithErrorCode($"VisibleMagnifyerRadiusZero");
            RuleFor(m => m.Magnification).NotEmpty().WithErrorCode($"MagnificationZero");
        }
    }
}
