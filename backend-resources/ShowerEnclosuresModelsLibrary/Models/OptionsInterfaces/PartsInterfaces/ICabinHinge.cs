using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface ICabinHinge
    {
        public CabinHinge Hinge { get; set; }
    }

    public interface ICabinHinge<T> : ICabinHinge where T : CabinHinge
    {
        new public T Hinge { get; set; }
    }
}
