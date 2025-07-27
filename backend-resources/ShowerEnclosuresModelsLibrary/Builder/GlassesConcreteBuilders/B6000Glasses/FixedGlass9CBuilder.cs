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
#nullable disable
    public class FixedGlass9CBuilder : GlassBuilderBase<Cabin9C>
    {
        public const int FIXEDGLASSLENGTH80 = 270;
        public const int FIXEDGLASSLENGTH90 = 320;

        public FixedGlass9CBuilder(Cabin9C cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            if (cabin.LengthMin <= 850)
            {
                int differenceFrom80 = cabin.LengthMin - 785;
                //Add the difference to the Fixed Glass
                glass.Length = FIXEDGLASSLENGTH80 + differenceFrom80;
            }
            else
            {
                int differenceFrom90 = cabin.LengthMin - 885;
                //Add the difference to the Fixed Glass
                glass.Length = FIXEDGLASSLENGTH90 + differenceFrom90;
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
    }
}
