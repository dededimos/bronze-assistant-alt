using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses
{
    public class FixedGlass94Builder : GlassBuilderBase<Cabin94>
    {
#nullable disable
        private readonly bool canHaveStep;

        public FixedGlass94Builder(Cabin94 cabin, bool canHaveStep = false, GlassBuilderOptions options = null) : base(cabin,options)
        {
            this.canHaveStep = canHaveStep;
        }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawF;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            // The Glass is a little inside the Horizontal Profiles on both ends (top-bottom)
            glass.Height = cabin.Height
                - cabin.Parts.HorizontalProfileTop.ThicknessView
                - cabin.Parts.HorizontalProfileBottom.ThicknessView

                + cabin.Parts.HorizontalProfileTop.GlassInProfileDepth
                + cabin.Parts.HorizontalProfileBottom.GlassInProfileDepth;
        }
        public override void SetDefaultGlassLength()
        {
            int stepLength = 0; //If cabin Has Step Set it otherwise its zero
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            int profile2Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }


            double length;
            if (canHaveStep)
            {
                length = (cabin.LengthMin
                    + 3 * stepLength //- 1*StepLength2 (if we put a second step!)
                    - profile1Thickness
                    - profile2Thickness
                    - 2 * cabin.Parts.Handle.GetSlidingDoorAirDistance()
                    - 2 * cabin.Constraints.CoverDistance
                    - 2 * cabin.Parts.CloseStrip.OutOfGlassLength
                    + 2 * cabin.Constraints.Overlap)
                    / 4d
                    + cabin.Parts.WallProfile1.GlassInProfileDepth;
            }
            else //glass on the right side (usually without step)
            {
                length = (cabin.LengthMin
                    - stepLength // +3*StepLength2 (if we put second step!)
                    - profile1Thickness
                    - profile2Thickness
                    - 2 * cabin.Parts.Handle.GetSlidingDoorAirDistance()
                    - 2 * cabin.Constraints.CoverDistance
                    - 2 * cabin.Parts.CloseStrip.OutOfGlassLength
                    + 2 * cabin.Constraints.Overlap)
                    / 4d
                    + cabin.Parts.WallProfile2.GlassInProfileDepth;
            }

            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLength94Calculation(cabin);   //Calculation of Supposed DoorGlassLength
            double lengthCorrection = 0;
            if (doorGlassCalculationLength > cabin.Constraints.MaxDoorGlassLength)                               //If the Length is Bigger than allowed
            {                                                                                           //If the Door Glass is More than the Allowed Maximum , we must correct the Fixed Part,to be that much Bigger
                lengthCorrection = doorGlassCalculationLength - cabin.Constraints.MaxDoorGlassLength;             //Then we correct with the difference passed to the Fixed Glass
            }

            glass.Length = length + lengthCorrection;
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
            //If we have a glass that can HaveStep and a cabin that Has Step
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
                CabinThicknessEnum.Thick5mm => GlassThicknessEnum.Thick5mm,
                CabinThicknessEnum.Thick6mm => GlassThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick8mm => GlassThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick6mm8mm => GlassThicknessEnum.Thick8mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }

        /// <summary>
        /// Returns the Step Height of the Fixed Glass that can have a Step in a 94 Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(Cabin94 cabin)
        {
            if (cabin.HasStep)
            {
                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance
                    - cabin.Parts.HorizontalProfileBottom.ThicknessView
                    + cabin.Parts.HorizontalProfileBottom.GlassInProfileDepth;
            }
            return 0;
        }
        /// <summary>
        /// Returns the Step Length of the Fixed Glass that can have a Step in a 94 Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Length or zero if there is no step</returns>
        public static double GetStepGlassLength(Cabin94 cabin)
        {
            //If we have a glass that can HaveStep and a cabin that Has Step
            if (cabin.HasStep)
            {
                return cabin.GetStepCut().StepLength;
            }
            return 0;
        }

    }
}
