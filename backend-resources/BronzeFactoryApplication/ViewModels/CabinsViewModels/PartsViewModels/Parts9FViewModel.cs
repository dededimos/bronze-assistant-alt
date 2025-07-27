using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class Parts9FViewModel : PartsViewModel<Cabin9FParts>
    {
        private readonly ICabinMemoryRepository repo;

        public Profile? WallProfile1
        {
            get => partsListObject?.WallProfile1;
            set
            {
                if (partsListObject is not null && partsListObject.WallProfile1?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.WallProfile1;
                    partsListObject.WallProfile1 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide1, Identifier, repo);
                    OnPropertyChanged(nameof(WallProfile1));
                    RaisePartChanged(PartSpot.WallSide1, partsListObject.WallProfile1, oldProfile);
                }
            }
        }
        public Profile? WallProfile2
        {
            get => partsListObject?.WallProfile2;
            set
            {
                if (partsListObject is not null && partsListObject.WallProfile2?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.WallProfile2;
                    partsListObject.WallProfile2 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide2, Identifier, repo);
                    OnPropertyChanged(nameof(WallProfile2));
                    RaisePartChanged(PartSpot.WallSide2, partsListObject.WallProfile2, oldProfile);
                }
            }
        }
        public Profile? StepBottomProfile
        {
            get => partsListObject?.StepBottomProfile;
            set
            {
                if (partsListObject is not null && partsListObject.StepBottomProfile?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.StepBottomProfile;
                    partsListObject.StepBottomProfile = value?.GetCloneWithSpotDefaultQuantity(PartSpot.StepBottomSide, Identifier, repo);
                    OnPropertyChanged(nameof(StepBottomProfile));
                    RaisePartChanged(PartSpot.StepBottomSide, partsListObject.StepBottomProfile, oldProfile);
                }
            }
        }
        /// <summary>
        /// Represents Both Top and Bottom Profile
        /// </summary>
        public Profile? HorizontalProfile
        {
            get => partsListObject?.HorizontalProfileTop;
            set
            {
                if (partsListObject is not null && partsListObject.HorizontalProfileTop?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.HorizontalProfileTop;
                    //Each side up or down must have a sepera   te object
                    partsListObject.HorizontalProfileBottom = value?.GetCloneWithSpotDefaultQuantity(PartSpot.HorizontalGuideBottom, Identifier, repo);
                    partsListObject.HorizontalProfileTop = value?.GetCloneWithSpotDefaultQuantity(PartSpot.HorizontalGuideTop, Identifier, repo);
                    OnPropertyChanged(nameof(HorizontalProfile));
                    OnPropertyChanged(nameof(HorizontalProfileTop));
                    OnPropertyChanged(nameof(HorizontalProfileBottom));
                    RaisePartChanged(PartSpot.HorizontalGuideTop, partsListObject.HorizontalProfileTop, oldProfile);
                }
            }
        }
        public Profile? HorizontalProfileTop => partsListObject?.HorizontalProfileTop;
        public Profile? HorizontalProfileBottom => partsListObject?.HorizontalProfileBottom;
        
        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallProfile1));
                    break;
                case PartSpot.NonWallSide:
                    OnPropertyChanged(nameof(WallProfile2));
                    break;
                case PartSpot.HorizontalGuideBottom:
                case PartSpot.HorizontalGuideTop:
                    OnPropertyChanged(nameof(HorizontalProfile));
                    break;
                case PartSpot.StepBottomSide:
                    OnPropertyChanged(nameof(StepBottomProfile));
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Sends a Request to Edit the Part in the Selected Spot
        /// </summary>
        /// <param name="spot">The Spot where the present part should be Edited</param>
        [RelayCommand]
        public void RequestPartEdit(PartSpot? spotArg)
        {
            PartSpot spot = spotArg is null ? PartSpot.Undefined : (PartSpot)spotArg;
            CabinPart? partToEdit;
            switch (spot)
            {
                case PartSpot.HorizontalGuideTop:
                    partToEdit = HorizontalProfileTop;
                    break;
                case PartSpot.HorizontalGuideBottom:
                    partToEdit = HorizontalProfileBottom;
                    break;
                case PartSpot.WallSide1:
                    partToEdit = WallProfile1;
                    break;
                case PartSpot.NonWallSide:
                    partToEdit = WallProfile2;
                    break;
                case PartSpot.StepBottomSide:
                    partToEdit = StepBottomProfile;
                    break;
                default:
                    return;
            }
            if (partToEdit is not null)
            {
                messenger.Send(new EditLivePartMessage(spot, partToEdit, this));
            }
        }

        public Parts9FViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as Cabin9FParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(Cabin9FParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
