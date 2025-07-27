using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;

public class Cabin9F : B6000
{
    public const int MaxPossibleLength = 1685;
    public const int MinPossibleLength = 185;
    public const int MinPossibleHeight = 600;
    public const int MaxPossibleHeight = 2100;
    public const int MinPossibleStepHeight = 60;

    private const int AL1 = 40; //Wall Side Profile Length
    private const int ALST = 10; //Glass in Aluminium
    private const int ALC = 39; //connecctor al Length
    public const int TOLLMINUS = 15;
    public const int TOLLPLUS = 10;

    public override Cabin9FParts Parts { get; }
    public override Constraints9F Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallProfile1?.Tollerance ?? 0) + 
        (Parts.WallProfile2?.Tollerance ?? 0)
        + totalTolleranceAdjustment;

    public Cabin9F(Constraints9F constraints, Cabin9FParts parts)
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
    public override Cabin9F GetDeepClone()
    {
        Cabin9F cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
