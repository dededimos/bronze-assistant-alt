using AccessoriesRepoMongoDB.Entities;
using FluentValidation;
using MongoDB.Bson;
using static AccessoriesRepoMongoDB.Validators.AccessoriesValidationErrorCodes;

namespace AccessoriesRepoMongoDB.Validators
{
    public class PrimaryTypeTraitEntityValidator : AbstractValidator<PrimaryTypeTraitEntity>
    {
        public PrimaryTypeTraitEntityValidator()
        {
            // Allowed Secondary Types cannot be Empty
            RuleFor(pt => pt.AllowedSecondaryTypes).NotEmpty().WithErrorCode(UndefinedAllowedSecondaryTypes);
            RuleForEach(pt => pt.AllowedSecondaryTypes).ChildRules(secondaryType =>
            {
                secondaryType.RuleFor(st => st).NotEqual(ObjectId.Empty).WithErrorCode(UndefiniedSecondaryType);
            });
        }
    }
}
