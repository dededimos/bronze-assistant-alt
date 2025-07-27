using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.NBGlasses
{
    public class DoorGlassNBBuilder : GlassBuilderBase<CabinNB>
    {
#nullable disable
        
        public DoorGlassNBBuilder(CabinNB cabin , GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            if (cabin.Constraints.CornerRadiusTopEdge == 200 && cabin.Parts.Handle is null)
            {
                glass.Draw = GlassDrawEnum.DrawNV;
            }
            else
            {
                glass.Draw = GlassDrawEnum.DrawNB;
            }
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            //The Door is A little shorter than the Height of the Shower (Has Distance from the Floor so to Open)
            //The Top of the Aluminium is not Considered as Height of the Cabin!!!!!
            glass.Height = cabin.Height
                - cabin.Parts.WallHinge.TopHeightAboveGlass
                - cabin.Parts.WallHinge.BottomHeightBelowGlass
                - cabin.Constraints.FinalHeightCorrection;
        }
        public override void SetDefaultGlassLength()
        {
            //If there is no strip exclude - if there is no close profile exclude
            glass.Length = cabin.LengthMin
                - cabin.Parts.WallHinge.ThicknessView
                + cabin.Parts.WallHinge.GlassInProfileDepth
                - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0)
                - (cabin.Parts.CloseProfile?.ThicknessView ?? 0);
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
            glass.CornerRadiusTopRight = cabin.Constraints.CornerRadiusTopEdge;
            glass.CornerRadiusTopLeft = 0;
        }
        public override void SetDefaultGlassThickness()
        {
            glass.Thickness = cabin.Thicknesses switch
            {
                CabinThicknessEnum.Thick6mm => GlassThicknessEnum.Thick6mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlass;
        }
    }
}
