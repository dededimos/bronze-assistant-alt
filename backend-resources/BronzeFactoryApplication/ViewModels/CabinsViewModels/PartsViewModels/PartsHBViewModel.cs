using BronzeFactoryApplication.ApplicationServices.MessangerService;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsHBViewModel : PartsViewModel<CabinHBParts>
    {
        private readonly ICabinMemoryRepository repo;

        public CabinPart? WallSideFixer
        {
            get => partsListObject?.WallSideFixer;
            set
            {
                if (partsListObject is not null && partsListObject.WallSideFixer?.Code != value?.Code)
                {
                    var oldProfile = partsListObject.WallSideFixer;
                    partsListObject.WallSideFixer = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallSide1, Identifier, repo);
                    OnPropertyChanged(nameof(WallSideFixer));
                    RaisePartChanged(PartSpot.WallSide1, partsListObject.WallSideFixer,oldProfile);
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
        public GlassStrip? CloseStrip
        {
            get => partsListObject?.CloseStrip;
            set
            {
                if (partsListObject is not null && partsListObject.CloseStrip?.Code != value?.Code)
                {
                    var oldCloseStrip = partsListObject.CloseStrip;
                    partsListObject.CloseStrip = value?.GetCloneWithSpotDefaultQuantity(PartSpot.CloseStrip, Identifier, repo);
                    OnPropertyChanged(nameof(CloseStrip));
                    RaisePartChanged(PartSpot.CloseStrip, partsListObject.CloseStrip, oldCloseStrip);
                }
            }
        }
        public Profile? CloseProfile
        {
            get => partsListObject?.CloseProfile;
            set
            {
                if (partsListObject is not null && partsListObject.CloseProfile?.Code != value?.Code)
                {
                    var oldCloseProfile = partsListObject.CloseProfile;
                    partsListObject.CloseProfile = value?.GetCloneWithSpotDefaultQuantity(PartSpot.CloseProfile, Identifier, repo);
                    OnPropertyChanged(nameof(CloseProfile));
                    RaisePartChanged(PartSpot.CloseProfile, partsListObject.CloseProfile, oldCloseProfile);
                }
            }
        }
        public bool IsWithoutCloser
        {
            get => SelectedCloser is null;
            set
            {
                //Change the Value only when not the same
                if (IsWithoutCloser != value)
                {
                    //If selected is without closer (true) then set closer to null
                    if (value is true)
                    {
                        SelectedCloser = null;
                    }
                    //Else set it to the first available
                    //If there is none available nothing will be able to change and Closer will remain null
                    else
                    {
                        SelectedCloser = SelectableClosers.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(IsWithoutCloser));
                }
            }
        }

        /// <summary>
        /// Can select Only when there are more than one options
        /// </summary>
        public bool CanSelectClosers { get => SelectableClosers.Count() > 1; }
        /// <summary>
        /// The Type of Closing (Only Selectable when there are more than one Closers)
        /// </summary>
        public IEnumerable<CabinPart> SelectableClosers
        {
            get
            {
                var selection = new List<CabinPart>();
                selection.AddRange(SelectableCloseProfiles);
                selection.AddRange(SelectableCloseStrips);
                return selection;
            }
        }
        public CabinPart? SelectedCloser
        {
            //Return the Close profile if any other wise the Strip if Any otherwise null for nothing
            get
            {
                if (CloseProfile is not null)
                {
                    return CloseProfile;
                }
                else
                {
                    return CloseStrip;
                }
            }
            //Set Closers According to selection
            set
            {
                if (value is null)
                {
                    CloseProfile = null;
                    CloseStrip = null;
                }
                else if (value is Profile p)
                {
                    CloseProfile = p;
                    CloseStrip = !string.IsNullOrEmpty(p.ContainedStripCode)
                        ? repo.GetStrip(p.ContainedStripCode,Identifier)
                        : throw new InvalidOperationException($"{nameof(p.ContainedStripCode)} in {nameof(CloseProfile)} was Null");
                }
                else if (value is GlassStrip strip)
                {
                    CloseProfile = null;
                    CloseStrip = strip;
                }
                OnPropertyChanged(nameof(SelectedCloser));
                OnPropertyChanged(nameof(IsWithoutCloser));
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

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallSideFixer));
                    break;
                case PartSpot.BottomSide1:
                    OnPropertyChanged(nameof(BottomFixer));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
                    break;
                case PartSpot.CloseProfile:
                    OnPropertyChanged(nameof(CloseProfile));
                    break;
                case PartSpot.Handle1:
                    OnPropertyChanged(nameof(Handle));
                    break;
                case PartSpot.MiddleHinge:
                    OnPropertyChanged(nameof(Hinge));
                    break;
                case PartSpot.SupportBar:
                    OnPropertyChanged(nameof(SupportBar));
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
                case PartSpot.CloseProfile:
                    partToEdit = CloseProfile;
                    break;
                case PartSpot.Handle1:
                    partToEdit = Handle;
                    break;
                case PartSpot.MiddleHinge:
                    partToEdit = Hinge;
                    break;
                case PartSpot.WallSide1:
                    partToEdit = WallSideFixer;
                    break;
                case PartSpot.CloseStrip:
                    partToEdit = CloseStrip;
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


        public PartsHBViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinHBParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinHBParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
