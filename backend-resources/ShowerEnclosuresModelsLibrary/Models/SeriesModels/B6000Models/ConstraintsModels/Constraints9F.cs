using ShowerEnclosuresModelsLibrary.Enums;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels
{
    public class Constraints9F : CabinConstraints
    {
        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        public override int FinalHeightCorrection => 0;
        /// <summary>
        /// The Correction Added to the Fixed Glass Size for the Length of the L0 Polycarbonic Sealers
        /// </summary>
        public int SealerL0LengthCorrection { get; set; }
        public Constraints9F()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public Constraints9F(Constraints9F constraints):base(constraints)
        {
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.SealerL0LengthCorrection = constraints.SealerL0LengthCorrection;
        }

        public override Constraints9F GetDeepClone()
        {
            return new Constraints9F(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
        }
    }
}
