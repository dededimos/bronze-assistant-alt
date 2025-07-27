using FluentValidation;
using MirrorsLib.MirrorElements.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators.Supports
{
    public class MirrorVisibleFrameSupportValidator : AbstractValidator<MirrorVisibleFrameSupport>
    {
        public MirrorVisibleFrameSupportValidator()
        {
            RuleFor(s => s).SetValidator(new MirrorSupportInfoBaseValidator());
            RuleFor(s => s).Must(s => s.FrontThickness > 0 || s.Depth > 0 || s.RearThickness1 > 0).WithErrorCode("AtLeastOneDimensionNotZero");
            RuleFor(s => s).Must(s => s.GlassInProfile == 0).When(s => s.FrontThickness == 0, ApplyConditionTo.CurrentValidator).WithErrorCode("GlassInProfileCannotHaveValueWhenFrontThicknessIsZero");
            RuleFor(s => s).Must(s => s.RearThickness1 > 0).When(s=>s.RearThickness2 > 0,ApplyConditionTo.CurrentValidator).WithErrorCode("RearThickness1NotZeroWhen2HasValue");
        }
    }
}
