using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.DBGlasses;

public class DoorGlassDBBuilder : GlassBuilderBase<CabinDB>
{
#nullable disable

    public DoorGlassDBBuilder(CabinDB cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

    public override void SetDefaultGlassDraw()
    {
        glass.Draw = GlassDrawEnum.DrawDB;
    }
    public override void SetDefaultGlassFinish()
    {
        glass.Finish = cabin.GlassFinish;
    }
    public override void SetDefaultGlassHeight()
    {
        //The Door is A little shorter than the Height of the Shower (Has Distance from the Floor so to Open)
        glass.Height = cabin.Height
            - cabin.Constraints.DoorDistanceFromBottom
            - cabin.Constraints.FinalHeightCorrection;
    }
    public override void SetDefaultGlassLength()
    {
        glass.Length = cabin.LengthMin
            //If there is Close Magnet Profile add its Thickness otherwise its zero
            - (cabin.Parts.CloseProfile?.ThicknessView ?? 0)
            //If there is a strip calculate its Length otherwise put zero
            - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0)
            - cabin.Parts.Hinge.GlassGapAER;
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
        glass.CornerRadiusTopRight = cabin.Constraints.CornerRadiusTopEdge;
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
