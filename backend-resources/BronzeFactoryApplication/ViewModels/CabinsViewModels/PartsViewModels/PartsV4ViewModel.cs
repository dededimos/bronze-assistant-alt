
using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsV4ViewModel : PartsViewModel<CabinV4Parts>
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

        public CabinPart? WallFixer2
        {
            get => partsListObject?.WallFixer2;
            set
            {
                if (partsListObject is not null && partsListObject.WallFixer2?.Code != value?.Code)
                {
                    var oldWallSideFixer = partsListObject.WallFixer2;
                    partsListObject.WallFixer2 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide2, Identifier, repo);
                    OnPropertyChanged(nameof(WallFixer2));
                    RaisePartChanged(PartSpot.WallSide2, partsListObject.WallFixer2, oldWallSideFixer);
                }
            }
        }
        public CabinPart? DefaultWallFixer2 => repo.GetDefaultPart(Identifier, PartSpot.WallSide2);
        public IEnumerable<CabinPart> SelectableWallFixers2
        {
            get
            {
                var selectables = repo.GetSpotValids(Identifier, PartSpot.WallSide2).Select(c => repo.GetPart(c,Identifier)).ToList();
                if (partsNotPresentInValids.TryGetValue(PartSpot.WallSide2, out CabinPart? part)
                    && part is not null
                    && (selectables.Any(s => s.Code == part.Code) == false))
                {
                    selectables.Add(part);
                }
                return selectables;
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
                    RaisePartChanged(PartSpot.SupportBar, partsListObject.SupportBar, oldSupportBar);
                }
            }
        }

        public CabinPart? BottomFixer1 => partsListObject?.BottomFixer1;
        public CabinPart? BottomFixer2 => partsListObject?.BottomFixer2;
        /// <summary>
        /// Defines the BottomFixer 1&2 for the V4 Structure
        /// </summary>
        public CabinPart? BottomFixer
        {
            get => partsListObject?.BottomFixer1;
            set
            {
                if (partsListObject is not null && partsListObject.BottomFixer1?.Code != value?.Code)
                {
                    var oldBottomFixer = partsListObject.BottomFixer1;
                    //Each side up or down must have a sepera   te object
                    partsListObject.BottomFixer1 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.BottomSide1, Identifier, repo);
                    partsListObject.BottomFixer2 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.BottomSide2, Identifier, repo);
                    OnPropertyChanged(nameof(BottomFixer));
                    OnPropertyChanged(nameof(BottomFixer1));
                    OnPropertyChanged(nameof(BottomFixer2));
                    RaisePartChanged(PartSpot.BottomSide1, partsListObject.BottomFixer1, oldBottomFixer);
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
                    RaisePartChanged(PartSpot.Handle1, partsListObject.Handle, oldHandle);
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
                case PartSpot.WallSide2:
                    OnPropertyChanged(nameof(WallFixer2));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
                    break;
                case PartSpot.SupportBar:
                    OnPropertyChanged(nameof(SupportBar));
                    break;
                case PartSpot.BottomSide1:
                case PartSpot.BottomSide2:
                    OnPropertyChanged(nameof(BottomFixer));
                    break;
                case PartSpot.HorizontalGuideTop:
                    OnPropertyChanged(nameof(HorizontalBar));
                    break;
                case PartSpot.Handle1:
                case PartSpot.Handle2:
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
                case PartSpot.Handle2:
                    partToEdit = Handle;
                    break;
                case PartSpot.HorizontalGuideTop:
                    partToEdit = HorizontalBar;
                    break;
                case PartSpot.WallSide1:
                    partToEdit = WallSideFixer;
                    break;
                case PartSpot.WallSide2:
                    partToEdit = WallFixer2;
                    break;
                case PartSpot.CloseStrip:
                    partToEdit = CloseStrip;
                    break;
                case PartSpot.BottomSide1:
                    partToEdit = BottomFixer1;
                    break;
                case PartSpot.BottomSide2:
                    partToEdit = BottomFixer2;
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


        public PartsV4ViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList, identifier);
            partsListObject = partsList as CabinV4Parts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinV4Parts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
