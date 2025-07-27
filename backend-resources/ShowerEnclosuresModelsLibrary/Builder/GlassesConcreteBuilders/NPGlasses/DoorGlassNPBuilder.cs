using ShowerEnclosuresModelsLibrary.Builder.GlassBuilderHelperMethods;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.NPGlasses;

#nullable disable
public class DoorGlassNPBuilder : GlassBuilderBase<CabinNP>
{
    private readonly bool isWallGlass;

    public DoorGlassNPBuilder(CabinNP cabin,bool isWallGlass , GlassBuilderOptions options = null) : base(cabin,options)
    {
        this.isWallGlass = isWallGlass; //True for DP3 (Wall glass) , False for DP1 (AfterGlass)
    }

    public override void SetDefaultGlassDraw()
    {
        if (isWallGlass)
        {
            glass.Draw = GlassDrawEnum.DrawDP3;
        }
        else
        {
            glass.Draw = GlassDrawEnum.DrawDP1;
        }
    }
    public override void SetDefaultGlassFinish()
    {
        glass.Finish = cabin.GlassFinish;
    }
    public override void SetDefaultGlassHeight()
    {
        //The Door is A little shorter than the Height of the Shower (Has Distance from the Floor so to Open)
        glass.Height = cabin.Height
            - cabin.Parts.WallHinge.TopHeightAboveGlass
            - cabin.Parts.WallHinge.BottomHeightBelowGlass
            - cabin.Constraints.FinalHeightCorrection;
    }
    public override void SetDefaultGlassLength()
    {
        double middleHingeLength = cabin.Parts.MiddleHinge switch
        {
            ProfileHinge ph => ph.ThicknessView - 2 * ph.GlassInProfileDepth, //glass goes in from both ways
            CabinHinge ch => ch.GlassGapAER,
            _ => throw new InvalidOperationException("Cannot Build NP Glasses Without Hinge"),
        };

        double length = (cabin.LengthMin
            - cabin.Parts.WallHinge.ThicknessView
            - cabin.Constraints.DoorsLengthDifference
            + cabin.Parts.WallHinge.GlassInProfileDepth
            - middleHingeLength
            - (cabin.Parts.CloseStrip?.OutOfGlassLength ?? 0)
            - (cabin.Parts.CloseProfile?.ThicknessView ?? 0))
            / 2d;

        if (isWallGlass)//Case for DP3 is a Little Longer than DP1
        {
            glass.Length = length + cabin.Constraints.DoorsLengthDifference;
        }
        else //Case for DP1
        {
            glass.Length = length;
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
        glass.CornerRadiusTopRight = isWallGlass ? 0 : cabin.Constraints.CornerRadiusTopEdge;
        glass.CornerRadiusTopLeft = 0;
    }
    public override void SetDefaultGlassThickness()
    {
        glass.Thickness = cabin.Thicknesses switch
        {
            CabinThicknessEnum.Thick6mm => GlassThicknessEnum.Thick6mm,
            _ => null,
        };
    }
    public override void SetDefaultGlassType()
    {
        glass.GlassType = GlassTypeEnum.DoorGlass;
    }
}
