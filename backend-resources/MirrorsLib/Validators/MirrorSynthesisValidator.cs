using FluentValidation;
using ShapesLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators
{
    public class MirrorSynthesisValidator : AbstractValidator<MirrorSynthesis>
    {
        public MirrorSynthesisValidator()
        {
            RuleFor(g=> g.DimensionsInformation).SetValidator(new ShapeInfoValidator());
        }
    }
}
