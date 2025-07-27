
using BronzeFactoryApplication.ApplicationServices.MessangerService;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class Parts94ViewModel : PartsViewModel<Cabin94Parts>
    {
        private readonly ICabinMemoryRepository repo;
        //ALL PARTS CHANGE ONLY WHEN THE CODE CHANGES

        public Profile? WallProfile1
        {
            get => partsListObject?.WallProfile1;
            set
            {
                if (partsListObject is not null && partsListObject.WallProfile1?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.WallProfile1;
                    partsListObject.WallProfile1 = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide1,Identifier,repo);
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
        public GlassStrip? CloseStrip
        {
            get => partsListObject?.CloseStrip;
            set
            {
                if (partsListObject is not null && partsListObject.CloseStrip?.Code != value?.Code)
                {
                    var oldStrip = partsListObject.CloseStrip;
                    partsListObject.CloseStrip = value?.GetCloneWithSpotDefaultQuantity(PartSpot.CloseStrip, Identifier, repo); ;
                    OnPropertyChanged(nameof(CloseStrip));
                    RaisePartChanged(PartSpot.CloseStrip, partsListObject.CloseStrip, oldStrip);
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
                    //Each side up or down must have a seperate object
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
                    RaisePartChanged(PartSpot.Handle1,partsListObject?.Handle, oldHandle);
                }
            }
        }

        [ObservableProperty]
        private bool isHandleAsPrimary = true;

        public Parts94ViewModel(ICabinMemoryRepository repo):base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList,CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as Cabin94Parts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(Cabin94Parts)}");
            //Inform all Changed in the Cabin ViewModel
        }
        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.Handle1:
                    OnPropertyChanged(nameof(Handle));
                    break;
                case PartSpot.Handle2:
                    OnPropertyChanged(nameof(Handle));
                    break;
                case PartSpot.HorizontalGuideTop:
                    OnPropertyChanged(nameof(HorizontalProfile));
                    break;
                case PartSpot.HorizontalGuideBottom:
                    OnPropertyChanged(nameof(HorizontalProfile));
                    break;
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallProfile1));
                    break;
                case PartSpot.WallSide2:
                    OnPropertyChanged(nameof(WallProfile2));
                    break;
                case PartSpot.StepBottomSide:
                    OnPropertyChanged(nameof(StepBottomProfile));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
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
                    partToEdit = HorizontalProfileTop;
                    break;
                case PartSpot.HorizontalGuideBottom:
                    partToEdit = HorizontalProfileBottom;
                    break;
                case PartSpot.WallSide1:
                    partToEdit = WallProfile1;
                    break;
                case PartSpot.WallSide2:
                    partToEdit = WallProfile2;
                    break;
                case PartSpot.StepBottomSide:
                    partToEdit = StepBottomProfile;
                    break;
                case PartSpot.CloseStrip:
                    partToEdit = CloseStrip;
                    break;
                default:
                    return;
            }
            if (partToEdit is not null)
            {
                messenger.Send(new EditLivePartMessage(spot, partToEdit, this));
            }
        }
    }
}
