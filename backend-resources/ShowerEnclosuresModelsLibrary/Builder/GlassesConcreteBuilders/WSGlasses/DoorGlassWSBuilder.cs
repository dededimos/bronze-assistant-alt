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
    public class DoorGlassWSBuilder : GlassBuilderBase<CabinWS>
    {
        public DoorGlassWSBuilder(CabinWS cabin , GlassBuilderOptions options = null) : base(cabin, options) { }

        public override void SetDefaultGlassDraw()
        {
            glass.Draw = GlassDrawEnum.DrawWS;
        }
        public override void SetDefaultGlassFinish()
        {
            glass.Finish = cabin.GlassFinish;
        }
        public override void SetDefaultGlassHeight()
        {
            // Factor in the Distance the Door has from the bottom (Actually the Driver Height minus any height taken from the wheels)
            double heightAdj = cabin.Constraints.DoorDistanceFromBottom;

            heightAdj += cabin.Constraints.FinalHeightCorrection;

            glass.Height = cabin.Height - heightAdj;
        }
        public override void SetDefaultGlassLength()
        {
            //********************************************************************************************************
            //Get the Calculation for the DoorLength , If its bigger than the Allowed , set the Door to the MaxLength instead
            double doorGlassCalculationLength = GlassBuilderHelpers.DoorGlassLengthWSCalculation(cabin);
            if (doorGlassCalculationLength < cabin.Constraints.MaxDoorLength)
            {
                glass.Length = doorGlassCalculationLength;
            }
            else
            {
                glass.Length = cabin.Constraints.MaxDoorLength;
            }
            double correctionLength = doorGlassCalculationLength - cabin.Constraints.MaxDoorLength; //We cannot correct more than the HD.
            if (correctionLength > (cabin.Parts.Handle?.GetSlidingDoorAirDistance() ?? 0)) //if there is no Handle there is No HD
            {//This way we have at least a zero Glass to handle instead of Wrong Glass to Give
                glass.Length = 0;
            }
            //We must Check this together with the Length of the Fixed so that the Door does not Fall Out
            //********************************************************************************************************
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
                CabinThicknessEnum.Thick8mm10mm => GlassThicknessEnum.Thick8mm,
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
