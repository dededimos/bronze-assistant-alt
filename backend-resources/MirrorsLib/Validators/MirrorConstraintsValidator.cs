using FluentValidation;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators
{
    public class MirrorConstraintsValidator : AbstractValidator<MirrorConstraints>
    {
        public MirrorConstraintsValidator()
        {
            RuleFor(c => c.ConcerningMirrorShape).Must(shape => shape != BronzeMirrorShape.UndefinedMirrorShape).WithErrorCode($"{nameof(MirrorConstraints.ConcerningMirrorShape)}Undefined");
            
            RuleFor(c => c.AllowedGlassTypes).NotEmpty().WithErrorCode($"{nameof(MirrorConstraints.AllowedGlassTypes)}Empty");
            
            RuleFor(c => c.AllowedGlassThicknesses).NotEmpty().WithErrorCode($"{nameof(MirrorConstraints.AllowedGlassThicknesses)}Empty");

            RuleFor(c => c.MaxMirrorLength).GreaterThan(0).WithErrorCode($"{nameof(MirrorConstraints.MaxMirrorLength)}Zero");
            
            RuleFor(c => c.MinMirrorLength).GreaterThan(0).WithErrorCode($"{nameof(MirrorConstraints.MinMirrorLength)}Zero");
            RuleFor(c => c.MinMirrorLength).LessThanOrEqualTo(c => c.MaxMirrorLength).WithErrorCode($"{nameof(MirrorConstraints.MinMirrorLength)}GreaterThan{nameof(MirrorConstraints.MaxMirrorLength)}");

            RuleFor(c => c.MaxMirrorHeight).GreaterThan(0).WithErrorCode($"{nameof(MirrorConstraints.MaxMirrorHeight)}Zero");

            RuleFor(c => c.MinMirrorHeight).GreaterThan(0).WithErrorCode($"{nameof(MirrorConstraints.MinMirrorHeight)}Zero");
            RuleFor(c => c.MinMirrorHeight).LessThanOrEqualTo(c => c.MaxMirrorHeight).WithErrorCode($"{nameof(MirrorConstraints.MinMirrorHeight)}GreaterThan{nameof(MirrorConstraints.MaxMirrorHeight)}");

            RuleFor(c=> c.AllowedLights).Must(l=> l.Count > 0).When(c=> c.AllowedIllumination > 0).WithErrorCode($"{nameof(MirrorConstraints.AllowedLights)}CannotBeEmptyWith{nameof(IlluminationOption)}");
        }
    }
}
