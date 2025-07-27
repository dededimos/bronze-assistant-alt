using AccessoriesRepoMongoDB.Entities;
using AzureBlobStorageLibrary;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.LabelingAccessoriesConversions;
using BronzeFactoryApplication.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class AccessoriesModuleViewModel : BaseViewModel, IOperationOnNavigatingAway
    {
        public override bool IsDisposable => false;

        public AccessoriesEntitiesModuleViewModel AccessoriesEntitiesVm { get; set; }

        public AccessoriesModuleViewModel(AccessoriesEntitiesModuleViewModel accessoriesEntitiesVm, 
            LabelingAccessoriesBuilder labelsBuilder)
        {
            AccessoriesEntitiesVm = accessoriesEntitiesVm;
            this.labelsBuilder = labelsBuilder;
        }

        [RelayCommand]
        private static void GetAccessoriesXls()
        {
            
        }
        [RelayCommand]
        private async Task CreateLabels()
        {
            try
            {
                AccessoriesEntitiesVm.OperationProgress.SetNewOperation("Creating Labels Database", 100);
                Progress<int> progress = new(percentage =>
                {
                    AccessoriesEntitiesVm.OperationProgress.RemainingCount = 100 - percentage;
                });
                await labelsBuilder.AddEntitiesToLabelsDatabase(progress);
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                AccessoriesEntitiesVm.OperationProgress.MarkAllOperationsFinished();
            }
        }


        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        private readonly LabelingAccessoriesBuilder labelsBuilder;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {

            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

        public async Task OnNavigatingAwayOperation()
        {
            // Saves the State of the Current Edited Accessory otherwise it keeps asking questions
            await Task.Delay(1);
            AccessoriesEntitiesVm.SaveStateOfCurrentEditedItem();
        }
    }
}
