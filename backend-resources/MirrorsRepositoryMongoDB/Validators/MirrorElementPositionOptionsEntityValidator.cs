using FluentValidation;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorElementPositionOptionsEntityValidator : AbstractValidator<MirrorElementPositionOptionsEntity>
    {
        public MirrorElementPositionOptionsEntityValidator(bool includeIdValidation)
        {
            // Validate the Entity
            RuleFor(e => e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            // No validator for the options
        }
    }
}
