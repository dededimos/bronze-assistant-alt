using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.DefaultPartsLists;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditPartSetsModalViewModel : ModalViewModel
    {
        private readonly ICabinMemoryRepository repo;
        private readonly CloseModalService closeModalService;
        private EditDefaultPartsViewModel? editDefaultPartsVm;
        private readonly IEnumerable<PartSet> _undoStore = Enumerable.Empty<PartSet>();

        /// <summary>
        /// The Sets presented to the User
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PartSetViewModel> sets = new();

        /// <summary>
        /// The Set Selected by the User
        /// </summary>
        [ObservableProperty]
        private PartSetViewModel? selectedSet;

        /// <summary>
        /// The Spot Selected by the User
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentSelectableParts))]
        private PartSpot selectedSpotToAdd = PartSpot.Undefined;
        /// <summary>
        /// The Available parts to Choose From for the Selected Spot
        /// </summary>
        public ObservableCollection<CabinPart> CurrentSelectableParts { get => new(PartSpotDefaults.FilterOnlyValidParts(SelectedSpotToAdd, repo.GetAllPartsOriginal()));}
        /// <summary>
        /// The Spots that can be selected for this Default List
        /// </summary>
        public IEnumerable<PartSpot> SelectableSpots { get => editDefaultPartsVm?.Defaults.Select(sd => sd.Spot) ?? Enumerable.Empty<PartSpot>(); }
        /// <summary>
        /// The Part that has Been Selected to be Added in the List of the Selected PartSet
        /// </summary>
        [ObservableProperty]
        private CabinPart? selectedPartToAdd;

        public EditPartSetsModalViewModel(ICabinMemoryRepository repo,CloseModalService closeModalService)
        {
            Title = "lngEditPartSetsModal".TryTranslateKey();
            this.repo = repo;
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this)
            {
                if (Sets.Any(s=> s.SetName == "????" || string.IsNullOrWhiteSpace(s.SetName)))
                {
                    MessageService.Warning("The Name of a PartSet is Empty or Unknown, Delete it or give it a Proper Name", "Cannot Continue!!!");
                    e.ShouldCancelClose = true;
                }
                else
                {
                    ApplyChangesToEditViewModel();
                }
            }
        }

        public void SetEditDefaultPartsViewModel(EditDefaultPartsViewModel defaultPartsViewModel)
        {
            //Store the ViewModel At the Begining to Apply any Changes to it before Closing
            editDefaultPartsVm = defaultPartsViewModel;
            Sets = new(editDefaultPartsVm.ConnectedParts.Select(partSet => new PartSetViewModel(repo, partSet)));
        }

        [RelayCommand]
        private void Undo()
        {
            if (editDefaultPartsVm is null) return;

            //Set all the Rest Selection to Null
            SelectedSet = null;
            SelectedPartToAdd = null;
            SelectedSpotToAdd = PartSpot.Undefined;
            //Pass the Undo List into the List shown to the User
            Sets = new(editDefaultPartsVm.ConnectedParts.Select(partSet => new PartSetViewModel(repo, partSet)));
        }

        /// <summary>
        /// Removes a Part and its Spot from the Currently Selected PartSet
        /// </summary>
        /// <param name="spotPart"></param>
        [RelayCommand]
        private void RemoveSpotPart(SpotPartViewModel spotPart)
        {
            SelectedSet?.SelectionSets.Remove(spotPart);
        }

        /// <summary>
        /// Adds a Part and its Spot to the Currently Selected PartSet
        /// </summary>
        [RelayCommand]
        private void AddSpotPart()
        {
            if (SelectedSet is null) return;
            if (SelectedSpotToAdd is PartSpot.Undefined)
            {
                MessageService.Warning("Part Spot must not be null or Undefined", "Failure".TryTranslateKey());
                return;
            }
            
            //Spot Already in
            if (SelectedSet.SelectionSets.Any(s=>s.Spot == SelectedSpotToAdd))
            {
                MessageService.Warning("The Spot you are Trying to Add is Already present in the List", "Failure".TryTranslateKey());
                return;
            }

            SpotPartViewModel sp = new(SelectedSpotToAdd, SelectedPartToAdd);
            SelectedSet.SelectionSets.Add(sp);
        }

        [RelayCommand]
        private void AddNewPartSet()
        {
            if (Sets.Any(s=> s.SetName == "????" || string.IsNullOrWhiteSpace(s.SetName)))
            {
                MessageService.Warning("There is already an Empty New Set not yet Configured", "Failure".TryTranslateKey());
                return;
            }
            Sets.Add(new PartSetViewModel());
        }

        [RelayCommand]
        private void DeleteSet()
        {
            if (SelectedSet is not null)
            {
                var storedSelected = SelectedSet;
                //Remove the Selection and remove the Selected afterwards
                SelectedSet = null;
                Sets.Remove(storedSelected);
            }
        }

        private void ApplyChangesToEditViewModel()
        {
            if (editDefaultPartsVm is null) throw new Exception($"Unexpected Error... {nameof(EditDefaultPartsViewModel)} was null...");

            IEnumerable<PartSet> partSets = Sets.Select(s => s.GetPartSet());
            editDefaultPartsVm.ConnectedParts = new(partSets);
        }

        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }

    public partial class PartSetViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string setName = string.Empty;
        [ObservableProperty]
        private ObservableCollection<SpotPartViewModel> selectionSets = new();

        [ObservableProperty]
        private bool isApplied;

        public PartSetViewModel(ICabinMemoryRepository repo, PartSet set)
        {
            SetName = set.SetName;
            IsApplied = set.IsApplied;
            foreach (var spotPart in set.SelectionSet)
            {
                var spotPartVm = new SpotPartViewModel(spotPart.Key, string.IsNullOrWhiteSpace(spotPart.Value) ? null : repo.GetPart(spotPart.Value));
                selectionSets.Add(spotPartVm);
            }
        }
        public PartSetViewModel()
        {
            
        }

        public PartSet GetPartSet()
        {
            PartSet set = new()
            {
                SetName = this.SetName,
                IsApplied = this.IsApplied
            };
            foreach (var item in SelectionSets)
            {
                if (item.Part is not null) set.SelectionSet.Add(item.Spot, item.Part.Code);
                //If the Spot Should be Empty add an Empty string as code
                else set.SelectionSet.Add(item.Spot, string.Empty);
            }
            return set;
        }

    }

    public partial class SpotPartViewModel : BaseViewModel
    {
        [ObservableProperty]
        private PartSpot spot;
        [ObservableProperty]
        private CabinPart? part;

        public SpotPartViewModel(PartSpot spot, CabinPart? part)
        {
            Spot = spot;
            Part = part;
        }
    }

}
