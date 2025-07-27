using FluentValidation;
using MongoDbCommonLibrary.CommonEntities;
using static MongoDbCommonLibrary.CommonValidators.CommonEntitiesValidationErrorCodes;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class DescriptiveEntityValidator<T> : DbEntityValidator<T> where T : DescriptiveEntity
    {
        /// <summary>
        /// The Descriptive Entities Validator Constructor
        /// </summary>
        /// <param name="includeIdValidation">Wheather to Include a Validator for the Id</param>
        /// <param name="includeNameValidation">Wheather to Include a Validator for the Name Property</param>
        public DescriptiveEntityValidator(bool includeIdValidation = true, bool includeNameValidation = true) : base(includeIdValidation)
        {
            if (includeNameValidation)
            {
                // Name cannot Have Empty Default Value
                RuleFor(de => de.Name.DefaultValue)
                    .NotEmpty()
                    .WithErrorCode(NameDefaultValueCannotBeEmpty);
            }

            When(de => de.Name.LocalizedValues.Count > 0, () =>
            {
                // Name Must have Present ALL LOCALES
                RuleFor(de => de.Name.LocalizedValues)
                    .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                    .WithErrorCode(NameAllLocales);
            });

            // When there is at least one Locale in Description All others must be Present as well as a Default Value
            When(de => de.Description.LocalizedValues.Count > 0, () =>
            {
                RuleFor(de => de.Description.DefaultValue)
                .NotEmpty()
                .WithErrorCode(DescriptionDefaultUndefined);

                RuleFor(de => de.Description.LocalizedValues)
                    .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                    .WithErrorCode(DescriptionAllOrNoneLocales);
            });

            // When there is at least one Locale in Extended Description All others must be Present as well as a Default Value
            When(de => de.ExtendedDescription.LocalizedValues.Count > 0, () =>
            {
                RuleFor(de => de.ExtendedDescription.DefaultValue)
                .NotEmpty()
                .WithErrorCode(ExtendedDescriptionDefaultUndefined);

                RuleFor(de => de.ExtendedDescription.LocalizedValues)
                    .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                    .WithErrorCode(ExtendedDescriptionAllOrNoneLocales);
            });
        }
    }
}
