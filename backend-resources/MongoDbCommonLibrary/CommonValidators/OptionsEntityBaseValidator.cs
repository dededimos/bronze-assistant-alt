using FluentValidation;
using MongoDbCommonLibrary.CommonEntities;
using static MongoDbCommonLibrary.CommonValidators.CommonEntitiesValidationErrorCodes;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class OptionsEntityBaseValidator : AbstractValidator<MongoOptionsEntity>
    {
        public OptionsEntityBaseValidator(bool includeIdValidation = true)
        {
            //Validate base Entity
            RuleFor(e=>e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            RuleFor(e => e.ConcerningApplication).NotEmpty().WithErrorCode("ConcerningApplicationIsUnspecified");
            RuleFor(e=> e.OptionsType).NotEmpty().WithErrorCode(UndefinedOptionsType);
        }
    }
}
