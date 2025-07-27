using FluentValidation;
using MongoDbCommonLibrary.CommonEntities;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class BronzeOrderEntityBaseValidator : AbstractValidator<BronzeOrderEntity>
    {
        public BronzeOrderEntityBaseValidator(bool includeIdValidation = true)
        {
            RuleFor(e=> e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            RuleFor(e => e.OrderNo).NotEmpty().WithErrorCode("OrderNoCannotBeEmpty");
        }
    }
}
