using FluentValidation;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// The Validator for the Shape Info Class . Validates any derived class of the Shape Info Inheritance chain.
    /// </summary>
    public class ShapeInfoValidator : AbstractValidator<ShapeInfo>
    {
        public ShapeInfoValidator()
        {
            RuleFor(s => s).SetInheritanceValidator(v =>
            {
                v.Add(new RectangleInfoValidator());
                v.Add(new CircleInfoValidator());
                v.Add(new EllipseInfoValidator());
                v.Add(new CapsuleInfoValidator());
                v.Add(new CircleSegmentInfoValidator());
                v.Add(new CircleQuadrantInfoValidator());
                v.Add(new EggShapeInfoValidator());
                v.Add(new RegularPolygonInfoValidator());
                v.Add<UndefinedShapeInfo>(new BaseShapeInfoValidator());
                v.Add<ShapeInfo>(new BaseShapeInfoValidator());
            });
        }
    }
}
