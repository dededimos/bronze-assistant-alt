using FluentValidation;
using MirrorsLib.MirrorElements.Sandblasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators.Sandblasts
{
    public class MirrorSandblastInfoValidator : AbstractValidator<MirrorSandblastInfo>
    {
        public MirrorSandblastInfoValidator()
        {
            RuleFor(s => s.Thickness).NotEmpty().WithErrorCode("ThicknessEmpty");
            RuleFor(s => s.SandblastType).NotEmpty().WithErrorCode("SandblastTypeUndefined");
        }
    }

}
