using FluentValidation;
using MirrorsLib.MirrorElements;

namespace MirrorsLib.Validators
{
    public class MirrorLightInfoValidator : AbstractValidator<MirrorLightInfo>
    {
        public MirrorLightInfoValidator()
        {
            RuleFor(l => l.Kelvin).NotEmpty().WithErrorCode("KelvinEmpty");
            RuleForEach(l => l.Kelvin).NotEmpty().WithErrorCode("KelvinZero");
            RuleFor(l => l.WattPerMeter).NotEmpty().WithErrorCode("WattPerMeterZero");
            RuleFor(l => l.Lumen).NotEmpty().WithErrorCode("LumenZero");
        }
    }
}
