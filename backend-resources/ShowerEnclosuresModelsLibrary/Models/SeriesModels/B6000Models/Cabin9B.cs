using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;

public class Cabin9B : B6000
{
    public const int MinPossibleLength = 685;
    public const int MaxPossibleLength = 1985;
    public const int MaxPossibleHeight = 1900;
    public const int MinPossibleHeight = 601;
    public const int MinPossibleStepHeight = 60;
    
    public const int FixedBreakpoint = 950; //The length after which we add a Fixed Part to the Door
    public const int DoorLengthWithFixed = 755;
    
    public const int MinPossibleFixedLength = 100;
    public const int MinPossibleDoorLength = 590;

    //Hinges Dimensions (NEED FIX REAL)
    public const int HingeLength = 40;
    public const int HingeHeight = 60;
    /// <summary>
    /// How many mm is the Hinge outside of the Glass
    /// </summary>
    public const int HingeOverlapingHeight = 2;
    public const int HingeUpperCornerRadius = 20;
    public const int HingeSupportTubeLength = 15;
    public const int HingeSupportTubeHeight = 40;


    public const int PivotHingeDistance = 100;   //Distance of Pivot Hinge from AL1
    public const int ABSHingeCoefficient = 110;  //Height Correction of Glass According to the Used Hinge
    public const int METALHingeCoefficient = 100;//Height Correction of Glass According to the Used Hinge
    public const int AER = 5;                    //The Gap between the Door and AL1 or the Door with the Fixed
    public const int ALST = 10;                 //When the 9B Structure has a fixed Part
    public const int AL1 = 42;                   
    public const int AL2 = 48;                  //AL2-AL3 = The Small extra length that has the magnet (AL2 Is the whole Magnet Aluminium)
    public const int AL3 = 35;                  //AL3 The magnet Aluminium WITHOUT the extra magnet length
    public const int L0Correction = 2;          //We always add 2mm to the LO as correction (Measurement tollerances - Roundings)
    public const int TOLLMINUS = 15;
    public const int TOLLPLUS = 10;

    public override Cabin9BParts Parts { get; }
    public override Constraints9B Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallProfile1?.Tollerance ?? 0) + 
        (Parts.WallProfile2?.Tollerance ?? 0) +
        totalTolleranceAdjustment;
    
    public Cabin9B(Constraints9B constraints, Cabin9BParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        //Opening is always dependending on the Door Glasses
        double opening = 0;
        if (Glasses != null && (Glasses.Count == 2 || Glasses.Count==1))
        {
            double doorGlassLength = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.DoorGlass)?.Length ?? 0;
            opening = doorGlassLength - Constraints.HingeDistanceFromDoorGlass - Constraints.GlassGapAERHorizontal ;
        }
        return opening;
    }
    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override Cabin9B GetDeepClone()
    {
        Cabin9B cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }
}
