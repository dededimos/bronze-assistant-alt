using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;

public class CabinW : Free
{
    public const int MinPossibleLength = 142; //Minimum Nominal = 142+TollMinus
    public const int MaxPossibleLength = 1592;
    public const int MaxPossibleLength6mm = 992;
    public const int MaxPossibleHeight = 2200;
    public const int MinPossibleHeight = 100;


    /// <summary>
    /// The Default MinOpening of Free Structures
    /// </summary>
    public const int DefaultMinFreeOpening = 600;
    private const int AL1 = 34;
    private const int ALST = 12;
    public const int WallSupportsGlassGap = 5;  //Glass Gap from the Wall when we use Wall Supports for Installation instead of WallAluminium.
    public const int StopperHeight = 0;         //Glass Height Shrink , Due to the Bottom Glass Stopper
    //INTERCHANGED WITH VALUE IN CABIN PARTS DIMENSIONS public const int SupportBarHeight = 20;     //Glass Height Shrink , Due to the Support Bar Height
    public const int TOLLMINUS = 8;             //Basic Tollerance Minus
    public const int TollMinusWithSupports = 0; //Tollerance Minus when we Choose Withj Supports instead of Aluminium
    public const int TOLLPLUS = 15;             //Basic Tollerance Plus
    public const int TollPlusWithSupports = 0;  //Tollerance Plus when we Choose Withj Supports instead of Aluminium
    //DEPRECATED CHANGED WITH CabinPartsDimensions -- public const int MAGNETCORRECTOR = 10;      //Has a Magnet when its the Side/Straight Part of a Draw NOTOPTIONAL

    public override CabinWParts Parts { get; }
    public override ConstraintsW Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallSideFixer is Profile profile ? profile.Tollerance : 0)
        + totalTolleranceAdjustment;
    
    public CabinW(ConstraintsW constraints , CabinWParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        return 0;
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinW GetDeepClone()
    {
        CabinW cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }
}
