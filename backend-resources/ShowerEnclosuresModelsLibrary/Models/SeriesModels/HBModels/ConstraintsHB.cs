using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels
{
    public class ConstraintsHB : CabinConstraints
    {
        public int MaxDoorLength { get; set; }
        public int MinDoorLength { get; set; }
        public int MaxFixedLength { get; set; }
        public int MinFixedLength { get; set; }
        public int DoorDistanceFromBottom { get; set; }
        public int CornerRadiusTopEdge { get; set; }
        /// <summary>
        /// The Length to be added to the Door Glass to find the front Sealer
        /// </summary>
        public int DoorSealerLengthCorrection { get; set; }

        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        public LengthCalculationOption LengthCalculation { get; set; }
        /// <summary>
        /// Partial Length is either the visible Length of the Fixed Part OR the Visible Length of the Door
        /// </summary>
        public int PartialLength { get; set; } 
        public ConstraintsHB()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsHB(ConstraintsHB constraints) : base(constraints)
        {
            this.MaxDoorLength = constraints.MaxDoorLength;
            this.MinDoorLength = constraints.MinDoorLength;
            this.MaxFixedLength = constraints.MaxFixedLength;
            this.MinFixedLength = constraints.MinFixedLength;
            this.DoorDistanceFromBottom = constraints.DoorDistanceFromBottom;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.LengthCalculation = constraints.LengthCalculation;
            this.PartialLength = constraints.PartialLength;
            this.DoorSealerLengthCorrection = constraints.DoorSealerLengthCorrection;
        }
        public override ConstraintsHB GetDeepClone()
        {
            return new ConstraintsHB(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawHB1, GlassTypeEnum.FixedGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawHB2, GlassTypeEnum.DoorGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
