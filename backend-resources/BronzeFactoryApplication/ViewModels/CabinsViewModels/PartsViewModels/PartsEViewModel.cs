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
    public partial class PartsEViewModel : PartsViewModel<CabinEParts>
    {
        private readonly ICabinMemoryRepository repo;

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
                    RaisePartChanged(PartSpot.SupportBar, partsListObject.SupportBar, oldSupportBar);
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
                    RaisePartChanged(PartSpot.BottomSide1, partsListObject.BottomFixer, oldBottomFixer);
                }
            }
        }

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.SupportBar:
                    OnPropertyChanged(nameof(SupportBar));
                    break;
                case PartSpot.BottomSide1:
                    OnPropertyChanged(nameof(BottomFixer));
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

        public PartsEViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinEParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinEParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
