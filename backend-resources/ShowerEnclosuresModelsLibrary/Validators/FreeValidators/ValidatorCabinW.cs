using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.FreeValidators
{
    public class ValidatorCabinW : BaseValidatorCabin<CabinW>
    {
        public ValidatorCabinW(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            When(c => c.Parts is IPerimetricalFixer fixer && fixer.HasPerimetricalFrame, () =>
            {
                RuleFor(c => c.Constraints.CornerRadiusTopEdge).Equal(0).WithErrorCode(CabinValidationErrorCodes.PerimetricalFrameWithRounding);
            });
        }
    }
}
