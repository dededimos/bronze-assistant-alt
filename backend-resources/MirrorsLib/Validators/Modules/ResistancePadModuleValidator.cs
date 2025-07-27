using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class ResistancePadModuleValidator : AbstractValidator<ResistancePadModuleInfo>
    {
        public ResistancePadModuleValidator()
        {
            RuleFor(d => d.Watt).NotEmpty().WithErrorCode($"WattZero");
            RuleFor(d => d.DemisterDimensions).Must(d => d.GetTotalHeight() > 0 && d.GetTotalLength() > 0).WithErrorCode("DemisterDimensionsZero");
        }
    }
}
