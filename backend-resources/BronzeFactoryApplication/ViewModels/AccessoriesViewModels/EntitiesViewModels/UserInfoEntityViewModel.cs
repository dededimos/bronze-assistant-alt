using AccessoriesRepoMongoDB.Entities;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.Functions.Isref;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRepoMongoDb;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels
{
    public partial class UserInfoEntityViewModel : BaseViewModel, IEditorViewModel<UserInfoEntity>
    {
        private readonly UserAccessoriesOptionsRepository accOptionsRepo;

        [ObservableProperty]
        private DbEntityViewModel baseEntity;

        [ObservableProperty]
        private bool isEnabled;
        [ObservableProperty]
        private string userName = string.Empty;
        [ObservableProperty]
        private string userPassword = string.Empty;
        [ObservableProperty]
        private UserAccessoriesOptionsEntity? selectedAccessoriesOptions;
        [ObservableProperty]
        private string graphUserObjectId = string.Empty;
        [ObservableProperty]
        private string graphUserDisaplayName = string.Empty;
        [ObservableProperty]
        private bool isGraphUser;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMachineRegistered))]
        private string registeredMachine = string.Empty;
        public bool IsMachineRegistered { get => !string.IsNullOrWhiteSpace(RegisteredMachine); }

        private bool canCopyMachineRegistry;
        public bool CanCopyMachineRegistry { get => canCopyMachineRegistry; private set => SetProperty(ref canCopyMachineRegistry, value); }

        public IEnumerable<UserAccessoriesOptionsEntity> AccessoriesOptions { get => accOptionsRepo.Cache; }

        public UserInfoEntityViewModel(Func<DbEntityViewModel> baseEntityVmFactory,
                                       UserAccessoriesOptionsRepository accOptionsRepo)
        {
            baseEntity = baseEntityVmFactory.Invoke();
            this.accOptionsRepo = accOptionsRepo;
            this.accOptionsRepo.OnCacheRefresh += AccOptionsRepo_OnCacheRefresh;
        }

        private void AccOptionsRepo_OnCacheRefresh(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AccessoriesOptions));
        }

        public UserInfoEntity CopyPropertiesToModel(UserInfoEntity model)
        {
            BaseEntity.CopyPropertiesToModel(model);
            model.IsEnabled = this.IsEnabled;
            model.UserName = this.UserName;
            model.UserPassword = this.UserPassword;
            model.AccessoriesOptionsId = this.SelectedAccessoriesOptions?.IdAsString ?? string.Empty;
            model.GraphUserObjectId = this.GraphUserObjectId;
            model.GraphUserDisplayName = this.GraphUserDisaplayName;
            model.IsGraphUser = this.IsGraphUser;
            model.RegisteredMachine = this.RegisteredMachine;
            return model;
        }

        public UserInfoEntity GetModel()
        {
            UserInfoEntity model = new();
            return CopyPropertiesToModel(model);
        }

        public void SetModel(UserInfoEntity model)
        {
            BaseEntity.SetModel(model);
            IsEnabled = model.IsEnabled;
            UserName = model.UserName;
            UserPassword = model.UserPassword;
            SelectedAccessoriesOptions = AccessoriesOptions.FirstOrDefault(o=> o.IdAsString == model.AccessoriesOptionsId);
            GraphUserObjectId = model.GraphUserObjectId;
            GraphUserDisaplayName = model.GraphUserDisplayName;
            IsGraphUser = model.IsGraphUser;
            RegisteredMachine = model.RegisteredMachine;
            
            //Can copy only when the machine is set for the first time
            CanCopyMachineRegistry = string.IsNullOrWhiteSpace(model.RegisteredMachine);
        }

        [RelayCommand]
        private void RegisterMachine()
        {
            if (IsMachineRegistered)
            {
                MessageService.Info("Machine has already been Registered, Cannot Register Twice or Override the old Registry", "Machine already Registered");
                return;
            }
            else
            {
                var newRegistry = Guid.NewGuid().ToString();
                RegisteredMachine = newRegistry;
                MessageService.Info($"Machine has been successfully Registered , Save User to Save Registry.{Environment.NewLine}{Environment.NewLine}Machine Registry: {newRegistry}","Registration Success");
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
                this.accOptionsRepo.OnCacheRefresh -= AccOptionsRepo_OnCacheRefresh;
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
