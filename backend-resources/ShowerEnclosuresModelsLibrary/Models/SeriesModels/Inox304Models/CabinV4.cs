using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;

public class CabinV4 : Inox304
{
#nullable disable
    public const int MinPossibleLength = 1200;
    public const int MaxPossibleLength = 2200;
    public const int MaxPossibleHeight = 2200;
    public const int MinPossibleHeight = 1000;

    public const int BreakpointHeight = 2000;        //Height after which the MaxDoorGlassLength Changes ((and FixedGlasses Thickness Change???))
    public const int MaxDoorLength2000H = 450;       //The Max Length of Door up to the Height BreakPoint
    public const int MaxDoorLength2200H = 400;      //The Max Length of Door for heights Bigger than the BreakPointHeight

    public const int MinOverlap = 5;

    public const int EPIK = 70;
    public const int HD = 70;
    public const int CD = -20;
    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;
    public const int L0Correction = 25;

    public override CabinV4Parts Parts { get; }
    public override ConstraintsV4 Constraints { get; }
    public override int TotalTollerance 
    {
        get
        {
            int tol1 = Parts.WallSideFixer is Profile profile ? profile.Tollerance : 0;
            int tol2 = Parts.WallFixer2 is Profile profile2 ? profile2.Tollerance : 0;
            return (tol1 + tol2 + totalTolleranceAdjustment);
        }
    }

    public CabinV4(ConstraintsV4 constraints, CabinV4Parts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        // Opening always depends on Glasses having been built

        // Early Escape
        if (Glasses.Count != 4)
        {
            return 0;
        }

        // To Calculate the Opening we need to find the Hiding Distance ,
        // how many mm the Sliding glass can hide behind the Fixed Part
        // Hiding Distance = PartsLengths + Fixed Part - Overlap - CoverDistance - StepLength (This is the Theoritical Maximum Opening)
        // Then We need to figure out the Length of the Sliding Glass that is available for Hiding
        // Length Available for Hiding = Sliding Glass - Overlap - HandleSlidingDoorAirDistance
        // If the Available Length for Hiding > HidingDistance then the Opening is the Hiding Distance , else its the Available Length for Hiding
        // This Model needs to Calculate two Openings

        IEnumerable<Glass> fixedGlasses = Glasses.Where(g => g.GlassType == GlassTypeEnum.FixedGlass);
        IEnumerable<Glass> doors = Glasses.Where(g => g.GlassType == GlassTypeEnum.DoorGlass);
        if (fixedGlasses.Count() != 2 || doors.Count() != 2)
        {
            return 0;
        }
        var firstFixed = fixedGlasses.First();
        var secondFixed = fixedGlasses.Skip(1).First();
        var firstDoor = doors.First();
        var secondDoor = doors.Skip(1).First();

        if (Parts.WallSideFixer is null || Parts.WallFixer2 is null || Parts.Handle is null)
        {
            return 0;
        }
        int wallSide1FixerLength = Parts.WallSideFixer is Profile p 
            ? p.ThicknessView 
            : (Parts.WallSideFixer is CabinSupport sup ? sup.GlassGapAER : 0);
        int wallSide2FixerLength = Parts.WallFixer2 is Profile p2 
            ? p2.ThicknessView 
            : (Parts.WallSideFixer is CabinSupport sup2 ? sup2.GlassGapAER : 0);

        int hidingDistance1 = wallSide1FixerLength
             + Convert.ToInt32(firstFixed.Length)
             - Convert.ToInt32(firstFixed.StepLength)
             - (Parts.WallSideFixer is Profile p3? p3.GlassInProfileDepth : 0)
             - Constraints.CoverDistance
             - Constraints.Overlap;

        int hidingDistance2 = wallSide2FixerLength
             + Convert.ToInt32(secondFixed.Length)
             - Convert.ToInt32(secondFixed.StepLength)
             - (Parts.WallFixer2 is Profile p4 ? p4.GlassInProfileDepth : 0)
             - Constraints.CoverDistance
             - Constraints.Overlap;

        int availableHiding1 = Convert.ToInt32(firstDoor.Length)
            - Constraints.Overlap
            - Parts.Handle.GetSlidingDoorAirDistance();
        int availableHiding2 = Convert.ToInt32(secondDoor.Length)
            - Constraints.Overlap
            - Parts.Handle.GetSlidingDoorAirDistance();

        int opening1 = availableHiding1 > hidingDistance1 ? hidingDistance1 : availableHiding1;
        int opening2 = availableHiding2 > hidingDistance2 ? hidingDistance2 : availableHiding2;

        return Convert.ToDouble(opening1 + opening2);
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinV4 GetDeepClone()
    {
        CabinV4 cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
