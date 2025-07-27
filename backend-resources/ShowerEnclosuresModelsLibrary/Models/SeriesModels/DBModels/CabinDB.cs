using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;

public class CabinDB : Hotel8000, IWithHingeCuts
{
    public const int MinPossibleLength = 200;
    public const int MaxPossibleLength = 800;
    public const int MaxPossibleHeight = 2050;
    public const int MinPossibleHeight = 700;

    public const int AER = 10;
    public const int ALMAG = 30;
    public const int DoorHeightAdjDefault = 15;    //Glass Distance from the Floor
    public const int TOLLMINUS = 0;
    public const int TOLLPLUS = 0;
    public const int TOLLMINUSMAGNET = 10; //When Single door With Magnet Minus Tollerance
    public const int TOLLPLUSMAGNET = 0;   //When Single door With Magnet Plus tollerance is again 0

    public override CabinDBParts Parts { get; }
    public override ConstraintsDB Constraints { get; }
    public override int TotalTollerance => 
        (Parts.CloseProfile is not null ? Parts.CloseProfile.Tollerance : 0) +
        totalTolleranceAdjustment;

    public CabinDB(ConstraintsDB constraints, CabinDBParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        if (Parts?.Hinge is not null)
        {
            double opening = LengthMin - Parts.Hinge.GlassGapAER - (Parts.CloseProfile?.ThicknessView ?? 0);
            return opening > 0 ? opening : 0;
        }
        return 0;
    }

    /// <summary>
    /// Determines the Number of Hinges based on the NominalLength & Height of the Cabin
    /// </summary>
    /// <returns></returns>
    public int GetHingesNumber()
    {
        if (NominalLength > 700 && Height > 2150)
        {
            return 4;
        }
        else if ((NominalLength > 500 && Height > 2050) ||
            (NominalLength > 650))
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }

    /// <summary>
    /// Returns a Deep Copy of this Cabin
    /// </summary>
    /// <returns></returns>
    public override CabinDB GetDeepClone()
    {
        CabinDB cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }

}
