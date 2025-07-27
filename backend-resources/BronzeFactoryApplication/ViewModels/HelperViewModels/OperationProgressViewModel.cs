using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.HelperViewModels
{
    /// <summary>
    /// A viewmodel to capture report of the progress of an operation
    /// </summary>
    public partial class OperationProgressViewModel : BaseViewModel
    {
        /// <summary>
        /// The Number of Total Items/Operations/Calculations that must be done to achieve 100% of Progress
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressCount))]
        [NotifyPropertyChangedFor(nameof(ProgressPercent))]
        private double countOfItems;
        /// <summary>
        /// The Remaining Items/Operations/Calculations to achieve 100% of Progress
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressCount))]
        [NotifyPropertyChangedFor(nameof(ProgressPercent))]
        [NotifyPropertyChangedFor(nameof(HasCompleted))]
        private double remainingCount;

        /// <summary>
        /// The Description of the Current operation
        /// </summary>
        [ObservableProperty]
        private string operationDescription = string.Empty;

        /// <summary>
        /// How many Items/Operations/Claculations from the Total have been already completed
        /// </summary>
        public double ProgressCount { get => CountOfItems - RemainingCount; }
        /// <summary>
        /// The Percentage of the Operation that has Completed
        /// </summary>
        public double ProgressPercent { get => CountOfItems != 0 ? Math.Round(ProgressCount / CountOfItems * 100, 2, MidpointRounding.ToPositiveInfinity) : 0; }
        /// <summary>
        /// Weather the Operation has been Completed
        /// </summary>
        public bool HasCompleted { get => RemainingCount == 0; }

        public OperationProgressViewModel()
        {
            //Subscribe to each own Property Changes so to Inform it has Finished
            this.PropertyChanged += OperationProgressViewModel_PropertyChanged;
        }

        /// <summary>
        /// Signals an Operation Has Finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationProgressViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(HasCompleted) && HasCompleted)
                {
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                MessageService.LogAndDisplayException(ex);
            }
        }

        public void SetNewOperation(string operationDescription, long countOfItems)
        {
            this.OperationDescription = operationDescription;
            CountOfItems = countOfItems;
            RemainingCount = countOfItems;
            if (CountOfItems > 0)
            {
                IsBusy = true;
            }
        }
        public void SetDatabaseOperation()
        {
            SetNewOperation("Querying Database...", 2);
            RemainingCount--;
        }
        public void SetNewOperation(string operationDescription)
        {
            SetNewOperation(operationDescription, 2);
            RemainingCount--;
        }
        public void SetUploadingToBlobOperation()
        {
            SetNewOperation("Uploading to Blob...", 2);
            RemainingCount--;
        }
        public void SetDeletingFromBlobOperation()
        {
            SetNewOperation("Deleting from Blob...", 2);
            RemainingCount--;
        }
        public void MarkAllOperationsFinished()
        {
            RemainingCount = 0;
        }

        /// <summary>
        /// Reduces the Count of the Current operation by 1 and Optionally changes the Description
        /// </summary>
        /// <param name="newDescription"></param>
        public void ReduceCount(string? newDescription = null)
        {
            if (RemainingCount != 0)
            {
                RemainingCount--;
            }
            if (newDescription != null)
            {
                OperationDescription = newDescription;
            }
        }
    }
}
