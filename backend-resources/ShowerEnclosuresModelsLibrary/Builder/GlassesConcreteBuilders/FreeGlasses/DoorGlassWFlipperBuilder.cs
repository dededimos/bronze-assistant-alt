using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.FreeGlasses
{
    internal class DoorGlassWFlipperBuilder : GlassBuilderBase<CabinWFlipper>
    {
#nullable disable

        public DoorGlassWFlipperBuilder(CabinWFlipper cabin , GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawFL;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            //The Door is A little shorter than the Height of the FixedPanel (Has Distance from the Floor so to Open)
            glass.Height = cabin.Height
                - cabin.Constraints.DoorDistanceFromBottom
                - cabin.Constraints.FinalHeightCorrection;
        }
        public override void SetDefaultGlassLength()
        {
            glass.Length = cabin.LengthMin - cabin.Parts.Hinge.GlassGapAER;
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
                CabinThicknessEnum.Thick6mm8mm => GlassThicknessEnum.Thick6mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlass;
        }
    }
}
