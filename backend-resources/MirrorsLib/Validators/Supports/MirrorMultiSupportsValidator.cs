using FluentValidation;
using MirrorsLib.MirrorElements.Supports;

namespace MirrorsLib.Validators.Supports
{
    public class MirrorMultiSupportsValidator : AbstractValidator<MirrorMultiSupports>
    {
        public MirrorMultiSupportsValidator()
        {
            RuleFor(s => s).SetValidator(new MirrorSupportInfoBaseValidator());
            RuleFor(s => s).Must(s => s.TopSupportsInstructions.SupportsNumber > 0 || s.BottomSupportsInstructions.SupportsNumber > 0).WithErrorCode("AtLeastOneSupportIsNeeded");
            RuleFor(s => s.TopSupportsInstructions).Must(ts => ts.Thickness > 0).When(s => s.TopSupportsInstructions.SupportsNumber > 0, ApplyConditionTo.CurrentValidator).WithErrorCode("TopSupportThicknessZero");
            RuleFor(s => s.BottomSupportsInstructions).Must(ts => ts.Thickness > 0).When(s => s.BottomSupportsInstructions.SupportsNumber > 0, ApplyConditionTo.CurrentValidator).WithErrorCode("BottomSupportThicknessZero");
        }
    }
}
