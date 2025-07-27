using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;

public class CabinWS : Cabin
{
    public const int MinPossibleLength = 500;
    public const int MaxPossibleLength = 1300;
    public const int MaxPossibleHeight = 2000;
    public const int MinPossibleHeight = 201;

    public const int MaxDoorLength = 750;

    public const int MinOverlap = 1;
    public const int DriverPartThickness = 33; //The Part that Drives the Door and Keeps the Support Bar at the Same Time
    //INTERCHANGED WITH VALUE IN CABIN PARTS public const int SupportBarHeight = 20;

    private const int AL1 = 32; 
    private const int ALST = 23;
    public const int EPIK = 100;
    private const int ALMAG = 30;
    public const int HD = 80;    //Handle Hole is Diam:50mm and Center Distance from Outer Glass is 55mm so Default HD = 25+55 =80mm
    public const int CD = -15;   //AL1 is 32mm and GlassDoor Goes in for 17mm So CD = 17-32 = -15mm
    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;

    //In this Model There is a Condition that is always applied by Glass Calculations
    //The Door Can Never Be Smaller Than the Fixed Part (Even When we Have a two Door Middle Entry Shower)

    public override CabinWSParts Parts { get; }
    public override ConstraintsWS Constraints { get; }
    public override int TotalTollerance => 
        (Parts.CloseProfile is not null ? Parts.CloseProfile.Tollerance : 0) + 
         totalTolleranceAdjustment;

    public override CabinSeries Series { get => CabinSeries.Smart; }

    public CabinWS(ConstraintsWS constraints, CabinWSParts parts)
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

        if (Parts.WallFixer is null || fixedGlass is null || door is null)
        {
            return 0;
        }

        int hidingDistance = Parts.WallFixer.ThicknessView
             + Convert.ToInt32(fixedGlass.Length)
             - Convert.ToInt32(fixedGlass.StepLength)
             - Parts.WallFixer.GlassInProfileDepth
             - Constraints.CoverDistance
             - Constraints.Overlap;

        int availableHiding = Convert.ToInt32(door.Length)
            - Constraints.Overlap
            - (Parts.Handle?.GetSlidingDoorAirDistance() ?? 0);

        int opening = availableHiding > hidingDistance ? hidingDistance : availableHiding;

        return Convert.ToDouble(opening);
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinWS GetDeepClone()
    {
        CabinWS cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
