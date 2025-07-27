using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;

public class Cabin9S : B6000
{
#nullable disable
    public const int MinPossibleLength = 885;
    public const int MaxPossibleLength = 1885;
    public const int MaxPossibleHeight = 2100;
    public const int MinPossibleHeight = 600;
    public const int MinPossibleStepHeight = 60;
    public const int MaxDoorGlassLength = 784;

    private const int AL1 = 40;     //Wall Aluminium Default
    private const int ALST = 10;    //ALST Default
    public const int EPIK = 7;      //OverlapDefault
    private const int AL2 = 49;     //MagnetAluminium Default
    public const int HD = 100;     //Handle Distance Default
    public const int CD = 27;      //Cover Distance Default
    private const int AL3 = 35;     //Magnet Aluminium Length without Magnet Default
    public const int TOLLMINUS = 15;   //Default Minus Tollerance
    public const int TOLLPLUS = 40;    //Default Plus Tollerance for Single 9S
    public const int TOLLPLUS9F = 10;  //Default Plus Tollerance for 9S With 9F(s) Combo

    public override Cabin9SParts Parts { get; }
    public override Constraints9S Constraints { get; }
    public override int TotalTollerance =>
        (Parts.WallProfile1?.Tollerance ?? 0) +
        (Parts.WallProfile2?.Tollerance ?? 0) +
        totalTolleranceAdjustment;

    public Cabin9S(Constraints9S constraints, Cabin9SParts parts)
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
    public override Cabin9S GetDeepClone()
    {
        Cabin9S cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }
}
