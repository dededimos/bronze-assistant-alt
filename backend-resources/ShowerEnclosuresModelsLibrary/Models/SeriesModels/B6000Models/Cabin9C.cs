using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Validators;
using ShowerEnclosuresModelsLibrary.Validators.B6000Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models
{
    public class Cabin9C : B6000
    {
        public const int MinPossibleLength = 685;
        public const int MaxPossibleLength = 985;
        public const int MaxPossibleHeight = 1850;
        public const int MinPossibleHeight = 1850;
        public const int MinPossibleStepHeight = 60;
        public const int AllowableSerigraphyLength1 = 800; //The Lengths Allowed on Serigraphy or Frosted
        public const int AllowableSerigraphyLength2 = 900; //The Length Allowed on Serigraphy or Frosted

        private const int AL1 = 42;     //Wall Aluminium Default
        private const int ALST = 10;    //ALST Default
        public const int EPIK = 7;      //OverlapDefault
        public const int HD = 100;     //Handle Distance Default
        public const int CD = 27;      //Cover Distance Default
        public const int TOLLMINUS = 15;   //Default Minus Tollerance
        public const int TOLLPLUS = 10;    //Default Plus Tollerance

        //public int WallAluminiumAL1 { get; set; }
        //public int GlassInAluminiumALST { get; set; }
        //public int OverlapEPIK { get; set; }
        //public int MagnetStripMAGN { get; set; }
        //public int HandleDistanceHD { get; set; }
        //public int CoverDistanceCD { get; set; }

        public override Cabin9CParts Parts { get; }
        public override Constraints9C Constraints { get; }
        public override int TotalTollerance => 
            (Parts.WallProfile1?.Tollerance ?? 0) +
            totalTolleranceAdjustment;

        public Cabin9C(Constraints9C constraints , Cabin9CParts parts)
        {
            this.Constraints = constraints;
            this.Parts = parts;
        }

        protected override double GetOpening()
        {
            return 0;
        }

        /// <summary>
        /// Returns a Deep Copy of this Cabin
        /// </summary>
        /// <returns></returns>
        public override Cabin9C GetDeepClone()
        {
            Cabin9C cabin = new(this.Constraints.GetDeepClone(), this.Parts.GetDeepClone());
            return CopyBaseProperties(cabin);
        }
    }
}
