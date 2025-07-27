using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface ICloseProfile : ICloseStrip
    {
        public Profile CloseProfile { get; set; }
    }

}
