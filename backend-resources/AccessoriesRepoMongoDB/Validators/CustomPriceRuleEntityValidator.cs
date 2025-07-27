using AccessoriesRepoMongoDB.Entities;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using FluentValidation;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Validators
{
    public class CustomPriceRuleEntityValidator : DescriptiveEntityValidator<CustomPriceRuleEntity>
    {
        public CustomPriceRuleEntityValidator(bool includeIdValidation = true) : base(includeIdValidation)
        {
            RuleFor(e => e.RuleValueType).NotEmpty().WithErrorCode($"{nameof(CustomPriceRuleEntity.RuleValueType)}Undefined");
            RuleFor(e => e.Conditions).NotEmpty().WithErrorCode("ConditionsListEmpty");
            
            // Name Must have Present Locales (The Rest Values Validation is taken care from the Descriptive Entity)
            RuleFor(de => de.Name.LocalizedValues).NotEmpty()
                .WithErrorCode("RulesMustHaveNameToLocales");
            
            // Description Must have Present Locales (The Rest Values Validation is taken care from the Descriptive Entity)
            RuleFor(de => de.Description.LocalizedValues).NotEmpty()
                .WithErrorCode("RulesMustHaveDescriptionToLocales");
            
            RuleForEach(e => e.Conditions).ChildRules(condition =>
            {
                // Target Condition Id Cannot be Empty when Rule Applies to Something Specific , Otherwise thaT specific (Trait/Price/Accessory) is not known
                condition.RuleFor(c => c.ConditionTargetValue).NotEmpty().When(c => PriceRuleCondition.ConditionsNotNeedingTargetValue().Any(cond=> cond == c.Condition) is false , ApplyConditionTo.CurrentValidator).WithErrorCode("TargetConditionIdEmpty");
            });
        }
    }
}
