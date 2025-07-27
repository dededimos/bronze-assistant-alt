using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using ClosedXML.Excel;
using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonOrderModels;
using CommunityToolkit.Diagnostics;
using MirrorsLib;
using MirrorsLib.MirrorsOrderModels;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Repositories;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels
{
    public partial class MirrorsOrderViewModel : UndoEditorViewModelBase<MirrorsOrder>
    {
        public MirrorsOrderViewModel(IWrappedViewsModalsGenerator modalsGenerator,
            Func<MirrorOrderRowViewModel> orderRowVmFactory,
            MirrorsOrdersRepository ordersRepo,
            OperationProgressViewModel progressVm,
            Func<MirrorOrderRowUndoViewModel> orderRowEditorVmFactory)
        {
            Rows.CollectionChanged += Rows_CollectionChanged;
            this.modalsGenerator = modalsGenerator;
            this.orderRowVmFactory = orderRowVmFactory;
            this.ordersRepo = ordersRepo;
            this.progressVm = progressVm;
            RowEditor = orderRowEditorVmFactory.Invoke();
        }

        private readonly IWrappedViewsModalsGenerator modalsGenerator;
        private readonly Func<MirrorOrderRowViewModel> orderRowVmFactory;
        private readonly MirrorsOrdersRepository ordersRepo;
        private readonly OperationProgressViewModel progressVm;

        private void Rows_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Rows)); //External consumers cannot know otherwise that the collection has changed
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPAOPAM));
        }

        public bool IsNewOrder { get => string.IsNullOrEmpty(Id); }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNewOrder))]
        private string id = string.Empty;

        [ObservableProperty]
        private string orderNo = MirrorsOrder.newOrderNo;

        /// <summary>
        /// Validates the Order No And Displays a Message if it is not Valid
        /// </summary>
        /// <returns></returns>
        private bool ValidateOrderNoAndDisplayMessage()
        {
            //Used Inside the Order No WrappedModal
            var isValid = MirrorsOrder.OrderNoRegex.IsMatch(OrderNo);
            if (!isValid)
            {
                MessageService.Warnings.InvalidOrderId();
            }
            return isValid;
        }


        public DateTime Created { get; private set; }
        
        [ObservableProperty]
        private DateTime lastModified;

        [ObservableProperty]
        private string notes = string.Empty;

        [ObservableProperty]
        private string lastOrderNo = string.Empty;


        public ObservableCollection<MirrorOrderRowViewModel> Rows { get; } = [];

        public MirrorOrderRowUndoViewModel RowEditor { get; }

        [ObservableProperty]
        private MirrorOrderRowViewModel? selectedRow;

        public CommonOrderModels.OrderStatus Status { get => MirrorsOrder.GetCombinedStatus(Rows.Select(r=> r.Status)); }
        public int TotalQuantity { get => Convert.ToInt32(Rows.Sum(r => r.Quantity)); }
        public int TotalPAOPAM { get => Rows.Select(r => r.RefPAOPAM).Distinct().Count(); }

        public void AddRow(MirrorSynthesis mirror , string notes, double quantity , string refPaoPam)
        {
            MirrorOrderRow row = new()
            {
                Notes = notes,
                Quantity = quantity,
                LineNumber = Rows.Count + 1,
                RowItem = mirror,
                ParentOrderNo = OrderNo,
            };
            row.AddMetadata(MirrorOrderRow.PaoPamMetadataKey, refPaoPam);

            var rowVm = orderRowVmFactory.Invoke();
            rowVm.SetModel(row);

            Rows.Add(rowVm);
        }

        [RelayCommand]
        private void RemoveRow(MirrorOrderRowViewModel? row)
        {
            if (row is null) return;
            if (MessageService.Questions.ThisWillRemoveEditedItemContinue() == MessageBoxResult.Cancel) return;
            bool removed = Rows.Remove(row);
            row.Dispose();
            if (!removed) MessageService.Warning($"Unexpected Error - Row with Mirror {row.Mirror?.Code ?? "NullMirror"} was not Found , there was nothing to Remove ...","Row Not Found");
            else
            {
                //rearrange Line numbers to fill up the missing gap
                int itemCount = 0;
                foreach (var item in Rows)
                {
                    itemCount++;
                    item.LineNumber = itemCount;
                }
            }
        }

        [RelayCommand]
        private void EditRow(MirrorOrderRowViewModel rowToEdit)
        {
            RowEditor.SetModel(rowToEdit.GetModel());
            modalsGenerator.OpenModal(RowEditor,
                                      $"{"lngEdit".TryTranslateKeyWithoutError()} {"lngRow".TryTranslateKeyWithoutError()}-{rowToEdit.LineNumber}",
                                      () => ExecuteEditRow(rowToEdit.LineNumber),
                                      WrappedModalCustomActionButtonOption.SaveAndClose,
                                      false,
                                      () =>
                                      {
                                          if (RowEditor.HasUnsavedChanges && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                                          {
                                              //Return false to prevent the closing of the Modal
                                              return false;
                                          }
                                          return true;
                                      });
        }
        private void ExecuteEditRow(int lineNumberToEdit)
        {
            var replacement = RowEditor.GetModel();
            var rowToEdit = Rows.FirstOrDefault(r => r.LineNumber == lineNumberToEdit);
            if (rowToEdit is null)
            {
                MessageService.Warning($"Row with Line Number {lineNumberToEdit} was not Found", "Row Not Found");
                return;
            }
            rowToEdit.SetModel(replacement);
            OnPropertyChanged(nameof(Rows)); //External consumers cannot know otherwise that the collection has changed
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(TotalPAOPAM));

            //Save changes so it can close the modal afterwards
            RowEditor.SaveChangesCommand.Execute(null);
        }

        [RelayCommand]
        private void EditOrderDetails()
        {
            modalsGenerator.OpenModal(this,
                                      "lngEditGlassesOrderDetailsModalTitle".TryTranslateKeyWithoutError(),
                                      () => { },
                                      WrappedModalCustomActionButtonOption.SaveAndClose,
                                      preventCloseIfFalseFunction:ValidateOrderNoAndDisplayMessage);
        }

        [RelayCommand]
        private async Task LoadLastOrderNo()
        {
            Application.Current.Dispatcher.Invoke(() => IsBusy = true);
            try
            {
                string id = await ordersRepo.GetLastOrderNoAsync();
                //We have to use the Dispatcher to update the UI other wise the Update might not happen in the UI Thread
                Application.Current.Dispatcher.Invoke(() => LastOrderNo = id);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageService.LogAndDisplayException(ex));
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => IsBusy = false);
            }

        }

        [RelayCommand]
        private async Task TrySaveOrder()
        {
            if (ValidateOrderNoAndDisplayMessage() is false) return;
            if (!HasUnsavedChanges && MessageService.Question("Order is already Saved , Overwrite Again the same ?!", "Order is Already saved", "Ok", "Cancel") == MessageBoxResult.Cancel) return;

            IsBusy = true;
            try
            {
                var entity = MirrorsOrderEntity.CreateFromModel(this.GetModel());
                
                if (IsNewOrder)
                {
                    progressVm.SetNewOperation($"Saving Order:{OrderNo}");
                    var id = await ordersRepo.InsertEntityAsync(entity);
                    Id = id;
                }
                else
                {
                    progressVm.SetNewOperation($"Updating Order:{OrderNo}");
                    await ordersRepo.UpdateEntityAsync(entity);
                }
                SaveChangesCommand.Execute(null);
                MessageService.Information.SaveSuccess();
            }
            catch (Exception ex)
            {
                progressVm.SetNewOperation("Save/Update Failed...");
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }
        }

        [RelayCommand]
        private async Task GenerateOrderXls(CancellationToken cancellationToken) 
        {
            if (ValidateOrderNoAndDisplayMessage() is false) return;
            //if (HasUnsavedChanges)
            //{
            //    MessageService.Warning($"You Have to Save the Order First... There are Unsaved Changes", "Order Not Saved");
            //    return;
            //}
            if(Rows.Count == 0)
            {
                MessageService.Warning($"You Have to Add at least One Row to the Order...", "Order is Empty");
                return;
            }
            if (HasUnsavedChanges)
            {
                MessageService.Warnings.UnsavedChangesCannotProceedWithoutSave();
                return;
            }

            IsBusy = true;
            try
            {
                using var wb = new XLWorkbook();
                string fileName = $"{"lngOrder".TryTranslateKeyWithoutError()} - {OrderNo} - {DateTime.Now:dd-MM-yyyy}";
                IProgress<TaskProgressReport> progress = new Progress<TaskProgressReport>(report =>
                {
                    progressVm.SetNewOperation(report.CurrentStepDescription, Convert.ToInt64(report.TotalSteps));
                    progressVm.RemainingCount = report.RemainingSteps;
                });
                var reportTemplate = await Task.Run(() =>
                {
                    return new MirrorGlassesOrderReport();
                });
                
                var orderModel = this.GetModel();
                await reportTemplate.GenerateReport(orderModel, wb, progress, cancellationToken);
                wb.Author = $"FactoryApp - {((App)(Application.Current)).ApplicationVersion}";

                var fullPath = ExcelService.SaveXlsFile(wb, fileName);

                if (MessageService.Questions.ExcelSavedAskOpenFile(fullPath) == MessageBoxResult.OK)
                {
                    //Open the file if users reply is positive
                    Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                }
            }
            catch (OperationCanceledException)
            {
                // The task was cancelled.
                // Rethrow to let the AsyncRelayCommand know about the cancellation.
                throw;
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

        public override MirrorsOrder CopyPropertiesToModel(MirrorsOrder model)
        {
            model.Id = this.Id;
            model.OrderNo = this.OrderNo;
            model.Created = this.Created;
            model.LastModified = this.LastModified;
            model.Notes = this.Notes;
            model.Rows = new(this.Rows.Select(r => r.GetModel()));
            return model;
        }

        public override MirrorsOrder GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        protected override void SetModelWithoutUndoStore(MirrorsOrder model)
        {
            this.Id = model.Id;
            this.OrderNo = model.OrderNo;
            this.Created = model.Created;
            this.LastModified = model.LastModified;
            this.Notes = model.Notes;
            Rows.Clear();
            foreach (var row in model.Rows)
            {
                var vm = orderRowVmFactory.Invoke();
                vm.SetModel(row);
                Rows.Add(vm);
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
                Rows.CollectionChanged -= Rows_CollectionChanged;
                foreach (var item in Rows)
                {
                    item.Dispose();
                }
                Rows.Clear();
                RowEditor.Dispose();
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
