using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface IPerimetricalFixer : IWallSideFixer, ISideFixer,IBottomFixer,ITopFixer
    {
        bool HasPerimetricalFrame
        {
            get
            {
                return
                    this.WallSideFixer is Profile &&
                    this.BottomFixer is Profile &&
                    this.SideFixer is Profile &&
                    this.TopFixer is Profile;
            }
        }
    }

}
