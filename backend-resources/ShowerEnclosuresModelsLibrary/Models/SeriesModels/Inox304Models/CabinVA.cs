using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;

public class CabinVA : Inox304
{
#nullable disable
    public const int MinPossibleLength = 300;
    public const int MaxPossibleLength = 900;
    public const int MaxPossibleHeight = 2000;
    public const int MinPossibleHeight = 201;

    public const int MaxDoorLength = 390;

    public const int MinOverlap = 5;

    public const int EPIK = 30;
    private const int ALMAG = 30;
    public const int HD = 70;
    public const int CD = -20;
    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;
    public const int L0Correction = 25;

    public override CabinVAParts Parts { get; }
    public override ConstraintsVA Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallSideFixer is Profile profile ? profile.Tollerance : 0)
        + totalTolleranceAdjustment;

    public CabinVA(ConstraintsVA constraints, CabinVAParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        // Opening always depends on Glasses having been built

        // Early Escape
        if (Glasses.Count != 2)
        {
            return 0;
        }

        // To Calculate the Opening we need to find the Hiding Distance ,
        // how many mm the Sliding glass can hide behind the Fixed Part
        // Hiding Distance = PartsLengths + Fixed Part - Overlap - CoverDistance - StepLength (This is the Theoritical Maximum Opening)
        // Then We need to figure out the Length of the Sliding Glass that is available for Hiding
        // Length Available for Hiding = Sliding Glass - Overlap - HandleSlidingDoorAirDistance
        // If the Available Length for Hiding > HidingDistance then the Opening is the Hiding Distance , else its the Available Length for Hiding

        var fixedGlass = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass);
        var door = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.DoorGlass);

        if (Parts.WallSideFixer is null || Parts.Handle is null || fixedGlass is null || door is null)
        {
            return 0;
        }
        int wallSide1FixerLength = Parts.WallSideFixer is Profile p
            ? p.ThicknessView
            : (Parts.WallSideFixer is CabinSupport sup ? sup.GlassGapAER : 0);

        int hidingDistance = wallSide1FixerLength
             + Convert.ToInt32(fixedGlass.Length)
             - Convert.ToInt32(fixedGlass.StepLength)
             - (Parts.WallSideFixer is Profile p3 ? p3.GlassInProfileDepth : 0)
             - Constraints.CoverDistance
             - Constraints.Overlap;

        int availableHiding = Convert.ToInt32(door.Length)
            - Constraints.Overlap
            - Parts.Handle.GetSlidingDoorAirDistance();

        int opening = availableHiding > hidingDistance ? hidingDistance : availableHiding;

        return Convert.ToDouble(opening);
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinVA GetDeepClone()
    {
        CabinVA cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
