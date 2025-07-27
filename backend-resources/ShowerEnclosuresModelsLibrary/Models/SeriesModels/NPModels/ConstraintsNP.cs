using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels
{
    public class ConstraintsNP : CabinConstraints
    {
        
        /// <summary>
        /// The Length of the Structure when itsFolded - This should be Calculatable, but CBA
        /// </summary>
        public int FoldedLength { get; set; }

        /// <summary>
        /// How many more mm is the Length of the Wall Door from the Second Door
        /// </summary>
        public int DoorsLengthDifference { get; set; }
        public int CornerRadiusTopEdge { get; set; }
        /// <summary>
        /// The Length to be added to the Door Glass to find the front Sealer
        /// </summary>
        public int DoorSealerLengthCorrection { get; set; }
        public ConstraintsNP()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsNP(ConstraintsNP constraints): base(constraints)
        {
            this.FoldedLength = constraints.FoldedLength;
            this.DoorsLengthDifference = constraints.DoorsLengthDifference;
            this.CornerRadiusTopEdge = constraints.CornerRadiusTopEdge;
            this.DoorSealerLengthCorrection = constraints.DoorSealerLengthCorrection;
        }
        public override ConstraintsNP GetDeepClone()
        {
            return new ConstraintsNP(this);
        }
        protected override void ConfigureAllowedGlasses()
        {
            AllowedGlass glass1 = new(GlassDrawEnum.DrawDP1, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawDP3, GlassTypeEnum.DoorGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
