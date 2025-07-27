using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.HBGlasses
{
    public class DoorGlassHBBuilder : GlassBuilderBase<CabinHB>
    {
#nullable disable

        public DoorGlassHBBuilder(CabinHB cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawHB2;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            int heightAdjustment = 0;
            heightAdjustment += cabin.Constraints.FinalHeightCorrection;

            //Door has a Distance from the Floor
            glass.Height = cabin.Height
                - cabin.Constraints.DoorDistanceFromBottom
                - heightAdjustment;
        }
        public override void SetDefaultGlassLength()
        {
            //If partial length is not set return zero
            if (cabin.Constraints.PartialLength == 0)
            {
                throw new InvalidOperationException($"Cannot Calculate DoorHB Glass when {cabin.Constraints.PartialLength} is 0");
            }

            if (cabin.Constraints.LengthCalculation == LengthCalculationOption.ByDoorLength)
            {
                glass.Length = cabin.Constraints.PartialLength
                    - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0);
            }
            else if (cabin.Constraints.LengthCalculation == LengthCalculationOption.ByFixedLength)
            {
                glass.Length = cabin.LengthMin
                    - cabin.Constraints.PartialLength
                    - cabin.Parts.Hinge.GlassGapAER
                    - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0)
                    - (cabin.Parts.CloseProfile?.ThicknessView ?? 0);
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
            glass.CornerRadiusTopRight = cabin.Constraints.CornerRadiusTopEdge;
            glass.CornerRadiusTopLeft = 0;
        }
        public override void SetDefaultGlassThickness()
        {
            glass.Thickness = cabin.Thicknesses switch
            {
                CabinThicknessEnum.Thick8mm => GlassThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm => GlassThicknessEnum.Thick10mm,
                CabinThicknessEnum.ThickTenplex10mm => GlassThicknessEnum.ThickTenplex10mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlass;
        }
    }
}
