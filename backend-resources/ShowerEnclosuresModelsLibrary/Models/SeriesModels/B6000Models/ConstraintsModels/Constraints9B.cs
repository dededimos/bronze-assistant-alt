using ShowerEnclosuresModelsLibrary.Enums;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels
{
    public class Constraints9B : CabinConstraints
    {
        public int MinPossibleDoorLength { get; set; }
        public int MinPossibleFixedLength { get; set; }
        public int MaxPossibleDoorLength { get; set; }
        /// <summary>
        /// The Cabin Length at Which a Fixed Part is Added
        /// </summary>
        public int AddedFixedGlassLengthBreakpoint { get; set; }
        public int HingeDistanceFromDoorGlass { get; set; }
        /// <summary>
        /// How much Distance/Air the Door has from the Wall Aluminium or for the Fixed Part
        /// </summary>
        public int GlassGapAERHorizontal { get; set; }
        /// <summary>
        /// How Much Distance/Air The Door Has from the Top and Bottom of the L0 Profiles
        /// </summary>
        public int GlassGapAERVertical { get; set; }
        /// <summary>
        /// Correction added to LO True Length
        /// </summary>
        public int CorrectionOfL0Length { get; set; }

        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        public override int FinalHeightCorrection => 0;
        /// <summary>
        /// The Correction Added to the Fixed Glass Size for the Length of the L0 Polycarbonic Sealers
        /// </summary>
        public int SealerL0LengthCorrection { get; set; }

        public Constraints9B()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public Constraints9B(Constraints9B constraints):base(constraints)
        {
            this.MinPossibleDoorLength = constraints.MinPossibleDoorLength;
            this.MinPossibleFixedLength = constraints.MinPossibleFixedLength;
            this.MaxPossibleDoorLength = constraints.MaxPossibleDoorLength;
            this.AddedFixedGlassLengthBreakpoint = constraints.AddedFixedGlassLengthBreakpoint;
            this.HingeDistanceFromDoorGlass = constraints.HingeDistanceFromDoorGlass;
            this.GlassGapAERHorizontal = constraints.GlassGapAERHorizontal;
            this.GlassGapAERVertical = constraints.GlassGapAERVertical;
            this.CorrectionOfL0Length = constraints.CorrectionOfL0Length;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.SealerL0LengthCorrection = constraints.SealerL0LengthCorrection;
        }

        public override Constraints9B GetDeepClone()
        {
            return new Constraints9B(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.Draw9B, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
