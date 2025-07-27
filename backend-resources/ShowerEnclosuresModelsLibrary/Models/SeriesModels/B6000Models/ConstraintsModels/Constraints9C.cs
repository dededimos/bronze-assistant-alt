using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.ModelsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels
{
    public class Constraints9C : CabinConstraints , ISlidingConstraints
    {
        /// <summary>
        /// The Min Length at which Serigrapgy Glass is Supported
        /// </summary>
        public IEnumerable<int> AllowableSerigraphyLengths { get; set; }

        public override int FinalHeightCorrection => 0;
        /// <summary>
        /// The Correction Added to the Fixed Glass Size for the Length of the L0 Polycarbonic Sealers
        /// </summary>
        public int SealerL0LengthCorrection { get; set; }
        public int MinDoorDistanceFromWallOpened { get; set; } = 0;
        public int CoverDistance { get; set; } = 0;
        public int Overlap { get; set; } = 0;

        public Constraints9C()
        {

        }

        public Constraints9C(Constraints9C constraints):base(constraints)
        {
            this.AllowableSerigraphyLengths = new List<int>(constraints.AllowableSerigraphyLengths);
            this.SealerL0LengthCorrection = constraints.SealerL0LengthCorrection;
        }

        public override Constraints9C GetDeepClone()
        {
            return new Constraints9C(this);
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
            AllowedGlass glass1 = new(GlassDrawEnum.Draw9C, GlassTypeEnum.DoorGlass, 1);
            AllowedGlass glass2 = new(GlassDrawEnum.DrawF, GlassTypeEnum.FixedGlass, 1);
            AllowedGlasses.Add(glass1);
            AllowedGlasses.Add(glass2);
        }
    }
}
