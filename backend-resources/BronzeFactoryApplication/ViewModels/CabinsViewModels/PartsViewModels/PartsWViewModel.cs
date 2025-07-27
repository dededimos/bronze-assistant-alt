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
    public partial class PartsWViewModel : PartsViewModel<CabinWParts>
    {
        private readonly ICabinMemoryRepository repo;

        public bool IsWithoutWallFixer
        {
            get => WallSideFixer is null;
            set
            {
                if (IsWithoutWallFixer != value)
                {
                    if (value is true)
                    {
                        WallSideFixer = null;
                    }
                    else
                    {
                        //Set to Default , if default is null set to first selectable
                        //, if no selections available set to null
                        WallSideFixer = DefaultWallFixer ?? SelectableWallFixers.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(IsWithoutWallFixer));
                }
            }
        }
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
                    OnPropertyChanged(nameof(IsWithoutWallFixer));
                    RaisePartChanged(PartSpot.WallSide1, partsListObject.WallSideFixer, oldWallSideFixer);
                }
            }
        }

        public bool IsWithoutTopFixer
        {
            get => TopFixer is null;
            set
            {
                if (IsWithoutTopFixer != value)
                {
                    if (value is true)
                    {
                        TopFixer = null;
                    }
                    else
                    {
                        TopFixer = DefaultTopFixer ?? SelectableTopFixers.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(IsWithoutTopFixer));
                }
            }
        }
        public CabinPart? TopFixer
        {
            get => partsListObject?.TopFixer;
            set
            {
                if (partsListObject is not null && partsListObject.TopFixer?.Code != value?.Code)
                {
                    var oldTopFixer = partsListObject.TopFixer;
                    partsListObject.TopFixer = value?.GetCloneWithSpotDefaultQuantity(PartSpot.TopSide, Identifier, repo);
                    OnPropertyChanged(nameof(TopFixer));
                    OnPropertyChanged(nameof(IsWithoutTopFixer));
                    RaisePartChanged(PartSpot.TopSide, partsListObject.TopFixer, oldTopFixer);
                }
            }
        }
        public CabinPart? DefaultTopFixer => repo.GetDefaultPart(Identifier, PartSpot.TopSide);
        public IEnumerable<CabinPart> SelectableTopFixers
        {
            get
            {
                var selectables = repo.GetSpotValids(Identifier, PartSpot.TopSide).Select(c => repo.GetPart(c,Identifier)).ToList();
                if (partsNotPresentInValids.TryGetValue(PartSpot.TopSide, out CabinPart? part)
                    && part is not null
                    && (selectables.Any(s => s.Code == part.Code) == false))
                {
                    selectables.Add(part);
                }
                return selectables;
            }
        }

        public bool IsWithoutSideFixer
        {
            get => SideFixer is null;
            set
            {
                if (IsWithoutSideFixer != value)
                {
                    if (value is true)
                    {
                        SideFixer = null;
                    }
                    else
                    {
                        SideFixer = DefaultSideFixer ?? SelectableSideFixers.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(IsWithoutSideFixer));
                }
            }
        }
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
                    OnPropertyChanged(nameof(IsWithoutSideFixer));
                    RaisePartChanged(PartSpot.NonWallSide, partsListObject.SideFixer,oldSideFixer);
                }
            }
        }
        public CabinPart? DefaultSideFixer => repo.GetDefaultPart(Identifier, PartSpot.NonWallSide);
        public IEnumerable<CabinPart> SelectableSideFixers
        {
            get
            {
                var selectables = repo.GetSpotValids(Identifier, PartSpot.NonWallSide).Select(c => repo.GetPart(c,Identifier)).ToList();
                if (partsNotPresentInValids.TryGetValue(PartSpot.NonWallSide, out CabinPart? part)
                    && part is not null
                    && (selectables.Any(s => s.Code == part.Code) == false))
                {
                    selectables.Add(part);
                }
                return selectables;
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
                    RaisePartChanged(PartSpot.BottomSide1, partsListObject.BottomFixer,oldBottomFixer);
                }
            }
        }

        /// <summary>
        /// Close Strip when in Combo Draw
        /// ,changes only from selections in the PrimaryModel
        /// </summary>
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
                    RaisePartChanged(PartSpot.CloseStrip, partsListObject.CloseStrip,oldStrip);
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

        public override void InformSpotPartChanged(PartSpot spot)
        {
            switch (spot)
            {
                case PartSpot.WallSide1:
                    OnPropertyChanged(nameof(WallSideFixer));
                    break;
                case PartSpot.TopSide:
                    OnPropertyChanged(nameof(TopFixer));
                    break;
                case PartSpot.NonWallSide:
                    OnPropertyChanged(nameof(SideFixer));
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
                case PartSpot.TopSide:
                    partToEdit = TopFixer;
                    break;
                case PartSpot.NonWallSide:
                    partToEdit = SideFixer;
                    break;
                case PartSpot.BottomSide1:
                    partToEdit = BottomFixer;
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

        public PartsWViewModel(ICabinMemoryRepository repo) : base(repo)
        {
            this.repo = repo;
        }

        public override void SetNewPartsList(CabinPartsList partsList, CabinIdentifier identifier)
        {
            base.SetNewPartsList(partsList,identifier);
            partsListObject = partsList as CabinWParts ?? throw new InvalidOperationException($"Provided Parts where of type {partsList.GetType().Name} -- and not of the expected type : {nameof(CabinWParts)}");
            //Inform all Changed in the Cabin ViewModel
        }
    }
}
