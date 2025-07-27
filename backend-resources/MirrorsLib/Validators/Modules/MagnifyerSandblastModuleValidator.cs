using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class MagnifyerSandblastModuleValidator : AbstractValidator<MagnifierSandblastedModuleInfo>
    {
        public MagnifyerSandblastModuleValidator()
        {
            Include(new MagnifyerModuleValidator());
            RuleFor(m => m.SandblastDimensions.Radius).NotEmpty().WithErrorCode($"SandblastRadiusZero");
        }
    }
}
