using AccessoriesRepoMongoDB.Entities;
using BathAccessoriesModelsLibrary;
using FluentValidation;
using MongoDbCommonLibrary.CommonValidators;
using static AccessoriesRepoMongoDB.Validators.AccessoriesValidationErrorCodes;

namespace AccessoriesRepoMongoDB.Validators
{
    public class TraitEntityValidator : DbEntityValidator<TraitEntity>
    {
        public TraitEntityValidator(bool includeIdValidation = true):base(includeIdValidation)
        {
            // Trait Type Cannot be Undefined
            RuleFor(t => t.TraitType).NotEmpty().WithErrorCode(UndefinedTraitType);
            
            // Code in Finish Cannot Be Undefined
            RuleFor(t => t.Code)
                .NotEmpty()
                .When(t => t.TraitType == TypeOfTrait.FinishTrait,ApplyConditionTo.CurrentValidator)
                .WithErrorCode(UndefinedFinishCode);

            RuleFor(t => t.Trait.DefaultValue)
                .NotEmpty()
                .WithErrorCode(UndefinedTraitDefaultValue);

            RuleFor(de => de.Trait.LocalizedValues)
                .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                .WithErrorCode(AllLocalesTrait);

            // When there is at least one Locale in Tooltip All others must be Present as well as a Default Value
            When(t => t.TraitTooltip.LocalizedValues.Count > 0, () =>
            {
                RuleFor(t => t.TraitTooltip.DefaultValue)
                .NotEmpty()
                .WithErrorCode(UndefiniedTraitTooltipDefaultValue);

                RuleFor(t => t.TraitTooltip.LocalizedValues)
                    .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                    .WithErrorCode(AllOrNoneLocalesTraitTooltip);
            });

            // Add the Validator only for the Primary Type Also
            RuleFor(t => t).SetInheritanceValidator(v => v.Add(new PrimaryTypeTraitEntityValidator()));
        }
    }
}
