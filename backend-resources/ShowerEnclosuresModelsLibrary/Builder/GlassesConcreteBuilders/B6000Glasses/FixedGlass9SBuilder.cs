using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses
{
    public class FixedGlass9SBuilder : GlassBuilderBase<Cabin9S>
    {
#nullable disable

        public FixedGlass9SBuilder(Cabin9S cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            //Calculate the Supposed length of the Fixed Glass , if the DoorCalculation is Bigger than Allowed
            //then Correct the Fixed Part with the Difference
            double stepLength = 0;
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

            double length = (cabin.LengthMin
                + stepLength
                - profile1Thickness
                - profile2Thickness
                - cabin.Constraints.CoverDistance
                - cabin.Parts.Handle.GetSlidingDoorAirDistance()
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength)
                / 2d
                + cabin.Parts.WallProfile1.GlassInProfileDepth;

            double correctionLength = 0;
            double glassDoorCalculationLength = GlassBuilderHelpers.DoorGlassLength9SCalculation(cabin);
            if (glassDoorCalculationLength > cabin.Constraints.MaxDoorGlassLength)
            {
                correctionLength = glassDoorCalculationLength - cabin.Constraints.MaxDoorGlassLength;
            }

            glass.Length = length + correctionLength;
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
        /// Returns the Step Height of the Fixed Glass that can have a Step in a 9S Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(Cabin9S cabin)
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
    }
}
