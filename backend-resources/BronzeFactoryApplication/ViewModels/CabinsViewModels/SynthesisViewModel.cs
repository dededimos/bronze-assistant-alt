using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using CommonHelpers;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Helpers.Custom_Exceptions;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels
{
    public partial class SynthesisViewModel : BaseViewModel, IRecipient<EditLivePartMessage>
    {
        private readonly CabinViewModelFactory cabinFactoryVM;
        private readonly OpenModalAdvancedCabinPropsService advancedCabinsModal;
        private readonly OpenLiveEditPartModalService editPartModal;
        private readonly PartSetsApplicatorService partsApplicator;
        private readonly IMessenger messenger;
        private readonly System.Timers.Timer _calculationsTimer = new();
        public event EventHandler? SynthesisCalculated;
        /// <summary>
        /// Raised whenever the Synthesis has been fully Calculated
        /// </summary>
        protected virtual void RaiseSynthesisCalculated()
        {
            SynthesisCalculated?.Invoke(this, EventArgs.Empty);
        }

        public override bool IsDisposable => false; //Has a Singletons Lifetime , will not get disposed on View Changes

        #region A. Primary 

        private CabinViewModel? primaryCabin;
        public CabinViewModel? PrimaryCabin
        {
            get => primaryCabin;
            set
            {
                if (primaryCabin != value)
                {
                    primaryCabin?.Dispose();
                    primaryCabin = value;
                    CabinCodeChangedMessage codeChangedMsg = new(primaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Primary);
                    messenger.Send(codeChangedMsg);
                    //Subscribe to Changes to Inform Accordingly The Other two Structure of Changes in this Structure
                    if (primaryCabin != null)
                    {
                        primaryCabin.PropertyChanged += PrimaryCabin_PropertyChanged;
                        primaryCabin.Parts.PartChanged += PrimaryParts_PartChanged;
                    }

                    PrimaryCalcTable.SetCabin(primaryCabin);

                    OnPropertyChanged(nameof(PrimaryCabin));
                    OnPropertyChanged(nameof(HasPrimary));
                    //OnPropertyChanged(nameof(PrimaryCalcTable));
                    //OnPropertyChanged(nameof(Opening));
                }
            }
        }

        public CabinCalculationsTableViewModel PrimaryCalcTable { get; init; }

        #endregion

        #region B. Secondary

        private CabinViewModel? secondaryCabin;
        public CabinViewModel? SecondaryCabin
        {
            get => secondaryCabin;
            set
            {
                if (secondaryCabin != value)
                {
                    secondaryCabin?.Dispose();
                    secondaryCabin = value;
                    CabinCodeChangedMessage codeChangedMsg = new(secondaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Secondary);
                    messenger.Send(codeChangedMsg);
                    //Subscribe to Changes to Inform When User Requests Same Props as the Primary Structure
                    if (secondaryCabin != null)
                    {
                        //Cabin Gets Disposed along with its event before this So its not use to unsubscrive each time ?
                        secondaryCabin.PropertyChanged += NonPrimary_PropertyChanged;
                        secondaryCabin.Parts.PartChanged += SecondaryParts_PartChanged;
                        secondaryCabin.Parts.RequestPrimaryPart += Parts_RequestPrimaryPart;
                    }
                    else //When null check that TabIndex (selected tab does not point to this model otherwise reset it to point to Primary)
                    {
                        //the tab index of secondary is 1 the tabindex of Primary is 0
                        if (SelectedTabIndex is 1) SelectedTabIndex = 0;
                    }

                    SecondaryCalcTable.SetCabin(secondaryCabin);

                    OnPropertyChanged(nameof(SecondaryCabin));
                    OnPropertyChanged(nameof(HasSecondary));
                    //OnPropertyChanged(nameof(SecondaryCalcTable));
                    //OnPropertyChanged(nameof(Opening));
                }
            }
        }

        public CabinCalculationsTableViewModel SecondaryCalcTable { get; init; }

        #endregion

        #region C. Tertiary

        private CabinViewModel? tertiaryCabin;
        public CabinViewModel? TertiaryCabin
        {
            get => tertiaryCabin;
            set
            {
                if (tertiaryCabin != value)
                {
                    tertiaryCabin?.Dispose();
                    tertiaryCabin = value;
                    CabinCodeChangedMessage codeChangedMsg = new(tertiaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Tertiary);
                    messenger.Send(codeChangedMsg);

                    //Subscribe to Changes to Inform When User Requests Same Props as the Primary Structure
                    if (tertiaryCabin != null)
                    {
                        tertiaryCabin.PropertyChanged += NonPrimary_PropertyChanged;
                        tertiaryCabin.Parts.PartChanged += TertiaryParts_PartChanged;
                        tertiaryCabin.Parts.RequestPrimaryPart += Parts_RequestPrimaryPart;
                    }
                    else //When null check that TabIndex (selected tab does not point to this model otherwise reset it to point to Primary)
                    {
                        //the tab index of tertiary is 2 the tabindex of Primary is 0
                        if (SelectedTabIndex is 2) SelectedTabIndex = 0;
                    }

                    TertiaryCalcTable.SetCabin(tertiaryCabin);

                    OnPropertyChanged(nameof(TertiaryCabin));
                    OnPropertyChanged(nameof(HasTertiary));
                    //OnPropertyChanged(nameof(TertiaryCalcTable));
                    //OnPropertyChanged(nameof(Opening));
                }
            }
        }
        public CabinCalculationsTableViewModel TertiaryCalcTable { get; init; }

        #endregion

        /// <summary>
        /// Weather any of the Tables are Still Calculating
        /// </summary>
        public bool IsCalculating { get => PrimaryCalcTable.IsCalculating || SecondaryCalcTable.IsCalculating || TertiaryCalcTable.IsCalculating; }

        public bool HasPrimary { get => PrimaryCabin is not null; }
        public bool HasSecondary { get => SecondaryCabin is not null; }
        public bool HasTertiary { get => TertiaryCabin is not null; }
        public double? Opening { get => GetSynthesis()?.Opening; }

        [ObservableProperty]
        private int selectedTabIndex;

        public SynthesisDrawViewModel Draw { get; set; }

        public void ResetViewModel()
        {
            PrimaryCabin = null;
            SecondaryCabin = null;
            TertiaryCabin = null;
        }

        public SynthesisViewModel(
            CabinViewModelFactory cabinFactoryVM,
            OpenModalAdvancedCabinPropsService advancedCabinsModal,
            Func<CabinCalculationsTableViewModel> calcTableFactory,
            OpenLiveEditPartModalService editPartModal,
            SynthesisDrawViewModel synthesisDraw,
            PartSetsApplicatorService partsApplicator,
            IMessenger messenger)
        {
            this.cabinFactoryVM = cabinFactoryVM;
            this.advancedCabinsModal = advancedCabinsModal;
            this.editPartModal = editPartModal;
            PrimaryCalcTable = calcTableFactory.Invoke();
            SecondaryCalcTable = calcTableFactory.Invoke();
            TertiaryCalcTable = calcTableFactory.Invoke();
            //Do not auto Reset Timer Start only when calculations are Received and then Stops
            _calculationsTimer.AutoReset = false;
            _calculationsTimer.Interval = 400;
            _calculationsTimer.Elapsed += _calculationsTimer_Elapsed;
            PrimaryCalcTable.CalculationsPerfomed += OnCalculationsPerfomed;
            SecondaryCalcTable.CalculationsPerfomed += OnCalculationsPerfomed;
            TertiaryCalcTable.CalculationsPerfomed += OnCalculationsPerfomed;
            Draw = synthesisDraw;
            this.partsApplicator = partsApplicator;
            this.messenger = messenger;
            messenger.RegisterAll(this);
        }

        /// <summary>
        /// Opens Edit Part Dialog when received
        /// </summary>
        /// <param name="message"></param>
        public void Receive(EditLivePartMessage message)
        {
            //Open the Edit only if the message comes from any of the Current PartsViewModels , otherwise its another channel of the Application
            if (message.Sender == PrimaryCabin?.Parts || message.Sender == SecondaryCabin?.Parts || message.Sender == TertiaryCabin?.Parts)
            {
                //Open the Modal
                editPartModal.OpenModal(message.PartToEdit, message.SpotToEdit, message.Sender);
            }
        }

        /// <summary>
        /// Executes whenever a Property changes in Primary , to Pass Changes to The other two models
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrimaryCabin_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Primary is never null here (the Event cannot be fired otherwise)

            switch (e.PropertyName)
            {
                case nameof(CabinViewModel.Code):
                    CabinCodeChangedMessage codeChangedMsg = new(PrimaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Primary);
                    messenger.Send(codeChangedMsg);
                    break;
                //Set the Input Height of the other two Structures
                case nameof(CabinViewModel.InputHeight):
                    if (secondaryCabin is not null && secondaryCabin.IsInputHeightAsPrimary)
                        secondaryCabin.InputHeight = primaryCabin!.InputHeight;
                    if (tertiaryCabin is not null && tertiaryCabin.IsInputHeightAsPrimary)
                        tertiaryCabin.InputHeight = primaryCabin!.InputHeight;
                    break;
                //Set the GlassFinish of the other two Structures
                case nameof(CabinViewModel.GlassFinish):
                    if (secondaryCabin is not null && secondaryCabin.IsGlassFinishAsPrimary)
                        secondaryCabin.GlassFinish = primaryCabin!.GlassFinish;
                    if (tertiaryCabin is not null && tertiaryCabin.IsGlassFinishAsPrimary)
                        tertiaryCabin.GlassFinish = primaryCabin!.GlassFinish;
                    break;
                //Set the Thicknesses of the other two Structures
                case nameof(CabinViewModel.Thicknesses):
                    if (secondaryCabin is not null && secondaryCabin.IsThicknessesAsPrimary)
                        secondaryCabin.Thicknesses = primaryCabin!.Thicknesses;
                    if (tertiaryCabin is not null && tertiaryCabin.IsThicknessesAsPrimary)
                        tertiaryCabin.Thicknesses = primaryCabin!.Thicknesses;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Executes Whenever secondary or Tertiary change their SameWithPrimary Setting
        /// So to Apply the primary's Value or their own custom value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NonPrimary_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CabinViewModel.Code):
                    if (sender == SecondaryCabin)
                    {
                        CabinCodeChangedMessage codeChangedMsgSecondary = new(SecondaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Secondary);
                        messenger.Send(codeChangedMsgSecondary);
                    }
                    else
                    {
                        CabinCodeChangedMessage codeChangedMsgTertiary = new(TertiaryCabin?.Code ?? string.Empty, CabinSynthesisModel.Tertiary);
                        messenger.Send(codeChangedMsgTertiary);
                    }
                    break;
                case nameof(CabinViewModel.IsInputHeightAsPrimary):
                    if (secondaryCabin is not null && secondaryCabin.IsInputHeightAsPrimary)
                        secondaryCabin.InputHeight = primaryCabin?.InputHeight;
                    if (tertiaryCabin is not null && tertiaryCabin.IsInputHeightAsPrimary)
                        tertiaryCabin.InputHeight = primaryCabin?.InputHeight;
                    break;
                case nameof(CabinViewModel.IsGlassFinishAsPrimary):
                    if (secondaryCabin is not null && secondaryCabin.IsGlassFinishAsPrimary)
                        secondaryCabin.GlassFinish = primaryCabin?.GlassFinish;
                    if (tertiaryCabin is not null && tertiaryCabin.IsGlassFinishAsPrimary)
                        tertiaryCabin.GlassFinish = primaryCabin?.GlassFinish;
                    break;
                case nameof(CabinViewModel.IsThicknessesAsPrimary):
                    if (secondaryCabin is not null && secondaryCabin.IsThicknessesAsPrimary)
                        secondaryCabin.Thicknesses = primaryCabin?.Thicknesses;
                    if (tertiaryCabin is not null && tertiaryCabin.IsThicknessesAsPrimary)
                        tertiaryCabin.Thicknesses = primaryCabin?.Thicknesses;
                    break;
                default:
                    break;
            }
        }

        #region ----OTHER EVENTS HANDLERS

        /// <summary>
        /// Changes the Handle to the One Of the Primary under certain conditions
        /// Calls the PartSet Applicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PrimaryParts_PartChanged(object? sender, PartChangedEventsArgs e)
        {
            //ONLY IMPLEMENTED FOR 9A MODEL CURRENTLY - NEED TO FIND A GENERIC IMPLEMENTATION FOR ALL MODELS AND ALL PARTS
            if (e.Spot is PartSpot.Handle1)
            {
                if (secondaryCabin is not null && secondaryCabin is Cabin9AViewModel vm && vm.Parts.IsHandleAsPrimary)
                {
                    vm.Parts.Handle = e.NewPart?.GetDeepClone() as CabinHandle ?? null;
                }
            }
            //Delegate the Set Part Changes to the Applicator
            if (e.NewPart is not null
                && PrimaryCabin is not null
                && partsApplicator.ShouldApplySetChange(e.Identifier, e.NewPart.Code))
            {
                var partsList = PrimaryCabin.Parts?.GetPartsObject();
                if (partsList != null)
                {
                    var spotsApplied = partsApplicator.TryApplySetChange(partsList, e.Spot, e.NewPart.Code, e.Identifier);
                    foreach (var spot in spotsApplied)
                    {
                        PrimaryCabin.Parts?.InformSpotPartChanged(spot);
                    }
                }
            }
        }

        private void SecondaryParts_PartChanged(object? sender, PartChangedEventsArgs e)
        {
            //Delegate the Set Part Changes to the Applicator
            if (e.NewPart is not null
                && SecondaryCabin is not null
                && partsApplicator.ShouldApplySetChange(e.Identifier, e.NewPart.Code))
            {
                var partsList = SecondaryCabin.Parts?.GetPartsObject();
                if (partsList != null)
                {
                    var spotsApplied = partsApplicator.TryApplySetChange(partsList, e.Spot, e.NewPart.Code, e.Identifier);
                    foreach (var spot in spotsApplied)
                    {
                        SecondaryCabin.Parts?.InformSpotPartChanged(spot);
                    }
                }
            }
        }

        private void TertiaryParts_PartChanged(object? sender, PartChangedEventsArgs e)
        {
            //Delegate the Set Part Changes to the Applicator
            if (e.NewPart is not null
                && TertiaryCabin is not null
                && partsApplicator.ShouldApplySetChange(e.Identifier, e.NewPart.Code))
            {
                var partsList = TertiaryCabin.Parts?.GetPartsObject();
                if (partsList != null)
                {
                    var spotsApplied = partsApplicator.TryApplySetChange(partsList, e.Spot, e.NewPart.Code, e.Identifier);
                    foreach (var spot in spotsApplied)
                    {
                        TertiaryCabin.Parts?.InformSpotPartChanged(spot);
                    }
                }

            }
        }

        /// <summary>
        /// Set a request for Primary Part into the NonPrimary Models
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Parts_RequestPrimaryPart(object? sender, RequestPrimaryPartArgs e)
        {
            if (e.RequestSpot is PartSpot.Handle1 && primaryCabin is not null && primaryCabin.Parts is IHandle primaryHandle)
            {
                switch (e.SynthesisModel)
                {
                    case CabinSynthesisModel.Secondary:
                        if (secondaryCabin is not null && secondaryCabin.Parts is IHandle secondaryHandle)
                        {
                            secondaryHandle.Handle = primaryHandle.Handle.GetDeepClone();
                        }
                        break;
                    case CabinSynthesisModel.Tertiary:
                        if (tertiaryCabin is not null && tertiaryCabin.Parts is IHandle tertiaryHandle)
                        {
                            tertiaryHandle.Handle = primaryHandle.Handle.GetDeepClone();
                        }
                        break;
                    case CabinSynthesisModel.Primary:
                    default:
                        return;
                }
            }
        }


        /// <summary>
        /// Start the Timer Whenever Calculations Have been Performed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalculationsPerfomed(object? sender, EventArgs e)
        {
            //If the Calculations timer is not enabled Enable it
            if (!_calculationsTimer.Enabled)
            {
                _calculationsTimer.Start();
            }
            else
            {
                //Reset the Timer if it was already notified to execute Calculations
                _calculationsTimer.Stop();
                _calculationsTimer.Start();
            }
        }
        /// <summary>
        /// Draw Whenever the Calculations timer Expires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _calculationsTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            //When Calculations have performed and a small interval has passed (to give time to all possible three calculations to finish)
            //Try Generate the Draw and Get the Total Synthesis Opening As soon as Calcs have Finished
            OnPropertyChanged(nameof(Opening));
            TryGenerateDraw();
            RaiseSynthesisCalculated();
        }

        #endregion

        [RelayCommand]
        private void OpenAdvancedModal(CabinViewModel cabinVm)
        {
            advancedCabinsModal.OpenModal(cabinVm);
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
                messenger.UnregisterAll(this);
                _calculationsTimer.Elapsed -= _calculationsTimer_Elapsed;
                PrimaryCalcTable.CalculationsPerfomed -= OnCalculationsPerfomed;
                SecondaryCalcTable.CalculationsPerfomed -= OnCalculationsPerfomed;
                TertiaryCalcTable.CalculationsPerfomed -= OnCalculationsPerfomed;
                if (primaryCabin is not null)
                {
                    primaryCabin.PropertyChanged -= PrimaryCabin_PropertyChanged;
                    primaryCabin.Parts.PartChanged -= PrimaryParts_PartChanged;
                }
                if (secondaryCabin is not null)
                {
                    secondaryCabin.PropertyChanged -= NonPrimary_PropertyChanged;
                    secondaryCabin.Parts.PartChanged -= SecondaryParts_PartChanged;
                    secondaryCabin.Parts.RequestPrimaryPart -= Parts_RequestPrimaryPart;
                }
                if (tertiaryCabin is not null)
                {
                    tertiaryCabin.PropertyChanged -= NonPrimary_PropertyChanged;
                    tertiaryCabin.Parts.PartChanged -= TertiaryParts_PartChanged;
                    tertiaryCabin.Parts.RequestPrimaryPart -= Parts_RequestPrimaryPart;
                }
                primaryCabin?.Dispose();
                secondaryCabin?.Dispose();
                tertiaryCabin?.Dispose();
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Passes a synthesis to the ViewModel
        /// </summary>
        /// <param name="synthesis">The Synthesis to Select</param>
        /// <param name="swap">The Swap that has been done to one of the Cabins</param>
        public void SelectSynthesis(CabinSynthesis synthesis, GlassSwap? swap = null)
        {
            //Create the New ViewModels
            var newPrimaryCabin = synthesis.Primary is not null ? cabinFactoryVM.Create(synthesis.Primary) : null;
            var newSecondaryCabin = synthesis.Secondary is not null ? cabinFactoryVM.Create(synthesis.Secondary) : null;
            var newTertiaryCabin = synthesis.Tertiary is not null ? cabinFactoryVM.Create(synthesis.Tertiary) : null;

            //Check if its in swap Glass Mode set it so Glasses are not Recalculated
            if (swap is not null)
            {
                if (newPrimaryCabin is not null && newPrimaryCabin.Glasses.Any(g=> g.Equals(swap.NewGlass)))
                {
                    newPrimaryCabin.GlassSwap = swap;
                }
                else if (newSecondaryCabin is not null && newSecondaryCabin.Glasses.Any(g => g.Equals(swap.NewGlass)))
                {
                    newSecondaryCabin.GlassSwap = swap;
                }
                else if (newTertiaryCabin is not null && newTertiaryCabin.Glasses.Any(g => g.Equals(swap.NewGlass)))
                {
                    newTertiaryCabin.GlassSwap = swap;
                }
            }
            
            //Pass the New ViewModels to the Synthesis ViewModel
            PrimaryCabin = newPrimaryCabin;
            SecondaryCabin = newSecondaryCabin;
            TertiaryCabin = newTertiaryCabin;

            //Manually Call Property Change for the Values different than Primary . User will almost never type this on purpose
            //This way the Height - Glass - Finish - Thickness will be as for the Primary
            //If he like so ,he can manually change the values afterwards
            NonPrimary_PropertyChanged(this, new(nameof(CabinViewModel.IsInputHeightAsPrimary)));
            NonPrimary_PropertyChanged(this, new(nameof(CabinViewModel.IsGlassFinishAsPrimary)));
            NonPrimary_PropertyChanged(this, new(nameof(CabinViewModel.IsThicknessesAsPrimary)));
        }

        /// <summary>
        /// Returns a NEW Synthesis that fully represents the Current State of this ViewModel
        /// </summary>
        /// <returns></returns>
        public CabinSynthesis? GetSynthesis()
        {
            //Primary - Secondary and Tertiary Change one By One , so it will try to create Synthesis that do not exist also in between that it will be
            //changing the properties of Primary-Secondary-Tertiary , until all change 
            //So in that case we will have moments that Draw of Primary will be different from those in Secondary and Tertiary

            try
            {
                //Primary is never Null when it HasPrimary
                return HasPrimary
                    ? CabinFactory.CreateSynthesis(
                        primaryCabin?.CabinObject!.GetDeepClone(),
                        secondaryCabin?.CabinObject?.GetDeepClone(),
                        tertiaryCabin?.CabinObject?.GetDeepClone())
                    : null;
            }
            catch (InvalidSynthesisDrawsException ex)
            {
                Log.Warning(ex.Message);
                return null;
            }


        }

        /// <summary>
        /// Checks weather there are any Errors in the Synthesis Selections
        /// </summary>
        /// <returns></returns>
        public bool HasErrors()
        {
            var hasErrorsList = new List<bool>();
            hasErrorsList.AddIf(HasPrimary, !PrimaryCalcTable.IsValid);
            hasErrorsList.AddIf(HasSecondary, !SecondaryCalcTable.IsValid);
            hasErrorsList.AddIf(HasTertiary, !TertiaryCalcTable.IsValid);

            //If at least one has errors return true , or if there are no cabins in the list (if no bools are added at all then there are not cabins and should return true)
            return hasErrorsList.Any(hasError => hasError == true) || (hasErrorsList.Any() == false);
        }

        /// <summary>
        /// Returns all The Errors for the Synthesis
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetErrors()
        {
            List<string> validationErrors = new();
            validationErrors.AddIf(HasPrimary && PrimaryCalcTable.HasErrors, PrimaryCalcTable.ValidationErrors);
            validationErrors.AddIf(HasSecondary && SecondaryCalcTable.HasErrors, SecondaryCalcTable.ValidationErrors);
            validationErrors.AddIf(HasTertiary && TertiaryCalcTable.HasErrors, TertiaryCalcTable.ValidationErrors);
            if (HasPrimary is false)
            {
                validationErrors.Add("lngEmptyCabin".TryTranslateKey());
            }
            return validationErrors;
        }

        private void TryGenerateDraw()
        {
            if (HasErrors())
            {
                Draw.SetSynthesis(null);
            }
            else
            {
                var synthesis = GetSynthesis();
                if (synthesis != null) Draw.SetSynthesis(synthesis);
            }
        }

        /// <summary>
        /// Returns the List of the Current ViewModels of the Cabins represented by this Synthesis ViewModel
        /// </summary>
        /// <returns></returns>
        public List<CabinViewModel> GetCabinViewModels()
        {
            List<CabinViewModel> vms = new();
            vms.AddNotNull(PrimaryCabin!);
            vms.AddNotNull(SecondaryCabin!);
            vms.AddNotNull(TertiaryCabin!);
            return vms;
        }

        /// <summary>
        ///Returns the First Found Swap that is not Null , otherwise Null (For the Current Synthesis Viewmodel by searching inside the Cabin Vms)
        /// </summary>
        /// <returns></returns>
        public GlassSwap? GetGlassSwap()
        {
            return GetCabinViewModels().Select(vm => vm.GlassSwap).FirstOrDefault(swap => swap is not null);
        }

    }
}
