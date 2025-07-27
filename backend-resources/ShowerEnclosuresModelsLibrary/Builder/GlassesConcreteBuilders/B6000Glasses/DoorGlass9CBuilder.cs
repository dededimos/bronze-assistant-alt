using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses
{
    public class DoorGlass9CBuilder : GlassBuilderBase<Cabin9C>
    {
#nullable disable
        public const int DOORGLASSLENGTH80 = 295;
        public const int DOORGLASSLENGTH90 = 345;

        public DoorGlass9CBuilder(Cabin9C cabin, GlassBuilderOptions options = null) : base(cabin, options) { }
        
        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.Draw9C;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            //The Door Glass Has A Distance from the top AND the bottom of the Structure
            glass.Height = cabin.Height
                - cabin.Parts.HorizontalProfileTop.SliderDistance
                - cabin.Parts.HorizontalProfileBottom.SliderDistance;
        }
        public override void SetDefaultGlassLength()
        {
            if (cabin.NominalLength <= 850)
            {
                glass.Length = DOORGLASSLENGTH80;
            }
            else
            {
                glass.Length = DOORGLASSLENGTH90;
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
            glass.Thickness = cabin.Thicknesses switch
            {
                CabinThicknessEnum.Thick6mm => GlassThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick6mm8mm => GlassThicknessEnum.Thick6mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlassSemicircle;
        }

    }
}
