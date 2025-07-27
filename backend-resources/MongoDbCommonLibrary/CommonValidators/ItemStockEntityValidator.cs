using FluentValidation;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class ItemStockEntityValidator : AbstractValidator<ItemStockEntity>
    {
        public ItemStockEntityValidator(bool includeIdValidation = true)
        {
            //Validate base Entity
            RuleFor(e => e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            RuleFor(e => e.Code).NotEmpty().WithErrorCode("StockedItemCodeIsUnspecified");
        }
    }
}
