using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;

public class Cabin9A : B6000
{
#nullable disable
    public const int MaxPossibleLength = 1685;
    public const int MinPossibleLength = 185;
    public const int MinPossibleHeight = 600;
    public const int MaxPossibleHeight = 2100;
    public const int MinPossibleStepHeight = 60;
    public const int MaxDoorGlassLength = 550;

    private const int AL1 = 42;
    private const int ALST = 10;
    public const int EPIK = 7;
    /// <summary>
    /// This Number Represents the Distance of the Glass door from the Outer Part of the Cabin Angle
    /// The Extra Length Used in the Cabin Dimensions Because there is an AnglePart -- THIS NUMBER IS TO BE USED IN GLASSES CALCULATIONS ONLY
    /// Glasses Remain the Same no Matter what angle we use -- Only the L0 Changes. 
    /// </summary>
    private const int ANGLE = 40;
    /// <summary>
    /// The Actual Length of the Angle Part Used for L0A Horizontal Profile
    /// </summary>
    public const int ANGLEL0A = 33;    //With Lo Type A
    /// <summary>
    /// The Actual Length of the Angle Part Used for L0B Horizontal Profile
    /// </summary>
    public const int ANGLEL0B = 23;    //With Lo Type B
    /// <summary>
    /// The Actual Length of the Angle Part Used for L0Q Horizontal Profile
    /// </summary>
    public const int ANGLEL0Q = 18;    //With Lo Type Q
    public const int HD = 90;
    public const int CD = 30;
    public const int TOLLMINUS = 15;      //The Negative Tollerance of the Cabin
    public const int TOLLPLUS = 10;       //The Positive Tollerance of the Cabin

    public override Cabin9AParts Parts { get;}
    public override Constraints9A Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallProfile1?.Tollerance ?? 0) +
        totalTolleranceAdjustment;

    public Cabin9A(Constraints9A constraints, Cabin9AParts parts)
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
        // Hiding Distance = PartsLengths + Fixed Part - Overlap - CoverDistance -StepLength (This is the Theoritical Maximum Opening)
        // Then We need to figure out the Length of the Sliding Glass that is available for Hiding
        // Length Available for Hiding = Sliding Glass - Overlap - HandleSlidingDoorAirDistance
        // If the Available Length for Hiding > HidingDistance then the Opening is the Hiding Distance , else its the Available Length for Hiding

        
        var fixedGlass = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass);
        var door = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.DoorGlass);

        if (Parts.WallProfile1 is null || Parts.Handle is null || door is null || fixedGlass is null)
        {
            return 0;
        }

        int hidingDistance = Parts.WallProfile1.ThicknessView
             + Convert.ToInt32(fixedGlass.Length)
             - Convert.ToInt32(fixedGlass.StepLength)
             - Parts.WallProfile1.GlassInProfileDepth
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
    public override Cabin9A GetDeepClone()
    {
        Cabin9A cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }
}
