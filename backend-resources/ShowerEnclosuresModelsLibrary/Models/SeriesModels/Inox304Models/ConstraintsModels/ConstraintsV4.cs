using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels
{
    public class ConstraintsV4 : CabinConstraints , ISlidingConstraints
    {
        /// <summary>
        /// The Height Breakpoint at which the MaxDoor Length Changes
        /// </summary>
        public int BreakpointHeight { get; set; }
        /// <summary>
        /// The MaximumDoor Length when Height is Before or Equal to the BreakPoint Height
        /// </summary>
        public int MaxDoorLengthBeforeBreakpoint { get; set; }
        /// <summary>
        /// The Maximum Door Length when Height is Greater than the Brakpoint height
        /// </summary>
        public int MaxDoorLengthAfterBreakpoint { get; set; }
        /// <summary>
        /// The Correction Length of the Main Bar (Used in Glass Calculations)
        /// </summary>
        public int BarCorrectionLength { get; set; }
        /// <summary>
        /// The minimum Overlap That the Doors Can Have with the Fixed Parts
        /// </summary>
        public int MinimumGlassOverlapping { get; set; }
        public int DoorDistanceFromBottom { get; set; }
        public int MinDoorDistanceFromWallOpened { get; set; }
        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        /// <summary>
        /// The Extra Tollerance on Step Length Cutting , when there is no Tollerance from Profile
        /// </summary>
        public int StepLengthTolleranceMin { get; set; }
        public int Overlap { get; set; }
        public int CoverDistance { get; set; }
        /// <summary>
        /// The Length to be added to the Door Glass to find the front Sealer
        /// </summary>
        public int DoorSealerLengthCorrection { get; set; }

        public ConstraintsV4()
        {

        }

        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsV4(ConstraintsV4 constraints) : base(constraints)
        {
            this.BreakpointHeight = constraints.BreakpointHeight;
            this.MaxDoorLengthBeforeBreakpoint = constraints.MaxDoorLengthBeforeBreakpoint;
            this.MaxDoorLengthAfterBreakpoint = constraints.MaxDoorLengthAfterBreakpoint;
            this.BarCorrectionLength = constraints.BarCorrectionLength;
            this.MinimumGlassOverlapping = constraints.MinimumGlassOverlapping;
            this.DoorDistanceFromBottom = constraints.DoorDistanceFromBottom;
            this.MinDoorDistanceFromWallOpened = constraints.MinDoorDistanceFromWallOpened;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.StepLengthTolleranceMin = constraints.StepLengthTolleranceMin;
            this.Overlap = constraints.Overlap;
            this.CoverDistance = constraints.CoverDistance;
            this.DoorSealerLengthCorrection = constraints.DoorSealerLengthCorrection;
        }
        public override ConstraintsV4 GetDeepClone()
        {
            return new ConstraintsV4(this);
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
            AllowedGlass glass1 = new(GlassDrawEnum.DrawVS, GlassTypeEnum.DoorGlass, 2);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawVA, GlassTypeEnum.FixedGlass, 2);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
