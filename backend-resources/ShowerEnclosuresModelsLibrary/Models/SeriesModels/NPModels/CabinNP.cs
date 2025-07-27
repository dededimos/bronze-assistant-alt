
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;

public class CabinNP : Niagara6000
{
#nullable disable
    public const int MinPossibleLength = 535;
    public const int MaxPossibleLength = 1000;
    public const int MaxPossibleHeight = 2000;
    public const int MinPossibleHeight = 201;
    //Only for MV2 and NV2 Models
    public const int MinPossibleLengthBathtub = 635;
    public const int MaxPossibleLengthBathtub = 1200;
    public const int MaxPossibleHeightBathtub = 1600;

    private const int AL1 = 75;
    private const int ALST = 20;
    private const int AER = 5;
    private const int ALC = 13;
    private const int ALMAG = 30;
    public const int TOLLMINUS = 15;
    public const int TOLLPLUS = 10;
    public const int TOLLMAGNET = 10;
    private const int FoldedDoorLength = 30; //Space the Glasses take when Folded (To Calculate Opening)
    public const int DoorsLengthDifference = 15;   //How much Bigger is the First door from the Second
    public const int DoorsLengthDiffBathtub = 145; //How much Bigger is the First door from the Second on a Bathtub MV2/NV2

    public override CabinNPParts Parts { get; }
    public override ConstraintsNP Constraints { get; }
    public override int TotalTollerance => 
        (Parts.CloseProfile is not null ? (Parts.CloseProfile.Tollerance + Parts.WallHinge.Tollerance) : Parts.WallHinge.Tollerance)
        +totalTolleranceAdjustment;

    public CabinNP(ConstraintsNP constraints, CabinNPParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        if (Parts?.WallHinge is not null)
        {
            //Approximation we have to measure the real folded Length of Glasses
            double opening =
                LengthMin
                - Parts.WallHinge.ThicknessView
                - FoldedDoorLength
                - (Parts.CloseProfile?.ThicknessView ?? 0);
            return opening > 0 ? opening : 0;
        }
        return 0;
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinNP GetDeepClone()
    {
        CabinNP cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
