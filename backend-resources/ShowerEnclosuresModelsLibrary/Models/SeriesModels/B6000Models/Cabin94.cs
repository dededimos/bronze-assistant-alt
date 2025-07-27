using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models
{
#nullable disable
    public class Cabin94 : B6000
    {
        public const int MinPossibleLength = 1085;
        public const int MaxPossibleLength = 2385;
        public const int MaxPossibleHeight = 2100;
        public const int MinPossibleHeight = 600;
        public const int MinPossibleStepHeight = 60;
        public const int MaxDoorGlassLength = 430;

        private const int AL1 = 40;      //Width Of Wall Aluminium 1
        private const int ALST = 10;     //Depth of Glass Cavity on Glass Aluminium
        public const int EPIK = 7;       //Overlap Of Glasses when Door is Closed
        private const int AL2 = 40;      //Width of Wall Aluminium 2
        public const int HD = 100;       //Distance of Handle from the End of the Door Glass
        public const int CD = 28;        //Cover Distance -- Same Glasses -90 ,MaxOpening 28 (How Far the Glass is from the Wall Aluminium when the Door is Fully Opened -- Positive Values mean it has Overlapped the Wall Aluminium)
        public const int CDsame = -90;   //Cover Distance Value In order To have the Same Length in Fixed and Door Glasses
        public const int TOLLMINUS = 15; //The Negative Tollerance of the Cabin
        public const int TOLLPLUS = 40;  //The Positive Tollerance of the Cabin

        public override Cabin94Parts Parts { get; }
        public override Constraints94 Constraints { get; }
        public override int TotalTollerance => 
            (Parts.WallProfile1?.Tollerance ?? 0) + 
            (Parts.WallProfile2?.Tollerance ?? 0) + 
             totalTolleranceAdjustment;

        public Cabin94(Constraints94 constraints , Cabin94Parts parts)
        {
            this.Constraints = constraints;
            this.Parts = parts;
        }

        protected override double GetOpening()
        {
            // Opening always depends on Glasses having been built

            // Early Escape
            if (Glasses.Count != 4)
            {
                return 0;
            }

            // To Calculate the Opening we need to find the Hiding Distance ,
            // how many mm the Sliding glass can hide behind the Fixed Part
            // Hiding Distance = PartsLengths + Fixed Part - Overlap - CoverDistance - StepLength (This is the Theoritical Maximum Opening)
            // Then We need to figure out the Length of the Sliding Glass that is available for Hiding
            // Length Available for Hiding = Sliding Glass - Overlap - HandleSlidingDoorAirDistance
            // If the Available Length for Hiding > HidingDistance then the Opening is the Hiding Distance , else its the Available Length for Hiding
            // This Model needs to Calculate two Openings

            IEnumerable<Glass> fixedGlasses = Glasses.Where(g => g.GlassType == GlassTypeEnum.FixedGlass);
            IEnumerable<Glass> doors = Glasses.Where(g => g.GlassType == GlassTypeEnum.DoorGlass);
            if (fixedGlasses.Count() != 2 || doors.Count() != 2)
            {
                return 0;
            }
            var firstFixed = fixedGlasses.First();
            var secondFixed = fixedGlasses.Skip(1).First();
            var firstDoor = doors.First();
            var secondDoor = doors.Skip(1).First();

            if (Parts.WallProfile1 is null || Parts.WallProfile2 is null || Parts.Handle is null)
            {
                return 0;
            }

            int hidingDistance1 = Parts.WallProfile1.ThicknessView
                 + Convert.ToInt32(firstFixed.Length)
                 - Convert.ToInt32(firstFixed.StepLength)
                 - Parts.WallProfile1.GlassInProfileDepth
                 - Constraints.CoverDistance
                 - Constraints.Overlap;

            int hidingDistance2 = Parts.WallProfile2.ThicknessView
                 + Convert.ToInt32(secondFixed.Length)
                 - Convert.ToInt32(secondFixed.StepLength)
                 - Parts.WallProfile2.GlassInProfileDepth
                 - Constraints.CoverDistance
                 - Constraints.Overlap;

            int availableHiding1 = Convert.ToInt32(firstDoor.Length)
                - Constraints.Overlap
                - Parts.Handle.GetSlidingDoorAirDistance();
            int availableHiding2 = Convert.ToInt32(secondDoor.Length)
                - Constraints.Overlap
                - Parts.Handle.GetSlidingDoorAirDistance();

            int opening1 = availableHiding1 > hidingDistance1 ? hidingDistance1 : availableHiding1;
            int opening2 = availableHiding2 > hidingDistance2 ? hidingDistance2 : availableHiding2;

            return Convert.ToDouble(opening1 + opening2);
        }

        /// <summary>
        /// Returns a Deep Copy of this Cabin
        /// </summary>
        /// <returns></returns>
        public override Cabin94 GetDeepClone()
        {
            Cabin94 cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
            return CopyBaseProperties(cabin);
        }
    }
}
