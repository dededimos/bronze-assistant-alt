using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses
{
    public class DoorGlass9BBuilder : GlassBuilderBase<Cabin9B>
    {
#nullable disable

        public DoorGlass9BBuilder(Cabin9B cabin, GlassBuilderOptions options = null) : base(cabin, options) { }
        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.Draw9B;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            //The Height of the Glass Door is Calculated According to the L0 and not the Hinge
            //Though The Hinge Used goes Along with the L0
            //This should be taken into consideration 
            glass.Height = cabin.Height
                - cabin.Parts.HorizontalProfileTop.ThicknessView
                - cabin.Parts.HorizontalProfileBottom.ThicknessView

                - 2 * cabin.Constraints.GlassGapAERVertical;
        }
        public override void SetDefaultGlassLength()
        {
            if (cabin.LengthMin <= cabin.Constraints.AddedFixedGlassLengthBreakpoint)
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


                glass.Length = cabin.LengthMin
                    - profile1Thickness
                    - cabin.Constraints.GlassGapAERHorizontal
                    - profile2Thickness;
            }
            else // Lengths Greater than the breakpoint has a fixed Panel Also
            {   // The Door is the same for all lengths after as if we had a length of the Breakpoint
                // This method does not Allow us to Manipulate the Door Length
                // 9B Should get a Partial Length Property as HB in order for the user to be able to change it.
                glass.Length = cabin.Constraints.MaxPossibleDoorLength;
            }
        }
        public override void SetDefaultGlassStepHeight()
        {
            glass.StepHeight = 0;
        }
        public override void SetDefaultGlassStepLength()
        {
            glass.StepLength = 0;
        }
        public override void SetDefaultCornerRadius()
        {
            glass.CornerRadiusTopLeft = 0;
            glass.CornerRadiusTopRight = 0;
        }
        public override void SetDefaultGlassThickness()
        {
            if (cabin.Thicknesses is CabinThicknessEnum.Thick6mm or CabinThicknessEnum.Thick6mm8mm)
            {
                glass.Thickness = GlassThicknessEnum.Thick6mm;
            }
            else
            {
                glass.Thickness = GlassThicknessEnum.GlassThicknessNotSet;
            }
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlass;
        }
    }
}
