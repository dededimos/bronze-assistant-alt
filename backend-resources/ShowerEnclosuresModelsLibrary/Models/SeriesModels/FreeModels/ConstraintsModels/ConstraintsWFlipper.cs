using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels
{
    public class ConstraintsWFlipper : CabinConstraints
    {
        public int DoorDistanceFromBottom { get; set; }
        public int CornerRadiusTopEdge { get; set; }

        public ConstraintsWFlipper()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsWFlipper(ConstraintsWFlipper constraints):base(constraints)
        {
            this.DoorDistanceFromBottom = constraints.DoorDistanceFromBottom;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
        }
        public override ConstraintsWFlipper GetDeepClone()
        {
            return new ConstraintsWFlipper(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawFL, GlassTypeEnum.DoorGlass, 1);
            AllowedGlasses.Add(glass1);
        }
    }
}
