using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class TouchButtonModuleValidator : AbstractValidator<TouchButtonModuleInfo>
    {
        public TouchButtonModuleValidator()
        {
            RuleFor(t => t.Watt).NotEmpty().WithErrorCode("WattZero");
            RuleFor(t => t.RearDimensions).Must(d => d.Height > 0 && d.Length > 0).WithErrorCode("RearButtonDimensionsZero");
            RuleFor(t => t.FrontDimensionsButton).Must(d => d.GetTotalHeight() > 0 && d.GetTotalLength() > 0).WithErrorCode("FrontButtonDimensionsZero");
        }
    }
}
