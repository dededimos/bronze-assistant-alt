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
    public class FixedGlass9BBuilder : GlassBuilderBase<Cabin9B>
    {
#nullable disable

        public FixedGlass9BBuilder(Cabin9B cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            
            // Apply also this as the Fixed Part always goes inside the Profile
            int glassInProfileLength = cabin.Parts.WallProfile1.GlassInProfileDepth;

            glass.Length = cabin.LengthMin
                - cabin.Constraints.MaxPossibleDoorLength
                - profile1Thickness
                - profile2Thickness
                - cabin.Constraints.GlassGapAERHorizontal
                + glassInProfileLength;
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
                CabinThicknessEnum.Thick6mm8mm => GlassThicknessEnum.Thick8mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }

        /// <summary>
        /// Returns the Step Height of the Fixed Glass that can have a Step in a 9B Structure
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns>The Glasses step Height or zero if there is no step</returns>
        public static double GetStepGlassHeight(Cabin9B cabin)
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
