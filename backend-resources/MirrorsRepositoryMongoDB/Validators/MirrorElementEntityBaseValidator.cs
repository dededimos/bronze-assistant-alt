using CommonHelpers;
using FluentValidation;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorElementEntityBaseValidator<T> : AbstractValidator<T> where T : MirrorElementEntity
    {
        public MirrorElementEntityBaseValidator(bool includeIdValidation, bool includeNameValidation)
        {
            // Validate the Entity
            RuleFor(e => e).SetValidator(new DatabaseEntityValidator(includeIdValidation));

            //Validate the Mirror Element
            RuleFor(e => e.LocalizedDescriptionInfo).SetValidator(new LocalizedDescriptionValidator(includeNameValidation));
            RuleFor(e => e.Code).NotEmpty().WithErrorCode("ElementCodeCannotBeEmpty");
            RuleForEach(e => e.Code).Must(c => c.IsCharachterGreek() == false).WithErrorCode("CodeContainsGreekCharacters");
            //RuleFor(e => e.PhotoUrl).NotEmpty().WithErrorCode("PhotoUrlCannotBeEmpty");
        }
    }
}
