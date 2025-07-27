using BronzeFactoryApplication.ApplicationServices.MessangerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsWFlipperViewModel : PartsViewModel<CabinWFlipperParts>
    {
        private readonly ICabinMemoryRepository repo;

        public GlassToGlassHinge? Hinge
        {
            get => partsListObject?.Hinge;
            set
            {
                if (partsListObject is not null && partsListObject.Hinge?.Code != value?.Code)
                {
                    var oldHinge = partsListObject.Hinge;
                    partsListObject.Hinge = value?.GetCloneWithSpotDefaultQuantity(PartSpot.MiddleHinge, Identifier, repo);
                    OnPropertyChanged(nameof(Hinge));
                    RaisePartChanged(PartSpot.MiddleHinge, partsListObject.Hinge, oldHinge);
                }
            }
        }

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.MiddleHinge:
                    OnPropertyChanged(nameof(Hinge));
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
                case PartSpot.MiddleHinge:
                    partToEdit = Hinge;
                    break;
                default:
                    return;
            }
            if (partToEdit is not null)
            {
                messenger.Send(new EditLivePartMessage(spot, partToEdit, this));
            }
        }

        public PartsWFlipperViewModel(ICabinMemoryRepository repo):base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinWFlipperParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinWFlipperParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
