using BronzeFactoryApplication.ApplicationServices.MessangerService;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using SVGCabinDraws.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class PartsNBViewModel : PartsViewModel<CabinNBParts>
    {
        private readonly ICabinMemoryRepository repo;

        /// <summary>
        /// Weather the Structure is without a Closing Profile and Without a Closing Strip
        /// </summary>
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
                    RaisePartChanged(PartSpot.CloseProfile, partsListObject.CloseProfile,oldCloseProfile);
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

        public bool IsWithoutHandle
        {
            get => partsListObject?.Handle is null;
            set
            {
                if (IsWithoutHandle != value)
                {
                    if (value is true)
                    {
                        Handle = null;
                    }
                    else
                    {
                        Handle = DefaultHandle?.GetDeepClone()
                            ?? SelectableHandles.FirstOrDefault()?.GetDeepClone();
                    }
                    OnPropertyChanged(nameof(IsWithoutHandle));
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
                    OnPropertyChanged(nameof(IsWithoutHandle));
                    RaisePartChanged(PartSpot.Handle1, partsListObject.Handle, oldHandle);
                }
            }
        }

        [ObservableProperty]
        private bool isHandleAsPrimary = true;

        public ProfileHinge? WallHinge
        {
            get => partsListObject?.WallHinge;
            set
            {
                if (partsListObject is not null && partsListObject.WallHinge?.Code != value?.Code)
                {
                    var oldHinge = partsListObject.WallHinge;
                    partsListObject.WallHinge = value?.GetCloneWithSpotDefaultQuantity(PartSpot.WallHinge, Identifier, repo);
                    OnPropertyChanged(nameof(WallHinge));
                    RaisePartChanged(PartSpot.WallHinge, partsListObject.WallHinge, oldHinge);
                }
            }
        }
        public IEnumerable<ProfileHinge> SelectableHinges
        {
            get
            {
                var selectables = repo.GetSpotValids(Identifier, PartSpot.WallHinge).Select(c => repo.GetPart(c,Identifier) as ProfileHinge).Where(hinge => hinge is not null)!.ToList();
                if (partsNotPresentInValids.TryGetValue(PartSpot.WallHinge, out CabinPart? part)
                    && (part is not null && part is ProfileHinge hinge)
                    && (selectables.Any(s => s!.Code == hinge.Code) == false))
                {
                    selectables.Add(hinge);
                }
                return selectables!;
            }
        }

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.CloseProfile:
                    OnPropertyChanged(nameof(CloseProfile));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
                    break;
                case PartSpot.WallHinge:
                    OnPropertyChanged(nameof(WallHinge));
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
                case PartSpot.CloseProfile:
                    partToEdit = CloseProfile;
                    break;
                case PartSpot.WallHinge:
                    partToEdit = WallHinge;
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


        public PartsNBViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinNBParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinNBParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
