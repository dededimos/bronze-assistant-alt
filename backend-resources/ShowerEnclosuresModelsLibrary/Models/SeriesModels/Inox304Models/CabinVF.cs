using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;

public class CabinVF : Inox304
{
    public const int MinPossibleLength = 150;
    public const int MaxPossibleLength = 1500;
    public const int MaxPossibleHeight = 2200;
    public const int MinPossibleHeight = 201;

    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;

    public override CabinVFParts Parts { get; }
    public override ConstraintsVF Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallSideFixer is Profile profile ? profile.Tollerance : 0)
        + totalTolleranceAdjustment;

    public CabinVF(ConstraintsVF constraints, CabinVFParts parts)
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
    public override CabinVF GetDeepClone()
    {
        CabinVF cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
