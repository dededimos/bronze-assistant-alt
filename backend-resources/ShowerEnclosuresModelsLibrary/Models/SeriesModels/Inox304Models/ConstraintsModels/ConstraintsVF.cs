using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels
{
    public class ConstraintsVF : CabinConstraints
    {
        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }
        /// <summary>
        /// The Extra Tollerance on Step Length Cutting , when there is no Tollerance from Profile
        /// </summary>
        public int StepLengthTolleranceMin { get; set; }
        public ConstraintsVF()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsVF(ConstraintsVF constraints) : base(constraints)
        {
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.StepLengthTolleranceMin = constraints.StepLengthTolleranceMin;
        }

        public override ConstraintsVF GetDeepClone()
        {
            return new ConstraintsVF(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawVF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
        }
    }

}