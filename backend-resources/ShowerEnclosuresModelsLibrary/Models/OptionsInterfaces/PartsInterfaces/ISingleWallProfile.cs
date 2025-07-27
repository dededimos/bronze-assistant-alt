using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface IWallProfile
    {
        Profile WallProfile1 { get; set; }
    }

    public interface IDoubleWallProfile : IWallProfile
    {
        Profile WallProfile2 { get; set; }
    }
    public interface ISideFixer
    {
        CabinPart SideFixer { get; set; }
    }

}
