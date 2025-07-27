using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;


namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.FreeGlasses;

public class FixedGlassWBuilder : GlassBuilderBase<CabinW>
{
    public FixedGlassWBuilder(CabinW cabin, GlassBuilderOptions options = null) : base(cabin, options) { }

    public override void SetDefaultGlassDraw()
    {
        if (cabin.Parts.WallSideFixer is CabinSupport)
        {
            glass.Draw = GlassDrawEnum.DrawH1;
        }
        else
        {
            if (cabin.Constraints.CornerRadiusTopEdge == 200)
            {
                glass.Draw = GlassDrawEnum.DrawNV;
            }
            else
            {
                glass.Draw = GlassDrawEnum.DrawF;
            }
        }
    }
    public override void SetDefaultGlassFinish()
    {
        glass.Finish = cabin.GlassFinish;
    }
    public override void SetDefaultGlassHeight()
    {
        //The Bottom or Top Fixers are always Profiles or Supports 
        int topAdjustment = cabin.Parts.TopFixer switch
        {
            Profile p => p.ThicknessView - p.GlassInProfileDepth,
            CabinSupport s => s.GlassGapAER,
            _ => 0,
        };
        int bottomAdjustment = cabin.Parts.BottomFixer switch
        {
            Profile p => p.ThicknessView - p.GlassInProfileDepth,
            CabinSupport s => s.GlassGapAER,
            _ => 0,
        };

        topAdjustment += cabin.Constraints.FinalHeightCorrection;

        glass.Height = cabin.Height
            - topAdjustment
            - bottomAdjustment;
    }
    public override void SetDefaultGlassLength()
    {
        //The Wall or Side Fixers are always Profiles or Supports 
        int wallAdjustment = cabin.Parts.WallSideFixer switch
        {
            Profile p => p.ThicknessView - p.GlassInProfileDepth,
            CabinSupport s => s.GlassGapAER,
            _ => 0,
        };
        int sideAdjustment = cabin.Parts.SideFixer switch
        {
            Profile p => p.ThicknessView - p.GlassInProfileDepth,
            CabinSupport s => s.GlassGapAER,
            _ => 0,
        };

        glass.Length = cabin.LengthMin
            - wallAdjustment
            - sideAdjustment
            - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0);
    }
    public override void SetDefaultGlassStepHeight()
    {
        glass.StepHeight = GetStepGlassHeight(cabin);
    }
    public override void SetDefaultGlassStepLength()
    {
        glass.StepLength = GetStepGlassLength(cabin);
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
            CabinThicknessEnum.Thick5mm => GlassThicknessEnum.Thick5mm,
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

    /// <summary>
    /// Returns the Step Height of the Fixed Glass that can have a Step in a W Structure
    /// </summary>
    /// <param name="cabin"></param>
    /// <returns>The Glasses step Height or zero if there is no step</returns>
    public static double GetStepGlassHeight(CabinW cabin)
    {
        if (cabin.HasStep)
        {
            int bottomAdjustment = cabin.Parts.BottomFixer switch
            {
                Profile p => p.ThicknessView - p.GlassInProfileDepth,
                CabinSupport s => s.GlassGapAER,
                _ => 0,
            };
            return cabin.GetStepCut().StepHeight
                + cabin.Constraints.StepHeightTollerance
                - bottomAdjustment;
        }
        return 0;
    }

    /// <summary>
    /// Returns the Step Length of the Fixed Glass that can have a Step in a W Structure
    /// </summary>
    /// <param name="cabin"></param>
    /// <returns>The Glasses step Height or zero if there is no step</returns>
    public static double GetStepGlassLength(CabinW cabin)
    {
        if (cabin.HasStep)
        {
            // If there is already available tollerance greater than 5 then 
            // do not Adjust , otherwise adjust until tollerance is 5
            int stepCutLengthAdjustment = cabin.Parts.WallSideFixer switch
            {
                Profile p => p.Tollerance < cabin.Constraints.StepLengthTolleranceMin
                ? Math.Abs(p.Tollerance - cabin.Constraints.StepLengthTolleranceMin)
                : 0,
                CabinSupport s => s.Tollerance < cabin.Constraints.StepLengthTolleranceMin
                ? Math.Abs(s.Tollerance - cabin.Constraints.StepLengthTolleranceMin)
                : 0,
                _ => cabin.Constraints.StepLengthTolleranceMin
            };
            return cabin.GetStepCut().StepLength + stepCutLengthAdjustment;
        }
        return 0;
    }

}
