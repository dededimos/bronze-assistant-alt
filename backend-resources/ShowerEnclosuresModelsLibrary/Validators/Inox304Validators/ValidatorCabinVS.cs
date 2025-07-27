using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.Inox304Validators
{
    public class ValidatorCabinVS : BaseValidatorCabin<CabinVS>
    {
        public ValidatorCabinVS(ValidationTypeEnum validationType=ValidationTypeEnum.ValidateAll):base(validationType)
        {
            //Boundary for Overlap
            RuleFor(c => c.Constraints.Overlap)
                .NotEmpty().WithErrorCode(CabinValidationErrorCodes.OverlapGreaterThanZero);

            When(c => c.Parts.Handle is not null, () =>
            {
                //HandleDistance not Zero
                RuleFor(c => c.Parts.Handle!.GetSlidingDoorAirDistance())
                    .NotEmpty().WithErrorCode(CabinValidationErrorCodes.HDGreaterThanZero);  //HandleDistanmce Cannot be Zero
            });

            //If With Profile -- Otherwise always with Supports
            When(c => c.Parts.WallSideFixer is Profile, () =>
            {
                RuleFor(cabin => cabin)
                .Must(c => (((Profile)c.Parts.WallSideFixer)!.ThicknessView - c.Constraints.CoverDistance)
                            >= c.Constraints.MinDoorDistanceFromWallOpened)
                .WithErrorCode(CabinValidationErrorCodes.CoverDistanceOutOfBounds);
            }).Otherwise(() =>
            {
                RuleFor(cabin => cabin)
                .Must(c => (c.Constraints.CoverDistance)
                           <= c.Constraints.MinDoorDistanceFromWallOpened)
                .WithErrorCode(CabinValidationErrorCodes.CoverDistanceOutOfBounds);
            });

            RuleFor(cabin => cabin.Parts.Handle).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutHandleError);
            RuleFor(cabin => cabin.Parts.CloseStrip).NotEmpty().WithErrorCode(CabinValidationErrorCodes.WithoutCloserError);

        }
    }
}
