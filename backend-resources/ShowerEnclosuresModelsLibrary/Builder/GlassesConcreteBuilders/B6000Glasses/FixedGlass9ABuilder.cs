using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses
{
    public class FixedGlass9ABuilder : GlassBuilderBase<Cabin9A>
    {
#nullable disable

        public FixedGlass9ABuilder(Cabin9A cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            //If cabin Does not Have step proceed with zero that has no effect in the equation
            double stepLength = 0;
            if (cabin.HasStep)
            {
                stepLength = cabin.GetStepCut().StepLength;
            }

            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }

            double length = (cabin.LengthMin
                + stepLength
                - cabin.Parts.Angle.AngleDistanceFromDoor
                - profile1Thickness
                - cabin.Constraints.CoverDistance
                - cabin.Parts.Handle.GetSlidingDoorAirDistance()
                + cabin.Constraints.Overlap
                - cabin.Parts.CloseStrip.OutOfGlassLength)
                / 2d
                + cabin.Parts.WallProfile1.GlassInProfileDepth;

            //Check if the Door Glass Surpasses maximum , if yes Add correction to the Fixed Glass
            double correctionLength = 0;
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLength9ACalculation(cabin);
            if (doorGlassCalculationLength > cabin.Constraints.MaxDoorGlassLength)
            {
                correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorGlassLength;
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
        /// Returns the Step Height of the Fixed Glass that can have a Step in a 9A Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(Cabin9A cabin)
        {
            if (cabin.HasStep)
            {
                //Step Height must factor the Height of the Horizontal Profile Bottom and How much the Glass is Inside it
                //Plus any additionall Air / Tollerance we want to give to the Glass
                return cabin.GetStepCut().StepHeight
                    + cabin.Constraints.StepHeightTollerance
                    - cabin.Parts.HorizontalProfileBottom.ThicknessView
                    + cabin.Parts.HorizontalProfileBottom.GlassInProfileDepth;
            }
            return 0;
        }
    }
}
