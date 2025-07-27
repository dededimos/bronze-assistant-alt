using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels
{
    public class Constraints9A : CabinConstraints , ISlidingConstraints
    {
        public int MaxDoorGlassLength { get; set; }
        public int MinDoorDistanceFromWallOpened { get; set; }
        public int Overlap { get; set; }
        public int CoverDistance { get; set; }

        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        public override int FinalHeightCorrection => 0;
        /// <summary>
        /// The Correction Added to the Fixed Glass Size for the Length of the L0 Polycarbonic Sealers
        /// </summary>
        public int SealerL0LengthCorrection { get; set; }
        public Constraints9A()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public Constraints9A(Constraints9A constraints):base(constraints)
        {
            this.MaxDoorGlassLength = constraints.MaxDoorGlassLength;
            this.MinDoorDistanceFromWallOpened = constraints.MinDoorDistanceFromWallOpened;
            this.TolleranceMinusDefaultMinimum = constraints.TolleranceMinusDefaultMinimum;
            this.Overlap = constraints.Overlap;
            this.CoverDistance = constraints.CoverDistance;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.SealerL0LengthCorrection = constraints.SealerL0LengthCorrection;
        }
        public override Constraints9A GetDeepClone()
        {
            return new Constraints9A(this);
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
            AllowedGlass glass1 = new(GlassDrawEnum.Draw9S, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
