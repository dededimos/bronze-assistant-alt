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
    public class DoorGlass9SBuilder : GlassBuilderBase<Cabin9S>
    {
#nullable disable

        public DoorGlass9SBuilder(Cabin9S cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.Draw9S;
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
            //Get the Calculation for the DoorLength , If its bigger than the Allowed , set the Door to the MaxLength instead
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLength9SCalculation(cabin);
            if (doorGlassCalculationLength < cabin.Constraints.MaxDoorGlassLength)
            {
                glass.Length = doorGlassCalculationLength;
            }
            else
            {
                glass.Length = cabin.Constraints.MaxDoorGlassLength;
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
