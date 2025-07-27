using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsVFViewModel : PartsViewModel<CabinVFParts>
    {
        private readonly ICabinMemoryRepository repo;

        public CabinPart? WallSideFixer
        {
            get => partsListObject?.WallSideFixer;
            set
            {
                if (partsListObject is not null && partsListObject.WallSideFixer?.Code != value?.Code)
                {
                    var oldWallSideFixer = partsListObject.WallSideFixer;
                    partsListObject.WallSideFixer = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide1, Identifier, repo);
                    OnPropertyChanged(nameof(WallSideFixer));
                    RaisePartChanged(PartSpot.WallSide1, partsListObject.WallSideFixer, oldWallSideFixer);
                }
            }
        }
        public SupportBar? SupportBar
        {
            get => partsListObject?.SupportBar;
            set
            {
                if (partsListObject is not null && partsListObject.SupportBar?.Code != value?.Code)
                {
                    var oldSupportBar = partsListObject.SupportBar;
                    partsListObject.SupportBar = value?.GetCloneWithSpotDefaultQuantity(PartSpot.SupportBar, Identifier, repo);
                    OnPropertyChanged(nameof(SupportBar));
                    RaisePartChanged(PartSpot.SupportBar, partsListObject.SupportBar,oldSupportBar);
                }
            }
        }

        /// <summary>
        /// Always Set by other Items
        /// </summary>
        public CabinPart? SideFixer
        {
            get => partsListObject?.SideFixer;
            set
            {
                if (partsListObject is not null && partsListObject.SideFixer?.Code != value?.Code)
                {
                    var oldSideFixer = partsListObject.SideFixer;
                    partsListObject.SideFixer = value?.GetCloneWithSpotDefaultQuantity(PartSpot.NonWallSide, Identifier, repo);
                    OnPropertyChanged(nameof(SideFixer));
                    RaisePartChanged(PartSpot.NonWallSide, partsListObject.SideFixer, oldSideFixer);
                }
            }
        }

        public bool IsWithoutBottomFixer
        {
            get => BottomFixer is null;
            set
            {
                if (IsWithoutBottomFixer != value)
                {
                    if (value is true)
                    {
                        BottomFixer = null;
                    }
                    else
                    {
                        BottomFixer = DefaultBottomFixer ?? SelectableBottomFixers.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(IsWithoutBottomFixer));
                }
            }
        }
        public CabinPart? BottomFixer
        {
            get => partsListObject?.BottomFixer;
            set
            {
                if (partsListObject is not null && partsListObject.BottomFixer?.Code != value?.Code)
                {
                    var oldBottomFixer = partsListObject.BottomFixer;
                    partsListObject.BottomFixer = value?.GetCloneWithSpotDefaultQuantity(PartSpot.BottomSide1, Identifier, repo);
                    OnPropertyChanged(nameof(BottomFixer));
                    OnPropertyChanged(nameof(IsWithoutBottomFixer));
                    RaisePartChanged(PartSpot.BottomSide1, partsListObject.BottomFixer, oldBottomFixer);
                }
            }
        }

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallSideFixer));
                    break;
                case PartSpot.SupportBar:
                    OnPropertyChanged(nameof(SupportBar));
                    break;
                case PartSpot.BottomSide1:
                    OnPropertyChanged(nameof(BottomFixer));
                    break;
                case PartSpot.NonWallSide:
                    OnPropertyChanged(nameof(SideFixer));
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
                case PartSpot.WallSide1:
                    partToEdit = WallSideFixer;
                    break;
                case PartSpot.NonWallSide:
                    partToEdit = SideFixer;
                    break;
                case PartSpot.BottomSide1:
                    partToEdit = BottomFixer;
                    break;
                case PartSpot.SupportBar:
                    partToEdit = SupportBar;
                    break;
                default:
                    return;
            }
            if (partToEdit is not null)
            {
                messenger.Send(new EditLivePartMessage(spot, partToEdit, this));
            }
        }

        public PartsVFViewModel(ICabinMemoryRepository repo):base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinVFParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinVFParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
