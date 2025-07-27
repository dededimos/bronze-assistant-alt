using FluentValidation;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorOrderRowEntityValidator : AbstractValidator<MirrorOrderRowEntity>
    {
        public MirrorOrderRowEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetValidator(new BronzeOrderRowEntityValidator(includeIdValidation));
            RuleFor(e => e.RowItem)
                .NotNull().WithErrorCode("RowItemCannotBeNull")
                .SetValidator(new MirrorSynthesisEntityValidator()!);
        }
    }
}
