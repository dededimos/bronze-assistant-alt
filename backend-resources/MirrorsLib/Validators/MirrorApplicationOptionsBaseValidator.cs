using FluentValidation;
using MirrorsLib.Services.CodeBuldingService;

namespace MirrorsLib.Validators
{
    /// <summary>
    /// The Validator for ALL MirrorApplicationOptions
    /// </summary>
    public class MirrorApplicationOptionsBaseValidator : AbstractValidator<MirrorApplicationOptionsBase>
    {
        public MirrorApplicationOptionsBaseValidator()
        {
            RuleFor(e => e).SetInheritanceValidator(v =>
            {
                v.Add<MirrorCodesBuilderOptions>(new MirrorCodesBuilderOptionsValidator());
            });
        }
    }
}
