using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.Inox304Glasses
{
    public class FixedGlassVFBuilder : GlassBuilderBase<CabinVF>
    {
        public FixedGlassVFBuilder(CabinVF cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawVF;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            var bottomFixer = cabin.Parts.BottomFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };

            glass.Height = cabin.Height
                - bottomFixer
                - cabin.Constraints.FinalHeightCorrection;
        }
        public override void SetDefaultGlassLength()
        {
            // Factor in the Lengths taken by the Fixing Parts of the Wall and the Side
            var wallFixer = cabin.Parts.WallSideFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };
            var sideFixer = cabin.Parts.SideFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };

            glass.Length = cabin.LengthMin
                - wallFixer
                - sideFixer;
        }
        public override void SetDefaultGlassStepHeight()
        {
            glass.StepHeight = GetStepGlassHeight(cabin);
        }
        public override void SetDefaultGlassStepLength()
        {
            glass.StepLength = GetStepGlassLength(cabin);
        }
        public override void SetDefaultCornerRadius()
        {
            glass.CornerRadiusTopLeft = 0;
            glass.CornerRadiusTopRight = 0;
        }
        public override void SetDefaultGlassThickness()
        {
            glass.Thickness = cabin.Thicknesses switch
            {
                CabinThicknessEnum.Thick8mm => GlassThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm => GlassThicknessEnum.Thick10mm,
                CabinThicknessEnum.ThickTenplex10mm => GlassThicknessEnum.ThickTenplex10mm,
                CabinThicknessEnum.Thick8mm10mm => GlassThicknessEnum.Thick10mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }

        /// <summary>
        /// Returns the Step Height of the Fixed Glass that can have a Step in a VF Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(CabinVF cabin)
        {
            if (cabin.HasStep)
            {
                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance;
            }
            return 0;
        }
        /// <summary>
        /// Returns the Step Length of the Fixed Glass that can have a Step in a VF Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Length or zero if there is no step</returns>
        public static double GetStepGlassLength(CabinVF cabin)
        {
            if (cabin.HasStep)
            {
                // If there is already available tollerance greater than 5 then 
                // do not Adjust , otherwise adjust until tollerance is 5
                int stepCutLengthAdjustment = cabin.Parts.WallSideFixer switch
                {
                    Profile p => p.Tollerance < cabin.Constraints.StepLengthTolleranceMin
                    ? Math.Abs(p.Tollerance - cabin.Constraints.StepLengthTolleranceMin)
                    : 0,
                    CabinSupport s => s.Tollerance < cabin.Constraints.StepLengthTolleranceMin
                    ? Math.Abs(s.Tollerance - cabin.Constraints.StepLengthTolleranceMin)
                    : 0,
                    _ => cabin.Constraints.StepLengthTolleranceMin
                };
                return cabin.GetStepCut().StepLength
                    + stepCutLengthAdjustment;
            }
            return 0;
        }
    }
}
