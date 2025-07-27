using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface ICloseStrip
    {
        public GlassStrip CloseStrip { get; set; }
    }
}
