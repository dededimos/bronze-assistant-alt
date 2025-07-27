using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels
{
    public class ConstraintsDB : CabinConstraints
    {
        public int MaxDoorGlassLength { get; set; }
        public int DoorDistanceFromBottom { get; set; }
        public int CornerRadiusTopEdge { get; set; }
        /// <summary>
        /// The Length to be added to the Door Glass to find the front Sealer
        /// </summary>
        public int DoorSealerLengthCorrection { get; set; }

        public ConstraintsDB()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsDB(ConstraintsDB constraints):base(constraints)
        {
            this.MaxDoorGlassLength = constraints.MaxDoorGlassLength;
            this.DoorDistanceFromBottom = constraints.DoorDistanceFromBottom;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
            this.DoorSealerLengthCorrection = constraints.DoorSealerLengthCorrection;
        }

        public override ConstraintsDB GetDeepClone()
        {
            return new ConstraintsDB(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawDB, GlassTypeEnum.DoorGlass, 1);
            AllowedGlasses.Add(glass1);
        }
    }

}
