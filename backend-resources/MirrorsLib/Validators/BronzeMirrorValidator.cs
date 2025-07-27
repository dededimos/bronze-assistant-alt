using FluentValidation;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Validators
{
    public class BronzeMirrorValidator : AbstractValidator<MirrorSynthesis>
    {
        public const string MaxAllowedWattError = "MaxAllowedWattError";
        public const string AllowedGlassTypeError = "AllowedGlassTypeError";
        public const string AllowedGlassThicknessError = "AllowedGlassThicknessError";
        public const string AllowedSandblastError = "AllowedSandblastError";
        public const string AllowedSupportError = "AllowedSupportError";
        public const string AllowedLightError = "AllowedLightError";
        public const string CanHaveLightError = "CanHaveLightError";
        public const string AllowedModuleError = "AllowedModuleError";


        /// <summary>
        /// Validates a mirror with the given constraints
        /// </summary>
        /// <param name="c">The Mirror Constraints</param>
        public BronzeMirrorValidator(MirrorConstraints c)
        {
            //RuleFor(m => m.DimensionsInformation.Height).GreaterThanOrEqualTo(c.MinMirrorHeight).WithMessage(c.MinMirrorHeightErrorMessage);
            //RuleFor(m => m.DimensionsInformation.Height).LessThanOrEqualTo(c.MaxMirrorHeight).WithMessage(c.MaxMirrorHeightErrorMessage);
            //RuleFor(m => m.DimensionsInformation.Length).GreaterThanOrEqualTo(c.MinMirrorLength).WithMessage(c.MinMirrorLengthErrorMessage);
            //RuleFor(m => m.DimensionsInformation.Length).LessThanOrEqualTo(c.MaxMirrorLength).WithMessage(c.MaxMirrorLengthErrorMessage);
            
            RuleFor(m => m.GetTransformerNominalPower()).LessThanOrEqualTo(c.MaxAllowedWattage).WithErrorCode(MaxAllowedWattError);
            RuleFor(m => m.GlassType).Must(t => c.AllowedGlassTypes.Contains(t)).WithErrorCode(AllowedGlassTypeError);
            RuleFor(m => m.GlassThickness).Must(t => c.AllowedGlassThicknesses.Contains(t)).WithErrorCode(AllowedGlassThicknessError);

            When(m => m.Sandblast is not null, () =>
            {
                RuleFor(m => m.Sandblast).Must(s => c.AllowedSandblasts.Contains(s!.ElementId)).WithErrorCode(AllowedSandblastError);
            });

            When(m => m.Support is not null, () =>
            {
                RuleFor(m => m.Support).Must(s => c.AllowedSupports.Contains(s!.ElementId)).WithErrorCode(AllowedSupportError);
            });
            if (!c.CanHaveLight)
            {
                RuleFor(m => m.Lights).Empty().WithErrorCode(CanHaveLightError);
            }
            else
            {
                RuleForEach(m => m.Lights).Must(l => c.AllowedLights.Contains(l.LightElement.ElementId)).WithErrorCode(AllowedLightError);
            }

            RuleForEach(m => m.ModulesInfo.GetAllModules()).Must(mod => c.AllowedModules.Contains(mod.ElementId)).WithErrorCode(AllowedModuleError);
        }
    }
}
