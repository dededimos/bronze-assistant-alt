
namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;

public class CabinNB : Niagara6000
{
#nullable disable
    public const int MinPossibleLength = 285;
    public const int MaxPossibleLength = 935;
    public const int MaxPossibleHeight = 2000;
    public const int MinPossibleHeight = 201;

    /// <summary>
    /// The Door Distance from the Upper and Bottom Part of the Cabin.(The Height of the Hinges Metal Parts)
    /// </summary>
    public const int DoorHeightCorrection = 20; 

    public const int AL1 = 75;
    public const int ALST = 20;
    public const int ALMAG = 30;
    public const int TOLLMINUS = 15;
    public const int TOLLPLUS = 10;
    public const int TOLLMAGNET = 10; //Tollerance when we have Magnet Aluminium

    public override CabinNBParts Parts { get;}
    public override ConstraintsNB Constraints { get; }

    public override int TotalTollerance => 
        (Parts.CloseProfile is not null ? (Parts.CloseProfile.Tollerance + Parts.WallHinge.Tollerance) : Parts.WallHinge.Tollerance) +
        totalTolleranceAdjustment;

    public CabinNB(ConstraintsNB constraints, CabinNBParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        if (Parts?.WallHinge is not null)
        {
            double opening =
                LengthMin
                - Parts.WallHinge.ThicknessView
                - (Parts.CloseProfile?.ThicknessView ?? 0);
            return opening > 0 ? opening : 0;
        }
        return 0;
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinNB GetDeepClone()
    {
        CabinNB cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
