using AccessoriesConversions;
using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BronzeFactoryApplication.ApplicationServices.JsonXmlSerilizers;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EntityToJsonXmlModalViewModel : ModalViewModel
    {
        private readonly AccessoriesMinConverter converter;
        private readonly OperationProgressViewModel progressVm;

        [ObservableProperty]
        private string selectedLanguageToExport = LocalizedString.ENGLISHIDENTIFIER;

        public IEnumerable<string> AvailableLanguages { get; } = new List<string>() { LocalizedString.GREEKIDENTIFIER, LocalizedString.ENGLISHIDENTIFIER,LocalizedString.ITALIANIDENTIFIER };

        public static List<string> AccessoryWebItemProperties { get => new(typeof(AccessoryWebItem).GetProperties().Select(p => p.Name)); }


        //To Deprecate//To Deprecate//To Deprecate
        public List<string> AccessoryMinProperties { get; } = new(typeof(AccessoryJson).GetProperties().Select(p => p.Name));
        public List<string> TraitMinProperties { get; } = new(typeof(TraitJson).GetProperties().Select(p => p.Name));
        public List<string> TraitClassMinProperties { get; } = new(typeof(TraitClassJson).GetProperties().Select(p => p.Name));
        //To Deprecate//To Deprecate//To Deprecate

        public ObservableCollection<string> WebItemSelectedProperties { get; }
        public ObservableCollection<string> WebItemUnselectedProperties { get; set; }
        [ObservableProperty]
        private string? webItemPropertyToAdd;

        [ObservableProperty]
        private bool isSingleAccessorySelected;
        [ObservableProperty]
        private bool isMultiAccessorySelected;

        public ObservableCollection<string> AccessoriesSelectedProperties { get; }
        public ObservableCollection<string> AccessoriesUnselectedProperties { get; set; } 
        [ObservableProperty]
        private string? accessoryPropertyToAdd;

        public ObservableCollection<string> TraitSelectedProperties { get; } 
        public ObservableCollection<string> TraitUnselectedProperties { get; set; } 
        [ObservableProperty]
        private string? traitPropertyToAdd;

        public ObservableCollection<string> TraitClassSelectedProperties { get; }
        public ObservableCollection<string> TraitClassUnselectedProperties { get; set; }
        [ObservableProperty]
        private string? traitClassPropertyToAdd;

        public IEnumerable<TraitGroupEntity> PriceGroups { get; }
        [ObservableProperty]
        private TraitGroupEntity? selectedPriceGroup;

        public EntityToJsonXmlModalViewModel(AccessoriesMinConverter converter, OperationProgressViewModel progressVm , ITraitGroupEntitiesRepository repo)
        {
            Title = "Export to XML or JSON";
            this.converter = converter;
            this.progressVm = progressVm;
            AccessoriesSelectedProperties = new(AccessoryMinProperties);
            AccessoriesUnselectedProperties = new();
            TraitSelectedProperties = new(TraitMinProperties);
            TraitUnselectedProperties = new();
            TraitClassSelectedProperties = new(TraitClassMinProperties);
            TraitClassUnselectedProperties = new();

            WebItemSelectedProperties = new(AccessoryWebItemProperties);
            WebItemUnselectedProperties = new();
            
            PriceGroups = repo.Cache.Where(g => g.PermittedTraitTypes.Any(ptt => ptt == TypeOfTrait.PriceTrait));
            //Preselect the first group
            SelectedPriceGroup = PriceGroups.FirstOrDefault();
        }

        [RelayCommand]
        private async Task SaveAsXmlFile()
        {
            if (IsMultiAccessorySelected)
            {
                await SaveAsXmlFileMultiAccessory();
            }
            else if (IsSingleAccessorySelected)
            {
                await SaveAsXmlFileSingleAccessory();
            }
            else
            {
                throw new Exception("There is no Selection of Accessory Single or Multi Method");
            }
        }

        private async Task SaveAsXmlFileMultiAccessory()
        {
            if (AccessoriesSelectedProperties.Count < 1 || TraitSelectedProperties.Count < 1 || TraitClassSelectedProperties.Count < 1)
            {
                MessageService.Warning("Accessories/Traits/TraitClasses Must All Have at least one Property Marked", "Improper Properties Selection");
                return;
            }
            if (string.IsNullOrEmpty(SelectedLanguageToExport))
            {
                MessageService.Warning("Please select a Language to Export First", "Language not Selected");
            }
            IsBusy = true;
            try
            {
                SaveFileDialog saveDialog = new()
                {
                    FileName = $"AccessoriesXML-{DateTime.Now:dd-MM-yyyy}",
                    DefaultExt = ".xml",
                    Filter = @"XML files (.xml)|*.xml|All files (*.*)|*.*",
                };
                if (saveDialog.ShowDialog() == true)
                {
                    progressVm.SetNewOperation("Saving as XML...", 2);
                    var stashMin = await Task.Run(() => GetStashMin());
                    progressVm.RemainingCount--;
                    await Task.Run(() => ObjectSerilizer.SaveAsXml(stashMin, saveDialog.FileName));
                    progressVm.RemainingCount--;
                    if (MessageService.Questions.FileSavedAskOpenFile(saveDialog.FileName) == MessageBoxResult.Yes)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; progressVm.MarkAllOperationsFinished(); }
        }
        private async Task SaveAsXmlFileSingleAccessory()
        {
            if (WebItemSelectedProperties.Count < 1 )
            {
                MessageService.Warning("Accessories Must have at least one Property Marked", "Improper Properties Selection");
                return;
            }
            if (string.IsNullOrEmpty(SelectedLanguageToExport))
            {
                MessageService.Warning("Please select a Language to Export First", "Language not Selected");
            }
            IsBusy = true;
            try
            {
                SaveFileDialog saveDialog = new()
                {
                    FileName = $"SingleEntityAccessoriesXML-{DateTime.Now:dd-MM-yyyy}",
                    DefaultExt = ".xml",
                    Filter = @"XML files (.xml)|*.xml|All files (*.*)|*.*",
                };
                if (saveDialog.ShowDialog() == true)
                {
                    progressVm.SetNewOperation("Saving as XML...", 2);
                    var webItemList = await Task.Run(() => GetConvertedWebItemList());
                    progressVm.RemainingCount--;
                    await Task.Run(() => ObjectSerilizer.SaveAsXml(webItemList, saveDialog.FileName));
                    progressVm.RemainingCount--;
                    if (MessageService.Questions.FileSavedAskOpenFile(saveDialog.FileName) == MessageBoxResult.Yes)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo("notepad.exe",saveDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; progressVm.MarkAllOperationsFinished(); }
        }


        [RelayCommand]
        private async Task SaveAsJsonFile()
        {
            if (IsMultiAccessorySelected)
            {
                await SaveAsJsonFileMultiAccessory();
            }
            else if (IsSingleAccessorySelected)
            {
                await SaveAsJsonFileSingleAccessory();
            }
            else
            {
                throw new Exception("There is no Selection of Accessory Single or Multi Method");
            }
        }

        private async Task SaveAsJsonFileMultiAccessory()
        {
            if (AccessoriesSelectedProperties.Count < 1 || TraitSelectedProperties.Count < 1 || TraitClassSelectedProperties.Count < 1)
            {
                MessageService.Warning("Accessories/Traits/TraitClasses Must All Have at least one Property Marked", "Improper Properties Selection");
                return;
            }
            if (string.IsNullOrEmpty(SelectedLanguageToExport))
            {
                MessageService.Warning("Please select a Language to Export First", "Language not Selected");
            }
            IsBusy = true;
            try
            {
                SaveFileDialog saveDialog = new()
                {
                    FileName = $"AccessoriesJson-{DateTime.Now:dd-MM-yyyy}",
                    DefaultExt = ".json",
                    Filter = @"JSON files (.json)|*.json|All files (*.*)|*.*",
                };
                if (saveDialog.ShowDialog() == true)
                {
                    progressVm.SetNewOperation("Saving as JSON...", 2);
                    var stashMin = await Task.Run(() => GetStashMin());
                    progressVm.RemainingCount--;
                    await Task.Run(() => ObjectSerilizer.SaveAsJson(stashMin, saveDialog.FileName));
                    progressVm.RemainingCount--;
                    if (MessageService.Questions.FileSavedAskOpenFile(saveDialog.FileName) == MessageBoxResult.Yes)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; progressVm.MarkAllOperationsFinished(); }
        }
        private async Task SaveAsJsonFileSingleAccessory()
        {
            if (WebItemSelectedProperties.Count < 1)
            {
                MessageService.Warning("Accessories Must have at least one Property Marked", "Improper Properties Selection");
                return;
            }
            if (string.IsNullOrEmpty(SelectedLanguageToExport))
            {
                MessageService.Warning("Please select a Language to Export First", "Language not Selected");
            }
            IsBusy = true;
            try
            {
                SaveFileDialog saveDialog = new()
                {
                    FileName = $"SingleAccessoriesJson-{DateTime.Now:dd-MM-yyyy}",
                    DefaultExt = ".json",
                    Filter = @"JSON files (.json)|*.json|All files (*.*)|*.*",
                };
                if (saveDialog.ShowDialog() == true)
                {
                    progressVm.SetNewOperation("Saving as JSON...", 2);
                    var webItems = await Task.Run(() => GetConvertedWebItemList());
                    progressVm.RemainingCount--;
                    await Task.Run(() => ObjectSerilizer.SaveAsJson(webItems, saveDialog.FileName));
                    progressVm.RemainingCount--;
                    if (MessageService.Questions.FileSavedAskOpenFile(saveDialog.FileName) == MessageBoxResult.Yes)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo("notepad.exe",saveDialog.FileName) { UseShellExecute = true });
                    }
                    //Test
                    foreach (var p in AccessoryWebItem.GetSerilizableProperties())
                    {
                        Log.Information("{name}:{value}", p.Key, p.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; progressVm.MarkAllOperationsFinished(); }
        }

        [RelayCommand]
        private async Task SaveAsXls()
        {
            await Task.Delay(1);
            throw new NotImplementedException();
        }

        [RelayCommand]
        private void RemoveAccessoryProperty(string propName)
        {
            AccessoriesSelectedProperties.Remove(propName);
            AccessoriesUnselectedProperties.Add(propName);
        }
        [RelayCommand]
        private void RemoveWebItemProperty(string propName)
        {
            WebItemSelectedProperties.Remove(propName);
            WebItemUnselectedProperties.Add(propName);
            OnPropertyChanged(nameof(WebItemSelectedProperties));
            OnPropertyChanged(nameof(WebItemUnselectedProperties));
        }
        [RelayCommand]
        private void RemoveTraitProperty(string propName)
        {
            TraitSelectedProperties.Remove(propName);
            TraitUnselectedProperties.Add(propName);
        }
        [RelayCommand]
        private void RemoveTraitClassProperty(string propName)
        {
            TraitClassMinProperties.Remove(propName);
            TraitClassUnselectedProperties.Add(propName);
        }
        [RelayCommand]
        private void AddAccessoryProperty()
        {
            if (string.IsNullOrEmpty(AccessoryPropertyToAdd))
            {
                MessageService.Warning("Please first Select a Property To Add", "Property not Selected");
                return;
            }
            AccessoriesSelectedProperties.Add(AccessoryPropertyToAdd);
            AccessoriesUnselectedProperties.Remove(AccessoryPropertyToAdd);
        }
        [RelayCommand]
        private void AddWebItemProperty()
        {
            if (string.IsNullOrEmpty(WebItemPropertyToAdd))
            {
                MessageService.Warning("Please first Select a Property To Add", "Property not Selected");
                return;
            }
            WebItemSelectedProperties.Add(WebItemPropertyToAdd);
            WebItemUnselectedProperties.Remove(WebItemPropertyToAdd);
            OnPropertyChanged(nameof(WebItemSelectedProperties));
            OnPropertyChanged(nameof(WebItemUnselectedProperties));
        }
        [RelayCommand]
        private void AddTraitProperty()
        {
            if (string.IsNullOrEmpty(TraitPropertyToAdd))
            {
                MessageService.Warning("Please first Select a Property To Add", "Property not Selected");
                return;
            }
            TraitSelectedProperties.Add(TraitPropertyToAdd);
            TraitUnselectedProperties.Remove(TraitPropertyToAdd);
        }
        [RelayCommand]
        private void AddTraitClassProperty()
        {
            if (string.IsNullOrEmpty(TraitClassPropertyToAdd))
            {
                MessageService.Warning("Please first Select a Property To Add", "Property not Selected");
                return;
            }
            TraitClassMinProperties.Add(TraitClassPropertyToAdd);
            TraitClassUnselectedProperties.Remove(TraitClassPropertyToAdd);
        }

        private AccessoriesJsonStash GetStashMin()
        {
            AccessoriesMinStashConversionOptions options = new()
            {
                LanguageIdentifier = SelectedLanguageToExport ?? LocalizedString.ENGLISHIDENTIFIER,
                PriceGroupId = this.SelectedPriceGroup?.IdAsString ?? string.Empty,
                AccessorySerilizableProps = AccessoriesSelectedProperties.ToArray(),
                TraitSeriliazableProps = TraitSelectedProperties.ToArray(),
                TraitClassSerilizableProps = TraitClassSelectedProperties.ToArray(),
            };

            var accessories = converter.GetAccessoriesMinStash(options);
            return accessories;
        }

        /// <summary>
        /// Returns the Accessories Entities Repository into a AccessoryWebItem List (Each accessory with a single Finish)
        /// </summary>
        /// <returns></returns>
        private List<AccessoryWebItem> GetConvertedWebItemList()
        {
            converter.SetConversionOptions((o) =>
            {
                o.LanguageIdentifier = SelectedLanguageToExport ?? LocalizedString.ENGLISHIDENTIFIER;
                o.PriceGroupId = this.SelectedPriceGroup?.IdAsString ?? string.Empty;
                o.WebItemSerilizableProperties = WebItemSelectedProperties.ToArray();
            });
            var items = converter.ConvertRepoToWebItem();
            return items;
        }

    }
}
