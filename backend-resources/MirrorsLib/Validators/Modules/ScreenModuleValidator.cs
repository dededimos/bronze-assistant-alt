using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class ScreenModuleValidator : AbstractValidator<ScreenModuleInfo>
    {
        public ScreenModuleValidator()
        {
            RuleFor(s => s.Watt).NotEmpty().WithErrorCode($"WattZero");
            RuleFor(s => s.RearDimensions).Must(d => d.Height > 0 && d.Length > 0).WithErrorCode("ScreenDimensionsZero");
        }
    }
}
