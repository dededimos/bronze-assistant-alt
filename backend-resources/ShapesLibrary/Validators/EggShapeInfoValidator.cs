using FluentValidation;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// Egg Shape Info Validator
    /// </summary>
    public class EggShapeInfoValidator: AbstractValidator<EggShapeInfo>
    {
        public EggShapeInfoValidator()
        {
            RuleFor(s => s).SetValidator(new BaseShapeInfoValidator());

            //Height cannot be smaller than half Length when Vertical and The opposite when Horizontal
            //This because the Length is determined by the Circles Radius . Thus Radius x 2 = length , and Height = RadiusCircle + Radius Ellipse.
            //The Ellipse's Radius cannot be Negative
            //IRRELEVANT ALWAYS TRUE ?! MUST CHECK VALIDATORS OF LENGTH HEIGHT 
            //RuleFor(s => s).Must(s => (s.EllipseRadius + s.Length) >= s.CircleRadius * 3).WithMessage($"Length + Height must Always be at least 3 Times the Circle's Radius in an Egg Shape");
            RuleFor(s => s.EllipseRadius).GreaterThan(0).WithErrorCode($"Ellipse Radius Must > 0");
            RuleFor(s => s.CircleRadius).GreaterThan(0).WithErrorCode($"Circle Radius Must be > 0 ");
        }
    }
}
