using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.FreeGlasses
{
    public class FixedGlassEBuilder : GlassBuilderBase<CabinE>
    {
        public FixedGlassEBuilder(CabinE cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            //Factor in the Extra Height of any Bottom Fixer
            double heightAdjustment = cabin.Parts.BottomFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };

            heightAdjustment += cabin.Constraints.FinalHeightCorrection;

            glass.Height = cabin.Height - heightAdjustment;
        }
        public override void SetDefaultGlassLength()
        {
            // Only for stopper that is a little outside
            int lengthAdjustment = cabin.Parts.BottomFixer switch
            {
                FloorStopperW s => 2 * s.OutOfGlassLength,
                _ => 0,
            };
            //Glass Length is always 5mm Less than LengthMin (Correction)
            glass.Length = cabin.LengthMin - lengthAdjustment;
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
            glass.CornerRadiusTopLeft = cabin.Constraints.CornerRadiusTopEdge;
        }
        public override void SetDefaultGlassThickness()
        {
            glass.Thickness = cabin.Thicknesses switch
            {
                CabinThicknessEnum.Thick6mm => GlassThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick8mm => GlassThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm => GlassThicknessEnum.Thick10mm,
                CabinThicknessEnum.ThickTenplex10mm => GlassThicknessEnum.ThickTenplex10mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }
    }
}
