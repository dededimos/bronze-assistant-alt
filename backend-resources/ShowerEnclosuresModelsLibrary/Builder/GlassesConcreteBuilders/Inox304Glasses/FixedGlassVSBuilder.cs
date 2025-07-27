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
#nullable disable
    public class FixedGlassVSBuilder : GlassBuilderBase<CabinVS>
    {
        public FixedGlassVSBuilder(CabinVS cabin,GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawVA;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            glass.Height = cabin.Height - cabin.Constraints.FinalHeightCorrection;
        }
        public override void SetDefaultGlassLength()
        {
            //Check if it has step and Add it otherwise it is zero
            //Then Calculate the Supposed FixedLength
            //Add the Difference (Correction Length) if the Door Glass Maximum is Exceeded 
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }
            // Find the ALST - AL1 If an Aluminium is There else glass Gap Only
            (int wallFixer, int innerGlassFixer) = cabin.Parts.WallSideFixer switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };

            double length = (cabin.LengthMin
                + stepLength
                - wallFixer
                - (cabin.Parts.CloseProfile?.ThicknessView ?? 0)
                - cabin.Constraints.CoverDistance
                - cabin.Parts.Handle.GetSlidingDoorAirDistance()
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength)
                / 2d
                + innerGlassFixer;

            double correctionLength = 0;
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLengthVSCalculation(cabin);

            if (cabin.Height <= cabin.Constraints.BreakpointHeight)
            {
                if (doorGlassCalculationLength > cabin.Constraints.MaxDoorLengthBeforeBreakpoint)
                {
                    correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLengthBeforeBreakpoint;
                }
                //else do Nothing CorrectionLength = 0
            }
            else //for Heights > than the BreakPointHeight
            {
                if (doorGlassCalculationLength > cabin.Constraints.MaxDoorLengthAfterBreakpoint)
                {
                    correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLengthAfterBreakpoint;
                }
                //else do Nothing CorrectionLength = 0;
            }

            glass.Length = length + correctionLength;
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
        /// Returns the Step Height of the Fixed Glass that can have a Step in a VS Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(CabinVS cabin)
        {
            if (cabin.HasStep)
            {

                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance;
            }
            return 0;
        }
        /// <summary>
        /// Returns the Step Length of the Fixed Glass that can have a Step in a VS Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Length or zero if there is no step</returns>
        public static double GetStepGlassLength(CabinVS cabin)
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
                return cabin.GetStepCut().StepLength + stepCutLengthAdjustment;
            }
            return 0;
        }
    }
}
