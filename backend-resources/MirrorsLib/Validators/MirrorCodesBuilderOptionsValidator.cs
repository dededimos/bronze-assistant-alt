using FluentValidation;
using MirrorsLib.Services.CodeBuldingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators
{
    /// <summary>
    /// The Validator for Mirror Codes Builder Options
    /// </summary>
    public class MirrorCodesBuilderOptionsValidator : AbstractValidator<MirrorCodesBuilderOptions>
    {
        public MirrorCodesBuilderOptionsValidator()
        {
            //Options must at least refer to One Element of the Mirror , otherwise the generator is a simple code from  Length and Height or Separators
            RuleFor(o => o.MirrorPropertiesCodeAffix).NotEmpty().WithErrorCode($"AtLeastOneElementTypeAffixMustBePresent");
        }
    }
}
