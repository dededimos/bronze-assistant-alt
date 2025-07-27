using FluentValidation;

namespace ShapesLibrary.Validators
{
    /// <summary>
    /// The Base Shape Info Validator , used to validate the base properties of all the Shape info Objects
    /// </summary>
    public class BaseShapeInfoValidator : AbstractValidator<ShapeInfo>
    {
        public BaseShapeInfoValidator()
        {
            RuleFor(s => s.ShapeType).NotEqual(Enums.ShapeInfoType.Undefined).WithErrorCode("ShapeType must be defined");
        }
    }
}
