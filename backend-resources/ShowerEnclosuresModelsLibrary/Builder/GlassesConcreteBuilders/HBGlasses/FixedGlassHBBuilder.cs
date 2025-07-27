using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.HBGlasses
{
    public class FixedGlassHBBuilder : GlassBuilderBase<CabinHB>
    {
        public FixedGlassHBBuilder(CabinHB cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawHB1;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            double heightAdjustment = 0;

            heightAdjustment += cabin.Constraints.FinalHeightCorrection;

            // Add Bottom Fixer Extra Height
            heightAdjustment += cabin.Parts.BottomFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };

            //Equal to height , later on we can add SupportBar Option
            glass.Height = cabin.Height - heightAdjustment;
        }
        public override void SetDefaultGlassLength()
        {
            //if Partial Length is not set Return zero
            if (cabin.Constraints.PartialLength == 0)
            {
                throw new InvalidOperationException($"Cannot Calculate FixedHB Glass when {cabin.Constraints.PartialLength} is 0");
            }

            // Calculate the Adjustment from the WallFixer
            int wallFixerAdjustment = cabin.Parts.WallSideFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };

            // According to what Partial Length Represents , get the Glass' Length
            if (cabin.Constraints.LengthCalculation == LengthCalculationOption.ByDoorLength)
            {
                glass.Length = cabin.LengthMin
                    - cabin.Constraints.PartialLength
                    - wallFixerAdjustment
                    - (cabin.Parts.Hinge?.GlassGapAER ?? throw new InvalidOperationException($"Cannot Calculate HB Glass without Hinge"))
                    - (cabin.Parts.CloseProfile?.ThicknessView ?? 0);
            }
            else if (cabin.Constraints.LengthCalculation == LengthCalculationOption.ByFixedLength)
            {
                glass.Length = cabin.Constraints.PartialLength - wallFixerAdjustment;
            }
        }
        public override void SetDefaultGlassStepHeight()
        {
            glass.StepHeight = GetStepGlassHeight(cabin);
        }
        public override void SetDefaultGlassStepLength()
        {
            if (cabin.HasStep)
            {
                glass.StepLength = cabin.GetStepCut().StepLength;
            }
            else
            {
                glass.StepLength = 0;
            }
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
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }

        /// <summary>
        /// Returns the Step Height of the Fixed Glass that can have a Step in a HB Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(CabinHB cabin)
        {
            if (cabin.HasStep)
            {
                int bottomFixerAdj = cabin.Parts.BottomFixer switch
                {
                    Profile p => p.ThicknessView - p.GlassInProfileDepth,
                    CabinSupport s => s.GlassGapAER,
                    _ => 0,
                };
                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance
                    - bottomFixerAdj;
            }
            return 0;
        }

    }
}
