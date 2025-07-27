using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels
{
    public partial class Parts9BViewModel : PartsViewModel<Cabin9BParts>
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

        public Hinge9B? Hinge
        {
            get => partsListObject?.Hinge;
            set
            {
                if (partsListObject is not null && partsListObject.Hinge?.Code != value?.Code)
                {
                    var oldHinge = partsListObject.Hinge;
                    partsListObject.Hinge = value?.GetCloneWithSpotDefaultQuantity(PartSpot.PivotHinge, Identifier, repo);
                    OnPropertyChanged(nameof(Hinge));
                    RaisePartChanged(PartSpot.PivotHinge, partsListObject.Hinge, oldHinge);
                }
            }
        }
        public IEnumerable<Hinge9B> SelectableHinges
        {
            get
            {
                var selectables = repo.GetSpotValids(Identifier, PartSpot.PivotHinge).Select(c => repo.GetPart(c,Identifier) as Hinge9B).Where(hinge => hinge is not null)!.ToList();
                if (partsNotPresentInValids.TryGetValue(PartSpot.PivotHinge, out CabinPart? part)
                    && (part is not null && part is Hinge9B hinge)
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
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallProfile1));
                    break;
                case PartSpot.WallSide2:
                    OnPropertyChanged(nameof(WallProfile2));
                    break;
                case PartSpot.StepBottomSide:
                    OnPropertyChanged(nameof(StepBottomProfile));
                    break;
                case PartSpot.HorizontalGuideBottom:
                case PartSpot.HorizontalGuideTop:
                    OnPropertyChanged(nameof(HorizontalProfile));
                    break;
                case PartSpot.CloseStrip:
                    OnPropertyChanged(nameof(CloseStrip));
                    break;
                case PartSpot.Handle1:
                case PartSpot.Handle2:
                    OnPropertyChanged(nameof(Handle));
                    break;
                case PartSpot.PivotHinge:
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
                case PartSpot.PivotHinge:
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

        public Parts9BViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList, identifier);
            partsListObject = partsList as Cabin9BParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(Cabin9BParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
