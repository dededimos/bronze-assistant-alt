using Azure.Storage.Blobs.Models;
using AzureBlobStorageLibrary;
using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using CommonInterfacesBronze;
using CommunityToolkit.Diagnostics;
using DocumentFormat.OpenXml.Office2010.Excel;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Graph.Identity.ApiConnectors.Item.UploadClientCertificate;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Validators;
using MongoDB.Bson;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonInterfaces;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public interface IMirrorsRepoManagerViewModel : IBaseViewModel
    {
        //The other members of the Generic interface not implemented here as i am not in need of them in the non generic type
        public IUndoViewModel? EntityEditor { get; }
    }


    /// <summary>
    /// A Manager ViewModel to Delete / Insert / Update and Edit Entities of <typeparamref name="TEntity"/>
    /// with Undo / Redo Functionality
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IMirrorsRepoManagerViewModel<TEntity> : IBaseViewModel
        where TEntity : MongoDatabaseEntity, IDeepClonable<TEntity>, IEqualityComparerCreator<TEntity>, IDatabaseEntity
    {
        public TEntity? SelectedEntity { get; set; }
        public ObservableCollection<TEntity> EntitiesCollection { get; }
        public IMirrorEntityEditorViewModel<TEntity>? EntityEditor { get; set; }
        public IAsyncRelayCommand SaveEntityCommand { get; }
        public IRelayCommand CreateNewEntityCommand { get; }
        public IAsyncRelayCommand DeleteEntityCommand { get; }
    }

    /// <summary>
    /// The Generic View Model for dealing with the Database for Entities <see cref="IDatabaseEntity"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class MirrorsRepoManagerViewModel<TEntity> : BaseViewModel, IMirrorsRepoManagerViewModel<TEntity> , IMirrorsRepoManagerViewModel
        where TEntity : MongoDatabaseEntity, IDeepClonable<TEntity>, IEqualityComparerCreator<TEntity>, IDatabaseEntity, new()
    {
        protected Func<IMirrorEntityEditorViewModel<TEntity>> editorVmFactory;
        protected IMongoDatabaseEntityRepoCache<TEntity> repo;
        protected OperationProgressViewModel progressVm;
        protected IBlobStorageRepository blobsRepo;
        protected BlobUrlHelper urlHelper;
        protected AbstractValidator<TEntity>? validator;

        private TEntity? selectedEntity;
        public TEntity? SelectedEntity
        {
            get => selectedEntity;
            set
            {
                if (((selectedEntity?.Equals(value) ?? false) == false)
                    && EntityEditor?.HasUnsavedChanges is true
                    && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel) return;
                else
                {
                    if (value != null)
                    {
                        var editor = editorVmFactory.Invoke();
                        editor.SetModel(value);
                        EntityEditor = editor;
                    }
                    else EntityEditor = null;
                    selectedEntity = value;
                    OnPropertyChanged(nameof(SelectedEntity));
                    OnPropertyChanged(nameof(IsNewEntity));
                    OnSelectedEntityChanged();
                }
            }
        }
        
        protected virtual void OnSelectedEntityChanged()
        {
            //reserved for override
            return;
        }

        public bool IsNewEntity => SelectedEntity is null && EntityEditor is not null;
        public bool IsEditActive => EntityEditor is not null;

        public ObservableCollection<TEntity> EntitiesCollection { get; } = [];
        IUndoViewModel? IMirrorsRepoManagerViewModel.EntityEditor { get => entityEditor; }

        private IMirrorEntityEditorViewModel<TEntity>? entityEditor;
        public IMirrorEntityEditorViewModel<TEntity>? EntityEditor
        {
            get => entityEditor;
            set
            {
                if (entityEditor != value)
                {
                    entityEditor?.Dispose();
                    entityEditor = value;
                    OnPropertyChanged(nameof(EntityEditor));
                    OnPropertyChanged(nameof(IsNewEntity));
                    OnPropertyChanged(nameof(IsEditActive));
                }
            }
        }

        [RelayCommand]
        private async Task SaveEntity()
        {
            IsBusy = true;
            progressVm.SetDatabaseOperation();
            try
            {
                (TEntity? oldEntity, TEntity newEntity) = GetEntitiesAndValidate();

                //Check weather its a Insert or Update
                if (string.IsNullOrWhiteSpace(EntityEditor!.BaseEntity.Id) || EntityEditor.BaseEntity.Id.Equals(ObjectId.Empty.ToString()))
                {
                    Guard.IsNull(oldEntity);
                    await ExecuteSaveNewEntity(newEntity);
                }
                else
                {
                    Guard.IsNotNull(oldEntity);
                    await ExecuteUpdateEntity(oldEntity, newEntity);
                }
            }
            catch (ValidationException valEx)
            {
                MessageService.Warning(valEx.Message, "Validation Failure");
            }
            catch (Exception ex)
            {
                //Anything else Unexpected
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }
        }
        [RelayCommand]
        private void CreateNewEntity()
        {
            ExecuteCreateNewEntity();
        }
        protected virtual void ExecuteCreateNewEntity()
        {
            SelectedEntity = null;
            var editor = editorVmFactory.Invoke();
            TEntity entity = new();
            //Set it as Default as it will be saved into the Database
            if (entity is IMirrorElementEntity element) element.IsDefaultElement = true;
            editor.SetModel(entity);
            EntityEditor = editor;
        }

        [RelayCommand]
        private async Task DeleteEntity()
        {
            IsBusy = true;
            progressVm.SetDatabaseOperation();
            try
            {
                await ExecuteDeleteEntity();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }
        }
        [RelayCommand]
        private async Task ReloadEntities()
        {
            IsBusy = true;
            progressVm.SetDatabaseOperation();
            try
            {
                await repo.BuildCacheAsync();
                EntitiesCollection.Clear();
                foreach (var entity in repo.Cache)
                {
                    EntitiesCollection.Add(entity);
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }
        }
        [RelayCommand]
        private void LoadEntitiesFromCache()
        {
            EntitiesCollection.Clear();
            foreach (var entity in repo.Cache)
            {
                EntitiesCollection.Add(entity);
            }
        }
        [RelayCommand]
        protected override async Task InitilizeAsync()
        {
            if (repo.Cache.Any())
            {
                LoadEntitiesFromCache();
            }
            else
            {
                await ReloadEntities();
            }
        }

        private (TEntity? oldEntity, TEntity newEntity) GetEntitiesAndValidate()
        {
            //Check that Editor is in order and not Null
            Guard.IsNotNull(EntityEditor);
            TEntity newEntity = EntityEditor.GetModel();
            TEntity? oldEntity = EntitiesCollection.FirstOrDefault(e => e.Id == newEntity.Id);
            var valResult = ValidateEntity(newEntity);

            //Validate the Entity that is going to be saved
            if (!valResult.IsValid)
            {
                var joinedErrors = string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode.TryTranslateKeyWithoutError()));
                throw new ValidationException($"Validation Failed with Errors :{Environment.NewLine}{joinedErrors}");
            }
            return (oldEntity, newEntity);
        }
        protected virtual async Task<IEnumerable<string>> ExecuteSaveBlobsAsync(TEntity? oldEntity, TEntity newEntity)
        {
            await Task.Delay(1);
            return Enumerable.Empty<string>();
        }
        private async Task ExecuteSaveNewEntity(TEntity newEntity)
        {
            try
            {
                Guard.IsNull(SelectedEntity); //In New Saves the Selected Entity Should be Null otherwise something is wrong...
                var blobUploadErrors = await ExecuteSaveBlobsAsync(null, newEntity);
                string id = await repo.InsertEntityAsync(newEntity);
                newEntity.Id = id;
                //Display Messages according to if there were errors in Image Uploads
                if (blobUploadErrors.Any()) MessageService.Warning($"The Entity has been saved Successfully with Image Errors:{Environment.NewLine}{string.Join(Environment.NewLine, blobUploadErrors)}", "Partial Save Success with Image Errors");
                else MessageService.Information.SaveSuccess();
            }
            catch (Exception ex)
            {
                MessageService.Warning($"{ex.Message}", $"Error While Saving Entity of type : {newEntity.GetType().Name}");
                //If the entity failed to be saved all the Uploaded photos must be deleted , if there were errors in the Photo uploads the urls are set to empty strings so those will not count to deletion
                if (newEntity is IMirrorElementEntity element)
                {
                    //Delete Any Images that are not of the file system or not empty strings
                    List<string> urlsToDelete = new() { element.PhotoUrl, element.PhotoUrl2, element.IconUrl };
                    urlsToDelete = urlsToDelete.Where(url => !string.IsNullOrWhiteSpace(url) && File.Exists(url) is false).ToList();
                    await DeleteImages(urlsToDelete);
                }

                //Do not Add the Entities or SaveChanges or Change the Entity to the NewOne if it failed to Save
                return;
            }
            //Set The New Entity in the ViewModel to reflect the Id Changes and Refresh Undos
            EntitiesCollection.Add(newEntity);
            EntityEditor!.SaveChangesCommand.Execute(null);
            SelectedEntity = newEntity;
        }
        protected virtual async Task ExecuteUpdateEntity(TEntity oldEntity, TEntity newEntity)
        {
            try
            {
                Guard.IsNotNull(SelectedEntity); //In Updates the Selected Entity Should NOT be Null otherwise something is wrong...
                if (SelectedEntity.Id != EntityEditor!.BaseEntity.Id) throw new Exception($"Unexpected Error , Selected Entity To Save has a Different Id from the One being Edited");
                var blobUploadErrors = await ExecuteSaveBlobsAsync(oldEntity, newEntity);
                await repo.UpdateEntityAsync(newEntity);

                //Display Messages according to if there were errors in Image Uploads
                if (blobUploadErrors.Any()) MessageService.Warning($"Entity was Partially Updated , some Images were not:{Environment.NewLine}{string.Join(Environment.NewLine, blobUploadErrors)}", "Entity Saved with Image Errors");
                else MessageService.Information.SaveSuccess();
            }
            catch (Exception ex)
            {
                MessageService.Warning($"{ex.Message}", $"Error While Saving Entity of type : {newEntity.GetType().Name}");
                //If the entity fails to Update then All photos must be Deleted that were uploaded ... to do this the newEntity Urls and Old Entity Urls must be checked
                var urlsToDelete = FindUrlsToDelete(oldEntity, newEntity, false);
                if (urlsToDelete.Any()) await DeleteImages(urlsToDelete);
                return;
            }

            //If Updated then Delete all photos that are old
            var urlsToDeleteOld = FindUrlsToDelete(oldEntity, newEntity, true);
            if (urlsToDeleteOld.Any()) await DeleteImages(urlsToDeleteOld);

            //Mark entity as saved in the viewmodel
            EntityEditor!.SaveChangesCommand.Execute(null);

            //Find the Entity to Replace locally
            var entityToReplace = EntitiesCollection.FirstOrDefault(e => e.Id == newEntity.Id);
            if (entityToReplace is null) MessageService.Warning("Entity was not Replaced in the Local List ,please Reload the List", "Error Updating Local List");
            else
            {
                //replace the local entity
                var index = EntitiesCollection.IndexOf(entityToReplace);
                EntitiesCollection[index] = newEntity;
                SelectedEntity = newEntity;
            }
        }

        /// <summary>
        /// Find which urls to Delete from the Blob
        /// </summary>
        /// <param name="oldEntity">The Old Entity</param>
        /// <param name="newEntity">The new Entity Saved or Not Saved</param>
        /// <param name="fromOldEntity">True to Delete from the Old entity or False to Delete from the NewEntity</param>
        /// <returns></returns>
        private static IEnumerable<string> FindUrlsToDelete(TEntity oldEntity, TEntity newEntity, bool fromOldEntity)
        {
            if (newEntity is IMirrorElementEntity newElement && oldEntity is IMirrorElementEntity oldElement)
            {
                List<string> oldEntityUrls = new() { oldElement.PhotoUrl, oldElement.PhotoUrl2, oldElement.IconUrl };
                List<string> newEntityUrls = new() { newElement.PhotoUrl, newElement.PhotoUrl2, newElement.IconUrl };

                //Find the urls that are the same in both the new and the old entity
                //These elements wont be deleted whatever happened
                List<string> sameUrls = oldEntityUrls.Intersect(newEntityUrls).ToList();

                //Depending on which entity is the one from which we need to keep the Blobs produce the deletion list
                List<string> urlsToDelete = new();
                if (fromOldEntity)
                {
                    urlsToDelete = oldEntityUrls.Except(sameUrls).ToList();
                }
                else
                {
                    urlsToDelete = newEntityUrls.Except(sameUrls).ToList();
                }

                //return the urls to delete except any empty ones or Files that are in the FileSystem
                return urlsToDelete.Where(url => string.IsNullOrWhiteSpace(url) is false && File.Exists(url) is false);
            }
            return Enumerable.Empty<string>();
        }

        private async Task DeleteImages(IEnumerable<string> urlsToDelete)
        {
            //To Delete images because the urls might include and variations of thumb / medium / large e.t.c. 
            //we delete the blob folder as the Images are save as ClassName/Code/Property/filename.extension
            //so this way we delete all the variations if they are there ;
            //If the url contains the Icon suffix delete only one variation otherwise delete all variations

            var urlsToDeleteNumber = urlsToDelete.Count();
            double incrementPercentage = 1d / urlsToDeleteNumber * 100;
            double currentPercentage = 0;

            progressVm.SetNewOperation("Deleting Old Images", 100);
            //100% for all urls to be deleted so each increment inside the DeleteBlobFolder operation must totally remove 1 incrementPercentage
            //so its percentage reeturned from the progress report will be actually the subpercentage of the increment percentage

            var urlsNotDeletedWithErrors = new List<string>();
            var errors = new List<string>();
            foreach (var url in urlsToDelete)
            {
                var progressHandler = new Progress<double>((percentageCompleted) =>
                {
                    //Find the % of completed of the increment percentage and report it to the Vm
                    double currentIncrementPercentage = incrementPercentage * percentageCompleted / 100d;
                    progressVm.RemainingCount = 100 - currentPercentage - currentIncrementPercentage;
                });

                try
                {
                    var folder = BlobUrlHelper.GetBlobPathWithoutFileName(url);
                    await blobsRepo.DeleteBlobFolderAsync(folder, BlobContainerOption.MirrorsBlobs, progressHandler);
                }
                catch (Exception ex)
                {
                    urlsNotDeletedWithErrors.Add(url);
                    errors.Add(ex.Message);
                    Log.Error($"Could not delete Image {url}", "Image Deletion Error");
                    Log.Error(ex, "Error while Trying to Delete Image");
                }

                currentPercentage += incrementPercentage;
            }
            if (urlsNotDeletedWithErrors.Any())
            {
                GeneralHelpers.WriteUndeletedUrlsToJsonFile(urlsNotDeletedWithErrors.ToArray());
                MessageService.Warning($"There where Images that were not Deleted, Undeleted Urls are written to the Log{Environment.NewLine}{Environment.NewLine}Errors:{Environment.NewLine}{string.Join(Environment.NewLine,errors)}", "Images Deletion Failures");
            }
        }
        protected virtual async Task ExecuteDeleteEntity()
        {
            Guard.IsNotNull(SelectedEntity);
            Guard.IsNotNull(EntityEditor);
            if (EntityEditor.BaseEntity.Id != SelectedEntity.Id) throw new Exception($"Unexpected Error , The Entity to Delete Does not have a Matching Id with the Entity Editor");
            if (MessageService.Question($"This Command Will Delete the Selected Entity with Id: {SelectedEntity.Id}{Environment.NewLine}Do you want to proceed ?{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}THIS PROCESS IS IRRIVERSIBLE", $"{typeof(TEntity).Name} Deletion", $"Delete : {typeof(TEntity).Name}- Id:{SelectedEntity.Id}", "Cancel") == MessageBoxResult.Cancel)
                return;

            try
            {
                await repo.DeleteEntityAsync(EntityEditor.BaseEntity.Id);
                MessageService.Information.DeletionSuccess();
            }
            catch (Exception ex)
            {
                MessageService.Warning($"{ex.Message}", $"Error While Deleting Entity of type : {SelectedEntity.GetType().Name}");
            }

            
            //Remove any photos
            if (SelectedEntity is IMirrorElementEntity element)
            {
                List<string> urlsToDelete = new() { element.PhotoUrl, element.PhotoUrl2, element.IconUrl };
                urlsToDelete = urlsToDelete.Where(url => !string.IsNullOrEmpty(url) && !File.Exists(url)).ToList();
                if (urlsToDelete.Any()) await DeleteImages(urlsToDelete);
            }

            EntityEditor.SaveChangesCommand.Execute(null);
            
            var entityToRemove = EntitiesCollection.FirstOrDefault(e => e.Id == EntityEditor.BaseEntity.Id);
            if (entityToRemove is null) MessageService.Warning("The Entity was Deleted from the Database , but its match could not be found to Remove from the Local List , Please refresh the local list to reflect the changes", "Unexpexted Local List was not Updated");
            else
            {
                SelectedEntity = null;
                EntitiesCollection.Remove(entityToRemove);
            }
        }
        protected virtual ValidationResult ValidateEntity(TEntity entity)
        {
            return validator?.Validate(entity) ?? new ValidationResult();
        }


        public MirrorsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<TEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<TEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo)
        {
            this.editorVmFactory = editorVmFactory;
            this.repo = repo;
            this.progressVm = progressVm;
            this.urlHelper = urlHelper;
            this.blobsRepo = blobsRepo;
            Title = typeof(TEntity).Name.TryTranslateKeyWithoutError();
        }
    }

    public class BaseEntityRepoManagerViewModel : MirrorsRepoManagerViewModel<MongoDatabaseEntity>
    {
        public BaseEntityRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MongoDatabaseEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MongoDatabaseEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
        }
    }

    /// <summary>
    /// The Mirror Constraints Repository Manager ViewModel
    /// </summary>
    public class ConstraintsRepoManagerViewModel : MirrorsRepoManagerViewModel<MirrorConstraintsEntity>
    {
        public ConstraintsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorConstraintsEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo)
            : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorConstraintsEntityValidator(false);
        }
    }

    public class MirrorAplicationOptionsRepoManagerViewModel : MirrorsRepoManagerViewModel<MirrorApplicationOptionsEntity>
    {
        public MirrorAplicationOptionsRepoManagerViewModel(Func<IMirrorEntityEditorViewModel<MirrorApplicationOptionsEntity>> editorVmFactory,
                                                           IMongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity> repo,
                                                           OperationProgressViewModel progressVm,
                                                           BlobUrlHelper urlHelper,
                                                           IBlobStorageRepository blobsRepo) 
            : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorApplicationOptionsEntityValidator(false);
        }
    }

    public class PositionOptionsRepoManagerViewModel : MirrorsRepoManagerViewModel<MirrorElementPositionOptionsEntity>
    {
        public PositionOptionsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorElementPositionOptionsEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo)
            : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorElementPositionOptionsEntityValidator(false);
        }



        /// <summary>
        /// Selects the Position Options entity that concerns the Positionable with the id Argument
        /// <para>If there are no Options for the requested id , then new Empty options are presented</para>
        /// <para>If the provided Id is null or Empty then no Options are selected or presented</para>
        /// </summary>
        /// <param name="concerningPositionableId">The Id of the Positionable that the selected Position Options concern</param>
        public void SetSelectedEntity(string concerningPositionableId)
        {
            if (string.IsNullOrEmpty(concerningPositionableId) || concerningPositionableId == ObjectId.Empty.ToString())
            {
                SelectedEntity = null;
                MessageService.Warning("The provided id was either null or Empty , No Options where selected", "Options Not Selected");
            }
            else
            {
                //find the entity with the concerned id
                var entity = EntitiesCollection.FirstOrDefault(e => e.ConcerningElementId == concerningPositionableId);
                //Save any previous changes if there is an Entity Editor Set to prevent UnsavedChangesPrompt from firing
                //This will actually deselect and scrap any previous changes in position in any other active view of PositionOptions Manager
                EntityEditor?.FullUndoCommand.Execute(null);
                if (entity is null)
                {
                    CreateNewEntityCommand.Execute(null);
                    //Set the Concerning Element Manually ... hack
                    if (EntityEditor is MirrorElementPositionOptionsEntityEditorViewModel posOptionsEditor)
                    {
                        posOptionsEditor.ConcerningElement = posOptionsEditor.SelectableModulePositionables.FirstOrDefault(p => p.ElementId == concerningPositionableId);
                    }
                }
                else
                {
                    SelectedEntity = entity;
                }
            }

        }


    }
    /// <summary>
    /// The Generic ViewModel for Dealing with the Database of Mirror Elements
    /// </summary>
    /// <typeparam name="TElementEntity"></typeparam>
    public class MirrorElementRepoManagerViewModel<TElementEntity> : MirrorsRepoManagerViewModel<TElementEntity>
        where TElementEntity : MirrorElementEntity, IDeepClonable<TElementEntity>, IEqualityComparerCreator<TElementEntity>, new()
    {
        public MirrorElementRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<TElementEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<TElementEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {

        }

        protected sealed override async Task<IEnumerable<string>> ExecuteSaveBlobsAsync(TElementEntity? oldEntity, TElementEntity newEntity)
        {
            List<string> imageUploadErrors = new();
            var error1 = await SaveImageInAllSizes(oldEntity, newEntity, false);
            var error2 = await SaveImageInAllSizes(oldEntity, newEntity, true);
            var errorIcon = await SaveIcon(oldEntity, newEntity);
            imageUploadErrors.AddIf(!string.IsNullOrWhiteSpace(error1), error1);
            imageUploadErrors.AddIf(!string.IsNullOrWhiteSpace(error2), error2);
            imageUploadErrors.AddIf(!string.IsNullOrWhiteSpace(errorIcon), errorIcon);

            return imageUploadErrors;
        }
        private async Task<string> SaveImageInAllSizes(TElementEntity? entityOld, TElementEntity entityToSave, bool isPhotoUrl2)
        {
            //Do not execute save to blob if url is Empty for the new entity or if its the same with the Old
            //The Deletion of the Images in the blob will take place at the end of the Save operation
            if (isPhotoUrl2 &&
               (string.IsNullOrWhiteSpace(entityToSave.PhotoUrl2) || entityOld?.PhotoUrl2 == entityToSave.PhotoUrl2))
                return string.Empty;

            if (!isPhotoUrl2 &&
                (string.IsNullOrWhiteSpace(entityToSave.PhotoUrl) || entityOld?.PhotoUrl == entityToSave.PhotoUrl))
                return string.Empty;

            var urlToSave = isPhotoUrl2 ? entityToSave.PhotoUrl2 : entityToSave.PhotoUrl;
            progressVm.SetNewOperation($"Saving Image {Path.GetFileName(urlToSave)}", 100);
            var progressHandler = new Progress<double>((percentComplete) => progressVm.RemainingCount = 100 - percentComplete);
            try
            {
                //save the Images in the folder structure of ClassName/Code/Code-suffix.png
                //this way on deletion the ClassName/Code/Property can be deleted to remove all the photos
                var relativeBlobFilePath = await blobsRepo.UploadImageWithAllSizesToBlobAsync(
                    BlobContainerOption.MirrorsBlobs,
                    urlToSave,
                    entityToSave.Code,
                    false,
                    false,
                    progressHandler,
                    entityToSave.GetType().Name,
                    entityToSave.Code,
                    isPhotoUrl2 ? nameof(IMirrorElementEntity.PhotoUrl2) : nameof(IMirrorElementEntity.PhotoUrl)
                    );
                //Pass the new Url to the Entity and return empty Error
                if (isPhotoUrl2) entityToSave.PhotoUrl2 = relativeBlobFilePath;
                else entityToSave.PhotoUrl = relativeBlobFilePath;
                return string.Empty;
            }
            catch (Exception ex)
            {
                //restore the Image FilePath to the Old entity
                if (isPhotoUrl2) entityToSave.PhotoUrl2 = entityOld?.PhotoUrl2 ?? string.Empty;
                else entityToSave.PhotoUrl = entityOld?.PhotoUrl ?? string.Empty;
                return $"IMAGE with url {urlToSave} has failed to Upload with Error:{Environment.NewLine}{ex.Message}";
            }
        }
        private async Task<string> SaveIcon(TElementEntity? entityOld, TElementEntity entityToSave)
        {
            //Do not execute save to blob if url is Empty for the new entity or if its the same with the Old
            //The Deletion of the Images in the blob will take place at the end of the Save operations
            if (string.IsNullOrWhiteSpace(entityToSave.IconUrl) || entityOld?.IconUrl == entityToSave.IconUrl) return string.Empty;

            var urlToSave = entityToSave.IconUrl;
            progressVm.SetNewOperation($"Saving Icon {Path.GetFileName(urlToSave)}", 100);
            var progressHandler = new Progress<double>((percentComplete) => progressVm.RemainingCount = 100 - percentComplete);
            try
            {
                //save the Images in the folder structure of ClassName/Code/Code-suffix.png
                //this way on deletion the ClassName/Code/Property can be deleted to remove all the photos
                var relativeBlobFilePath = await blobsRepo.UploadImageToBlobAsync(
                    BlobContainerOption.MirrorsBlobs,
                    urlToSave,
                    BlobPhotoSize.ThumbSizePhoto,
                    entityToSave.Code,
                    string.Empty,
                    false,
                    false,
                    progressHandler,
                    entityToSave.GetType().Name,
                    entityToSave.Code,
                    nameof(IMirrorElementEntity.IconUrl));
                //Pass the new Url to the Entity and return empty Error
                entityToSave.IconUrl = relativeBlobFilePath;
                return string.Empty;
            }
            catch (Exception ex)
            {
                //Restore the url if the image failed to Upload
                entityToSave.IconUrl = entityOld?.IconUrl ?? string.Empty;
                return $"ICON with url {urlToSave} has failed to Upload with Error:{Environment.NewLine}{ex.Message}";
            }
        }
    }

    public partial class MirrorModulesRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorModuleEntity>
    {
        private readonly PositionOptionsRepoManagerViewModel positionOptionsRepoManager;
        private readonly MirrorPositionsRepoManagerViewModel positionsRepoManager;
        private readonly IWrappedViewsModalsGenerator wrappedViewsModalsGenerator;

        public MirrorModulesRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorModuleEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorModuleEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo,
            PositionOptionsRepoManagerViewModel positionOptionsRepoManager,
            IWrappedViewsModalsGenerator wrappedViewsModalsGenerator,
            MirrorPositionsRepoManagerViewModel positionsRepoManager) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorModuleEntityValidator(false);
            this.positionOptionsRepoManager = positionOptionsRepoManager;
            this.wrappedViewsModalsGenerator = wrappedViewsModalsGenerator;
            this.positionsRepoManager = positionsRepoManager;
        }

        protected override async Task InitilizeAsync()
        {
            await base.InitilizeAsync();
            await positionsRepoManager.InitilizeCommand.ExecuteAsync(null);
            await positionOptionsRepoManager.InitilizeCommand.ExecuteAsync(null);
        }

        public bool IsPositionable { get => IsPositionableModule(); }

        /// <summary>
        /// Returns weather the Module of the Selected Entity is a Positionable Module
        /// </summary>
        /// <returns>True if it is Positionable , False if it is Not</returns>
        private bool IsPositionableModule()
        {
            return SelectedEntity?.ModuleInfo is IMirrorPositionable;
        }

        /// <summary>
        /// Informs the Selected Entity changed , if its a Positionable Module
        /// </summary>
        protected override void OnSelectedEntityChanged()
        {
            OnPropertyChanged(nameof(IsPositionable));
            EditPositionOptionsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute =nameof(IsPositionable))]
        private void EditPositionOptions()
        {
            //If the Selected Entity is null or its New then Do not Fire the Command , as there are no options available
            if (SelectedEntity is null || SelectedEntity.Id == ObjectId.Empty.ToString())
            {
                MessageService.Warning($"The Entity is Either new or Not Selected , Cannot Open Position Options for New or Not Selected Entities", "Entity is New or Not Selected");
                return;
            }
            //Set the Selected Entity in the PositionOptionsManager , this way when the Modal Opens the User Gets the Position of the Selected Item
            positionOptionsRepoManager.SetSelectedEntity(SelectedEntity?.Id ?? string.Empty);
            wrappedViewsModalsGenerator.OpenModal(positionOptionsRepoManager, $"{nameof(PositionOptionsRepoManagerViewModel).TryTranslateKeyWithoutError()}");
        }

    }
    public class CustomMirrorElementsRepoManagerViewModel : MirrorElementRepoManagerViewModel<CustomMirrorElementEntity>
    {
        public CustomMirrorElementsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<CustomMirrorElementEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorElementEntityBaseValidator<CustomMirrorElementEntity>(false, true);
        }
    }
    public class MirrorLightElementsRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorLightElementEntity>
    {
        public MirrorLightElementsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorLightElementEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorLightElementEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorLightElementEntityValidator(false);
        }
    }
    public class MirrorSandblastsRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorSandblastEntity>
    {
        public MirrorSandblastsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorSandblastEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorSandblastEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorSandblastEntityValidator(false);
        }
    }
    public class MirrorSupportsRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorSupportEntity>
    {
        public MirrorSupportsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorSupportEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorSupportEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorSupportEntityValidator(false);
        }
    }
    public class MirrorSeriesRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorSeriesElementEntity>
    {
        public MirrorSeriesRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorSeriesElementEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorSeriesElementEntityValidator(false);
        }
    }
    public class MirrorPositionsRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorElementPositionEntity>
    {
        public MirrorPositionsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorElementPositionEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorElementPositionEntityValidator(false);
        }
    }
    public class MirrorFinishesRepoManagerViewModel : MirrorElementRepoManagerViewModel<MirrorFinishElementEntity>
    {
        public MirrorFinishesRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<MirrorFinishElementEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<MirrorFinishElementEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new MirrorFinishElementEntityValidator(false);
        }
    }
    public class MirrorCustomTraitsRepoManagerViewModel : MirrorElementRepoManagerViewModel<CustomMirrorTraitEntity>
    {
        public MirrorCustomTraitsRepoManagerViewModel(
            Func<IMirrorEntityEditorViewModel<CustomMirrorTraitEntity>> editorVmFactory,
            IMongoDatabaseEntityRepoCache<CustomMirrorTraitEntity> repo,
            OperationProgressViewModel progressVm,
            BlobUrlHelper urlHelper,
            IBlobStorageRepository blobsRepo) : base(editorVmFactory, repo, progressVm, urlHelper, blobsRepo)
        {
            validator = new CustomMirrorTraitEntityValidator(false);
        }
    }

}
