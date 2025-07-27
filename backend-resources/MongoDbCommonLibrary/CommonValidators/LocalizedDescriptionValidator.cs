using CommonInterfacesBronze;
using FluentValidation;
using MongoDbCommonLibrary.CommonInterfaces;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class LocalizedDescriptionValidator : AbstractValidator<LocalizedDescription>
    {
        /// <summary>
        /// The Localized Description's  Validator Constructor
        /// </summary>
        /// <param name="includeNameValidation">Wheather to Include a Validator for the Name Property</param>
        public LocalizedDescriptionValidator(bool includeNameValidation = true)
        {
            RuleFor(d => d.Name).SetValidator(new LocalizedStringValidator(includeNameValidation));
            RuleFor(d => d.Description).SetValidator(new LocalizedStringValidator(false));
            RuleFor(d => d.ExtendedDescription).SetValidator(new LocalizedStringValidator(false));
        }
    }
}
