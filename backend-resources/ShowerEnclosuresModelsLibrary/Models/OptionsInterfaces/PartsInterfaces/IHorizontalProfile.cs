using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface IHorizontalProfile
    {
        public Profile HorizontalProfileTop { get; set; }
        public Profile HorizontalProfileBottom { get; set; }
    }
}
