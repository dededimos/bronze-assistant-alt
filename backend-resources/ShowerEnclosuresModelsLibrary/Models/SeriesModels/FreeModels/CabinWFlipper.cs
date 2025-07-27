using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;

public class CabinWFlipper : Free
{
    public const int MinPossibleLength = 150;
    public const int MaxPossibleLength = 350;
    public const int MaxPossibleHeight = 2000;
    public const int MinPossibleHeight = 200;
    public const int DefaultDoorDistanceFromBottom = 15;
    public const int AER = 5;

    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;

    public override CabinWFlipperParts Parts { get; }
    public override ConstraintsWFlipper Constraints { get;}

    public CabinWFlipper(ConstraintsWFlipper constraints,CabinWFlipperParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    /// <summary>
    /// Gets the Opening for the Flipper Model
    /// </summary>
    /// <returns></returns>
    protected override double GetOpening()
    {
        return LengthMin - 20;
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinWFlipper GetDeepClone()
    {
        CabinWFlipper cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
