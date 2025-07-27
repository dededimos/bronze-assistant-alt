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
    public class FixedGlassV4Builder : GlassBuilderBase<CabinV4>
    {
#nullable disable
        private readonly bool canHaveStep;

        public FixedGlassV4Builder(CabinV4 cabin, bool canHaveStep = false , GlassBuilderOptions options = null) : base(cabin,options)
        {
            this.canHaveStep = canHaveStep;
        }

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
            //Check if the Is step and Add it otherwise it is zero
            //Then Calculate the Supposed FixedLength
            //Add the Difference (Correction Length) if the Door Glass Maximum is Exceeded 
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            // Find the ALST - AL1 If an Aluminium is There else glass Gap Only
            (int wallFixerLength1, int innerGlassFixerLength1) = cabin.Parts.WallSideFixer switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };
            (int wallFixerLength2, int innerGlassFixerLength2) = cabin.Parts.WallFixer2 switch
            {
                Profile p => (p.ThicknessView, p.GlassInProfileDepth),
                CabinSupport s => (s.GlassGapAER, 0),
                _ => (0, 0),
            };

            double length;

            if (canHaveStep)
            {
                length = (cabin.LengthMin
                    + 3 * stepLength // -1*stepLength2 if we add second step!
                    - wallFixerLength1
                    - wallFixerLength2
                    - 2 * cabin.Parts.Handle.GetSlidingDoorAirDistance()
                    - 2 * cabin.Constraints.CoverDistance
                    - 2 * cabin.Parts.CloseStrip.OutOfGlassLength
                    + 2 * cabin.Constraints.Overlap)
                    / 4d
                    + innerGlassFixerLength1;
            }
            else //Second Fixed Glass (ususally does not have step)
            {
                length = (cabin.LengthMin
                    - stepLength // +3*stepLength2 if we add second step!
                    - wallFixerLength1
                    - wallFixerLength2
                    - 2 * cabin.Parts.Handle.GetSlidingDoorAirDistance()
                    - 2 * cabin.Constraints.CoverDistance
                    - 2 * cabin.Parts.CloseStrip.OutOfGlassLength
                    + 2 * cabin.Constraints.Overlap)
                    / 4d
                    + innerGlassFixerLength2;
            }

            double correctionLength = 0;
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLengthV4Calculation(cabin);
            if (cabin.Height <= cabin.Constraints.BreakpointHeight)
            {
                if (doorGlassCalculationLength > cabin.Constraints.MaxDoorLengthBeforeBreakpoint)
                {
                    correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLengthBeforeBreakpoint;
                    //Else do Nothing correctionLength = 0
                }
            }
            else        //For Heights > Than BreakPointHeight
            {
                if (doorGlassCalculationLength > cabin.Constraints.MaxDoorLengthAfterBreakpoint)
                {
                    correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLengthAfterBreakpoint;
                }
                //Else do Nothing correctionLength = 0
            }

            glass.Length = length + correctionLength;
        }
        public override void SetDefaultGlassStepHeight()
        {
            if (canHaveStep)
            {
                glass.StepHeight = GetStepGlassHeight(cabin);
            }
            else
            {
                glass.StepHeight = 0;
            }
        }
        public override void SetDefaultGlassStepLength()
        {
            if (canHaveStep)
            {
                glass.StepLength = GetStepGlassLength(cabin);
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
                CabinThicknessEnum.Thick8mm10mm => GlassThicknessEnum.Thick10mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }

        /// <summary>
        /// Returns the Step Height of the Fixed Glass that can have a Step in a V4 Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(CabinV4 cabin)
        {
            if (cabin.HasStep)
            {
                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance;
            }
            return 0;
        }
        /// <summary>
        /// Returns the Step Length of the Fixed Glass that can have a Step in a V4 Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassLength(CabinV4 cabin)
        {
            //If we have a glass that can HaveStep and a cabin that Has Step
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
