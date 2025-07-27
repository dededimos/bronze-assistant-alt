using CommonInterfacesBronze;
using FluentValidation;
using static MongoDbCommonLibrary.CommonValidators.CommonEntitiesValidationErrorCodes;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class LocalizedStringValidator : AbstractValidator<LocalizedString>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeDefaultValueValidation">Weather to Include the NotEmpty Validation on the Default Value</param>
        public LocalizedStringValidator(bool includeDefaultValueValidation)
        {
            if (includeDefaultValueValidation)
            {
                // Name cannot Have Empty Default Value
                RuleFor(s => s.DefaultValue)
                    .NotEmpty()
                    .WithErrorCode(LocalizedDefaultValueCannotBeEmpty);
            }

            When(s => s.LocalizedValues.Count > 0, () =>
            {
                RuleFor(st => st.DefaultValue)
                .NotEmpty()
                .WithErrorCode(LocalizedDefaultValueCannotBeEmpty);
                
                RuleFor(s => s.LocalizedValues)
                    .Must(lv => LocalizedValuesMustHaveAllLocales(lv))
                    .WithErrorCode(LocalizedStringAllLocales);
            });
        }

        /// <summary>
        /// Used in a Must Block to indicate that All Locales should exists
        /// When ther are any Locales at all
        /// </summary>
        /// <param name="localizedValues"></param>
        /// <returns></returns>
        public static bool LocalizedValuesMustHaveAllLocales(Dictionary<string, string> localizedValues)
        {
            return
                    localizedValues.ContainsKey(LocalizedString.GREEKIDENTIFIER) &&
                    localizedValues.ContainsKey(LocalizedString.ENGLISHIDENTIFIER) &&
                    localizedValues.ContainsKey(LocalizedString.ITALIANIDENTIFIER) &&
                    localizedValues.Values.All(v => string.IsNullOrEmpty(v) is false);
        }
    }
}
