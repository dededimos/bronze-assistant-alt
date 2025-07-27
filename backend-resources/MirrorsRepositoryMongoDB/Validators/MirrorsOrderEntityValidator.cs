using FluentValidation;
using MirrorsLib.MirrorsOrderModels;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorsOrderEntityValidator : AbstractValidator<MirrorsOrderEntity> 
    {
        public MirrorsOrderEntityValidator(bool includeIdValidation)
        {
            RuleFor(e=>e).SetValidator(new BronzeOrderEntityBaseValidator(includeIdValidation));
            RuleFor(e => e.OrderNo).Must(n => MirrorsOrder.OrderNoRegex.IsMatch(n) && n != MirrorsOrder.newOrderNo).WithErrorCode("OrderNoCannotBeEmptyOrIncorrectType");
            RuleForEach(e => e.Rows).SetValidator(new MirrorOrderRowEntityValidator(includeIdValidation));
            RuleFor(e=> e).Must(e=> e.Rows.Select(r=> r.LineNumber).Distinct().Count() == e.Rows.Count).WithErrorCode("OrderRowsCannotHaveDuplicateLineNumbers");
        }
    }
}
