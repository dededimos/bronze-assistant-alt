using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Helpers;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using AzureBlobStorageLibrary;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.SignalR.Protocol;
using MongoDB.Bson;
using SharpCompress.Compressors.PPMd;
using SVGDrawingLibrary.Helpers;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text.Json;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class AccessoriesEntitiesModuleViewModel : BaseViewModel
    {
        private readonly IAccessoryEntitiesRepository repo;
        private readonly UserAccessoriesOptionsRepository accOptionsRepo;
        private readonly IBlobStorageRepository blobsRepo;
        private readonly Func<BathAccessoryEntityViewModel> bathAccessoryEntityVmFactory;
        private readonly OpenEditTraitClassModalService openEditTraitClassModelService;
        private readonly OpenEntityToJsonXmlModal openExportToJsonXmlModalService;
        private readonly OpenEditUserInfoModalService openEditUserInfoModalService;
        private readonly BathAccessoryEntityValidator entityValidator = new(false);
        private readonly CloseModalService closeModalService;
        private readonly EditItemContext<BathAccessoryEntity> bathAccessoryEditContext = new(new BathAccessoryEntityEqualityComparer());
        public override bool IsDisposable => false;

        public ObservableCollection<BathAccessoryEntity> BathAccessories { get; set; } = new();

        [ObservableProperty]
        private OperationProgressViewModel operationProgress;

        public bool CanDeleteAccessory { get => SelectedEntity != null && SelectedEntity.Id != ObjectId.Empty; }

        private BathAccessoryEntity? selectedEntity;
        public BathAccessoryEntity? SelectedEntity
        {
            get => selectedEntity;
            set
            {
                if (value != selectedEntity)
                {
                    if (selectedEntity is not null &&
                        bathAccessoryEditContext.HasUnsavedChanges() &&
                        MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    selectedEntity = value;
                    StartEditAccessory(selectedEntity ?? new());
                    OnPropertyChanged(nameof(SelectedEntity));
                    OnPropertyChanged(nameof(CanEditAccessory));
                    OnPropertyChanged(nameof(CanDeleteAccessory));
                }
            }
        }

        public bool CanEditAccessory { get => SelectedEntity is not null; }
        public bool CanSaveAccessory { get => AccessoryUnderEdit is not null; }

        [ObservableProperty]
        private bool isCacheDirty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSaveAccessory))]
        private BathAccessoryEntityViewModel? accessoryUnderEdit;

        public AccessoriesEntitiesModuleViewModel(
            IAccessoryEntitiesRepository repo,
            UserAccessoriesOptionsRepository accOptionsRepo,
            IBlobStorageRepository blobsRepo,
            Func<BathAccessoryEntityViewModel> bathAccessoryEntityVmFactory,
            OpenEditTraitClassModalService openEditTraitClassModelService,
            OpenEntityToJsonXmlModal openExportToJsonXmlModalService,
            OpenEditUserInfoModalService openEditUserInfoModalService,
            CloseModalService closeModalService,
            OperationProgressViewModel operationProgressVm)
        {
            this.repo = repo;
            this.accOptionsRepo = accOptionsRepo;
            this.blobsRepo = blobsRepo;
            this.bathAccessoryEntityVmFactory = bathAccessoryEntityVmFactory;
            this.openEditTraitClassModelService = openEditTraitClassModelService;
            this.openExportToJsonXmlModalService = openExportToJsonXmlModalService;
            this.openEditUserInfoModalService = openEditUserInfoModalService;
            this.closeModalService = closeModalService;
            operationProgress = operationProgressVm;
            closeModalService.ModalClosed += CloseModalService_ModalClosed;
            repo.OnCacheBecomingDirty += Repo_OnCacheBecomingDirty;
        }

        /// <summary>
        /// Prompts  that the Cache is Dirty so that no Controls are Available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Repo_OnCacheBecomingDirty(object? sender, EventArgs e)
        {
            IsCacheDirty = true;
        }

        /// <summary>
        /// When the Modal Closes Checks if it should Reretrieve the Accessories , Traits and Trait Classes due to the Cache being Dirty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CloseModalService_ModalClosed(object? sender, ModalClosedEventArgs e)
        {
            if (e.TypeOfClosedModal == typeof(EditTraitClassesModalViewModel) && IsCacheDirty)
            {
                await RetrieveAccessories();
            }
        }
        [RelayCommand]
        private void OpenUserInfoModal()
        {
            if (BathAccessories.Count < 1)
            {
                MessageService.Warning("Please Download Database Data First before Selecting this Option", "Database Data not Retrieved");
                return;
            }
            // Set to Null this way will scrap Changes if User Chooses So
            SelectedEntity = null;
            // if user chooses no will not open anything
            if (SelectedEntity is not null)
            {
                return;
            }
            openEditUserInfoModalService.OpenModal();
        }

        [RelayCommand]
        private void OpenExportToJsonXml()
        {
            if (BathAccessories.Count < 1)
            {
                MessageService.Warning("Please Download Database Data First before Selecting this Option", "Database Data not Retrieved");
                return;
            }
            // Set to Null this way will scrap Changes if User Chooses So
            SelectedEntity = null;
            // if user chooses no will not open anything
            if (SelectedEntity is not null)
            {
                return;
            }
            openExportToJsonXmlModalService.OpenModal();
        }

        /// <summary>
        /// Opens the Trait Classes and Traits Edit Window
        /// </summary>
        [RelayCommand]
        private void OpenEditTraitClasses()
        {
            if (BathAccessories.Count < 1)
            {
                MessageService.Warning("Please Download Database Data First before Selecting this Option", "Database Data not Retrieved");
                return;
            }
            // Set to Null this way will scrap Changes if User Chooses So
            SelectedEntity = null;
            // if user chooses no will not open anything
            if (SelectedEntity is not null)
            {
                return;
            }
            openEditTraitClassModelService.OpenModal();
        }

        /// <summary>
        /// Retrieves Accessories , Traits and Trait Classes
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task RetrieveAccessories()
        {
            // Nullify the Selection (this will prompt weather to continue or not if there are changes)
            SelectedEntity = null;
            // If the User Answers no the SelectedEntity will not Change thus canceling the Command
            if (SelectedEntity is not null)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Get All Trait Classes
                OperationProgress.SetDatabaseOperation();
                OperationProgress.SetNewOperation("Retrieving Trait Classes...");
                var allTraitClasses = await repo.Traits.TraitClasses.GetAllTraitClassesAsync();
                OperationProgress.RemainingCount--;

                // Get All TraitGroups
                OperationProgress.SetNewOperation("Retrieving TraitGroups ...");
                var allTraitGroups = await repo.Traits.TraitGroups.GetAllGroupsAsync();
                OperationProgress.RemainingCount--;
                // Order them by their lang Description
                allTraitGroups = allTraitGroups.OrderBy(tg => tg.Name.GetLocalizedValue(((App)Application.Current).SelectedLanguage)).ToList();

                // Get All Traits
                OperationProgress.SetNewOperation("Retrieving Traits...");
                var allTraits = await repo.Traits.GetAllTraitsAsync();
                // Order them by their lang Description
                allTraits = allTraits.OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)).ToList();
                OperationProgress.RemainingCount--;

                // Get All UserOptions
                OperationProgress.SetNewOperation("Retrieving User Options...");
                await accOptionsRepo.GetAllEntitiesAsync();
                OperationProgress.RemainingCount--;

                // Get All Accessories
                OperationProgress.SetNewOperation("Retrieving Accessories...");
                var allAccessories = await repo.GetAllAccessoriesAsync();

                OperationProgress.SetNewOperation("Building Accessories List...", allAccessories.Count());
                BathAccessories.Clear();
                //The Delegate passed in the Constructor will be executed each time by the Report Method
                //and will be executed
                IProgress<BathAccessoryEntity> progress = new Progress<BathAccessoryEntity>(a =>
                {
                    Application.Current.Dispatcher.InvokeAsync(() => BathAccessories.Add(a));
                    OperationProgress.RemainingCount--;
                });

                foreach (var accessory in allAccessories)
                {
                    // Executes the Delegate passed in the Above Constructor ,
                    // for every item
                    progress.Report(accessory);
                    await Task.Delay(4);
                }
                //Mark that the Cache is as the Repos
                IsCacheDirty = repo.IsCacheDirty;
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }

        }

        /// <summary>
        /// Adds a new Accessory to Edit
        /// </summary>
        [RelayCommand]
        private void AddNewAccessory()
        {
            SelectedEntity = new();
        }

        [RelayCommand]
        private async Task DeleteAccessory()
        {
            if (SelectedEntity is null || SelectedEntity.Id == ObjectId.Empty)
            {
                MessageService.Warning("Please Select a Saved Accessory to Delete , You Cannot Delete a New or Null Accessory", "Invalid Accessory Selection");
                return;
            }
            var userSelection = MessageService.Question(
                $"This will Delete All Photos and Pdf Sheets of the Accessory. As Well as any Data of the Accessory{Environment.NewLine}Would you Like to Proceed?{Environment.NewLine}{Environment.NewLine}If the Procedure Fails the Photos and the Pdf File might still get Deleted.",
                $"Delete Accessory {SelectedEntity.Code} ?",
                $"Delete {SelectedEntity.Code}",
                "Cancel");
            if (userSelection == MessageBoxResult.OK)
            {
                var urlsToDelete = GetUrlsToDelete(null, BathAccessories.First(a => a.Id == AccessoryUnderEdit!.BaseDescriptiveEntity.BaseEntity.Id), false);
                IsBusy = true;
                try
                {
                    await repo.DeleteAccessoryAsync(AccessoryUnderEdit!.BaseDescriptiveEntity.BaseEntity.Id);
                    /// if the accessory was Deleted Successfully , then Delete also the urls 
                    foreach (var url in urlsToDelete)
                    {
                        await DeleteBlobAndVariations(url);
                    }
                    MessageService.Info($"Deletion Success of {AccessoryUnderEdit.Code}", "Accessory Deletion");
                    var accessoryToRemove = BathAccessories.FirstOrDefault(a => a.Id == AccessoryUnderEdit!.BaseDescriptiveEntity.BaseEntity.Id)
                        ?? throw new Exception($"Accessory with Id:{AccessoryUnderEdit!.BaseDescriptiveEntity.BaseEntity.Id} was not found in the List of Accessories , Deletion has Failed Only Locally...");
                    BathAccessories.Remove(accessoryToRemove);
                }
                catch (Exception ex)
                {
                    MessageService.LogAndDisplayException(ex);
                    return;
                }
                finally { IsBusy = false; }
            }
        }

        [RelayCommand]
        private async Task SaveAccessory()
        {
            #region 1.Retrieve the Old and New Entity
            // Get the New entity and the OldEntity that it will Replace
            var entityToSave = AccessoryUnderEdit?.GetModel() ?? throw new InvalidOperationException($"{nameof(AccessoryUnderEdit)} cannot Be Null When Saving an Entity ...");

            // If the Old Entity is Null then the Accessory Under Edit is a New Accessory
            var oldEntity = BathAccessories.FirstOrDefault(a => a.Id == entityToSave.Id);

            // If the Entity to Save has no Empty Id then the Old entity CANNOT be Null
            if (entityToSave.Id != ObjectId.Empty && oldEntity is null)
            {
                throw new Exception($"The Accessory Under Edit Was not Found in the Bath Accessories List for an Unexpected Reason , Expected Id Not Found : {entityToSave.Id} , Code: {entityToSave.Code}");
            }
            var valResult = entityValidator.Validate(entityToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"Accessory Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            #endregion

            IsBusy = true;

            #region 2.Upload all the New Photos if Any
            // Upload MainUrl and keep Errors
            var mainUrlUploadError = await UploadMainUrlIfNew(oldEntity, entityToSave);
            // Upload Pdf and keep Errors
            var pdfUrlUploadError = await UploadPdfUrlIfNew(oldEntity, entityToSave);
            // Upload Dimension Photo Url and Keep errors
            var dimUrlUploadError = await UploadDimensionUrlIfNew(oldEntity, entityToSave);
            // Upload Extra Photos and Keep Errros
            var extraPhotosUrlUploadErrors = await UploadExtraPhotosUrlIfExist(oldEntity, entityToSave);
            // Upload Any Photos from Available Finishes
            var availableFinishesFilesErrors = await UploadAvailableFinishesFilesIfExist(oldEntity, entityToSave);
            // Combine all the Errors into a single String
            var errors = CombineErrorStrings(mainUrlUploadError, pdfUrlUploadError,dimUrlUploadError, extraPhotosUrlUploadErrors);
            #endregion

            #region 3.Update the Accessory
            bool isNewAccessory;
            try
            {
                // Update Accessory (will change the Id to Entity if new)
                isNewAccessory = await SaveAccessoryToDatabase(entityToSave);
            }
            catch (Exception ex)
            {
                //If an Error in the Save Happened then Delete all the new Files that were Uploaded
                var urls = GetUrlsToDelete(oldEntity, entityToSave, false);
                foreach (var url in urls)
                {
                    await DeleteBlobAndVariations(url);
                }
                MessageService.LogAndDisplayException(ex);
                //if the Save Operation Failed then 
                return;
            }
            finally
            {
                IsBusy = false;
            }
            #endregion

            #region 4.Delete The Old Files that were Changed
            if (!isNewAccessory)
            {
                IsBusy = true;
                var urlsToDelete = GetUrlsToDelete(oldEntity, entityToSave, true);
                foreach (var url in urlsToDelete)
                {
                    await DeleteBlobAndVariations(url);
                }
                IsBusy = false;
            }
            #endregion

            #region 5.Retain Selections
            //save the Selection of the Previously Selected Finish (so that photos Module can display the one that was also previously selected)
            var selectedAvailableFinish = AccessoryUnderEdit.SelectedAvailableFinish?.Finish.Id;
            bathAccessoryEditContext.SaveCurrentState();
            if (isNewAccessory) BathAccessories.Add(entityToSave);
            else
            {
                //Nullify Selection
                SelectedEntity = null;
                ReplaceInBathAccessories(entityToSave);
            }
            // Select the newly Saved Entity
            SelectedEntity = BathAccessories.First(a => a.Id == entityToSave.Id);
            // Retain the Selection of the Available Finish
            if (selectedAvailableFinish is not null && SelectedEntity is not null)
                AccessoryUnderEdit.SelectedAvailableFinish = AccessoryUnderEdit.AvailableFinishes.FirstOrDefault(f => f.Finish.Id == selectedAvailableFinish);
            #endregion

            #region 6.Display The Errors if Any
            if (string.IsNullOrWhiteSpace(errors))
            {
                MessageService.Information.SaveSuccess();
            }
            else
            {
                MessageService.Warning($"Save Success with Errors:{Environment.NewLine}{Environment.NewLine}{errors}", "Save Success with Errors");
            }
            #endregion

        }

        /// <summary>
        /// Returns the urls of the Blobs to Delete ,
        /// Either from the old Entity because a Save Operation Succeded ,
        /// Or from the SavedEntity which failed to Save , so Any uploaded Files must be gone
        /// </summary>
        /// <param name="oldEntity">The Old Entity before the Save</param>
        /// <param name="savedEntity">The new Entity after the Save or after the Save Operation Failed</param>
        /// <param name="fromOldEntity">True to Delete from the Old entity or False to Delete from the SavedEntity</param>
        /// <returns>The List of Blob Urls to Delete</returns>
        private List<string> GetUrlsToDelete(BathAccessoryEntity? oldEntity, BathAccessoryEntity savedEntity, bool fromOldEntity)
        {
            List<string> oldEntityUrls = new();
            if (oldEntity is not null)
            {
                oldEntityUrls.AddRange(oldEntity.ExtraPhotosURL);
                oldEntityUrls.Add(oldEntity.MainPhotoURL);
                oldEntityUrls.Add(oldEntity.PdfURL);
                oldEntityUrls.Add(oldEntity.DimensionsPhotoUrl);
                foreach (var finishInfo in oldEntity.AvailableFinishes)
                {
                    oldEntityUrls.Add(finishInfo.PhotoUrl);
                    oldEntityUrls.Add(finishInfo.PdfUrl);
                    oldEntityUrls.Add(finishInfo.DimensionsPhotoUrl);
                    oldEntityUrls.AddRange(finishInfo.ExtraPhotosUrl);
                }
            }

            List<string> savedEntityUrls = new() { savedEntity.MainPhotoURL, savedEntity.PdfURL,savedEntity.DimensionsPhotoUrl };
            savedEntityUrls.AddRange(savedEntity.ExtraPhotosURL);
            foreach (var avFinish in savedEntity.AvailableFinishes)
            {
                savedEntityUrls.Add(avFinish.PhotoUrl);
                savedEntityUrls.Add(avFinish.PdfUrl);
                savedEntityUrls.Add(avFinish.DimensionsPhotoUrl);
                savedEntityUrls.AddRange(avFinish.ExtraPhotosUrl);
            }

            var intactUrls = oldEntityUrls.Intersect(savedEntityUrls).ToList();
            IEnumerable<string> urlsToDelete;

            //When The Save Opearion Has Succeded we delete the Different Urls in the Old Entity
            if (fromOldEntity)
            {
                urlsToDelete = oldEntityUrls.Except(intactUrls).ToList();
            }
            //When the Save Operation Has Failed we delete the Different Urls in the new Entity (those that were saved before the accessory save happened)
            else
            {
                urlsToDelete = savedEntityUrls.Except(intactUrls).ToList();

            }
            return urlsToDelete.Where(url => string.IsNullOrWhiteSpace(url) is false).ToList();
        }
        private async Task<string> UploadAccessoryImageToBlob(string imageFilePath, string fileName)
        {
            OperationProgress.SetUploadingToBlobOperation();
            string imageUrl = await blobsRepo.UploadAccessoryImagesToBlob(imageFilePath, fileName, AccessoriesBlobSubFolder.AccessoriesPhotosFolder, false);
            OperationProgress.RemainingCount--;
            return imageUrl;
        }
        private async Task<string> UploadPdfToBlob(string pdfFilePath, string fileName)
        {
            OperationProgress.SetUploadingToBlobOperation();
            string pdfUrl = await blobsRepo.UploadAccessoryPdfSheetToBlob(pdfFilePath, fileName);
            OperationProgress.RemainingCount--;
            return pdfUrl;
        }
        private async Task DeleteBlob(string blobRelativePath)
        {
            OperationProgress.SetDeletingFromBlobOperation();
            await blobsRepo.DeleteBlobAsync(blobRelativePath, BlobContainerOption.AccessoriesBlobs);
            OperationProgress.RemainingCount--;
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

            // If its a pdf File do not Search for Variations
            if (blobUrl.EndsWith(".pdf"))
            {
                try
                {
                    await DeleteBlob(blobUrl);
                }
                catch (Exception ex)
                {
                    GeneralHelpers.WriteUndeletedUrlsToJsonFile(blobUrl);
                    string msg = $"Failed to Delete File with Blob Url {blobUrl}{Environment.NewLine}Exception Message :{Environment.NewLine}{ex.Message}";
                    errors = string.IsNullOrEmpty(errors) ? msg : $"{errors}{Environment.NewLine}{msg}";
                }
                return errors;
            }

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

        public void SaveStateOfCurrentEditedItem()
        {
            if (AccessoryUnderEdit is not null)
            {
                bathAccessoryEditContext.SaveCurrentState();
            }
        }

        /// <summary>
        /// Uploads the Main Url Photo and returns if there where Any Errors
        /// If it fails it wipes the New Photo and uses the Old or nothing if there is no Old Photo
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadMainUrlIfNew(BathAccessoryEntity? oldEntity, BathAccessoryEntity newEntity)
        {
            // if the Main Url is Empty in the new Entity just Return
            if (string.IsNullOrWhiteSpace(newEntity.MainPhotoURL)) return string.Empty;

            // The NewEntity will have Local Urls for any File that has Changed ,
            // Otherwise the Urls will be the Same to the New Entity
            try
            {
                if (oldEntity is null ||
                    !string.IsNullOrWhiteSpace(newEntity.MainPhotoURL) &&
                    oldEntity.MainPhotoURL != newEntity.MainPhotoURL)
                {
                    //Upload the Photo of the Main Url
                    var newUrl = await UploadAccessoryImageToBlob(newEntity.MainPhotoURL, newEntity.MainCode);

                    //Retrieve the url string and Pass it to the New Entity
                    newEntity.MainPhotoURL = newUrl;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Keep the Error to Display it
                string failureMsg = $"Failed to upload {nameof(BathAccessoryEntity.MainPhotoURL)} with Local Path: {newEntity.MainPhotoURL}{Environment.NewLine}and Failure Exception:{Environment.NewLine}{ex.Message}";
                // If operation Fails replace the new Entity MainPhotoUrl with the Old Files url
                newEntity.MainPhotoURL = oldEntity?.MainPhotoURL ?? string.Empty;
                return failureMsg;
            }
        }
        /// <summary>
        /// Uploads the Dimension Url Photo and returns if there where Any Errors
        /// If it fails it wipes the New Photo and uses the Old or nothing if there is no Old Photo
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadDimensionUrlIfNew(BathAccessoryEntity? oldEntity, BathAccessoryEntity newEntity)
        {
            // if the Main Url is Empty in the new Entity just Return
            if (string.IsNullOrWhiteSpace(newEntity.DimensionsPhotoUrl)) return string.Empty;

            // The NewEntity will have Local Urls for any File that has Changed ,
            // Otherwise the Urls will be the Same to the New Entity
            try
            {
                if (oldEntity is null ||
                    !string.IsNullOrWhiteSpace(newEntity.DimensionsPhotoUrl) &&
                    oldEntity.DimensionsPhotoUrl != newEntity.DimensionsPhotoUrl)
                {
                    //Upload the Photo of the Main Url
                    var newUrl = await UploadAccessoryImageToBlob(newEntity.DimensionsPhotoUrl, $"{newEntity.MainCode}-DIM");

                    //Retrieve the url string and Pass it to the New Entity
                    newEntity.DimensionsPhotoUrl = newUrl;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Keep the Error to Display it
                string failureMsg = $"Failed to upload {nameof(BathAccessoryEntity.DimensionsPhotoUrl)} with Local Path: {newEntity.DimensionsPhotoUrl}{Environment.NewLine}and Failure Exception:{Environment.NewLine}{ex.Message}";
                // If operation Fails replace the new Entity MainPhotoUrl with the Old Files url
                newEntity.DimensionsPhotoUrl = oldEntity?.DimensionsPhotoUrl ?? string.Empty;
                return failureMsg;
            }
        }
        /// <summary>
        /// Uploads the Pdf File Sheet and returns if there where Any Errors
        /// If it fails it wipes the New Entities PdfUrl and uses the Old or nothing if there is no Old Pdf
        /// </summary>
        /// <param name="oldEntity">The Old Entity Before the Update</param>
        /// <param name="newEntity">The New Entity</param>
        /// <returns>The error message if Any</returns>
        private async Task<string> UploadPdfUrlIfNew(BathAccessoryEntity? oldEntity, BathAccessoryEntity newEntity)
        {
            // if the PdfUrl is Empty in the new Entity just Return
            if (string.IsNullOrWhiteSpace(newEntity.PdfURL)) return string.Empty;

            // The NewEntity will have Local Urls for any File that has Changed ,
            // Otherwise the Urls will be the Same to the New Entity
            try
            {
                if (oldEntity is null ||
                    !string.IsNullOrWhiteSpace(newEntity.PdfURL) &&
                    oldEntity.PdfURL != newEntity.PdfURL)
                {
                    //Upload the Pdf to the Blob
                    var newUrl = await UploadPdfToBlob(newEntity.PdfURL, newEntity.MainCode);

                    //Retrieve the url string and Pass it to the New Entity
                    newEntity.PdfURL = newUrl;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Keep the Error to Display it
                string failureMsg = $"Failed to upload {nameof(BathAccessoryEntity.PdfURL)} with Local Path: {newEntity.PdfURL}{Environment.NewLine}and Failure Exception:{Environment.NewLine}{ex.Message}";
                // If operation Fails replace the new Entity PdfUrl with the Old Files url
                newEntity.PdfURL = oldEntity?.PdfURL ?? string.Empty;
                return failureMsg;
            }
        }
        /// <summary>
        /// Uploads the Extra Url Photos and Returns if there where any Errors 
        /// Any Photos that failed to Upload are not and the Error messages are returned Accordingly
        /// </summary>
        /// <param name="oldEntity"></param>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        private async Task<string> UploadExtraPhotosUrlIfExist(BathAccessoryEntity? oldEntity, BathAccessoryEntity newEntity)
        {
            //For The Extra Photos Urls
            // Find the Items that are the Same , Those will remain The Same as they have not changed
            // If there is no Old Entity then the Intersect will return All the New Photos
            var intactUrls = newEntity.ExtraPhotosURL.Intersect(oldEntity?.ExtraPhotosURL ?? new()).ToList();

            // Find which items in the Old List are no Longer Present in the Old by removing the intersect items
            //var urlsToDelete = newEntity.ExtraPhotosURL.Except(intactUrls);

            // Find the filePaths of the new Urls that are now the items of Old List minus the intactItems and by eliminating any empty strings
            var filePathsOfNewUrls = newEntity.ExtraPhotosURL.Except(intactUrls).Where(path => string.IsNullOrWhiteSpace(path) is false).ToList();

            StringBuilder errorsBuilder = new();
            // Upload Images for all those Urls
            foreach (var filePath in filePathsOfNewUrls)
            {
                try
                {
                    // remove the Local File Path from the New Entity
                    newEntity.ExtraPhotosURL.Remove(filePath);
                    // Upload the Image
                    var uploadedPhotoUrl = await UploadAccessoryImageToBlob(filePath, newEntity.MainCode);
                    // Add the Url of the Uploaded Image 
                    newEntity.ExtraPhotosURL.Add(uploadedPhotoUrl);
                }
                // If there is any failurte the local File Path is Removed anyways
                catch (Exception ex)
                {
                    //If there are any Errors during the Upload of the Photo
                    AddUploadFileFailureMessageToBuilder(errorsBuilder, filePath, nameof(BathAccessoryEntity.ExtraPhotosURL), ex);
                    //No need to Remove Anything whatever thrown will be in the Await Method so all Links are removed anyways
                }
            }
            return errorsBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
        /// <summary>
        /// Uploads the Extra Url Photos and Returns if there where any Errors 
        /// Any Photos that failed to Upload are not and the Error messages are returned Accordingly
        /// </summary>
        /// <param name="oldEntity"></param>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        private async Task<string> UploadAvailableFinishesFilesIfExist(BathAccessoryEntity? oldEntity, BathAccessoryEntity newEntity)
        {
            var newFinishes = newEntity.AvailableFinishes;
            var oldFinishes = oldEntity?.AvailableFinishes;
            StringBuilder errorsBuilder = new();

            foreach (var finish in newFinishes)
            {
                var matchingOldFinish = oldFinishes?.FirstOrDefault(f => f.FinishId == finish.FinishId);
                //Get the finish code to pass it inside the Photos
                var finishCode = repo.Traits.Cache.FirstOrDefault(f => f.IdAsString == finish.FinishId)?.Code ?? "notFoundCode";

                if (!string.IsNullOrWhiteSpace(finish.PhotoUrl) && finish.PhotoUrl != matchingOldFinish?.PhotoUrl)
                {
                    try
                    {
                        //Upload the Photo of the Url and replace the local with the actual
                        var newUrl = await UploadAccessoryImageToBlob(finish.PhotoUrl, $"{newEntity.Code}--{finishCode}");

                        //Replace the Local Url with the New
                        finish.PhotoUrl = newUrl;
                    }
                    catch (Exception ex)
                    {
                        AddUploadFileFailureMessageToBuilder(errorsBuilder, finish.PhotoUrl, nameof(AccessoryFinishInfo.PhotoUrl), ex);
                        // Make sure the Old Photo Url is restored
                        finish.PhotoUrl = matchingOldFinish?.PhotoUrl ?? string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(finish.PdfUrl) && finish.PdfUrl != matchingOldFinish?.PdfUrl)
                {
                    try
                    {
                        //Upload the Pdf to the Blob
                        var newUrl = await UploadPdfToBlob(finish.PdfUrl, $"{newEntity.Code}--{finishCode}");

                        //Retrieve the url string and Pass it to the New Entity
                        finish.PdfUrl = newUrl;
                    }
                    catch (Exception ex)
                    {
                        AddUploadFileFailureMessageToBuilder(errorsBuilder, finish.PdfUrl, nameof(AccessoryFinishInfo.PdfUrl), ex);
                        // Make sure the Old Url is restored
                        finish.PdfUrl = matchingOldFinish?.PdfUrl ?? string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(finish.DimensionsPhotoUrl) && finish.DimensionsPhotoUrl != matchingOldFinish?.DimensionsPhotoUrl)
                {
                    try
                    {
                        //Upload the Photo of the Url and replace the local with the actual
                        var newUrl = await UploadAccessoryImageToBlob(finish.DimensionsPhotoUrl, $"{newEntity.Code}--{finishCode}-DIM");

                        //Replace the Local Url with the New
                        finish.DimensionsPhotoUrl = newUrl;
                    }
                    catch (Exception ex)
                    {
                        AddUploadFileFailureMessageToBuilder(errorsBuilder, finish.DimensionsPhotoUrl, nameof(AccessoryFinishInfo.DimensionsPhotoUrl), ex);
                        // Make sure the Old Photo Url is restored
                        finish.DimensionsPhotoUrl = matchingOldFinish?.DimensionsPhotoUrl ?? string.Empty;
                    }
                }

                var intactPhotosUrls = finish.ExtraPhotosUrl.Intersect(matchingOldFinish?.ExtraPhotosUrl ?? new()).ToList();
                var newPhotosUrls = finish.ExtraPhotosUrl.Except(intactPhotosUrls).Where(path => string.IsNullOrWhiteSpace(path) is false).ToList();

                if (newPhotosUrls.Count > 0)
                {
                    foreach (var filePath in newPhotosUrls)
                    {
                        if (string.IsNullOrWhiteSpace(filePath)) continue;
                        try
                        {
                            // remove the Local File Path from the New Entity
                            finish.ExtraPhotosUrl.Remove(filePath);
                            // Upload the Image
                            var uploadedPhotoUrl = await UploadAccessoryImageToBlob(filePath, $"{newEntity.MainCode}--{finishCode}");
                            // Add the Url of the Uploaded Image 
                            finish.ExtraPhotosUrl.Add(uploadedPhotoUrl);
                        }
                        catch (Exception ex)
                        {
                            AddUploadFileFailureMessageToBuilder(errorsBuilder, filePath, nameof(AccessoryFinishInfo.ExtraPhotosUrl), ex);
                            // No need to remove something , the Exception is thrown in the Await method if any so all is removed prior or added after!
                        }
                    }
                }
            }
            return errorsBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
        /// <summary>
        /// Saves an Accessory Entity to the Database .If the entity is New its Id Changes to the One provided by the Database
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveAccessoryToDatabase(BathAccessoryEntity entityToSave)
        {
            bool isNew = false;
            if (entityToSave.Id == ObjectId.Empty)
            {
                var newId = await repo.InsertAccessoryAsync(entityToSave);
                entityToSave.Id = ObjectId.Parse(newId);
                isNew = true;
            }
            else
            {
                await repo.UpdateAccessoryAsync(entityToSave);
            }
            return isNew;
        }
        private void StartEditAccessory(BathAccessoryEntity accessory)
        {
            var vm = bathAccessoryEntityVmFactory.Invoke();
            vm.SetModel(accessory);

            // Unsubscribe from the Previous one and Dispose of It
            if (AccessoryUnderEdit is not null)
            {
                AccessoryUnderEdit.PropertyChanged -= AccessoryUnderEdit_PropertyChanged;
                AccessoryUnderEdit.Dispose();
            }

            AccessoryUnderEdit = vm;

            //bathAccessoryEditContext.ResetContext();
            bathAccessoryEditContext.SetUndoStore(accessory);

            // Subscribe to the New One
            if (AccessoryUnderEdit is not null)
            {
                AccessoryUnderEdit.PropertyChanged += AccessoryUnderEdit_PropertyChanged;
            }
        }

        /// <summary>
        /// Replaces a Bath Accessory Entity in the BathAccessories List , Id Must not be New
        /// </summary>
        /// <param name="replacement"></param>
        /// <exception cref="Exception"></exception>
        private void ReplaceInBathAccessories(BathAccessoryEntity replacement)
        {
            var entityToReplace = BathAccessories.FirstOrDefault(a => a.Id == replacement.Id) ?? throw new Exception($"Accessory with id :{replacement.Id} was not Found in the BathAccessories List , Replacement Failed");
            var indexOfItem = BathAccessories.IndexOf(entityToReplace);
            BathAccessories[indexOfItem] = replacement;
        }

        /// <summary>
        /// Combines non Empty Strings and adds two 'NewLine Charachters between them , If the errors are empty returns an empty string
        /// </summary>
        /// <param name="errors">The Errors</param>
        /// <returns></returns>
        private string CombineErrorStrings(params string[] errors)
        {
            var nonEmptyErrors = errors.Where(e => !string.IsNullOrWhiteSpace(e));
            if (nonEmptyErrors.Count() is 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join($"{Environment.NewLine}{Environment.NewLine}", nonEmptyErrors);
            }
        }

        private void AddUploadFileFailureMessageToBuilder(in StringBuilder errorsBuilder, string filePath, string propName, Exception ex)
        {
            errorsBuilder.Append("Failed to upload one of the ")
                                 .Append(propName)
                                 .Append(" with Local Path: ")
                                 .Append(filePath)
                                 .Append(Environment.NewLine)
                                 .Append("and Failure Exception:")
                                 .Append(Environment.NewLine)
                                 .Append(ex.Message)
                                 .Append(Environment.NewLine)
                                 .Append(Environment.NewLine);
        }

        /// <summary>
        /// Pushes Edits to the Edit Context whenever there is a Proeprty Change in the Accessory under Edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccessoryUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (AccessoryUnderEdit is not null)
            {
                bathAccessoryEditContext.PushEdit(AccessoryUnderEdit.GetModel());
            }
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
                closeModalService.ModalClosed -= CloseModalService_ModalClosed;
                repo.OnCacheBecomingDirty -= Repo_OnCacheBecomingDirty;
                if (AccessoryUnderEdit is not null)
                {
                    AccessoryUnderEdit.PropertyChanged -= AccessoryUnderEdit_PropertyChanged;
                    AccessoryUnderEdit.Dispose();
                }
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }


        [RelayCommand]
        private void FindInvalidEntities()
        {
            BathAccessoryEntityValidator validator = new();
            List<(BathAccessoryEntity a, string error)> invalidEntities = new();
            foreach (var a in BathAccessories)
            {
                if (validator.Validate(a).IsValid == false)
                {
                    invalidEntities.Add((a, string.Join(Environment.NewLine, validator.Validate(a).Errors.Select(e => e.ErrorCode))));
                }
            }
            string message = string.Empty;
            foreach (var (a, error) in invalidEntities)
            {
                message += $"{a.Code}:{error}{Environment.NewLine}";
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageService.Info("No Invalids found , all Entities are Valid", "No Invalids Found");
            }
            else
            {
                MessageService.Warning(message, "Invalids");
            }
        }

    }
}
