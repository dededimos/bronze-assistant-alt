using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels
{
    public class ConstraintsW : CabinConstraints
    {
        /// <summary>
        /// The minimum Opening that is left when a Fixed Panel is Installed 
        /// </summary>
        public int MinimumFreeOpening { get; set; }
        
        public int CornerRadiusTopEdge { get; set; }

        /// <summary>
        /// The Default Tollerance being applied to the Cut Glass with a Step
        /// </summary>
        public int StepHeightTollerance { get; set; }

        /// <summary>
        /// The Extra Tollerance on Step Length Cutting , when there is no Tollerance from Profile
        /// </summary>
        public int StepLengthTolleranceMin { get; set; }

        /// <summary>
        /// If this Fixed Panel can have a Perimetrical Frame Option
        /// </summary>
        public bool CanHavePerimetricalFrame { get; set; }

        public override int MaximumAllowedGlasses { get => 1; }

        public ConstraintsW()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsW(ConstraintsW constraints) : base(constraints)
        {
            this.MinimumFreeOpening = constraints.MinimumFreeOpening;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
            this.StepHeightTollerance = constraints.StepHeightTollerance;
            this.StepLengthTolleranceMin = constraints.StepLengthTolleranceMin;
            this.CanHavePerimetricalFrame = constraints.CanHavePerimetricalFrame;
        }

        public override ConstraintsW GetDeepClone()
        {
            return new ConstraintsW(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawH1, GlassTypeEnum.FixedGlass, 1);
            AllowedGlass glass3 = new(GlassDrawEnum.DrawNV, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
            AllowedGlasses.Add(glass3);
        }
    }
}
