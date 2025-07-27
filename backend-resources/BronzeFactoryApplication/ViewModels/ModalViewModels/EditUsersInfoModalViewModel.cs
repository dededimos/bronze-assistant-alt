using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BronzeFactoryApplication.ApplicationServices.AuthenticationServices;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRepoMongoDb;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditUsersInfoModalViewModel : ModalViewModel
    {
        private readonly UserInfoEntityValidator validator = new(false);
        private readonly EditItemContext<UserInfoEntity> editContext = new(new UserEntityComparer());
        private readonly UsersRepositoryMongo usersRepo;
        private readonly UserAccessoriesOptionsRepository accOptionsRepo;
        private readonly MongoPriceRuleEntityRepo pricesRulesRepo;
        private readonly OperationProgressViewModel operationProgress;
        private readonly CloseModalService closeModalService;
        private readonly OpenEditUserAccOptionsModalService openEditAccOptionsModalService;
        private readonly GraphUsersRepository graphUsersRepo;

        [ObservableProperty]
        private UserInfoEntityViewModel userInfoUnderEdit;

        /// <summary>
        /// Connected with a Collection ViewSource cannot be newed Up
        /// </summary>
        public ObservableCollection<UserInfoEntity> Users { get; set; } = new();

        public bool IsSelectedUserNew { get => SelectedUserInfo?.Id == ObjectId.Empty; }
        public bool CanEditUser { get => SelectedUserInfo != null; }

        private UserInfoEntity? selectedUserInfo;
        public UserInfoEntity? SelectedUserInfo
        {
            get => selectedUserInfo;
            set
            {
                if (value != selectedUserInfo)
                {
                    if (editContext.HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editContext.SaveCurrentState();
                        selectedUserInfo = value;
                        OnPropertyChanged(nameof(SelectedUserInfo));
                        OnPropertyChanged(nameof(IsSelectedUserNew));
                        OnPropertyChanged(nameof(CanEditUser));
                        StartEditUser(selectedUserInfo ?? new());
                    }
                }
            }
        }

        public EditUsersInfoModalViewModel(UsersRepositoryMongo usersRepo,
                                           UserAccessoriesOptionsRepository accOptionsRepo,
                                           MongoPriceRuleEntityRepo pricesRulesRepo,
                                           OperationProgressViewModel operationProgress,
                                           CloseModalService closeModalService,
                                           Func<UserInfoEntityViewModel> userEntitiesVmFactory,
                                           OpenEditUserAccOptionsModalService openEditAccOptionsModalService,
                                           GraphUsersRepository graphUsersRepo)
        {
            this.usersRepo = usersRepo;
            this.accOptionsRepo = accOptionsRepo;
            this.pricesRulesRepo = pricesRulesRepo;
            this.operationProgress = operationProgress;
            this.closeModalService = closeModalService;
            this.openEditAccOptionsModalService = openEditAccOptionsModalService;
            this.userInfoUnderEdit = userEntitiesVmFactory.Invoke();
            this.graphUsersRepo = graphUsersRepo;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;
            this.closeModalService.ModalClosed += CloseModalService_ModalClosed;
        }

        private async void CloseModalService_ModalClosed(object? sender, ModalClosedEventArgs e)
        {
            if (e.TypeOfClosedModal == typeof(EditUserAccOptionsModalViewModel) && accOptionsRepo.IsCacheDirty)
            {
                await GetUsersData();
            }
        }

        /// <summary>
        /// Iniitilizes the Modal , Retrieves Needed Items
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        protected override async Task InitilizeAsync()
        {
            if (Initilized) return;
            await GetUsersData();
        }

        private async Task GetUsersData()
        {
            SelectedUserInfo = null;
            if (SelectedUserInfo is not null) return;

            try
            {
                IsBusy = true;

                if (pricesRulesRepo.IsCacheDirty)
                {
                    //Get All Price Rules
                    operationProgress.SetNewOperation("Retrieving Price Rules...");
                    _ = await pricesRulesRepo.GetAllEntitiesAsync();
                }

                if (accOptionsRepo.IsCacheDirty)
                {
                    //Get All User Acc.Options
                    operationProgress.SetNewOperation("Retrieving UserAccessories Options...");
                    _ = await accOptionsRepo.GetAllEntitiesAsync();
                }

                if (usersRepo.IsCacheDirty)
                {
                    //Get All Users
                    operationProgress.SetNewOperation("Retrieving Users...");
                    _ = await usersRepo.GetAllEntitiesAsync();
                }

                //Initilize The Users
                Users.Clear();
                foreach (var user in usersRepo.Cache.OrderBy(u => u.GraphUserDisplayName))
                {
                    Users.Add(user);
                }

            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false;operationProgress.MarkAllOperationsFinished(); }
        }


        [RelayCommand]
        private async Task GetAllocateGraphUsers()
        {
            try
            {
                //Get All Graph Users
                IEnumerable<GraphUser> graphUsersResult = await graphUsersRepo.GetAllUsers();
                //Tranform all Users to Entities
                var graphUsers = graphUsersResult.Select(u => new UserInfoEntity() { GraphUserObjectId = u.Id, GraphUserDisplayName = u.DisplayName, IsGraphUser = true });

                // Find which options are present in Graph but have no Open Options in Mongo
                var usersInGraphNotMongo = graphUsers.Where(gu => !Users.Any(u => u.GraphUserObjectId == gu.GraphUserObjectId)).ToList();

                // Find which options are present in Mongo but not in Graph (local users)
                // Must make it a list otherwise because it enumerates the Users below it will throw an exception as they get changed
                var usersInMongoNotGraph = Users.Where(u => !graphUsers.Any(gu => gu.GraphUserObjectId == u.GraphUserObjectId)).ToList();

                // Add any NonExistent Users to the list so they can be Saved
                foreach (var user in usersInGraphNotMongo)
                {
                    Users.Add(user);
                }

                // Mark any Is not Graph User (Not Found) to the list
                foreach (var user in usersInMongoNotGraph)
                {
                    Users.Remove(user);
                    user.IsGraphUser = false;
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            #region Retrieve Old and New User and Validate
            UserInfoEntity userToSave = UserInfoUnderEdit.GetModel();
            var oldUser = Users.FirstOrDefault(u => u.GraphUserObjectId == userToSave.GraphUserObjectId);
            if (userToSave.Id != ObjectId.Empty && oldUser is null)
            {
                throw new Exception($"The User Under Edit Was not Found in the Users List for an Unexpected Reason , Expected Id Not Found : {userToSave.Id} , UserDisplayName: {userToSave.GraphUserDisplayName}");
            }
            var valResult = validator.Validate(userToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"User Validation Failed with ErrorCodes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            #endregion

            #region Save/Update the User
            try
            {
                IsBusy = true;
                if (userToSave.Id == ObjectId.Empty)
                {
                    await usersRepo.InsertEntityAsync(userToSave);
                }
                else
                {
                    await usersRepo.UpdateEntityAsync(userToSave);
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
            // prevent prompt of unsaved Changes
            editContext.SaveCurrentState();
            // Clear Selection
            SelectedUserInfo = null;

            // Replace Old if there 
            if (oldUser != null) ReplaceInUsers(userToSave);
            // if old is not there just add New
            else Users.Add(userToSave);

            // Select the new added option
            SelectUserByObjectId(userToSave.Id);
            MessageService.Information.SaveSuccess();
            #endregion
        }
        [RelayCommand]
        private async Task DeleteUser()
        {
            try
            {
                IsBusy = true;
                if (SelectedUserInfo is null) throw new Exception("User to Delete cannot be Null...");
                if (SelectedUserInfo.UserName == IMongoAccessoriesDTORepository.DefaultUserName)
                {
                    MessageService.Warning("Cannot Delete Defaults...", "Defaults cannot be Deleted");
                    return;
                }
                if (MessageService.Question($"This will Delete User: {SelectedUserInfo.GraphUserDisplayName} with ObjectId:{SelectedUserInfo.GraphUserObjectId}{Environment.NewLine}Would you like to Proceed ?", "User Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                var userToDeleteId = SelectedUserInfo.Id;
                await usersRepo.DeleteEntityAsync(userToDeleteId);

                //Save context so it does not prompt for any changes
                editContext.SaveCurrentState();
                //Cancel Out the Selection of the TraitGroup that was deleted
                SelectedUserInfo = null;
                var userToDeleteLocally = Users.FirstOrDefault(o => o.Id == userToDeleteId);
                if (userToDeleteLocally != null)
                {
                    if (Users.Remove(userToDeleteLocally) is false)
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
                MessageService.Information.DeletionSuccess();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally { IsBusy = false; }
        }
        [RelayCommand]
        private void OpenEditAccessoriesOptions()
        {
            SelectedUserInfo = null;
            if (SelectedUserInfo is not null) return;
            openEditAccOptionsModalService.OpenModal();
        }


        private void ReplaceInUsers(UserInfoEntity replacement)
        {
            var entityToReplace = Users.FirstOrDefault(o => o.GraphUserObjectId == replacement.GraphUserObjectId) ?? throw new Exception($"User with id :{replacement.Id} was not Found in the Users List , Replacement Failed");
            var indexOfItem = Users.IndexOf(entityToReplace);
            Users[indexOfItem] = replacement;
        }
        private void SelectUserByObjectId(ObjectId id)
        {
            SelectedUserInfo = Users.FirstOrDefault(o => o.Id == id);
        }
        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this &&
                editContext.HasUnsavedChanges() &&
                MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                e.ShouldCancelClose = true;
            }
        }

        private void StartEditUser(UserInfoEntity user)
        {
            //Unsubscribe from any previous user under Edit
            UserInfoUnderEdit.PropertyChanged -= UserInfoUnderEdit_PropertyChanged;

            //Set the new User to Edit and its undo Store
            UserInfoUnderEdit.SetModel(user);
            editContext.SetUndoStore(user);

            //Subscribe again => so that edit context knows about any changes
            UserInfoUnderEdit.PropertyChanged += UserInfoUnderEdit_PropertyChanged;
        }

        private void UserInfoUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            editContext.PushEdit(UserInfoUnderEdit.GetModel());
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
                UserInfoUnderEdit.PropertyChanged -= UserInfoUnderEdit_PropertyChanged;
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                this.closeModalService.ModalClosed -= CloseModalService_ModalClosed;
                UserInfoUnderEdit.Dispose();
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
