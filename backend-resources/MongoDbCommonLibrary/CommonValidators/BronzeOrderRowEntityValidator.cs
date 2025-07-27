using FluentValidation;
using MongoDbCommonLibrary.CommonEntities;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class BronzeOrderRowEntityValidator : AbstractValidator<BronzeOrderRowEntity>
    {
        public BronzeOrderRowEntityValidator(bool includeIdValidation)
        {
            RuleFor(e=> e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            RuleFor(dto=> dto.Quantity).NotEmpty().WithErrorCode($"OrderRowsCannotHaveZeroQuantity");
            RuleFor(dto=> dto).Must(dto=> dto.Quantity >= dto.PendingQuantity && dto.Quantity >= dto.FilledQuantity && dto.Quantity >= dto.CancelledQuantity).WithErrorCode("OrderRowQuantitiesCannotExceedTotalQuantity");
            RuleFor(dto=> dto.LineNumber).GreaterThanOrEqualTo(0).WithErrorCode("LineNumberCannotBeNegative");
        }
    }
}
