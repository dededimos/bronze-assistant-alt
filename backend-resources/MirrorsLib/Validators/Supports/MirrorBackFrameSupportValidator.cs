using FluentValidation;
using MirrorsLib.MirrorElements.Supports;

namespace MirrorsLib.Validators.Supports
{
    public class MirrorBackFrameSupportValidator : AbstractValidator<MirrorBackFrameSupport>
    {
        public MirrorBackFrameSupportValidator()
        {
            RuleFor(s => s).SetValidator(new MirrorSupportInfoBaseValidator());
            RuleFor(s => s.Depth).GreaterThan(0).WithErrorCode($"{nameof(MirrorBackFrameSupport.Depth)} must be greater than zero");
            RuleFor(s => s.Thickness).GreaterThan(0).WithErrorCode($"{nameof(MirrorBackFrameSupport.Thickness)} must be greater than zero");
            RuleFor(s => s.DistanceFromEdge).GreaterThanOrEqualTo(0).WithErrorCode($"{nameof(MirrorBackFrameSupport.DistanceFromEdge)} must not be negative");
        }
    }
}
