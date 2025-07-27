using FluentValidation;
using MirrorsRepositoryMongoDB.Entities;
using ShapesLibrary.Validators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorSynthesisEntityValidator : AbstractValidator<MirrorSynthesisEntity>
    {
        public MirrorSynthesisEntityValidator()
        {
            RuleFor(e => e.Code).NotEmpty().WithErrorCode("MirrorSynthesisCodeCannotBeEmpty");
            RuleFor(e => e.GlassCode).NotEmpty().WithErrorCode("MirrorSynthesisGlassCodeCannotBeEmpty");
            RuleFor(e => e.DimensionsInformation).SetValidator(new ShapeInfoValidator());
            RuleFor(e => e.MirrorGlassShape).SetValidator(new ShapeInfoValidator());
        }
    }
}
