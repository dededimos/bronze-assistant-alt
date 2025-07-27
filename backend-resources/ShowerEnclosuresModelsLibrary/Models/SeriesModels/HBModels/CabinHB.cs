using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;

public class CabinHB : Hotel8000 , IWithHingeCuts
{
    public const int MaxPossibleLength = 1785;
    public const int MinPossibleLength = 385;
    public const int MinPossibleHeight = 201;
    public const int MaxPossibleHeight = 2050;
    public const int MaxPossibleDoorLength = 750;
    public const int MinPossibleDoorLength = 200;
    public const int MinPossibleFixedLength = 155;
    public const int MaxPossibleFixedLength = 1385;

    public const int AL1 = 34;
    public const int ALST = 12;
    public const int AER = 5;
    public const int ALMAG = 30;
    public const int StopperHeight = 0;     //Glass Height Shrink , Due to the Bottom Glass Stopper
    public const int DoorHeightAdj = 15;    //Glass Distance from the Floor
    public const int DefaultDoorLength = 600;
    public const int TOLLMINUS = 15;
    public const int TOLLPLUS = 10;
    public const int TOLLMAGNET = 10; // When at Draw 34 With Magnet Extra Tollerance of 10mm

    /*NOT CONSIDERED IN THIS MODEL -- MAYBE WE HAVE TO INCLUDE IN THE FUTURE - ITS NOT CORRECT TO HAVE FINAL HEIGHT WITHOUT THIS
     IF WE TAKE IT INTO ACCOUNT WE HAVE TO CHANGE ALSO THE DOOR HEIGHT AFTERWARDS NOT ONLY THE FIXED PART HEIGHT*/
    //public const int SupportBarHeight = 20;  //Glass Height Shrink , Due to the Support Bar Height
    /*NOT CONSIDERED IN THIS MODEL*/


    public override CabinHBParts Parts { get; }
    public override ConstraintsHB Constraints { get; }
    public override int TotalTollerance => 
        (Parts.WallSideFixer is Profile profile ? profile.Tollerance : 0)
        + totalTolleranceAdjustment;

    public CabinHB(ConstraintsHB constraints, CabinHBParts parts)
    {
        Constraints = constraints;
        Parts = parts;
    }

    protected override double GetOpening()
    {
        if (Parts is not null)
        {
            var door = Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.DoorGlass);

            return door is null ? 0 : (door.Length + (Parts.CloseStrip?.OutOfGlassLength ?? 0));
        }
        return 0;
    }

    /// <summary>
    /// Determines the Number of Hinges based on the DoorLength & Height of the Cabin
    /// </summary>
    /// <returns></returns>
    public int GetHingesNumber()
    {
        //Partial Length refers to the Door Length of HB (We have to fix that at some point... this should have been an option for the ui and not the model)
        //Two new properties must be introduced that depend on each other (Or we could only set the Door length instead of Fixed and Fixed gets calculated ?)
        //The UI Can take responsibility for the Rest
        
        //Find the Door Length
        int doorLength = 0;
        if (Constraints.LengthCalculation is LengthCalculationOption.ByDoorLength)
        {
            doorLength = Constraints.PartialLength;
        }
        else
        {
            doorLength = NominalLength - Constraints.PartialLength;
        }
        
        //Check for Hinges
        if ( doorLength > 700 && Height > 2150)
        {
            return 4;
        }
        else if ((doorLength > 500 && Height > 2050) ||
            (doorLength > 650))
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
    public override CabinHB GetDeepClone()
    {
        CabinHB cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
        return CopyBaseProperties(cabin);
    }


}
