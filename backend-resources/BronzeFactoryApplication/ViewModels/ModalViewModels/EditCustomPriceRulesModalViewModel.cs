using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using BronzeFactoryApplication.ApplicationServices.AuthenticationServices;
using BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRepoMongoDb;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditCustomPriceRulesModalViewModel : ModalViewModel
    {
        private readonly CustomPriceRuleEntityValidator validator = new(false);
        private readonly EditItemContext<CustomPriceRuleEntity> editContext = new(new CustomPriceRuleEntityComparer());
        private readonly MongoPriceRuleEntityRepo rulesRepo;
        private readonly OperationProgressViewModel operationProgress;
        private readonly OpenEditLocalizedStringModalService openEditLocStringModalService;
        private readonly CloseModalService closeModalService;

        public EditCustomPriceRulesModalViewModel(MongoPriceRuleEntityRepo rulesRepo,
                                                  OperationProgressViewModel operationProgress,
                                                  Func<CustomPriceRuleEntityViewModel> rulesEntitiesVmFactory,
                                                  OpenEditLocalizedStringModalService openEditLocStringModalService,
                                                  CloseModalService closeModalService)
        {
            this.rulesRepo = rulesRepo;
            this.ruleUnderEdit = rulesEntitiesVmFactory.Invoke();
            this.operationProgress = operationProgress;
            this.openEditLocStringModalService = openEditLocStringModalService;
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;

            //Initilize The Rules
            foreach (var rule in rulesRepo.Cache.OrderBy(r => r.SortNo))
            {
                Rules.Add(rule);
            }
        }

        [ObservableProperty]
        private CustomPriceRuleEntityViewModel ruleUnderEdit;
        /// <summary>
        /// Connected with a Collection ViewSource cannot be newed Up
        /// </summary>
        public ObservableCollection<CustomPriceRuleEntity> Rules { get; set; } = new();
        public bool IsSelectedRuleNew { get => SelectedRule?.Id == ObjectId.Empty; }
        public bool CanEditRule { get => SelectedRule != null; }

        private CustomPriceRuleEntity? selectedRule;
        public CustomPriceRuleEntity? SelectedRule
        {
            get => selectedRule;
            set
            {
                if (value != selectedRule)
                {
                    if (editContext.HasUnsavedChanges() && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        editContext.SaveCurrentState();
                        selectedRule = value;
                        OnPropertyChanged(nameof(SelectedRule));
                        OnPropertyChanged(nameof(IsSelectedRuleNew));
                        OnPropertyChanged(nameof(CanEditRule));
                        StartEditRule(selectedRule ?? new());
                    }
                }
            }
        }

        [RelayCommand]
        private async Task SaveRule()
        {
            #region Retrieve Old and New Rule and Validate
            CustomPriceRuleEntity ruleToSave = RuleUnderEdit.GetModel();
            var oldRule = Rules.FirstOrDefault(r => r.Id == ruleToSave.Id);
            if (ruleToSave.Id != ObjectId.Empty && oldRule is null)
            {
                throw new Exception($"The Rule Under Edit Was not Found in the Rules List for an Unexpected Reason , Expected Id Not Found : {ruleToSave.Id} , RuleName: {ruleToSave.Name}");
            }
            var valResult = validator.Validate(ruleToSave);
            if (!valResult.IsValid)
            {
                MessageService.Warning($"Rule Validation Failed with ErrorCodes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}", "Validation Failed");
                return;
            }
            #endregion

            #region Save/Update the Rule
            try
            {
                IsBusy = true;
                if (ruleToSave.Id == ObjectId.Empty)
                {
                    await rulesRepo.InsertEntityAsync(ruleToSave);
                }
                else
                {
                    await rulesRepo.UpdateEntityAsync(ruleToSave);
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
            SelectedRule = null;

            // Replace Old if there 
            if (oldRule != null) ReplaceInRules(ruleToSave);
            // if old is not there just add New
            else Rules.Add(ruleToSave);

            // Select the new added option
            SelectRuleByObjectId(ruleToSave.Id);

            MessageService.Information.SaveSuccess();
            #endregion
        }

        [RelayCommand]
        private async Task DeleteRule()
        {
            try
            {
                IsBusy = true;
                if (SelectedRule is null) throw new Exception("Rule to Delete cannot be Null...");
                
                if (MessageService.Question($"This will Delete Rule: {SelectedRule.Name} with Id:{SelectedRule.Id}{Environment.NewLine}Would you like to Proceed ?", "Rule Deletion", "DELETE", "Cancel") == MessageBoxResult.Cancel) return;
                var ruleToDeleteId = SelectedRule.Id;
                await rulesRepo.DeleteEntityAsync(ruleToDeleteId);

                //Save context so it does not prompt for any changes
                editContext.SaveCurrentState();
                //Cancel Out the Selection of the TraitGroup that was deleted
                SelectedRule = null;
                var ruleToDeleteLocally = Rules.FirstOrDefault(r => r.Id == ruleToDeleteId);
                if (ruleToDeleteLocally != null)
                {
                    if (Rules.Remove(ruleToDeleteLocally) is false)
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
        private void CreateNewRule()
        {
            SelectedRule = null;
            if (SelectedRule != null)
            {
                return;
            }
            SelectedRule = new();
        }

        [RelayCommand]
        private void OpenEditLocalizedString(string localizedStringPropertyName)
        {
            LocalizedStringViewModel vm = localizedStringPropertyName switch
            {
                nameof(DescriptiveEntityViewModel.Name) => RuleUnderEdit.BaseDescriptiveEntity.Name,
                nameof(DescriptiveEntityViewModel.Description) => RuleUnderEdit.BaseDescriptiveEntity.Description,
                nameof(DescriptiveEntityViewModel.ExtendedDescription) => RuleUnderEdit.BaseDescriptiveEntity.ExtendedDescription,
                _ => throw new Exception("Unrecognized Localized String Entity to Edit"),
            };
            openEditLocStringModalService.OpenModal(vm, $"{"lngEdit".TryTranslateKey()} - {string.Concat("lng", localizedStringPropertyName).TryTranslateKey()}");
        }

        private void StartEditRule(CustomPriceRuleEntity rule)
        {
            //Unsubscribe from any previous rule under Edit
            RuleUnderEdit.PropertyChanged -= RuleUnderEdit_PropertyChanged;

            //Set the new User to Edit and its undo Store
            RuleUnderEdit.SetModel(rule);
            editContext.SetUndoStore(rule);

            //Subscribe again => so that edit context knows about any changes
            RuleUnderEdit.PropertyChanged += RuleUnderEdit_PropertyChanged;
        }
        private void RuleUnderEdit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            editContext.PushEdit(RuleUnderEdit.GetModel());
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
                modal.LocalizedStringVm == RuleUnderEdit.BaseDescriptiveEntity.Name ||
                modal.LocalizedStringVm == RuleUnderEdit.BaseDescriptiveEntity.Description ||
                modal.LocalizedStringVm == RuleUnderEdit.BaseDescriptiveEntity.ExtendedDescription)
                && modal.HasMadeChanges)
            {
                editContext.PushEdit(RuleUnderEdit.GetModel());
            }
        }
        private void ReplaceInRules(CustomPriceRuleEntity replacement)
        {
            var entityToReplace = Rules.FirstOrDefault(r => r.Id == replacement.Id) ?? throw new Exception($"Rule with id :{replacement.Id} was not Found in the Rules List , Replacement Failed");
            var indexOfItem = Rules.IndexOf(entityToReplace);
            Rules[indexOfItem] = replacement;
        }
        private void SelectRuleByObjectId(ObjectId id)
        {
            SelectedRule = Rules.FirstOrDefault(r => r.Id == id);
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
                RuleUnderEdit.PropertyChanged -= RuleUnderEdit_PropertyChanged;
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                RuleUnderEdit.Dispose();
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
