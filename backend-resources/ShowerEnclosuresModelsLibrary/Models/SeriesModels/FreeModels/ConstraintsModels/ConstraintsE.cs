using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels
{
    public class ConstraintsE : CabinConstraints
    {
        public int CornerRadiusTopEdge { get; set; }

        public ConstraintsE()
        {
            
        }

        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsE(ConstraintsE constraints):base(constraints)
        {
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
        }

        public override ConstraintsE GetDeepClone()
        {
            return new ConstraintsE(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
        }
    }
}
