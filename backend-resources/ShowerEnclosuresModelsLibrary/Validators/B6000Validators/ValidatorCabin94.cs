using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.B6000Validators
{
    public class ValidatorCabin94 : BaseValidatorCabin<Cabin94>
    {
        public ValidatorCabin94(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll) : base(validationType)
        {
            //Boundary for Overlap
            RuleFor(c => c.Constraints.Overlap)
                .NotEmpty().WithErrorCode(CabinValidationErrorCodes.OverlapGreaterThanZero);

            //Boundary for CoverDistance (Door must not hit wall on Opening)
            RuleFor(cabin => cabin)
                .Must(c => (c.Parts.WallProfile1 != null) && ((c.Parts.WallProfile1.ThicknessView - c.Constraints.CoverDistance)
                            >= c.Constraints.MinDoorDistanceFromWallOpened))
                .WithErrorCode(CabinValidationErrorCodes.CoverDistanceOutOfBounds);

            RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);
            When(c => c.Parts.Handle is not null, () =>
            {
                //HandleDistance not Zero
                RuleFor(c => c.Parts.Handle!.GetSlidingDoorAirDistance())
                    .NotEmpty().WithErrorCode(CabinValidationErrorCodes.HDGreaterThanZero);  //HandleDistanmce Cannot be Zero
            });
        }
    }
}
