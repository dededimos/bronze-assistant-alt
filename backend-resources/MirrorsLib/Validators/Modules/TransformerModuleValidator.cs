using FluentValidation;
using MirrorsLib.MirrorElements.MirrorModules;

namespace MirrorsLib.Validators.Modules
{
    public class TransformerModuleValidator : AbstractValidator<TransformerModuleInfo>
    {
        public TransformerModuleValidator()
        {
            RuleFor(t => t.Watt).NotEmpty().WithErrorCode("WattZero");
            RuleFor(t => t.TransformerDimensions).Must(d => d.Height > 0 && d.Length > 0).WithErrorCode("TransformerDimensionsZero");
        }
    }
}
