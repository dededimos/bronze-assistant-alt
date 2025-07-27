using BronzeFactoryApplication.ApplicationServices.MessangerService;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels
{
    /// <summary>
    /// Functionality : 
    /// ViewModel gets the Codes of Primary - Secondary - Tertiary 
    /// Sends them over to The Synthesis CodeTranslator which returns a SynthesisTranslationResult
    /// The Result Contains all the Potential Synthesis for the Typed Codes
    /// The Translate Codes Command Executes and Fills all the Properties with RecentGenSynthesis / Available Draw Numbers / and Selected Draw
    /// As soon As the Seelcted Draw Changes the Event SynthesisSelected is Raised and passes the Best matched model to Listeners.
    /// If the Selected Draw Changes without calling Translate Codes then the Selected Draw is picked from the ready recent GeneratedSynthesis Property
    /// </summary>
    public partial class ChooseCabinModelViewModel : BaseViewModel , IRecipient<ImportCabinsMessage> , IRecipient<CabinCodeChangedMessage>
    {
        public override bool IsDisposable => false;

        /// <summary>
        /// The Default Draws List , Minus the NONE draw
        /// </summary>
        private static readonly IEnumerable<CabinDrawNumber> drawNumbersDefault = Enum.GetValues(typeof(CabinDrawNumber)).Cast<CabinDrawNumber>().Where(d => d != CabinDrawNumber.None);
        /// <summary>
        /// Used to translate Codes into Synthesis Structures , Best Match as well as further Selections
        /// </summary>
        private readonly SynthesisCodeTranslator codeTranslator;
        /// <summary>
        /// Used to Generate Synthesis when no code has been provided but a Draw Selection has been Made
        /// </summary>
        private readonly CabinFactory cabinFactory;
        private readonly CloseModalService closeModalService;
        private readonly IMessenger messenger;

        /// <summary>
        /// Raised whenever a new Synthesis Selection is done (from auto selecting best match or DrawSelection)
        /// </summary>
        public event EventHandler<SynthesisSelectedArgs>? SynhtesisSelected;

        /// <summary>
        /// The Result of the Code Translation if Any
        /// </summary>
        private SynthesisTranslationResult? translationResult;

        /// <summary>
        /// Weather the underlying view should be focused
        /// </summary>
        [ObservableProperty]
        private bool shouldGetFocus;

        /// <summary>
        /// The Code of the Primary Cabin
        /// </summary>
        [ObservableProperty]
        private string codePrimary = string.Empty;
        /// <summary>
        /// The Code of the Secondary Cabin
        /// </summary>
        [ObservableProperty]
        private string codeSecondary = string.Empty;
        /// <summary>
        /// The Code of the Tertiary Cabin
        /// </summary>
        [ObservableProperty]
        private string codeTertiary = string.Empty;

        /// <summary>
        /// The Currently available draws to Select from
        /// </summary>
        [ObservableProperty]
        private IEnumerable<CabinDrawNumber> drawNumbers = drawNumbersDefault;

        private CabinDrawNumber? selectedDraw;
        public CabinDrawNumber? SelectedDraw
        {
            get => selectedDraw;
            set
            {
                if (value != selectedDraw)
                {
                    selectedDraw = value;
                    OnPropertyChanged(nameof(SelectedDraw));
                }
                //Must always raise this when setting (dimensions might change and draw might remain the same)
                //Raise the Synthesis Selection Event by passing the Best Match
                RaiseSynthesisSelected(SelectedStructure,importedPA0Number);
                OnPropertyChanged(nameof(SelectedStructure));
            }
        }
        
        private string importedPA0Number = string.Empty;

        /// <summary>
        /// The Best Matching Structure
        /// </summary>
        public CabinSynthesis SelectedStructure 
        {
            //Pass the Created Synthesis Matching the Selected Draw , Calculate its Time a New Synthesis so that all the Cabin Models are Refreshed on Selection
            //Else Create one as the Selected Draw (none or not none)
            get => translationResult?.GeneratePotentialSynthesis(cabinFactory).FirstOrDefault(s => s.DrawNo == SelectedDraw)
                ?? cabinFactory.CreateSynthesis(SelectedDraw ?? CabinDrawNumber.None);
        }

        

        public ChooseCabinModelViewModel(SynthesisCodeTranslator codeTranslator , 
                                         CabinFactory cabinFactory,
                                         CloseModalService closeModalService,
                                         IMessenger messenger)
        {
            this.codeTranslator = codeTranslator;
            this.cabinFactory = cabinFactory;
            this.closeModalService = closeModalService;
            this.messenger = messenger;
            messenger.RegisterAll(this);
            this.closeModalService.ModalClosed += OnModalClose;
        }

        private void OnModalClose(object? sender, ModalClosedEventArgs e)
        {
            //Inform that should get Focus if the Closed Modal was an AddToOrderModal
            if (e.TypeOfClosedModal == typeof(AddSynthesisToOrderModalViewModel))
            {
                ShouldGetFocus = true;
            }
        }

        /// <summary>
        /// Devises a Translation from an Import Cabins Message
        /// </summary>
        /// <param name="message"></param>
        public void Receive(ImportCabinsMessage message)
        {
            ClearSelections();
            importedPA0Number = message.RefPA0; //Set the PA0 Number Imported from the Message
            translationResult = codeTranslator.TranslateSynthesis(message.GetPrimaryMessage(), message.GetSecondaryMessage(), message.GetTertiaryMessage());
            //Pass the new Draw Numbers from the Selection
            DrawNumbers = translationResult.PotentialDrawNumbers.Any(d => d != CabinDrawNumber.None) ? translationResult.PotentialDrawNumbers : drawNumbersDefault;

            //Pass the Best Match
            SelectedDraw = translationResult.BestMatchingDraw is CabinDrawNumber.None ? null : translationResult.BestMatchingDraw;
        }
        /// <summary>
        /// When a Code Has Changed for the Cabin that is Being Currently Built
        /// </summary>
        /// <param name="message">the Message</param>
        public void Receive(CabinCodeChangedMessage message)
        {
            switch (message.SynthesisModel)
            {
                case CabinSynthesisModel.Primary:
                    if (string.IsNullOrEmpty(message.NewCode)) ClearSelections();
                    else CodePrimary = message.NewCode;
                    break;
                case CabinSynthesisModel.Secondary:
                    CodeSecondary = message.NewCode;
                    break;
                case CabinSynthesisModel.Tertiary:
                    CodeTertiary = message.NewCode;
                    break;
                default:
                    //do nothing
                    break;
            }
        }

        /// <summary>
        /// Translates the Codes into Cabins and Draws
        /// Passes the Results to this View Model (BestMatch - Rest Selections - CodeValidation Errors)
        /// </summary>
        [RelayCommand]
        private void TranslateCodes()
        {
            importedPA0Number = string.Empty; //Reset this
            translationResult = codeTranslator.TranslateSynthesis((CodePrimary, CodeSecondary, CodeTertiary));
            
            //Pass the new Draw Numbers from the Selection
            DrawNumbers = translationResult.PotentialDrawNumbers.Any(d=>d != CabinDrawNumber.None) ? translationResult.PotentialDrawNumbers : drawNumbersDefault;
            
            //Pass the Best Match
            SelectedDraw = translationResult.BestMatchingDraw is CabinDrawNumber.None ? null : translationResult.BestMatchingDraw;
        }

        /// <summary>
        /// Clears All the Chosen Cabin Selections
        /// </summary>
        [RelayCommand]
        private void ClearSelections()
        {
            CodePrimary = string.Empty;
            CodeSecondary = string.Empty;
            CodeTertiary = string.Empty;
            DrawNumbers = drawNumbersDefault;
            SelectedDraw = null;
        }

        /// <summary>
        /// Raises the Synthesis Selection Event
        /// </summary>
        /// <param name="synthesis">The Selected Synthesis</param>
        public void RaiseSynthesisSelected(CabinSynthesis synthesis,string refPA0Number)
        {
            SynhtesisSelected?.Invoke(this, new(synthesis,refPA0Number));
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
                closeModalService.ModalClosed -= OnModalClose;
                messenger.UnregisterAll(this);
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

    public class SynthesisSelectedArgs : EventArgs
    {
        public CabinSynthesis Synthesis { get; set; }
        public string RefPA0Number { get; set; }

        public SynthesisSelectedArgs(CabinSynthesis synthesis, string refPA0Number)
        {
            Synthesis = synthesis;
            RefPA0Number = refPA0Number;
        }
    }


}
