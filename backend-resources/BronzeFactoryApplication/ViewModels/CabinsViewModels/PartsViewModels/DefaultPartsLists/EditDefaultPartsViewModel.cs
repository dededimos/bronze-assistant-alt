using BronzeFactoryApplication.Views.Modals;
using DataAccessLib.NoSQLModels;
using HandyControl.Tools.Extension;
using MongoDB.Driver.Core.Misc;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.DefaultPartsLists
{
    public partial class EditDefaultPartsViewModel : BaseViewModel
    {
        private readonly ICabinMemoryRepository repo;

        public IEnumerable<PartSpot> AllSpots { get => Enum.GetValues(typeof(PartSpot)).Cast<PartSpot>().Skip(1); }

        /// <summary>
        /// The Generic Parts of a DefaultPartsList
        /// </summary>
        public ObservableCollection<CabinPart> GenericParts { get; set; } = new();

        /// <summary>
        /// The Connected Parts of the Default List passed into the ViewModel
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PartSet> connectedParts = new();

        /// <summary>
        /// The Identifier of the Cabin that Consumes this Defaults Parts List
        /// </summary>
        [ObservableProperty]
        private CabinIdentifier identifier;

        /// <summary>
        /// The Defaults of this DefaultsParts List
        /// </summary>
        public ObservableCollection<PartsSpotDefaultsVideModel> Defaults { get; set; } = new();

        /// <summary>
        /// The Selected Default for Edition
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AllPartsToBeSelected))]
        [NotifyPropertyChangedFor(nameof(IsAnyDefaultSelected))]
        private PartsSpotDefaultsVideModel? selectedDefault;
        
        /// <summary>
        /// Weather any default has been already selected
        /// </summary>
        public bool IsAnyDefaultSelected { get => SelectedDefault != null; }

        /// <summary>
        /// The List of Parts of which a User can Add to the List of Selectable Parts for a Selected Default
        /// </summary>
        public IEnumerable<CabinPart> AllPartsToBeSelected 
        {
            get 
            {
                var spot = SelectedDefault?.Spot ?? PartSpot.Undefined;
                return PartSpotDefaults.FilterOnlyValidParts(spot, repo.GetAllParts()).OrderBy(p=>p.Code);
            }
        }
        /// <summary>
        /// All the Parts to select for the Generic Items
        /// </summary>
        public IEnumerable<CabinPart> AllParts => repo.GetAllParts().OrderBy(p=> p.Code);
        
        /// <summary>
        /// The Part to be Added to the List of Selectables
        /// </summary>
        [ObservableProperty]
        private CabinPart? partToAddToSelectables;
        [ObservableProperty]
        private CabinPart? partToAddToGenerics;
        [ObservableProperty]
        private int quantityOfPartToAddToGenerics;

        #region Constructor
        public EditDefaultPartsViewModel(
            DefaultPartsList parts,
            CabinIdentifier identifier,
            ICabinMemoryRepository repo)
        {
            this.Identifier = identifier;
            this.repo = repo;

            //Fix the Generics List of Parts
            foreach (var partDefaults in parts.GenericParts)
            {
                var partToAdd = repo.GetPart(partDefaults.PartCode);
                partToAdd.Quantity = partDefaults.PartQuantity;
                GenericParts.Add(partToAdd);
            }

            foreach (var partDefaults in parts.SpotDefaults.Values)
            {
                PartsSpotDefaultsVideModel vm = new(partDefaults, repo);
                Defaults.Add(vm);
            }

            foreach (var set in parts.ConnectedParts)
            {
                ConnectedParts.Add(set.GetDeepClone());
            }
        } 
        #endregion

        /// <summary>
        /// Adds a Part to the List of Selectable Parts only for the Selected Default
        /// </summary>
        [RelayCommand]
        private void AddPartToSelectables()
        {
            if (PartToAddToSelectables is not null 
                && SelectedDefault is not null)
            {
                //If the part to Add is already in the List
                if (SelectedDefault.SelectableParts.Any(p=> p.Code == PartToAddToSelectables.Code))
                {
                    MessageService.Info("lngAlreadyInTheListOfSelectableParts".TryTranslateKey(), "lngFailedToAddPart".TryTranslateKey());
                    return;
                }
                SelectedDefault.SelectableParts.Add(PartToAddToSelectables);
                OnPropertyChanged(nameof(SelectedDefault));
            }
        }
        [RelayCommand]
        private void AddGenericPart()
        {
            if (PartToAddToGenerics is not null)
            {
                var partToAdd = PartToAddToGenerics.GetDeepClone();
                partToAdd.Quantity = QuantityOfPartToAddToGenerics;
                if (QuantityOfPartToAddToGenerics > 0)
                {
                    GenericParts.Add(partToAdd);
                    //Just to Trigger changed Have been Made (Any Property Can Go Here)
                    OnPropertyChanged(nameof(IsAnyDefaultSelected));
                }
                else
                {
                    MessageService.Warning("Please Select Quantity above 0", "lngFailure".TryTranslateKey());
                }
            }
        }

        /// <summary>
        /// Removes a Part from the List of Selectable Parts only for the SelectedDefault
        /// </summary>
        /// <param name="code"></param>
        [RelayCommand]
        private void RemovePartFromSelectables(string code)
        {
            if (SelectedDefault is not null)
            {
                var partToRemove = SelectedDefault.SelectableParts.FirstOrDefault(p => p.Code == code);
                if (partToRemove is null)
                {
                    MessageService.Error("lngPartNotFoundForRemoval".TryTranslateKey(), "lngUnexpectedError".TryTranslateKey());
                    return;
                }
                bool removed = SelectedDefault.SelectableParts.Remove(partToRemove);
                if (!removed)
                {
                    MessageService.Error("lngPartNotFoundForRemoval".TryTranslateKey(), "lngUnexpectedError".TryTranslateKey());
                    Log.Error("Unexpected Error in EditDefaultPartsViewModel--RemovePartFromSelectables , Part Was Not Found");
                    return;
                }
                OnPropertyChanged(nameof(SelectedDefault));
            }
        }
        [RelayCommand]
        private void RemoveGenericPart(CabinPart part)
        {
            bool isRemoved = GenericParts.Remove(part);
            if (!isRemoved)
            {
                MessageService.Error("Part to Remove was Not Found ?!?! wtf?!", "lngFailure".TryTranslateKey());
            }
            else
            {
                //Just to Trigger changed Have been Made (Any Property Can Go Here)
                OnPropertyChanged(nameof(IsAnyDefaultSelected));
            }
        }
        /// <summary>
        /// Removes a Spot Default
        /// </summary>
        /// <param name="spotDefaultsToRemove">The Spot Default to Remove</param>
        [RelayCommand]
        private void RemoveSpotDefault(PartsSpotDefaultsVideModel spotDefaultsToRemove)
        {
            if(MessageService.Question($"Are you Sure you want to remove the Selected Spot ?{Environment.NewLine}{spotDefaultsToRemove.Spot}", "Spot Defaults Removal", "Ok", "Cancel") == MessageBoxResult.OK)
            {
                //Reset the Selection
                SelectedDefault = null;
                if (Defaults.Remove(spotDefaultsToRemove))
                {
                    MessageService.Info($"Successfully Removed Spot{Environment.NewLine}Please Save if you want to Persist this Change", "Removal Success");
                }
            }
        }

        [RelayCommand]
        private void AddPartSpotDefault(PartSpot spot)
        {
            if (Defaults.Any(d=> d.Spot == spot))
            {
                MessageService.Warning($"This Spot already Exists in the List of Spots{Environment.NewLine}{spot}", "Spot Already Exists...");
                return;
            }
            PartSpotDefaults newDefault = new(spot, true, string.Empty,1);
            PartsSpotDefaultsVideModel vm = new(newDefault, repo);
            Defaults.Add(vm);
        }

        /// <summary>
        /// Returns the Default List that has been edited
        /// </summary>
        /// <returns></returns>
        public DefaultPartsList GetDefaultList() 
        {
            DefaultPartsList partsListDefaults = new()
            {
                GenericParts = this.GenericParts.Select(p => new GenericPartDefaults(p.Code, p.Quantity)).ToList()
            };

            foreach (var spotDefault in Defaults)
            {
                partsListDefaults.SpotDefaults.Add(spotDefault.Spot,spotDefault.GetPartSpotDefaultsObject());
            }
            foreach (var set in ConnectedParts)
            {
                partsListDefaults.ConnectedParts.Add(set.GetDeepClone());
            }

            return partsListDefaults;
        }
    }

    /// <summary>
    /// A ViewModel for a Spots Defaults
    /// </summary>
    public partial class PartsSpotDefaultsVideModel : BaseViewModel
    {
        [ObservableProperty]
        private PartSpot spot;

        [ObservableProperty]
        private CabinPart? defaultPart;

        [ObservableProperty]
        private double defaultQuantity;

        [ObservableProperty]
        private bool isOptional;

        public ObservableCollection<CabinPart> SelectableParts { get; set; } = new();

        public PartsSpotDefaultsVideModel(PartSpotDefaults defaults , ICabinMemoryRepository r)
        {
            Spot = defaults.Spot;
            IsOptional = defaults.IsOptional;
            //Fix the List of Parts
            foreach (var partCode in defaults.ValidCodes)
            {
                if (string.IsNullOrEmpty(partCode)) throw new InvalidOperationException($"Unexpected Code Value in ValidCodes : a Code inside a PartSpotDefaults was null or Empty");
                
                SelectableParts.Add(r.GetPart(partCode));
            }
            //Select from the made list the Default Code (this way the ref for the Parts that are Clones will be the same , both in DefaultPart and SelectableParts)
            DefaultPart = SelectableParts.FirstOrDefault(p => defaults.DefaultCode == p?.Code);
            DefaultQuantity = defaults.DefaultQuantity;
        }

        /// <summary>
        /// Retruns the PartSpotDefaults Object represented by the ViewModel
        /// </summary>
        /// <returns></returns>
        public PartSpotDefaults GetPartSpotDefaultsObject()
        {
            PartSpotDefaults defaults = new()
            {
                Spot = Spot,
                IsOptional = IsOptional,
                DefaultCode = DefaultPart?.Code ?? string.Empty,
                DefaultQuantity = DefaultQuantity,
                ValidCodes = SelectableParts.Select(p => p?.Code ?? string.Empty).ToList()
            };
            return defaults;
        }

    }


}
