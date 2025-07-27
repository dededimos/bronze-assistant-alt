using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsVAViewModel : PartsViewModel<CabinVAParts>
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
                    RaisePartChanged(PartSpot.WallSide1, partsListObject.WallSideFixer,oldWallSideFixer);
                }
            }
        }

        public GlassStrip? CloseStrip
        {
            get => partsListObject?.CloseStrip;
            set
            {
                if (partsListObject is not null && partsListObject.CloseStrip?.Code != value?.Code)
                {
                    var oldStrip = partsListObject.CloseStrip;
                    partsListObject.CloseStrip = value?.GetCloneWithSpotDefaultQuantity(PartSpot.CloseStrip, Identifier, repo);
                    OnPropertyChanged(nameof(CloseStrip));
                    RaisePartChanged(PartSpot.CloseStrip, partsListObject.CloseStrip, oldStrip);
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
        public Profile? HorizontalBar
        {
            get => partsListObject?.HorizontalBar;
            set
            {
                if (partsListObject is not null && partsListObject.HorizontalBar?.Code != value?.Code)
                {
                    var oldHorizontalBar = partsListObject.HorizontalBar;
                    partsListObject.HorizontalBar = value?.GetCloneWithSpotDefaultQuantity(PartSpot.HorizontalGuideTop, Identifier, repo);
                    OnPropertyChanged(nameof(HorizontalBar));
                    RaisePartChanged(PartSpot.HorizontalGuideTop, partsListObject.HorizontalBar, oldHorizontalBar);
                }
            }
        }
        public CabinAngle? Angle
        {
            get => partsListObject?.Angle;
            set
            {
                if (partsListObject is not null && partsListObject.Angle?.Code != value?.Code)
                {
                    var oldAngle = partsListObject.Angle;
                    partsListObject.Angle = value?.GetCloneWithSpotDefaultQuantity(PartSpot.Angle, Identifier, repo);
                    OnPropertyChanged(nameof(Angle));
                    RaisePartChanged(PartSpot.Angle, partsListObject.Angle, oldAngle);
                }
            }
        }
        public CabinHandle? Handle
        {
            get => partsListObject?.Handle;
            set
            {
                if (partsListObject is not null && partsListObject.Handle?.Code != value?.Code)
                {
                    var oldHandle = partsListObject.Handle;
                    partsListObject.Handle = value?.GetCloneWithSpotDefaultQuantity(PartSpot.Handle1, Identifier, repo);
                    OnPropertyChanged(nameof(Handle));
                    RaisePartChanged(PartSpot.Handle1, partsListObject.Handle,oldHandle);
                }
            }
        }

        [ObservableProperty]
        private bool isHandleAsPrimary = true;

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallSideFixer));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
                    break;
                case PartSpot.SupportBar:
                    OnPropertyChanged(nameof(SupportBar));
                    break;
                case PartSpot.BottomSide1:
                    OnPropertyChanged(nameof(BottomFixer));
                    break;
                case PartSpot.HorizontalGuideTop:
                    OnPropertyChanged(nameof(HorizontalBar));
                    break;
                case PartSpot.Angle:
                    OnPropertyChanged(nameof(Angle));
                    break;
                case PartSpot.Handle1:
                    OnPropertyChanged(nameof(Handle));
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
                case PartSpot.Handle1:
                    partToEdit = Handle;
                    break;
                case PartSpot.HorizontalGuideTop:
                    partToEdit = HorizontalBar;
                    break;
                case PartSpot.WallSide1:
                    partToEdit = WallSideFixer;
                    break;
                case PartSpot.CloseStrip:
                    partToEdit = CloseStrip;
                    break;
                case PartSpot.BottomSide1:
                    partToEdit = BottomFixer;
                    break;
                case PartSpot.SupportBar:
                    partToEdit = SupportBar;
                    break;
                case PartSpot.Angle:
                    partToEdit = Angle;
                    break;
                default:
                    return;
            }
            if (partToEdit is not null)
            {
                messenger.Send(new EditLivePartMessage(spot, partToEdit, this));
            }
        }

        public PartsVAViewModel(ICabinMemoryRepository repo):base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinVAParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinVAParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
