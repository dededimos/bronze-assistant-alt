using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface ITopFixer
    {
        CabinPart TopFixer { get; set; }
    }
}
