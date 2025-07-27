using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.ViewModels.SettingsViewModels;
using CommonHelpers;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels
{
    /// <summary>
    /// A ViewModel Taking Care of all the Glass Matches
    /// </summary>
    public partial class CabinsGlassMatchesViewModel : BaseViewModel, IRecipient<SynthesisSwapMessage>
    {
        private readonly GlassMatchingService matchingService;
        private readonly IMessenger messenger;
        private readonly OpenSelectSwapOptionsModalService openSelectSwapOptionsService;

        public GlassesStockSettingsViewModel PrimaryMatchSettings { get; }
        public GlassesStockSettingsViewModel SecondaryMatchSettings { get; }
        public GlassesStockSettingsViewModel TertiaryMatchSettings { get; }

        public GlassesStockSettingsViewModel? SelectedSettingsVm
        {
            get
            {
                if (SelectedCabinVm?.SynthesisModel is CabinSynthesisModel.Primary) return PrimaryMatchSettings;
                else if (SelectedCabinVm?.SynthesisModel is CabinSynthesisModel.Secondary) return SecondaryMatchSettings;
                else if (SelectedCabinVm?.SynthesisModel is CabinSynthesisModel.Tertiary) return TertiaryMatchSettings;
                return null;
            }
        }

        /// <summary>
        /// The Current Made Synthesis of the User
        /// </summary>
        [ObservableProperty]
        private SynthesisViewModel synthesisVm;

        /// <summary>
        /// The List of ViewModels of the Cabin Synthesis
        /// </summary>
        public List<CabinViewModel> CabinsList { get => SynthesisVm.GetCabinViewModels(); }

        /// <summary>
        /// The Selected Cabin ViewModel
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedCabinVmGlasses))]
        [NotifyPropertyChangedFor(nameof(SelectedSettingsVm))]
        private CabinViewModel? selectedCabinVm;

        /// <summary>
        /// The Glasses of the Selected Cabin ViewModel
        /// </summary>
        public List<Glass> SelectedCabinVmGlasses { get => SelectedCabinVm?.Glasses ?? new(); }

        /// <summary>
        /// The Synthesis after the Swapping of a Glass
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SwappedCabinsList))]
        private CabinSynthesis? swappedSynthesis;

        [ObservableProperty]
        private GlassSwap? currentSwap;

        public List<Cabin> SwappedCabinsList
        {
            get
            {
                Log.Information("Read SwappedCabins List");
                return SwappedSynthesis?.GetCabinList() ?? new();
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMatches))]
        private ObservableCollection<GlassMatchesViewModel> matches = new();

        public bool HasMatches { get => Matches.Any(); }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedMatch))]
        [NotifyPropertyChangedFor(nameof(SelectedStock))]
        private Glass? selectedGlass;

        public GlassMatchesViewModel? SelectedMatch { get => Matches.FirstOrDefault(m => m.ConcerningGlass.Equals(SelectedGlass)); }

        [ObservableProperty]
        private StockedGlassViewModel? selectedStock;

        public CabinsGlassMatchesViewModel(Func<GlassMatchingService> matchingServiceFactory,
            SynthesisViewModel synthesisVm,
            IMessenger messenger,
            OpenSelectSwapOptionsModalService openSelectSwapOptionsService,
            Func<GlassesStockSettingsViewModel> matchSettingsVmFactory)
        {
            this.matchingService = matchingServiceFactory.Invoke();
            matchingService.MatchingStarted += MatchingService_MatchingStarted;
            matchingService.MatchingFinished += MatchingService_MatchingFinished;
            synthesisVm.SynthesisCalculated += SynthesisVm_SynthesisCalculated;
            SynthesisVm = synthesisVm;
            this.openSelectSwapOptionsService = openSelectSwapOptionsService;

            PrimaryMatchSettings = matchSettingsVmFactory.Invoke();
            SecondaryMatchSettings = matchSettingsVmFactory.Invoke();
            TertiaryMatchSettings = matchSettingsVmFactory.Invoke();

            this.messenger = messenger;
            messenger.RegisterAll(this);
        }

        /// <summary>
        /// Executes Matches when a Synthesis has been Calculated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SynthesisVm_SynthesisCalculated(object? sender, EventArgs e)
        {
            //The Vm might have Finished Calculations but if they have started again it means the Cabins have also changed so
            //we must check if it is idle to start the matcher with the correct Cabins
            if (!SynthesisVm.IsCalculating)
            {
                //Find any matches
                this.MatchGlasses(SynthesisVm.PrimaryCabin?.CabinObject, SynthesisVm.SecondaryCabin?.CabinObject, SynthesisVm.TertiaryCabin?.CabinObject);
            }
        }

        /// <summary>
        /// Informs when the Matching Service has Finished Matching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MatchingService_MatchingFinished(object? sender, EventArgs e)
        {
            IsBusy = false;
        }
        /// <summary>
        /// Informs when the Matching Service has Started Matching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MatchingService_MatchingStarted(object? sender, EventArgs e)
        {
            IsBusy = true;
        }
        /// <summary>
        /// Matches Glasses for the Cabins of the Synthesis whenever the Synthesis Finishes Calculations
        /// </summary>
        /// <param name="cabinsToMatch"></param>
        public void MatchGlasses(Cabin? primary, Cabin? secondary, Cabin? tertiary, bool selectFirstAvailable = true)
        {
            //Fire and Forget MatchStockGlasses(); if any errors are thrown marshal them back by throwing again their exception once the task completes as faulted
            Task.Run(async () =>
            {
                //Get the Options First
                await PrimaryMatchSettings.SetModelSettings(SynthesisVm.PrimaryCabin?.Model);
                await SecondaryMatchSettings.SetModelSettings(SynthesisVm.SecondaryCabin?.Model);
                await TertiaryMatchSettings.SetModelSettings(SynthesisVm.TertiaryCabin?.Model);

                var foundMatches = await matchingService.MatchCabinsGlassesFromStock((primary, PrimaryMatchSettings.SessionSettings), (secondary, SecondaryMatchSettings.SessionSettings), (tertiary, TertiaryMatchSettings.SessionSettings));
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Matches.Clear();
                    foreach (var match in foundMatches)
                    {
                        Matches.Add(new(match));
                    }
                    OnPropertyChanged(nameof(HasMatches));
                    
                    if (selectFirstAvailable)
                    {
                        FindAndSelectFirstMatch();
                    }
                    //find the First Cabin and First Glass that Has Matches and Select it
                });

            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    //Throw the inner exception of the Aggragate exception (Tasks always wrap their exceptions into aggragate exceptions and store the exception into the inner exception property)
                    var ex = t.Exception?.InnerException ?? new Exception("Unknown Exception - No Aggragate was found...");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageService.LogAndDisplayException(ex);
                    });
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /// <summary>
        /// Whenever Matches is Executed this Finds and Selects the First Cabin that has Viable Glass Matches
        /// </summary>
        private void FindAndSelectFirstMatch()
        {
            if (HasMatches)
            {
                //If there are Matches select the first found matching glass and Cabin
                foreach (var cabin in CabinsList)
                {
                    foreach (var glass in cabin.Glasses)
                    {
                        if (Matches.FirstOrDefault(m => m.ConcerningGlass.Equals(glass)) is not null)
                        {
                            SelectedCabinVm = cabin;
                            SelectedGlass = glass;
                            break;
                        }
                    }
                }
            }
            else
            {
                SelectedCabinVm = null;
                SelectedGlass = null;
            }
        }

        /// <summary>
        /// Swaps a Glass with one from the Stock Manipulating the Synthesis according to the Selected Swap Options
        /// </summary>
        /// <param name="glassToSwapVm">The Selected Swap</param>
        [RelayCommand]
        private void SwapGlass(StockedGlassViewModel glassToSwapVm)
        {
            if (glassToSwapVm is null)
            {
                MessageService.Warning("Glass to Swap is null", "Glass to Swap not Selected");
                return;
            }
            if (SelectedCabinVm is null || SelectedCabinVm.SynthesisModel is null)
            {
                MessageService.Warning("Selected Cabin is Null", "Null Cabin in Matcher");
                return;
            }
            if (SelectedGlass is null)
            {
                MessageService.Warning("Selected Glass is Null", "Null Glass in Matcher");
                return;
            }
            var glassToSwap = glassToSwapVm.Glass.GetGlass();
            var indexOfGlass = SelectedCabinVmGlasses.IndexOf(SelectedGlass);
            GlassSwap swap = new(indexOfGlass, glassToSwap);

            var synthesis = SynthesisVm.GetSynthesis() ?? throw new Exception("Synthesis for Swapping is Null");
            openSelectSwapOptionsService.OpenModal(synthesis, (CabinSynthesisModel)SelectedCabinVm.SynthesisModel, swap);
        }

        [RelayCommand]
        private void RedoGlassMatching()
        {
            if (!SynthesisVm.IsCalculating && IsNotBusy)
            {
                //Find any matches
                this.MatchGlasses(
                    SynthesisVm.PrimaryCabin?.CabinObject, 
                    SynthesisVm.SecondaryCabin?.CabinObject, 
                    SynthesisVm.TertiaryCabin?.CabinObject,
                    false);
            }
            else
            {
                MessageService.Info("Please try again in a second , Calculations are still being run", "lngFailure");
            }
        }

        /// <summary>
        /// Confirms the Swapping by Changing the Synthesis in the Synthesis ViewModel
        /// </summary>
        public void ConfirmSwaps()
        {
            if (SwappedSynthesis is not null)
            {
                SynthesisVm.SelectSynthesis(SwappedSynthesis, CurrentSwap);
            }
        }

        public void ScrapSwaps()
        {
            SwappedSynthesis = null;
            CurrentSwap = null;
        }

        public void Receive(SynthesisSwapMessage message)
        {
            // Get the Swapped Synthesis from the Message
            SwappedSynthesis = message.NewSynthesis;
            CurrentSwap = message.Swap;
        }

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
                matchingService.MatchingStarted -= MatchingService_MatchingStarted;
                matchingService.MatchingFinished -= MatchingService_MatchingFinished;
                SynthesisVm.SynthesisCalculated -= SynthesisVm_SynthesisCalculated;
                matchingService.Dispose();
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

    }
}
