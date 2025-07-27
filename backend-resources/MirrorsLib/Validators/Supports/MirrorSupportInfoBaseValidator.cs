using FluentValidation;
using MirrorsLib.MirrorElements.Supports;

namespace MirrorsLib.Validators.Supports
{
    public class MirrorSupportInfoBaseValidator : AbstractValidator<MirrorSupportInfo>
    {
        public MirrorSupportInfoBaseValidator()
        {
            RuleFor(s => s.SupportType).NotEmpty().WithErrorCode("UndefinedSupportType");
        }
    }
}
