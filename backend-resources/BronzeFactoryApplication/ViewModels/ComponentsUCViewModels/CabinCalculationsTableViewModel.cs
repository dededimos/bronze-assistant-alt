
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using FluentValidation.Results;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels
{
    public partial class CabinCalculationsTableViewModel : BaseViewModel
    {
        /// <summary>
        /// Fires whenever Calculation have Performed
        /// </summary>
        public event EventHandler? CalculationsPerfomed;
        private readonly CabinCalculationsService calculationsService;
        private CabinViewModel? cabinVm;
        /// <summary>
        /// A timer to control the intervals of the Calculations
        /// If Calculations are needed they will be executed only once every second 
        /// This way multiple property changes will only trigger a single calculation in the end
        /// </summary>
        private readonly System.Timers.Timer _timer = new();

        public ObservableCollection<Glass> Glasses { get => new(cabinVm?.Glasses ?? new()); }

        public bool IsInSwapGlassMode { get => cabinVm?.IsInSwapGlassMode ?? false; }

        /// <summary>
        /// Weather the ViewModel is Calculating
        /// </summary>
        public bool IsCalculating { get => _timer.Enabled; }

        public bool HasHorizontalProfile { get => HorizontalProfile is not null; }
        public IHorizontalProfile? HorizontalProfile
        {
            get
            {
                var parts = cabinVm?.Parts?.GetPartsObject();
                if (parts is not null && parts is IHorizontalProfile hProfile) return hProfile;
                return null;
            }
        }

        /// <summary>
        /// The Validation Result from Calculations , Notifies the Whole Class when changed
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValidationErrors))]
        [NotifyPropertyChangedFor(nameof(HasErrors))]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        [NotifyPropertyChangedFor(nameof(Opening))]
        [NotifyPropertyChangedFor(nameof(Glasses))]
        [NotifyPropertyChangedFor(nameof(HasHorizontalProfile))]
        [NotifyPropertyChangedFor(nameof(HorizontalProfile))]
        [NotifyPropertyChangedFor(nameof(IsInSwapGlassMode))]
        private ValidationResult? validationResult;

        /// <summary>
        /// Translates the Collection of Validation Errors into Readable Content for the Connected View
        /// </summary>
        public IEnumerable<string> ValidationErrors
        {
            get => IsCalculating
                ? new string[] { "lngStillCalculating".TryTranslateKey() } //Never Hits this the calculations manage to finish before getting the Error
                : ValidationResult?.Errors
                .Select(e => calculationsService.TranslateErrorCode(e.ErrorCode, cabinVm?.CabinObject))
                ?? Enumerable.Empty<string>();
        }
        public bool HasErrors { get => !ValidationResult?.IsValid ?? false; }
        public bool IsValid { get => !IsCalculating && (ValidationResult?.IsValid ?? false); }
        public double? Opening
        {
            get
            {
                if (cabinVm?.CabinObject is not null && cabinVm.CabinObject.Opening != 0)
                {
                    return cabinVm.CabinObject.Opening;
                }
                return null;
            }
        }

        public CabinCalculationsTableViewModel(CabinCalculationsService calculationsService)
        {
            this.calculationsService = calculationsService;

            //Calculations Bulk and Execute only after 400ms
            _timer.Interval = 400;
            // Calculations execute only once The timer should be restarted in order to execute again
            _timer.AutoReset = false;
            // Listen to when the timer Interval has Elapsed
            _timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// Sets the Cabin to the Table and Triggers Validation / Glass Building / Parts Building 
        /// </summary>
        /// <param name="cabinVm">The Viewmodel of the Cabin to Present Calculations for</param>
        /// /// <param name="shouldSetAndCalculate">If false it will only calculate when a Property on the ViewModel changes , otherwise it will also calculate upon setting it for the first time also</param>
        public void SetCabin(CabinViewModel? cabinVm, bool shouldSetAndCalculate = true)
        {
            //Unsubscribe from previous CabinChanged events to stop Calculations
            if (this.cabinVm is not null)
            {
                this.cabinVm.CabinChanged -= CabinVm_CabinChanged;
            }

            this.cabinVm = cabinVm;
            if (shouldSetAndCalculate) NotifyCalculationsNeeded();
            else ValidationResult = new(); //If it does not calculate for the first time it must inform at least the glasses it has from the passed ViewModel

            //Subscribe to new CabinChanged events to Perform Calculations
            if (this.cabinVm is not null)
            {
                this.cabinVm.CabinChanged += CabinVm_CabinChanged;
            }
        }

        /// <summary>
        /// Performs Calculations whenever the underlying Cabin changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CabinVm_CabinChanged(object? sender, Cabin e)
        {
            NotifyCalculationsNeeded();
        }

        public void Calculate()
        {
            if (cabinVm?.IsInSwapGlassMode is true)
            {
                //If the Cabin is in Swap Glass Mode then DO NOT CALCULATE ITS GLASSES WHEN CHANGING ANYTHING FROM THE CABIN
                //ONLY VALIDATES IT
                ValidationResult = calculationsService.BuildOnlyPartsAndValidate(cabinVm);
                Log.Information("NON GLASS Calculations Run for : {code}", cabinVm?.Code ?? "Empty-Structure");
            }
            else
            {
                ValidationResult = calculationsService.BuildGlasses(cabinVm);
                Log.Information("Calculations Run for : {code}", cabinVm?.Code ?? "Empty-Structure");
            }
        }

        private void NotifyCalculationsNeeded()
        {
            //If the Calculations timer is not enabled Enable it
            if (!_timer.Enabled)
            {
                _timer.Start();
            }
            else
            {
                //Reset the Timer if it was already notified to execute Calculations
                _timer.Stop();
                _timer.Start();
            }
        }

        /// <summary>
        /// Fired when the Interval of the System.Timers.Timer is reached (Default is 1000ms)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // Marshal the Calculation Back to the UI Thread otherwise the Glass Collections will throw exceptions when Glasses are Added to them.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Calculate();
                CalculationsPerfomed?.Invoke(this, EventArgs.Empty);
            });
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW .
        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                // Sets the Vm to Null and Unsubscribes from everything
                SetCabin(null);

                // Dispose the timer
                _timer.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

    }

}
