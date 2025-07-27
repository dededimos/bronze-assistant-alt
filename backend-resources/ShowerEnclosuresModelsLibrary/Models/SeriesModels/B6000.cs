using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels
{
    /// <summary>
    /// Series Bronze6000 Cabin
    /// </summary>
    public abstract class B6000:Cabin
    {
        /// <summary>
        /// The Height of the Horizontal Aluminium Rod (L0)
        /// </summary>
        public const int L0Height = 50;
        /// <summary>
        /// The Distance of the Fixed Glass from the Top or the Bottom of the Cabin (The Glass is framed so its not touching bottom or top)
        /// </summary>
        public const int FixedGlassMargin = 36;
        /// <summary>
        /// The Distance of the Door Glass from the Top or the Bottom of the Cabin. The Door is Suspended and never reaches the Full height of the Cabin
        /// </summary>
        public const int DoorGlassMargin = 10;
        
        public override CabinSeries Series { get => CabinSeries.Bronze6000; }

        public B6000()
        {

        }
    }
}
