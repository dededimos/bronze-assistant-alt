using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;

public class CabinE : Free
{
    public const int MinPossibleLength = 300;
    public const int MaxPossibleLength = 1800;
    public const int MaxPossibleHeight = 2200;
    public const int MinPossibleHeight = 100;

    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;
    public const int StopperHeight = 0;      //Glass Height Shrink , Due to the Bottom Glass Stopper

    public override CabinEParts Parts { get; }
    public override ConstraintsE Constraints { get; }

    public CabinE(ConstraintsE constraints, CabinEParts parts)
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
    public override CabinE GetDeepClone()
    {
        CabinE cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
