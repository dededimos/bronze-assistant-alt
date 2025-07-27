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
    public abstract class Inox304 : Cabin
    {
        public override CabinSeries Series { get => CabinSeries.Inox304; }

        public Inox304()
        {

        }
    }
}
