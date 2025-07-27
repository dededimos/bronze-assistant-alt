using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.Inox304Glasses
{
    public class DoorGlassV4Builder : GlassBuilderBase<CabinV4>
    {
        public DoorGlassV4Builder(CabinV4 cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawVS;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            glass.Height = cabin.Height
                - cabin.Constraints.DoorDistanceFromBottom
                - cabin.Constraints.FinalHeightCorrection;
        }
        public override void SetDefaultGlassLength()
        {
            //Get the Glass DoorCalculation Length , If its bigger than the Max (According to the Cabin Height) correct it
            double glassDoorCalculationLength = GlassBuilderHelpers.DoorGlassLengthV4Calculation(cabin);
            if (cabin.Height <= cabin.Constraints.BreakpointHeight)
            {
                if (glassDoorCalculationLength <= cabin.Constraints.MaxDoorLengthBeforeBreakpoint)
                {
                    glass.Length = glassDoorCalculationLength;
                }
                else
                {
                    glass.Length = cabin.Constraints.MaxDoorLengthBeforeBreakpoint;
                }
            }
            else //Heights up to 2200
            {
                if (glassDoorCalculationLength <= cabin.Constraints.MaxDoorLengthAfterBreakpoint)
                {
                    glass.Length = glassDoorCalculationLength;
                }
                else
                {
                    glass.Length = cabin.Constraints.MaxDoorLengthAfterBreakpoint;
                }
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
                CabinThicknessEnum.Thick8mm => GlassThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm => GlassThicknessEnum.Thick10mm,
                CabinThicknessEnum.ThickTenplex10mm => GlassThicknessEnum.ThickTenplex10mm,
                CabinThicknessEnum.Thick8mm10mm => GlassThicknessEnum.Thick8mm,
                _ => null,
            };
        }
        public override void SetDefaultGlassType()
        {
            glass.GlassType = GlassTypeEnum.DoorGlass;
        }
    }
}
