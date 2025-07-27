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
#nullable disable
    public class DoorGlass9ABuilder : GlassBuilderBase<Cabin9A>
    {
        public DoorGlass9ABuilder(Cabin9A cabin , GlassBuilderOptions options = null) : base(cabin,options){}

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
            // Get the Calculation for the Door 
            double doorCalculationLength = GlassBuilderHelpers.DoorGlassLength9ACalculation(cabin);
            
            if (doorCalculationLength < cabin.Constraints.MaxDoorGlassLength)
            {
                // If its Less than the Max Proceed 
                glass.Length = doorCalculationLength;
            }
            else
            {
                // Otherwise Set it to Max
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
