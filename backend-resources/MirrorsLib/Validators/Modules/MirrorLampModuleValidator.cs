using FluentValidation;
using MirrorsLib.MirrorElements.MirrorExtras;

namespace MirrorsLib.Validators.Modules
{
    public class MirrorLampModuleValidator : AbstractValidator<MirrorLampModuleInfo>
    {
        public MirrorLampModuleValidator()
        {
            RuleFor(l => l.Watt).NotEmpty().WithErrorCode($"WattZero");
            RuleFor(l => l.LampBodyDimensions).Must(d => d.Height > 0 && d.Length > 0).WithErrorCode("LampBodyDimensionsZero");
            RuleFor(l => l.Kelvin).NotEmpty().WithErrorCode("KelvinZero");
            RuleFor(l => l.Lumen).NotEmpty().WithErrorCode("LumenZero");
            RuleFor(l => l.TotalLength).NotEmpty().WithErrorCode("TotalLengthZero");
            RuleFor(l => l.TotalHeight).NotEmpty().WithErrorCode("TotalHeightZero");
            RuleFor(l => l.TotalDepth).NotEmpty().WithErrorCode("TotalDepthZero");

        }
    }
}
