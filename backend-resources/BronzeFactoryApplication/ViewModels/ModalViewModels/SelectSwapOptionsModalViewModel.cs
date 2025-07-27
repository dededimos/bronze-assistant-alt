using BronzeFactoryApplication.ApplicationServices.MessangerService;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using EnumsNET;
using SVGGlassDrawsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class SelectSwapOptionsModalViewModel : ModalViewModel
    {
        private readonly CloseModalService closeModalService;
        private readonly CabinGlassSwapper swapper;
        private readonly IMessenger messenger;
        private CabinSynthesis synthesisToSwap = new();
        private GlassSwap swap = GlassSwap.Empty();

        private bool IsNothingSelected 
        {
            get =>
                !isSwapAndAdjustLengthsInCabinSelected &&
                !isSwapAndKeepGlassesLengthsSelected &&
                !isSwapAndAdjustLengthsInStructureSelected &&
                !isOnlySwapSelected;
        }

        /// <summary>
        /// The New Synthesis after executing a Swap - This is passed to the Consumers of this Modal
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CabinsOfSwappedSynthesis))]
        private CabinSynthesis? swappedSynthesis;

        public List<Cabin> CabinsOfSynthesisToSwap { get => synthesisToSwap.GetCabinList(); }
        public List<Cabin> CabinsOfSwappedSynthesis { get => SwappedSynthesis?.GetCabinList() ?? new(); }

        private bool isSwapAndAdjustLengthsInCabinSelected;
        public bool IsSwapAndAdjustLengthsInCabinSelected
        {
            get => isSwapAndAdjustLengthsInCabinSelected;
            set
            {
                if (value != isSwapAndAdjustLengthsInCabinSelected)
                {
                    isSwapAndAdjustLengthsInCabinSelected = value;
                    OnPropertyChanged(nameof(IsSwapAndAdjustLengthsInCabinSelected));
                    if (isSwapAndAdjustLengthsInCabinSelected)
                    {
                        //The others must be false;
                        IsOnlySwapSelected = false;
                        IsSwapAndKeepGlassesLengthsSelected = false;
                        IsSwapAndAdjustLengthsInStructureSelected = false;
                        SetSwappedSynthesis();
                    }
                    else if (IsNothingSelected) SetSwappedSynthesis();
                }
            }
        }
        [ObservableProperty]
        private bool isSwapAndAdjustLengthsInCabinValid;

        private bool isSwapAndKeepGlassesLengthsSelected;
        public bool IsSwapAndKeepGlassesLengthsSelected
        {
            get => isSwapAndKeepGlassesLengthsSelected;
            set
            {
                if (value != isSwapAndKeepGlassesLengthsSelected)
                {
                    isSwapAndKeepGlassesLengthsSelected = value;
                    OnPropertyChanged(nameof(IsSwapAndKeepGlassesLengthsSelected));
                    if (isSwapAndKeepGlassesLengthsSelected)
                    {
                        //All the Rest are always false otherwise
                        IsOnlySwapSelected = false;
                        IsSwapAndAdjustLengthsInCabinSelected = false;
                        IsSwapAndAdjustLengthsInStructureSelected = false;
                        SetSwappedSynthesis();
                    }
                    else if (IsNothingSelected) SetSwappedSynthesis();
                }
            }
        }
        [ObservableProperty]
        private bool isSwapAndKeepGlassesLengthsValid;


        private bool isSwapAndAdjustLengthsInStructureSelected;
        public bool IsSwapAndAdjustLengthsInStructureSelected
        {
            get => isSwapAndAdjustLengthsInStructureSelected;
            set
            {
                if (value != isSwapAndAdjustLengthsInStructureSelected)
                {
                    isSwapAndAdjustLengthsInStructureSelected = value;
                    OnPropertyChanged(nameof(IsSwapAndAdjustLengthsInStructureSelected));
                    if (isSwapAndAdjustLengthsInStructureSelected)
                    {
                        //All the Rest are always false otherwise
                        IsOnlySwapSelected = false;
                        IsSwapAndAdjustLengthsInCabinSelected = false;
                        IsSwapAndKeepGlassesLengthsSelected = false;
                        SetSwappedSynthesis();
                    }
                    else if (IsNothingSelected) SetSwappedSynthesis();
                }
            }
        }
        [ObservableProperty]
        private bool isSwapAndAdjustLengthsInStructureValid;

        private bool isOnlySwapSelected;
        public bool IsOnlySwapSelected
        {
            get => isOnlySwapSelected;
            set
            {
                if (value != isOnlySwapSelected)
                {
                    isOnlySwapSelected = value;
                    OnPropertyChanged(nameof(IsOnlySwapSelected));
                    if (isOnlySwapSelected)
                    {
                        //All the Rest are always false otherwise
                        IsSwapAndAdjustLengthsInCabinSelected = false;
                        IsSwapAndKeepGlassesLengthsSelected = false;
                        IsSwapAndAdjustLengthsInStructureSelected = false;
                        SetSwappedSynthesis();
                    }
                    else if (IsNothingSelected) SetSwappedSynthesis();
                }
            }
        }
        [ObservableProperty]
        private bool isOnlySwapValid;

        public SelectSwapOptionsModalViewModel(CloseModalService closeModalService, CabinGlassSwapper swapper, IMessenger messenger)
        {
            Title = "lngExchange".TryTranslateKey();
            this.closeModalService = closeModalService;
            this.swapper = swapper;
            this.messenger = messenger;
        }

        public void ConfigureSwapProperties(CabinSynthesis synthesisToSwap, CabinSynthesisModel modelUnderSwap, GlassSwap glassSwap)
        {
            this.synthesisToSwap = synthesisToSwap;
            OnPropertyChanged(nameof(CabinsOfSynthesisToSwap));
            swap = glassSwap;

            // Configure the Swapper
            swapper.SetItemsToSwap(synthesisToSwap, modelUnderSwap, swap);

            // Get Which Options are Available for Selection
            SwapOption availableSwapOptions = swapper.GetSwapOptions();

            IsSwapAndAdjustLengthsInCabinValid = availableSwapOptions.HasFlag(SwapOption.SwapAdjustLengthHeightGlassesCabin);
            IsSwapAndAdjustLengthsInStructureValid = availableSwapOptions.HasFlag(SwapOption.SwapAndAdjustLengthsInStructure);
            IsSwapAndKeepGlassesLengthsValid = availableSwapOptions.HasFlag(SwapOption.SwapKeepGlassesChangeCabinLength);
            IsOnlySwapValid = availableSwapOptions.HasFlag(SwapOption.SwapOnly);

            // Select as Default the first of the Above options that is available
            if (IsSwapAndAdjustLengthsInCabinValid)
            {
                IsSwapAndAdjustLengthsInCabinSelected = true;
            }
            else if (IsSwapAndAdjustLengthsInStructureValid)
            {
                IsSwapAndAdjustLengthsInStructureSelected = true;
            }
            else if (IsSwapAndKeepGlassesLengthsValid)
            {
                IsSwapAndKeepGlassesLengthsSelected = true;
            }
            else if (IsOnlySwapValid)
            {
                IsOnlySwapSelected = true;
            }
        }

        [RelayCommand]
        private void ExchangeGlass()
        {
            messenger.Send(new SynthesisSwapMessage(synthesisToSwap , SwappedSynthesis,swap.GetDeepClone()));
            closeModalService.CloseModal(this);
        }

        [RelayCommand]
        private void CancelSwap()
        {
            IsOnlySwapSelected = false;
            IsSwapAndAdjustLengthsInCabinSelected = false;
            IsSwapAndAdjustLengthsInStructureSelected = false;
            IsSwapAndKeepGlassesLengthsSelected = false;
            // Do not send any message just Close the Modal of Options
            closeModalService.CloseModal(this);
        }

        /// <summary>
        /// Runs whenever An Option is Selected
        /// </summary>
        private void SetSwappedSynthesis()
        {
            // Get the Selected Options
            SwapOption options = GetSwapOptionSelection();
            SwappedSynthesis = swapper.ExecuteSwap(options);
        }

        /// <summary>
        /// Combines the Flags of the Selected Options
        /// </summary>
        /// <returns></returns>
        private SwapOption GetSwapOptionSelection()
        {
            SwapOption option = SwapOption.DoNotSwap;

            if (IsSwapAndAdjustLengthsInCabinSelected)
            {
                option = option.CombineFlags(SwapOption.SwapAdjustLengthHeightGlassesCabin);
            }
            else if (IsSwapAndKeepGlassesLengthsSelected)
            {
                option = option.CombineFlags(SwapOption.SwapKeepGlassesChangeCabinLength);
            }
            else if (IsSwapAndAdjustLengthsInStructureSelected)
            {
                option = option.CombineFlags(SwapOption.SwapAndAdjustLengthsInStructure);
            }
            else if (IsOnlySwapSelected)
            {
                option = option.CombineFlags(SwapOption.SwapOnly);
            }

            return option;
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
