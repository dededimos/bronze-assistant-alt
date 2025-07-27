using FluentValidation;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators.B6000Validators
{
    public class ValidatorCabin9C : BaseValidatorCabin<Cabin9C>
    {
        public ValidatorCabin9C(ValidationTypeEnum validationType = ValidationTypeEnum.ValidateAll) : base(validationType)
        {

            ////Boundary for Overlap
            //RuleFor(c => c.OverlapEPIK)
            //    .GreaterThanOrEqualTo(Cabin9C.EPIK).WithMessage($"Eπικάλυψη >{Cabin9C.EPIK}mm");    //Overlap Always Greater than Default

            ////HandleDistance not Zero
            //RuleFor(c => c.HandleDistanceHD)
            //    .NotEmpty().WithMessage($"H Απόσταση Λαβής από την Άκρη πρέπει να είναι >0mm");  //HandleDistanmce Cannot be Zero

            ////Boundary for CoverDistance
            //RuleFor(c => c.CoverDistanceCD)
            //    .LessThanOrEqualTo(Cabin9C.CD).WithMessage($"Το CoverDistance πρέπει να είναι <{Cabin9C.CD}mm");

            When(c => c.GlassFinish == GlassFinishEnum.Serigraphy || c.GlassFinish == GlassFinishEnum.Frosted, () =>
            {
                RuleFor(c => c.NominalLength)
                    .Must(NominalLength => NominalLength is Cabin9C.AllowableSerigraphyLength1 or Cabin9C.AllowableSerigraphyLength2)
                    .WithErrorCode(CabinValidationErrorCodes.Cabin9CSerigraphyOutOfRangeLength);
            });

            When(c => c.HasStep, () =>
            {
                RuleFor(c => c.GlassFinish)
                    .Equal(GlassFinishEnum.Transparent)
                    .WithErrorCode(CabinValidationErrorCodes.Cabin9CStepWithoutTransparentGlass);
            });
        }
    }
}
