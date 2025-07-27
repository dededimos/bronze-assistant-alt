using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels
{
    public class ConstraintsVA : CabinConstraints , ISlidingConstraints
    {
        public int MaxDoorLength { get; set; }
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
        public ConstraintsVA()
        {

        }

        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsVA(ConstraintsVA constraints) : base(constraints)
        {
            this.MaxDoorLength = constraints.MaxDoorLength;
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
        public override ConstraintsVA GetDeepClone()
        {
            return new ConstraintsVA(this);
        }
        /// <summary>
        /// Matches the Overlap Of this Structure as it only has one Door
        /// </summary>
        /// <returns>The Total Overlap</returns>
        public int GetTotalOverlap()
        {
            return Overlap;
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawVS, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawVA, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }

}
