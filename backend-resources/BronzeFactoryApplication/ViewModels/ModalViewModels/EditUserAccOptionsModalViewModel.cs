using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using BronzeFactoryApplication.ApplicationServices.AuthenticationServices;
using BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditUserAccOptionsModalViewModel : ModalViewModel
    {
        private readonly UserAccessoriesOptionsEntityValidator validator = new(false);
        private readonly EditItemContext<UserAccessoriesOptionsEntity> editContext = new(new UserAccessoriesOptionsEntityComparer());
        private readonly CloseModalService closeModalService;
        private readonly UserAccessoriesOptionsRepository repo;
        private readonly OpenEditCustomPriceRuleModalService openPriceRulesModalService;
        private readonly OpenEditLocalizedStringModalService openEditLocalizedStringModalService;
        [ObservableProperty]
        private UserAccessoriesOptionsViewModel optionsUnderEdit;

        /// <summary>
        /// Connected with a Collection ViewSource cannot be Newed Up
        /// </summary>
        public ObservableCollection<UserAccessoriesOptionsEntity> Options { get; } = new();

        public bool IsSelectedOptionsNew { get => SelectedOptions?.Id == ObjectId.Empty; }
        public bool CanEditOptions { get => SelectedOptions is not null; }

        private UserAccessoriesOptionsEntity? selectedOptions;
        public UserAccessoriesOptionsEntity? SelectedOptions
        {
            get => selectedOptions;
            set
            {
                if (value != selectedOptions)
                {
                    if (editContext.HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editContext.SaveCurrentState();
                        selectedOptions = value;
                        OnPropertyChanged(nameof(SelectedOptions));
                        OnPropertyChanged(nameof(IsSelectedOptionsNew));
                        OnPropertyChanged(nameof(CanEditOptions));
                        StartEditOptions(selectedOptions ?? new());
                    }
                }
            }
        }


        public EditUserAccOptionsModalViewModel(UserAccessoriesOptionsRepository repo,
                                                OpenEditCustomPriceRuleModalService openPriceRulesModalService,
                                                OpenEditLocalizedStringModalService openEditLocalizedStringModalService,
                                                CloseModalService closeModalService,
                                                Func<UserAccessoriesOptionsViewModel> userOptionsVmFactory)
        {
            Title = "lngEditAccessoriesUserOptions".TryTranslateKey();
            this.repo = repo;
            this.openPriceRulesModalService = openPriceRulesModalService;
            this.openEditLocalizedStringModalService = openEditLocalizedStringModalService;
            this.optionsUnderEdit = userOptionsVmFactory.Invoke();
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;

            var lng = ((App)Application.Current).SelectedLanguage;
            //Initilize the Options
            foreach (var option in repo.Cache.OrderBy(o => o.Name.GetLocalizedValue(lng)))
            {
                Options.Add(option);
            }
        }

        [RelayCommand]
        private void OpenCustomPriceRules()
        {
            SelectedOptions = null;
            if (SelectedOptions != null) return;
            openPriceRulesModalService.OpenModal();
        }
        
        [RelayCommand]
        private async Task SaveOption()
        {
            #region Retrieve Old and New Options and Validate
            UserAccessoriesOptionsEntity optionToSave = OptionsUnderEdit.GetModel();
            var oldOption = Options.FirstOrDefault(o => o.Id == optionToSave.Id);
            if (optionToSave.Id != ObjectId.Empty && oldOption is null)
            {
                throw new Exception($"The Option Under Edit Was not Found in the Options List for an Unexpected Reason , Expected Id Not Found : {optionToSave.Id} , OptionsName: {optionToSave.Name}");
            }
            var valResult = validator.Validate(optionToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"Option Validation Failed with ErrorCodes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            #endregion

            #region Save/Update the Option
            try
            {
                IsBusy = true;
                if (optionToSave.Id == ObjectId.Empty)
                {
                    await repo.InsertEntityAsync(optionToSave);
                }
                else
                {
                    await repo.UpdateEntityAsync(optionToSave);
                }
                repo.MarkCacheAsDirty();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
                //if operation failed return
                return;
            }
            finally { IsBusy = false; }
            #endregion

            #region Restore any Previous Selections
            // prevent prompt of unsaved Changes
            editContext.SaveCurrentState();
            // Clear Selection
            SelectedOptions = null;
            
            // Replace Old if there 
            if (oldOption != null) ReplaceInOptions(optionToSave);
            // if old is not there just add New
            else Options.Add(optionToSave);
            
            // Select the new added option
            SelectOptionByObjectId(optionToSave.Id);
            #endregion

            MessageService.Information.SaveSuccess();
        }
        [RelayCommand]
        private async Task DeleteOption()
        {
            try
            {
                IsBusy = true;
                if (SelectedOptions is null) throw new Exception("Options to Delete cannot be Null...");
                if(SelectedOptions.Name.DefaultValue == IMongoAccessoriesDTORepository.DefaultUserName)
                {
                    MessageService.Warning("Cannot Delete Defaults...", "Defaults cannot be Deleted");
                    return;
                }
                if (MessageService.Question($"This will Delete Options with Name: {SelectedOptions.Name} and Id:{SelectedOptions.Id}{Environment.NewLine}Would you like to Proceed ?", "User Options Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                var optionToDeleteId = SelectedOptions.Id;
                await repo.DeleteEntityAsync(optionToDeleteId);
                
                //Save context so it does not prompt for any changes
                editContext.SaveCurrentState();
                //Cancel Out the Selection of the TraitGroup that was deleted
                SelectedOptions = null;
                var optionToDeleteLocally = Options.FirstOrDefault(o => o.Id == optionToDeleteId);
                if (optionToDeleteLocally != null)
                {
                    if (Options.Remove(optionToDeleteLocally) is false) 
                    {
                        MessageService.Warning("The Local List did not contain the deleted Item...Unexpectedly", "Deleted Item Was not Found Locally");
                        return;
                    }
                }
                else
                {
                    MessageService.Warning("The Local List did not contain the deleted Item...Unexpectedly", "Deleted Item Was not Found Locally");
                    return;
                }
                repo.MarkCacheAsDirty();
                MessageService.Information.DeletionSuccess();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void CreateNewOption()
        {
            SelectedOptions = null;
            if (SelectedOptions != null)
            {
                return;
            }
            SelectedOptions = new();
        }
        [RelayCommand]
        private void EditLocalizedString(string localizedStringPropertyName)
        {
            LocalizedStringViewModel vm = localizedStringPropertyName switch
            {
                nameof(DescriptiveEntityViewModel.Name) => OptionsUnderEdit.BaseDescriptiveEntity.Name,
                nameof(DescriptiveEntityViewModel.Description) => OptionsUnderEdit.BaseDescriptiveEntity.Description,
                nameof(DescriptiveEntityViewModel.ExtendedDescription) => OptionsUnderEdit.BaseDescriptiveEntity.ExtendedDescription,
                _ => throw new Exception("Unrecognized Localized String Entity to Edit"),
            };
            openEditLocalizedStringModalService.OpenModal(vm, $"{"lngEdit".TryTranslateKey()} - {string.Concat("lng", localizedStringPropertyName).TryTranslateKey()}");
        }

        private void SelectOptionByObjectId(ObjectId id)
        {
            SelectedOptions = Options.FirstOrDefault(o => o.Id == id);
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this &&
                editContext.HasUnsavedChanges() &&
                MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
            }
            // Have to check that the Localized String Vms are thos of this Reference
            if (e.ClosingModal is LocalizedStringEditModalViewModel modal && (
                modal.LocalizedStringVm == OptionsUnderEdit.BaseDescriptiveEntity.Name ||
                modal.LocalizedStringVm == OptionsUnderEdit.BaseDescriptiveEntity.Description ||
                modal.LocalizedStringVm == OptionsUnderEdit.BaseDescriptiveEntity.ExtendedDescription)
                && modal.HasMadeChanges)
            {
                editContext.PushEdit(OptionsUnderEdit.GetModel());
            }
        }
        private void StartEditOptions(UserAccessoriesOptionsEntity options)
        {
            // Unsubscribe to PropertyChanges of ViewModel of Options
            OptionsUnderEdit.PropertyChanged -= OptionsUnderEdit_PropertyChanged;

            //Set The New options Under Edit
            OptionsUnderEdit.SetModel(options);
            editContext.SetUndoStore(options);

            // To Submit changes to Edit Context
            OptionsUnderEdit.PropertyChanged += OptionsUnderEdit_PropertyChanged;
        }
        private void OptionsUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            editContext.PushEdit(OptionsUnderEdit.GetModel());
        }

        private void ReplaceInOptions(UserAccessoriesOptionsEntity replacement) 
        {
            var entityToReplace = Options.FirstOrDefault(o => o.Id == replacement.Id) ?? throw new Exception($"Option with id :{replacement.Id} was not Found in the Options List , Replacement Failed");
            var indexOfItem = Options.IndexOf(entityToReplace);
            Options[indexOfItem] = replacement;
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                OptionsUnderEdit.PropertyChanged -= OptionsUnderEdit_PropertyChanged;
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                OptionsUnderEdit.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
