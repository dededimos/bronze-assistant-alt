using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditTraitGroupModalViewModel : ModalViewModel
    {
        private readonly TraitGroupEntityValidator validator = new(false);
        private readonly IAccessoryEntitiesRepository repo;
        private readonly OperationProgressViewModel operationProgress;
        private readonly OpenEditLocalizedStringModalService openEditLocalizedStringModalService;
        private readonly CloseModalService closeModalService;
        private readonly EditItemContext<TraitGroupEntity> editContext = new(new TraitGroupEntityEqualityComparer());
        
        [ObservableProperty]
        private TraitGroupEntityViewModel traitGroupUnderEdit;

        [ObservableProperty]
        private ObservableCollection<TraitGroupEntity> traitGroups;

        public bool IsSelectedGroupNew { get => SelectedGroup?.Id == ObjectId.Empty; }
        private TraitGroupEntity? selectedGroup;
        public TraitGroupEntity? SelectedGroup
        {
            get => selectedGroup;
            set 
            {
                if (value != selectedGroup)
                {
                    if (editContext.HasUnsavedChanges() &&
                        MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editContext.SaveCurrentState();
                        selectedGroup = value;
                        OnPropertyChanged(nameof(SelectedGroup));
                        OnPropertyChanged(nameof(CanEditTraitGroup));
                        OnPropertyChanged(nameof(IsSelectedGroupNew));
                        StartEditTraitGroup(selectedGroup ?? new());
                    }
                }
            }
        }

        public bool CanEditTraitGroup { get => selectedGroup is not null; }

        public EditTraitGroupModalViewModel(IAccessoryEntitiesRepository repo,
                                            OperationProgressViewModel operationProgress,
                                            OpenEditLocalizedStringModalService openEditLocalizedStringModalService,
                                            Func<TraitGroupEntityViewModel> traitGroupVmFactory,
                                            CloseModalService closeModalService)
        {
            Title = "lngEditTraitGroups".TryTranslateKey();
            this.repo = repo;
            traitGroups = new(repo.Traits.TraitGroups.Cache.OrderBy(tg => tg.SortNo));
            traitGroupUnderEdit = traitGroupVmFactory.Invoke();
            this.operationProgress = operationProgress;
            this.openEditLocalizedStringModalService = openEditLocalizedStringModalService;
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        /// <summary>
        /// Weather this Edit Modal has any Pending Changes (Can be used by parents to not Close the Modal)
        /// </summary>
        /// <returns></returns>
        public bool HasUnsavedChanges()
        {
            return editContext.HasUnsavedChanges();
        }


        //Pushes any Changes to Edit Context for the Localized Strings of the TraitGroup
        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            // Have to check that the Localized String Vms are thos of this Reference
            if (e.ClosingModal is LocalizedStringEditModalViewModel modal && modal.HasMadeChanges)
            {
                if (modal.LocalizedStringVm == TraitGroupUnderEdit.BaseDescriptiveEntity.Name ||
                    modal.LocalizedStringVm == TraitGroupUnderEdit.BaseDescriptiveEntity.Description ||
                    modal.LocalizedStringVm == TraitGroupUnderEdit.BaseDescriptiveEntity.ExtendedDescription)
                {
                    editContext.PushEdit(TraitGroupUnderEdit.GetModel());
                }
            }

            if (e.ClosingModal == this && HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
            }
        }
        private void StartEditTraitGroup(TraitGroupEntity traitGroup)
        {
            // Unsubscribe to PropertyChanges of ViewModel of Trait Group
            TraitGroupUnderEdit.PropertyChanged -= TraitGroupUnderEdit_PropertyChanged;

            //Set The New TraitGroup Under Edit
            TraitGroupUnderEdit.SetModel(traitGroup);
            editContext.SetUndoStore(traitGroup);

            // To Submit changes to Edit Context
            TraitGroupUnderEdit.PropertyChanged += TraitGroupUnderEdit_PropertyChanged;
        }
        private void TraitGroupUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            editContext.PushEdit(TraitGroupUnderEdit.GetModel());
        }
        /// <summary>
        /// Opens the Modal to Edit a Localized String Property
        /// </summary>
        /// <param name="localizedStringPropertyName"></param>
        /// <exception cref="Exception"></exception>
        [RelayCommand]
        private void EditLocalizedString(string localizedStringPropertyName)
        {
            LocalizedStringViewModel vm = localizedStringPropertyName switch
            {
                nameof(DescriptiveEntityViewModel.Name) => TraitGroupUnderEdit.BaseDescriptiveEntity.Name,
                nameof(DescriptiveEntityViewModel.Description) => TraitGroupUnderEdit.BaseDescriptiveEntity.Description,
                nameof(DescriptiveEntityViewModel.ExtendedDescription) => TraitGroupUnderEdit.BaseDescriptiveEntity.ExtendedDescription,
                _ => throw new Exception("Unrecognized Localized String Entity to Edit"),
            };
            openEditLocalizedStringModalService.OpenModal(vm, $"{"lngEdit".TryTranslateKey()} - {string.Concat("lng", localizedStringPropertyName).TryTranslateKey()}");
        }

        /// <summary>
        /// Saves/Updates the Currently Edited Trait Group
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [RelayCommand]
        private async Task SaveTraitGroup()
        {
            #region Retrieve Old and New Group and Validate
            TraitGroupEntity traitGroupToSave = TraitGroupUnderEdit.GetModel();
            var oldTraitGroup = TraitGroups.FirstOrDefault(tg => tg.Id == traitGroupToSave.Id);
            if (traitGroupToSave.Id != ObjectId.Empty && oldTraitGroup is null)
            {
                throw new Exception($"The TraitGroup Under Edit Was not Found in the TraitGroups List for an Unexpected Reason , Expected Id Not Found : {traitGroupToSave.Id} , DefaultName: {traitGroupToSave.Name.DefaultValue}");
            }
            var valResult = validator.Validate(traitGroupToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"TraitGroup Validation Failed with ErrorCodes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            } 
            #endregion

            #region Save/Update the Trait
            try
            {
                IsBusy = true;
                if (traitGroupToSave.Id == ObjectId.Empty)
                {
                    await repo.Traits.TraitGroups.InsertNewTraitGroupAsync(traitGroupToSave);
                }
                else
                {
                    await repo.Traits.TraitGroups.UpdateTraitGroupAsync(traitGroupToSave);
                }
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
            editContext.SaveCurrentState();
            RefreshTraitGroupList();
            SelectTraitGroupByObjectId(traitGroupToSave.Id);
            #endregion

            repo.MarkCacheAsDirty();
            MessageService.Information.SaveSuccess();
        }

        /// <summary>
        /// Creates a new Trait Group
        /// </summary>
        [RelayCommand]
        private void NewTraitGroup()
        {
            SelectedGroup = null;
            if (SelectedGroup != null)
            {
                return;
            }
            SelectedGroup = new();
        }

        [RelayCommand]
        private async Task DeleteTraitGroup()
        {
            try
            {
                IsBusy = true;
                if (SelectedGroup is null) throw new Exception("TraitGroup To Delete cannot be Null...");
                if (MessageService.Question($"This will Delete TraitGroup {SelectedGroup.Name.DefaultValue}{Environment.NewLine}Would you like to Proceed ?", "TraitGroup Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                await repo.Traits.TraitGroups.DeleteTraitGroupAsync(SelectedGroup.Id);
                //Save context so it does not prompt for any changes
                editContext.SaveCurrentState();
                //Cancel Out the Selection of the TraitGroup that was deleted
                SelectedGroup = null;
                // Refresh the List of Trait Groups
                RefreshTraitGroupList();
                repo.MarkCacheAsDirty();
                MessageService.Information.DeletionSuccess();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
        }

        private void RefreshTraitGroupList() 
        {
            SelectedGroup = null;
            TraitGroups = new(repo.Traits.TraitGroups.Cache.OrderBy(tg => tg.SortNo));
        }
        private void SelectTraitGroupByObjectId(ObjectId id)
        {
            SelectedGroup = TraitGroups.FirstOrDefault(tg => tg.Id == id);
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
                TraitGroupUnderEdit.PropertyChanged -= TraitGroupUnderEdit_PropertyChanged;
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
}
