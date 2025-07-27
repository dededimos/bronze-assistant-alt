using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels
{
    public class ConstraintsWS : CabinConstraints , ISlidingConstraints
    {
        public int MaxDoorLength { get; set; }
        /// <summary>
        /// The minimum Overlap That the Doors Can Have with the Fixed Parts
        /// </summary>
        public int MinimumGlassOverlapping { get; set; }

        public int DoorDistanceFromBottom { get; set; }
        public int Overlap { get; set; }
        public int CoverDistance { get; set; }
        public int MinDoorDistanceFromWallOpened { get; set; } = 0;

        public ConstraintsWS()
        {

        }
        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public ConstraintsWS(ConstraintsWS constraints) : base(constraints)
        {
            this.MaxDoorLength = constraints.MaxDoorLength;
            this.MinimumGlassOverlapping = constraints.MinimumGlassOverlapping;
            this.DoorDistanceFromBottom = constraints.DoorDistanceFromBottom;
            this.Overlap = constraints.Overlap;
            this.CoverDistance = constraints.CoverDistance;
        }
        public override ConstraintsWS GetDeepClone()
        {
            return new ConstraintsWS(this);
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
            AllowedGlass glass1 = new(GlassDrawEnum.DrawWS, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
