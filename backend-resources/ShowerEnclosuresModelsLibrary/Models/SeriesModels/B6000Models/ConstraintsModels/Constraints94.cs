using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels
{
    public class Constraints94 : CabinConstraints , ISlidingConstraints
    {
        public int MaxDoorGlassLength { get; set; }
        /// <summary>
        /// The Minimum Distance the Door Glass Must have from the Wall when Opened
        /// </summary>
        public int MinDoorDistanceFromWallOpened { get; set; }
        /// <summary>
        /// The Cover Distance needed to achieve max opening for the structure
        /// </summary>
        public int CoverDistanceMaxOpening { get; set; }
        /// <summary>
        /// The Cover Distance needed to achieve Similar Length to All glasses of the Structure
        /// without concern about the opening
        /// </summary>
        public int CoverDistanceSameGlasses { get; set; }
        /// <summary>
        /// The overlap between the Fixed and Door glasses when the structure is closed
        /// </summary>
        public int Overlap { get; set; }
        /// <summary>
        /// The Length which is overlaped after the end of the Fixed glass when the door is fully Open
        /// </summary>
        public int CoverDistance { get; set; }
        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        /// <summary>
        /// The Correction Added to the Fixed Glass Size for the Length of the L0 Polycarbonic Sealers
        /// </summary>
        public int SealerL0LengthCorrection { get; set; }

        /// <summary>
        /// The List at which the Lengths of Glasses are Equal (Fixed Glasses with Door Glasses)
        /// </summary>
        public IEnumerable<int> SameGlassesLengths { get; set; } = new List<int>();

        public override int FinalHeightCorrection => 0;

        public Constraints94()
        {

        }
        
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public Constraints94(Constraints94 constraints):base(constraints)
        {
            this.MaxDoorGlassLength = constraints.MaxDoorGlassLength;
            this.MinDoorDistanceFromWallOpened = constraints.MinDoorDistanceFromWallOpened;
            this.CoverDistanceMaxOpening = constraints.CoverDistanceMaxOpening;
            this.CoverDistanceSameGlasses = constraints.CoverDistanceSameGlasses;
            this.Overlap = constraints.Overlap;
            this.CoverDistance = constraints.CoverDistance;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.SameGlassesLengths = new List<int>(constraints.SameGlassesLengths);
            this.SealerL0LengthCorrection = constraints.SealerL0LengthCorrection;
        }

        public override Constraints94 GetDeepClone()
        {
            return new Constraints94(this);
        }

        /// <summary>
        /// Returns the Combined overlap for All the Glasses of the Structure
        /// </summary>
        /// <returns>The Total Overlap</returns>
        public int GetTotalOverlap()
        {
            return Overlap * 2;
        }

        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.Draw94, GlassTypeEnum.DoorGlass, 2);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
