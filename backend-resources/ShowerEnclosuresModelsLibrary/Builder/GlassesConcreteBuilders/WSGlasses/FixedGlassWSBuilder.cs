using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.WSGlasses
{
#nullable disable
    public class FixedGlassWSBuilder : GlassBuilderBase<CabinWS>
    {
        public FixedGlassWSBuilder(CabinWS cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

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
            double heightAdj = 0;

            heightAdj += cabin.Constraints.FinalHeightCorrection;

            glass.Height = cabin.Height - heightAdj;
        }
        public override void SetDefaultGlassLength()
        {
            //Then Calculate the Supposed FixedLength
            //Add the Difference (Correction Length) if the Door Glass Maximum is Exceeded 
            int stepLength = 0;

            //NEVER WITH STEP
            //if (cabin.HasStep)
            //{
            //    stepLength = cabin.GetStepCut().StepLength;
            //}

            double length = (cabin.LengthMin
                + stepLength
                - cabin.Parts.WallFixer.ThicknessView
                - (cabin.Parts.CloseProfile?.ThicknessView ?? 0)
                - cabin.Constraints.CoverDistance
                - (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? 0)
                + cabin.Constraints.Overlap
                - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0))
                / 2d
                + cabin.Parts.WallFixer.GlassInProfileDepth;

            double correctionLength = 0;
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLengthWSCalculation(cabin);

            if (doorGlassCalculationLength > cabin.Constraints.MaxDoorLength)
            {
                correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLength;
            }

            glass.Length = length + correctionLength;

            //**********************************************************************************************************
            //Constraint here , This Boolean Should never come true upon runtime !!!!! otherwise the door can fall off
            if (correctionLength > (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? 0)) //Without Handle there Can be No Correction Length
            {
                glass.Length = 0;
            }
            //We have to Validate the Glasses from Calculations (Need to Create a Validation Method inside their model?)
            //**********************************************************************************************************
        }
        public override void SetDefaultGlassStepHeight()
        {
            //CANNOT HAVE STEP
            glass.StepHeight = 0;
        }
        public override void SetDefaultGlassStepLength()
        {
            //CANNOT HAVE STEP
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
                CabinThicknessEnum.Thick10mm => GlassThicknessEnum.Thick10mm,
                CabinThicknessEnum.ThickTenplex10mm => GlassThicknessEnum.ThickTenplex10mm,
                CabinThicknessEnum.Thick8mm10mm => GlassThicknessEnum.Thick10mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.FixedGlass;
        }
    }
}
