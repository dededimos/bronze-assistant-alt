using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels
{
    public class ConstraintsNB : CabinConstraints
    {
        /// <summary>
        /// DEPRECATED-DEPRECATED
        /// This is Used Nowhere ... appears NoWhere
        /// Instead the ProfileHinge Properties are Used to Calculate the Glass 
        /// </summary>
        public int DoorHeightAdjustment { get; set; }
        public int CornerRadiusTopEdge { get; set; }
        /// <summary>
        /// The Length to be added to the Door Glass to find the front Sealer
        /// </summary>
        public int DoorSealerLengthCorrection { get; set; }
        public override int MaximumAllowedGlasses => 1;
        public ConstraintsNB()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsNB(ConstraintsNB constraints) : base(constraints)
        {
            this.DoorHeightAdjustment = constraints.DoorHeightAdjustment;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
            this.DoorSealerLengthCorrection = constraints.DoorSealerLengthCorrection;
        }

        public override ConstraintsNB GetDeepClone()
        {
            return new ConstraintsNB(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawNB, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawNV, GlassTypeEnum.DoorGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
