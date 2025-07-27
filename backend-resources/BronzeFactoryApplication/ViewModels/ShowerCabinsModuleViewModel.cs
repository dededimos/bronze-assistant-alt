

using Azure;
using BronzeFactoryApplication.ApplicationServices.DialogService;
using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.Helpers.Behaviours;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.Views.ComponentsUC.DrawsRelevantUcs;
using DataAccessLib;
using FluentValidation.Results;
using GlassesOrdersModels.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using SharpCompress.Common;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class ShowerCabinsModuleViewModel : BaseViewModel,
        IRecipient<GlassesOrderSelectedMessage>, IRecipient<AddCabinRowsMessage>
    {
        private readonly Func<GlassViewModel> glassVmFactory;
        private readonly OpenAddSynthesisToOrderModalService openAddToOrderModalService;
        private readonly OpenRetrieveOrdersModalService openRetrieveOrdersModalService;
        private readonly OpenXlsSettingsSelectionModalService openXlsSettingsService;
        private readonly OpenImportOrderModalService openImportOrdersService;
        private readonly OpenGlassesStockModalService openGlassesStockModalService;
        private readonly OpenGlassMatchingModalService openGlassMatchingModalService;
        private readonly OpenPrintPreviewGlassModalService openGlassPrintPreviewService;
        private readonly IXlsSettingsProvider settingsProvider;
        private readonly IMessenger messenger;
        private readonly ValidatorGlass glassValidator = new();

        public override bool IsDisposable => false; //Has a Singletons Lifetime , will not get disposed on View Changes
        public ChooseCabinModelViewModel ChooseModelVm { get; set; }
        public SynthesisViewModel SynthesisVm { get; set; }
        public GlassViewModel SingleGlass { get; set; }
        public GlassDrawViewModel GlassDraw { get; set; }
        public CabinsGlassMatchesViewModel GlassMatchingVm { get; private set; }

        /// <summary>
        /// Weather the Addition to Order is Done only for a Single Glass
        /// </summary>
        [ObservableProperty]
        private bool useGlassOnly = false;

        /// <summary>
        /// Weather it should Match Glasses Automatically
        /// </summary>
        [ObservableProperty]
        private bool isMatchingStockedGlasses = true;

        /// <summary>
        /// The Current Glasses Order
        /// </summary>
        [ObservableProperty]
        private GlassesOrderViewModel glassesOrder;

        /// <summary>
        /// The PA0 Number that will be used for the Next Added Structure
        /// </summary>
        [ObservableProperty]
        private string nextStructurePA0Number = string.Empty;

        public ShowerCabinsModuleViewModel(
            SynthesisViewModel synthesisVm,
            ChooseCabinModelViewModel chooseModelVm,
            Func<GlassesOrderViewModel> glassesOrderVmFactory,
            Func<GlassViewModel> glassVmFactory,
            Func<CabinsGlassMatchesViewModel> glassMatchingVmFactory,
            OpenAddSynthesisToOrderModalService openAddToOrderModalService,
            OpenRetrieveOrdersModalService openRetrieveOrdersModalService,
            OpenXlsSettingsSelectionModalService openXlsSettingsService,
            OpenImportOrderModalService openImportOrdersService,
            OpenGlassesStockModalService openGlassesStockModalService,
            OpenGlassMatchingModalService openGlassMatchingModalService,
            OpenPrintPreviewGlassModalService openGlassPrintPreviewService,
            IXlsSettingsProvider settingsProvider,
            IMessenger messenger)
        {
            SynthesisVm = synthesisVm;
            ChooseModelVm = chooseModelVm;
            this.glassVmFactory = glassVmFactory;
            this.openAddToOrderModalService = openAddToOrderModalService;
            this.openRetrieveOrdersModalService = openRetrieveOrdersModalService;
            this.openXlsSettingsService = openXlsSettingsService;
            this.openImportOrdersService = openImportOrdersService;
            this.openGlassesStockModalService = openGlassesStockModalService;
            this.openGlassMatchingModalService = openGlassMatchingModalService;
            this.openGlassPrintPreviewService = openGlassPrintPreviewService;
            this.settingsProvider = settingsProvider;
            this.messenger = messenger;
            glassesOrder = glassesOrderVmFactory.Invoke();

            GlassMatchingVm = glassMatchingVmFactory.Invoke();

            //Configure the Glass Only Control and its Draw
            SingleGlass = this.glassVmFactory.Invoke();

            //Set starting Values for Glass (as default values)
            SingleGlass.Length = 800;
            SingleGlass.Height = 1980;
            SingleGlass.Draw = GlassDrawEnum.DrawF;
            SingleGlass.Finish = GlassFinishEnum.Transparent;
            SingleGlass.Thickness = GlassThicknessEnum.Thick8mm;
            SingleGlass.GlassType = GlassTypeEnum.FixedGlass;

            GlassDraw = new();
            GlassDraw.SetGlassDraw(SingleGlass);

            ChooseModelVm.SynhtesisSelected += ChooseModelVm_SynhtesisSelected;
            this.messenger.RegisterAll(this);

            BusyPrompt = GENERATING;
        }

        /// <summary>
        /// Executes whenever there is a Synthesis Selection Trhough Codes or Draw Selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseModelVm_SynhtesisSelected(object? sender, SynthesisSelectedArgs e)
        {
            UseGlassOnly = false; //Set mode to Cabin Always when Import Or Code Translation Is Done
            SynthesisVm.SelectSynthesis(e.Synthesis);
            if (!string.IsNullOrEmpty(e.RefPA0Number)) NextStructurePA0Number = e.RefPA0Number;
        }

        [RelayCommand]
        private void AddSynthesisToOrder()
        {
            if (UseGlassOnly)
            {
                SingleGlassCabinContainer glassContainer = new(SingleGlass.GetGlass());
                CabinSynthesis synthesisGlassOnly = new()
                {
                    DrawNo = glassContainer.IsPartOfDraw,
                    Primary = glassContainer
                };
                ValidationResult validationResult = glassValidator.Validate(glassContainer.Glasses.First());

                if (validationResult.IsValid)
                {
                    openAddToOrderModalService.OpenModal(synthesisGlassOnly, NextStructurePA0Number,null);
                }
                else
                {
                    var errorCodes = validationResult.Errors.Select(e => e.ErrorCode);
                    StringBuilder builder = new();
                    int index = 0;
                    foreach (string error in errorCodes)
                    {
                        index++;
                        builder.Append(index)
                               .Append('.')
                               .Append(' ')
                               .Append(error.TryTranslateKey())
                               .Append(Environment.NewLine)
                               .Append(Environment.NewLine);
                    }
                    MessageService.Warning(builder.ToString(), "lngFailure".TryTranslateKey());
                }
                return;
            }

            if (SynthesisVm.IsCalculating)
            {
                MessageService.Warning("lngStillCalculating".TryTranslateKey(), "lngFailure".TryTranslateKey());
                return;
            }
            //Check if Synthesis Has Any Errors
            //Displays the Cabin Errors if User Trys to Add in Invalid-State
            if (SynthesisVm.HasErrors())
            {
                StringBuilder builder = new();
                int index = 0;
                foreach (string error in SynthesisVm.GetErrors())
                {
                    index++;
                    builder.Append(index)
                           .Append('.')
                           .Append(' ')
                           .Append(error)
                           .Append(Environment.NewLine)
                           .Append(Environment.NewLine);
                }
                MessageService.Warning(builder.ToString(), "lngFailure".TryTranslateKey());
                return;
            }

            //Get the Synthesis and create a new Viewmodel for each Cabin
            //Then Pass those viewmodels to the Modal for insertion to the Order
            var synthesis = SynthesisVm.GetSynthesis();

            if (synthesis is null) MessageService.Warning("Cabin is Empty", "lngInformation".TryTranslateKey());
            
            //Check if there is any swap in the Passed Synthesis
            else openAddToOrderModalService.OpenModal(synthesis, NextStructurePA0Number, SynthesisVm.GetGlassSwap());

        }

        [RelayCommand]
        private void OpenRetrieveOrders()
        {
            openRetrieveOrdersModalService.OpenModal();
        }
        [RelayCommand]
        private async Task SaveToExcel()
        {
            IsBusy = true;
            try
            {
                var xlsSettings = await settingsProvider.GetSelectedSettingsAsync();
                xlsSettings.SelectedCultureString = ((App)App.Current).SelectedLanguage;
                var filename = await Task.Run(() =>
                    {
                        var filename = ExcelService.GlassOrdersXls.SaveGlassesOrderToXls(GlassesOrder.ToGlassesOrder(), xlsSettings);
                        return filename;
                    });
                if (GlassesOrder.GlassRows.Any(r=>r.IsFromStock))
                {
                    StringBuilder builder = new("lngStockedGlassesNotAddedToXls".TryTranslateKey());
                    var glassesInStock = GlassesOrder.GlassRows.Where(r => r.IsFromStock).Select(row => row.OrderedGlass.ToString());
                    int incrementor = 1;
                    builder.Append(Environment.NewLine);
                    foreach (var glass in glassesInStock)
                    {
                        builder.Append(incrementor++).Append('.').Append(glass).Append(Environment.NewLine);
                    }
                    MessageService.Info(builder.ToString(), "lngInformation".TryTranslateKey());
                }
                if (MessageService.Questions.ExcelSavedAskOpenFile(filename) == MessageBoxResult.OK)
                {
                    //Open the file if users reply is positive
                    Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
                }

            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task OpenXlsSettings()
        {
            await openXlsSettingsService.OpenModal();
        }
        [RelayCommand]
        private void NewOrder()
        {
            GlassesOrder.SetNewOrder();
            GlassesOrder.OpenOrderDetailsEditCommand.Execute(null);
        }
        [RelayCommand]
        private void OpenImportOrders()
        {
            //openImportOrdersService.OpenModal();
            openImportOrdersService.OpenModalAsWindow();
        }
        [RelayCommand]
        private void OpenGlassesStock()
        {
            openGlassesStockModalService.OpenModal();
        }
        [RelayCommand]
        private void OpenGlassMatches()
        {
            openGlassMatchingModalService.OpenModal(GlassMatchingVm);
        }
        [RelayCommand]
        private void OpenGlassDrawPrintPreview()
        {
            openGlassPrintPreviewService.OpenModal(GlassDraw);
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
                ChooseModelVm.SynhtesisSelected -= ChooseModelVm_SynhtesisSelected;
                messenger.UnregisterAll(this);
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

        /// <summary>
        /// Whenever a Glasses Order is Selected
        /// </summary>
        /// <param name="message"></param>
        public void Receive(GlassesOrderSelectedMessage message)
        {
            if (message.SenderType == typeof(GlassesOrdersDisplayModalViewModel))
            {
                GlassesOrder.SetExistingOrder(message.Value);
            }
        }
        /// <summary>
        /// Whenever a CabinRow is Inserted by the User
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Receive(AddCabinRowsMessage message)
        {
            SynthesisVm.ResetViewModel();
        }
    }
}
