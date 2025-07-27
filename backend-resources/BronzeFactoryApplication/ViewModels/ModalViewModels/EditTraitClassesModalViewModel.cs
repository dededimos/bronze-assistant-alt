using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using AzureBlobStorageLibrary;
using BathAccessoriesModelsLibrary;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditTraitClassesModalViewModel : ModalViewModel
    {
        private readonly TraitEntityValidator traitValidator = new(false);
        private readonly TraitClassEntityValidator traitClassValidator = new(false);
        private readonly IAccessoryEntitiesRepository repo;
        private readonly IBlobStorageRepository blobsRepo;
        private readonly OperationProgressViewModel operationProgress;
        private readonly OpenEditLocalizedStringModalService openEditLocalizedStringModalService;
        private readonly CloseModalService closeModalService;
        private readonly OpenImageViewerModalService openImageViewerService;
        private readonly OpenEditTraitGroupModalService openEditTraitGroupsModalService;
        private readonly EditItemContext<TraitEntity> editTraitContext = new(new TraitEntityEqualityComparer());
        private readonly EditItemContext<TraitClassEntity> editTraitClassContext = new(new TraitClassEntityEqualityComparer());

        /// <summary>
        /// The List of the Trait Classes Taken from the Repo
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<TraitClassEntity> traitClasses;
        /// <summary>
        /// The List of the Traits Taken from the Repo
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SecondaryTraits))]
        private ObservableCollection<TraitEntity> traits;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentAssignableGroups))]
        private List<TraitGroupEntity> traitGroups;

        /// <summary>
        /// The Groups that can be Currently assigned to the Trait being Edited (Based on its Trait Type)
        /// </summary>
        public List<TraitGroupEntity> CurrentAssignableGroups { get => TraitGroups.Where(tg => tg.PermittedTraitTypes.Contains(TraitUnderEdit.TraitType)).ToList(); }

        public List<TraitEntity> SecondaryTraits { get => Traits.Where(t => t.TraitType == TypeOfTrait.SecondaryTypeTrait).OrderBy(t => t.SortNo).ToList(); }


        /// <summary>
        /// The Trait Class Selected by the User
        /// </summary>
        private TraitClassEntity? selectedTraitClass;
        public TraitClassEntity? SelectedTraitClass
        {
            get => selectedTraitClass;
            set
            {
                if (value != selectedTraitClass)
                {
                    if ((editTraitContext.HasUnsavedChanges() || editTraitClassContext.HasUnsavedChanges()) &&
                        MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editTraitClassContext.SaveCurrentState();
                        editTraitContext.SaveCurrentState();
                    }
                    selectedTraitClass = value;
                    OnPropertyChanged(nameof(SelectedTraitClass));
                    OnPropertyChanged(nameof(CanEditTraitClass));
                    OnPropertyChanged(nameof(IsSelectedTraitClassNew));
                    var traitsOfSelectedClass = Traits.Where(t => t.TraitType == selectedTraitClass?.TraitType).OrderBy(t => t.SortNo);

                    StartEditTraitClass(selectedTraitClass ?? new());

                    // Nullify trait Selection
                    SelectedTrait = null;
                    // Add the Traits Matching the Selected Trait Class
                    SelectedTraitClassTraits.Clear();
                    foreach (var trait in traitsOfSelectedClass)
                    {
                        SelectedTraitClassTraits.Add(trait);
                    }
                }
            }
        }

        /// <summary>
        /// Weather the Selected Trait Class is new or an Old one being Edited
        /// </summary>
        public bool IsSelectedTraitClassNew { get => SelectedTraitClass?.Id == ObjectId.Empty; }

        /// <summary>
        /// The Trait Selected By the User (in the Corresponding Trait Class)
        /// </summary>
        private TraitEntity? selectedTrait;
        public TraitEntity? SelectedTrait
        {
            get => selectedTrait;
            set
            {
                if (value != selectedTrait)
                {
                    if (editTraitContext.HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editTraitClassContext.SaveCurrentState();
                        editTraitContext.SaveCurrentState();
                    }
                    selectedTrait = value;
                    StartEditTrait(selectedTrait ?? new());
                    OnPropertyChanged(nameof(SelectedTrait));
                    OnPropertyChanged(nameof(CanEditTrait));
                    OnPropertyChanged(nameof(IsSelectedTraitNew));
                }
            }
        }

        /// <summary>
        /// Weather the Selected Trait is new or an Old one being Edited
        /// </summary>
        public bool IsSelectedTraitNew { get => SelectedTrait?.Id == ObjectId.Empty; }

        /// <summary>
        /// The ViewModel of the Selected Trait Class that is Currently being Edited
        /// </summary>
        [ObservableProperty]
        private TraitClassEntityViewModel traitClassUnderEdit;
        /// <summary>
        /// Weather the current ViewModel of the Trait Class is Editable
        /// </summary>
        public bool CanEditTraitClass { get => selectedTraitClass is not null; }

        /// <summary>
        /// The ViewModel of the Selected Trait that is Currently being Edited
        /// </summary>
        [ObservableProperty]
        private TraitEntityViewModel traitUnderEdit;
        /// <summary>
        /// Weather the current ViewModel of the Trait is Editable
        /// </summary>
        public bool CanEditTrait { get => selectedTrait is not null; }

        /// <summary>
        /// The Available Traits on the Selected Trait Class
        /// </summary>
        public ObservableCollection<TraitEntity> SelectedTraitClassTraits { get; set; } = new();

        public EditTraitClassesModalViewModel(IAccessoryEntitiesRepository repo,
            IBlobStorageRepository blobsRepo,
            OperationProgressViewModel operationProgressVm,
            Func<TraitClassEntityViewModel> traitClassVmFactory,
            Func<TraitEntityViewModel> traitVmFactory,
            OpenEditLocalizedStringModalService openEditLocalizedStringModalService,
            CloseModalService closeModalService,
            OpenImageViewerModalService openImageViewerService,
            OpenEditTraitGroupModalService openEditTraitGroupsModalService)
        {
            this.repo = repo;
            this.blobsRepo = blobsRepo;
            this.operationProgress = operationProgressVm;
            this.openEditLocalizedStringModalService = openEditLocalizedStringModalService;
            this.closeModalService = closeModalService;
            this.openImageViewerService = openImageViewerService;
            this.openEditTraitGroupsModalService = openEditTraitGroupsModalService;
            traitClasses = new(repo.Traits.TraitClasses.Cache.OrderBy(tc => tc.SortNo));
            traits = new(repo.Traits.Cache.OrderBy(t => t.SortNo));
            traitGroups = new(repo.Traits.TraitGroups.Cache.OrderBy(tg => tg.SortNo));
            traitClassUnderEdit = traitClassVmFactory.Invoke();
            traitUnderEdit = traitVmFactory.Invoke();
            Title = "lngEditTraitClasses".TryTranslateKey();
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;
            this.closeModalService.ModalClosed += CloseModalService_ModalClosed;
        }

        private void CloseModalService_ModalClosed(object? sender, ModalClosedEventArgs e)
        {
            if (e.TypeOfClosedModal == typeof(EditTraitGroupModalViewModel))
            {
                // Refresh the Groups List whenever the TraitGroups Modal Closes (Do not check for equalities e.t.c.)
                TraitGroups = new(repo.Traits.TraitGroups.Cache.OrderBy(tg => tg.SortNo));
            }
        }

        /// <summary>
        /// Informs a Localized String Has Changes so that edits can be Pushed to the Edit Context
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            // Have to check that the Localized String Vms are thos of this Reference
            if (e.ClosingModal is LocalizedStringEditModalViewModel modal && modal.HasMadeChanges)
            {
                if (modal.LocalizedStringVm == TraitClassUnderEdit.BaseDescriptiveEntity.Name ||
                    modal.LocalizedStringVm == TraitClassUnderEdit.BaseDescriptiveEntity.Description ||
                    modal.LocalizedStringVm == TraitClassUnderEdit.BaseDescriptiveEntity.ExtendedDescription)
                {
                    editTraitClassContext.PushEdit(TraitClassUnderEdit.GetModel());
                }
                else if (modal.LocalizedStringVm == TraitUnderEdit.Trait ||
                    modal.LocalizedStringVm == TraitUnderEdit.TraitTooltip)
                {
                    editTraitContext.PushEdit(TraitUnderEdit.GetModel());
                }
            }
            else if (e.ClosingModal == this && HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
                return;
            }
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
                nameof(DescriptiveEntityViewModel.Name) => TraitClassUnderEdit.BaseDescriptiveEntity.Name,
                nameof(DescriptiveEntityViewModel.Description) => TraitClassUnderEdit.BaseDescriptiveEntity.Description,
                nameof(DescriptiveEntityViewModel.ExtendedDescription) => TraitClassUnderEdit.BaseDescriptiveEntity.ExtendedDescription,
                nameof(TraitEntityViewModel.Trait) => TraitUnderEdit.Trait,
                nameof(TraitEntityViewModel.TraitTooltip) => TraitUnderEdit.TraitTooltip,
                _ => throw new Exception("Unrecognized Localized String Entity to Edit"),
            };
            openEditLocalizedStringModalService.OpenModal(vm, $"{"lngEdit".TryTranslateKey()} - {string.Concat("lng", localizedStringPropertyName).TryTranslateKey()}");
        }

        [RelayCommand]
        private void OpenEditTraitGroups()
        {
            openEditTraitGroupsModalService.OpenModal();
        }

        /// <summary>
        /// Saves the Currently Selected Trait
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveTrait()
        {
            #region Retrieve the Old and the New Traits
            TraitEntity traitToSave = TraitUnderEdit.GetModel();
            var oldTrait = Traits.FirstOrDefault(t => t.Id == traitToSave.Id);
            if (traitToSave.Id != ObjectId.Empty && oldTrait is null)
            {
                throw new Exception($"The Trait Under Edit Was not Found in the Traits List for an Unexpected Reason , Expected Id Not Found : {traitToSave.Id} , DefaultValue: {traitToSave.Trait.DefaultValue}");
            }
            var valResult = traitValidator.Validate(traitToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"Trait Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            //Find which groups are assigned to the Trait being Saved
            var assignedGroups = TraitGroups.Where(tg => traitToSave.AssignedGroups.Any(ag => ag == tg.IdAsString));
            //Check that those groups can be indeed assigned to this Trait
            foreach (var group in assignedGroups)
            {
                if (group.PermittedTraitTypes.Contains(traitToSave.TraitType) is false)
                {
                    MessageService.Warning($"Failed To Save Trait with Error:{Environment.NewLine}{Environment.NewLine}TraitGroup: {group.Name.DefaultValue} with Id:{group.IdAsString}{Environment.NewLine}{Environment.NewLine}Cannot be Assigned to Traits with Type:{traitToSave.TraitType}", "Invalid Assigned Trait Groups");
                    return;
                }
            }
            #endregion

            #region Upload New Photos if Any
            var photoUploadError = await UploadTraitPhotoUrlIfNew(oldTrait, traitToSave);
            #endregion

            #region Save/Update the Trait
            try
            {
                IsBusy = true;
                if (traitToSave.Id == ObjectId.Empty)
                {
                    await repo.Traits.InsertNewTraitAsync(traitToSave);
                }
                else
                {
                    await repo.Traits.UpdateTraitAsync(traitToSave);
                }
            }
            catch (Exception ex)
            {
                // Delete the Photo that was Uploaded if the Update or Inser Failed and the Photo did Upload
                if (string.IsNullOrEmpty(traitToSave.PhotoURL) == false &&
                    traitToSave.PhotoURL != oldTrait?.PhotoURL)
                {
                    await DeleteBlobAndVariations(traitToSave.PhotoURL);
                }
                MessageService.LogAndDisplayException(ex);
                // if the Update operation Failed then Return
                return;
            }
            finally { IsBusy = false; }

            // if the Save Operation Succedeed then Delete the old photo if it was Replaced (old Photo is not Empty)
            if (string.IsNullOrEmpty(oldTrait?.PhotoURL) == false &&
                traitToSave.PhotoURL != oldTrait?.PhotoURL)
            {
                await DeleteBlobAndVariations(oldTrait!.PhotoURL);
            }
            #endregion

            #region Restore any Previous Selections
            editTraitContext.SaveCurrentState();
            RefreshTraitLists();
            SelectTraitByObjectId(traitToSave.Id);
            #endregion

            #region Display Errors If Any
            if (string.IsNullOrWhiteSpace(photoUploadError))
            {
                MessageService.Information.SaveSuccess();
            }
            else
            {
                MessageService.Warning($"Save Success with Errors:{Environment.NewLine}{Environment.NewLine}{photoUploadError}", "Save Success with Errors");
            }
            #endregion

            repo.MarkCacheAsDirty();
        }

        /// <summary>
        /// Saves the Currently Selected Trait Class
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveTraitClass()
        {
            #region Retrieve the Old and the New TraitClasses
            TraitClassEntity traitClassToSave = TraitClassUnderEdit.GetModel();
            var oldTraitClass = TraitClasses.FirstOrDefault(t => t.Id == traitClassToSave.Id);
            if (traitClassToSave.Id != ObjectId.Empty && oldTraitClass is null)
            {
                throw new Exception($"The TraitClass Under Edit Was not Found in the TraitsClasses List for an Unexpected Reason , Expected Id Not Found : {traitClassToSave.Id} , DefaultValue: {traitClassToSave.Name.DefaultValue}");
            }
            var valResult = traitClassValidator.Validate(traitClassToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"TraitClass Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            #endregion
            #region Upload New Photos if Any
            var photoUploadError = await UploadTraitClassPhotoUrlIfNew(oldTraitClass, traitClassToSave);
            #endregion
            #region Save Update the Trait Class

            // Save the Currently Selectd Trait to Resotre it After the Update
            var idOfCurrentlySelectedTrait = SelectedTrait?.Id ?? ObjectId.Empty;
            try
            {
                IsBusy = true;
                if (traitClassToSave.Id == ObjectId.Empty)
                {
                    await repo.Traits.TraitClasses.InsertNewTraitClassAsync(traitClassToSave);
                }
                else
                {
                    await repo.Traits.TraitClasses.UpdateTraitClassAsync(traitClassToSave);
                }
            }
            catch (Exception ex)
            {
                // Delete the Photo that was Uploaded if the Update or Insert Failed and the Photo did Upload
                if (string.IsNullOrEmpty(traitClassToSave.PhotoURL) == false &&
                    traitClassToSave.PhotoURL != oldTraitClass?.PhotoURL)
                {
                    await DeleteBlobAndVariations(traitClassToSave.PhotoURL);
                }
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
            #endregion
            #region Restore any Previous Selections
            editTraitClassContext.SaveCurrentState();
            RefreshTraitLists();
            SelectedTraitClass = TraitClasses.FirstOrDefault(tc => tc.Id == traitClassToSave.Id);
            #endregion

            repo.MarkCacheAsDirty();

            #region Display Errors If Any
            if (string.IsNullOrWhiteSpace(photoUploadError))
            {
                MessageService.Information.SaveSuccess();
            }
            else
            {
                MessageService.Warning($"Save Success with Errors:{Environment.NewLine}{Environment.NewLine}{photoUploadError}", "Save Success with Errors");
            }
            #endregion
        }
        /// <summary>
        /// Deletes the Currently Selected Trait Class
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [RelayCommand]
        private async Task DeleteTraitClass()
        {
            try
            {
                IsBusy = true;
                if (SelectedTraitClass is null) throw new Exception("TraitClass To Delete cannot be Null...");
                if (MessageService.Question($"This will Delete TraitClass {SelectedTraitClass.Name.DefaultValue}{Environment.NewLine}Would you like to Proceed ?", "TraitClass Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                await repo.Traits.TraitClasses.DeleteTraitClassAsync(SelectedTraitClass.Id);

                // Delete the Photo of the Trait if there
                if (string.IsNullOrEmpty(SelectedTraitClass.PhotoURL) is false)
                {
                    await DeleteBlobAndVariations(SelectedTraitClass.PhotoURL);
                }

                MessageService.Information.DeletionSuccess();
                editTraitContext.SaveCurrentState();
                editTraitClassContext.SaveCurrentState();
                //Cancel Out the Selection of Trait
                SelectedTrait = null;
                SelectedTraitClass = null;
                // Refresh the List to reflect Changes
                RefreshTraitLists();
                repo.MarkCacheAsDirty();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
        }
        /// <summary>
        /// Selects a New Trait to Add in the Traits List
        /// </summary>
        [RelayCommand]
        private void NewTrait()
        {
            SelectedTraitClass = null;
            if (SelectedTraitClass != null)
            {
                return;
            }
            SelectedTrait = new();
        }
        /// <summary>
        /// Deletes the Currently Selected Trait
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [RelayCommand]
        private async Task DeleteTrait()
        {
            try
            {
                IsBusy = true;
                if (SelectedTrait is null) throw new Exception("Trait To Delete cannot be Null...");
                if (MessageService.Question($"This will Delete Trait {SelectedTrait.Trait.DefaultValue}{Environment.NewLine}Would you like to Proceed ?", "Trait Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                await repo.Traits.DeleteTraitAsync(SelectedTrait.Id);

                // Delete the Photo of the Trait if there
                if (string.IsNullOrEmpty(SelectedTrait.PhotoURL) is false)
                {
                    await DeleteBlobAndVariations(SelectedTrait.PhotoURL);
                }

                MessageService.Information.DeletionSuccess();
                editTraitContext.SaveCurrentState();
                editTraitClassContext.SaveCurrentState();
                //Cancel Out the Selection of Trait
                SelectedTrait = null;
                //Save the Selected Trait Class
                var idOfSelectedTraitClass = SelectedTraitClass?.Id ?? ObjectId.Empty;
                // Refresh the List to reflect Changes
                RefreshTraitLists();
                // Reselect the Trait Class from before
                SelectedTraitClass = TraitClasses.FirstOrDefault(tc => tc.Id == idOfSelectedTraitClass);
                repo.MarkCacheAsDirty();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
        }
        /// <summary>
        /// Selects a New Trait Class to Add in the Trait Classes List
        /// </summary>
        [RelayCommand]
        private void NewTraitClass()
        {
            SelectedTraitClass = new();
        }

        public bool HasUnsavedChanges()
        {
            return editTraitContext.HasUnsavedChanges() || editTraitClassContext.HasUnsavedChanges();
        }

        /// <summary>
        /// Initilizes properly the Edit ViewModel for a Selected Trait Class
        /// </summary>
        /// <param name="traitClass"></param>
        private void StartEditTraitClass(TraitClassEntity traitClass)
        {
            // Unsubscribe from Prop Changes to It
            TraitClassUnderEdit.PropertyChanged -= TraitClassUnderEdit_PropertyChanged;

            // Set the New One
            TraitClassUnderEdit.SetModel(traitClass);
            editTraitClassContext.SetUndoStore(traitClass);

            // Resubscribe to Property Changes
            TraitClassUnderEdit.PropertyChanged += TraitClassUnderEdit_PropertyChanged;
        }

        /// <summary>
        /// Whenever there is a change in the Trait Class being Edited informs this viewModel of it so to push the Edit on the Edit Context
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TraitClassUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            editTraitClassContext.PushEdit(TraitClassUnderEdit.GetModel());
        }
        /// <summary>
        /// Initilizes properly the Edit ViewModel for a Selected Trait
        /// </summary>
        /// <param name="trait"></param>
        private void StartEditTrait(TraitEntity trait)
        {
            // Unsubscribe from Prop Changes to It
            TraitUnderEdit.PropertyChanged -= TraitUnderEdit_PropertyChanged;

            // Set the New One
            TraitUnderEdit.SetModel(trait);
            editTraitContext.SetUndoStore(trait);
            //inform trait changed to change also the assignable groups because at this state The We have unsubscribe from the vent above
            OnPropertyChanged(nameof(CurrentAssignableGroups));

            // Resubscribe to Property Changes
            TraitUnderEdit.PropertyChanged += TraitUnderEdit_PropertyChanged;
        }
        /// <summary>
        /// Whenever there is a change in the Trait being Edited informs this viewModel of it so to push the Edit on the Edit Context
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TraitUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Push any Edits made to the Trait Under Edit into the EditContext;
            editTraitContext.PushEdit(TraitUnderEdit.GetModel());

            // Inform that the Trait Type has Changed so that the Assignable Groups also change based on the New Type
            if (e.PropertyName == nameof(TraitEntityViewModel.TraitType))
            {
                OnPropertyChanged(nameof(CurrentAssignableGroups));
            }
        }

        /// <summary>
        /// Replaces a Trait in the Local Observable Collection, by comparing Ids
        /// </summary>
        /// <param name="replacement"></param>
        /// <exception cref="Exception"></exception>
        private void ReplaceUpdatedTrait(TraitEntity replacement)
        {
            var traitToReplace = Traits.FirstOrDefault(t => t.Id == replacement.Id) ?? throw new Exception($"Trait to Replace with Id {replacement.Id} was not Found in the List of Traits, Local Replacement Failed...");
            var indexOfTraitToReplace = Traits.IndexOf(traitToReplace);
            Traits[indexOfTraitToReplace] = replacement;
        }
        /// <summary>
        /// Replaces a Trait Class in the Local Observable Collection, by comparing Ids
        /// </summary>
        /// <param name="replacement"></param>
        /// <exception cref="Exception"></exception>
        private void ReplaceUpdatedTraitClass(TraitClassEntity replacement)
        {
            var traitClassToReplace = TraitClasses.FirstOrDefault(tc => tc.Id == replacement.Id) ?? throw new Exception($"Trait Class to Replace with Id {replacement.Id} was not Found in the List of Trait Classes, Local Replacement Failed...");
            var indexOfTraitClassToReplace = TraitClasses.IndexOf(traitClassToReplace);
            TraitClasses[indexOfTraitClassToReplace] = replacement;
        }

        /// <summary>
        /// Refreshes the List of Traits and Trait Classes from the Repo.Cache
        /// </summary>
        private void RefreshTraitLists()
        {
            SelectedTraitClass = null;
            SelectedTrait = null;
            // Reget Traits and Trait Classes and Select the Trait that has been Saved
            TraitClasses = new(repo.Traits.TraitClasses.Cache.OrderBy(tc => tc.SortNo));
            Traits = new(repo.Traits.Cache.OrderBy(t => t.SortNo));
        }
        /// <summary>
        /// Selects a Trait and Trait Class from the Provided Id
        /// </summary>
        /// <param name="id"></param>
        private void SelectTraitByObjectId(ObjectId id)
        {
            SelectedTraitClass = TraitClasses.FirstOrDefault(tc => tc.Traits.Any(t => t == id));
            SelectedTrait = SelectedTraitClassTraits.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Uploads the Main Url Photo and returns if there where Any Errors
        /// If it fails it wipes the New Photo and uses the Old or nothing if there is no Old Photo
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadTraitClassPhotoUrlIfNew(TraitClassEntity? oldEntity, TraitClassEntity newEntity)
        {
            // if the Main Url is Empty in the new Entity just Return
            if (string.IsNullOrWhiteSpace(newEntity.PhotoURL)) return string.Empty;

            // The NewEntity will have Local Urls for any File that has Changed ,
            // Otherwise the Urls will be the Same to the New Entity
            try
            {
                if (oldEntity is null ||
                    !string.IsNullOrWhiteSpace(newEntity.PhotoURL) &&
                    oldEntity.PhotoURL != newEntity.PhotoURL)
                {
                    //Upload the Photo of the Main Url
                    var newUrl = await UploadTraitClassImageToBlob(newEntity.PhotoURL, newEntity.Name.DefaultValue);

                    //Retrieve the url string and Pass it to the New Entity
                    newEntity.PhotoURL = newUrl;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Keep the Error to Display it
                string failureMsg = $"Failed to upload {nameof(TraitClassEntity.PhotoURL)} with Local Path: {newEntity.PhotoURL}{Environment.NewLine}and Failure Exception:{Environment.NewLine}{ex.Message}";
                // If operation Fails replace the new Entity MainPhotoUrl with the Old Files url
                newEntity.PhotoURL = oldEntity?.PhotoURL ?? string.Empty;
                return failureMsg;
            }
        }
        /// <summary>
        /// Uploads the PhotoUrl Photo and returns if there where Any Errors
        /// If it fails it wipes the New Photo and uses the Old or nothing if there is no Old Photo
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadTraitPhotoUrlIfNew(TraitEntity? oldEntity, TraitEntity newEntity)
        {
            // if the Main Url is Empty in the new Entity just Return
            if (string.IsNullOrWhiteSpace(newEntity.PhotoURL)) return string.Empty;

            // The NewEntity will have Local Urls for any File that has Changed ,
            // Otherwise the Urls will be the Same to the New Entity
            try
            {
                if (oldEntity is null ||
                    !string.IsNullOrWhiteSpace(newEntity.PhotoURL) &&
                    oldEntity.PhotoURL != newEntity.PhotoURL)
                {
                    //Upload the Photo of the Main Url
                    var newUrl = await UploadTraitImageToBlob(newEntity.PhotoURL, newEntity.Trait.DefaultValue, newEntity.TraitType);

                    //Retrieve the url string and Pass it to the New Entity
                    newEntity.PhotoURL = newUrl;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Keep the Error to Display it
                string failureMsg = $"Failed to upload {nameof(TraitEntity.PhotoURL)} with Local Path: {newEntity.PhotoURL}{Environment.NewLine}and Failure Exception:{Environment.NewLine}{ex.Message}";
                // If operation Fails replace the new Entity MainPhotoUrl with the Old Files url
                newEntity.PhotoURL = oldEntity?.PhotoURL ?? string.Empty;
                return failureMsg;
            }
        }
        /// <summary>
        /// Uploads the PhotoUrl Photo and returns if there where Any Errors
        /// If it fails it wipes the New Photo and uses the Old or nothing if there is no Old Photo
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadTraitClassImageToBlob(string imageFilePath, string fileName)
        {
            operationProgress.SetUploadingToBlobOperation();
            string imageUrl = await blobsRepo.UploadAccessoryImagesToBlob(imageFilePath, fileName, AccessoriesBlobSubFolder.TraitClassesFolder, false);
            operationProgress.RemainingCount--;
            return imageUrl;
        }
        private async Task<string> UploadTraitImageToBlob(string imageFilePath, string fileName, TypeOfTrait traitType)
        {
            operationProgress.SetUploadingToBlobOperation();
            AccessoriesBlobSubFolder subFolder = traitType switch
            {
                TypeOfTrait.FinishTrait => AccessoriesBlobSubFolder.FinishesFolder,
                TypeOfTrait.MaterialTrait => AccessoriesBlobSubFolder.MaterialsFolder,
                TypeOfTrait.CategoryTrait => AccessoriesBlobSubFolder.CategoriesFolder,
                TypeOfTrait.SizeTrait => AccessoriesBlobSubFolder.SizesFolder,
                TypeOfTrait.DimensionTrait => AccessoriesBlobSubFolder.DimensionsFolder,
                TypeOfTrait.SeriesTrait => AccessoriesBlobSubFolder.SeriesFolder,
                TypeOfTrait.ShapeTrait => AccessoriesBlobSubFolder.ShapesTypesFolder,
                TypeOfTrait.PrimaryTypeTrait => AccessoriesBlobSubFolder.PrimaryTypesFolder,
                TypeOfTrait.SecondaryTypeTrait => AccessoriesBlobSubFolder.SecondaryTypesFolder,
                TypeOfTrait.MountingTypeTrait => AccessoriesBlobSubFolder.MountingTypesFolder,
                TypeOfTrait.PriceTrait => AccessoriesBlobSubFolder.PricesFolder,
                _ => throw new NotSupportedException("This type of Trait has no Folder connected with it")
            };
            string imageUrl = await blobsRepo.UploadAccessoryImagesToBlob(imageFilePath, fileName, subFolder, false);
            operationProgress.RemainingCount--;
            return imageUrl;
        }

        /// <summary>
        /// Deletes a blob
        /// </summary>
        /// <param name="blobRelativePath">The Blobs Relative Path</param>
        /// <returns></returns>
        private async Task DeleteBlob(string blobRelativePath)
        {
            operationProgress.SetDeletingFromBlobOperation();
            await blobsRepo.DeleteBlobAsync(blobRelativePath, BlobContainerOption.AccessoriesBlobs);
            operationProgress.RemainingCount--;
        }

        /// <summary>
        /// Deletes Images along with its size Variations , returns any errors , and stores the Undeleted Urls to a Json File
        /// </summary>
        /// <param name="blobUrl"></param>
        /// <returns>Any Errors</returns>
        public async Task<string> DeleteBlobAndVariations(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl)) return string.Empty;
            string errors = string.Empty;

            // Else do the Variations
            var sizes = Enum.GetValues(typeof(BlobPhotoSize))
                            .Cast<BlobPhotoSize>()
                            .Where(x => x != BlobPhotoSize.Undefined)
                            .ToList();
            foreach (var size in sizes)
            {
                var url = BlobUrlHelper.GetImageVariationUrl(blobUrl, size);
                try
                {
                    await DeleteBlob(url);
                }
                catch (Exception ex)
                {
                    GeneralHelpers.WriteUndeletedUrlsToJsonFile(url);
                    string msg = $"Failed to Delete File with Blob Url {url}{Environment.NewLine}Exception Message :{Environment.NewLine}{ex.Message}";
                    errors = string.IsNullOrEmpty(errors) ? msg : $"{errors}{Environment.NewLine}{msg}";
                }
            }
            return errors;
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
                TraitClassUnderEdit.PropertyChanged -= TraitClassUnderEdit_PropertyChanged;
                TraitUnderEdit.PropertyChanged -= TraitUnderEdit_PropertyChanged;
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                this.closeModalService.ModalClosed -= CloseModalService_ModalClosed;
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
