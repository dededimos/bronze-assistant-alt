using FluentValidation;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// Validator for the Rectangle Info Class
    /// </summary>
    public class RectangleInfoValidator : AbstractValidator<RectangleInfo>
    {
        public RectangleInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());
            RuleFor(s=>s.Height).GreaterThan(0).WithErrorCode("Height Must be Greater than 0");
            RuleFor(s=>s.Length).GreaterThan(0).WithErrorCode("Length Must be Greater than 0");
            RuleFor(s => s).Must(s => (s.TopLeftRadius + s.BottomLeftRadius) <= s.Height).WithErrorCode($"Corner Radius Cannot be Bigger than The Shapes Measure");
            RuleFor(s => s).Must(s => (s.TopRightRadius + s.BottomRightRadius) <= s.Height).WithErrorCode($"Corner Radius Cannot be Bigger than The Shapes Measure");
            RuleFor(s => s).Must(s => (s.TopLeftRadius + s.TopRightRadius) <= s.Length).WithErrorCode($"Corner Radius Cannot be Bigger than The Shapes Measure");
            RuleFor(s => s).Must(s => (s.BottomLeftRadius + s.BottomRightRadius) <= s.Length).WithErrorCode($"Corner Radius Cannot be Bigger than The Shapes Measure");
        }
    }
}
