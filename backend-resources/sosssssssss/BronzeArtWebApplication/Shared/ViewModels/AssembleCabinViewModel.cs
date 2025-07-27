using AKSoftware.Localization.MultiLanguages;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Builders;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Helpers.Custom_Exceptions;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Shared.ViewModels
{
    public class AssembleCabinViewModel : INotifyPropertyChanged, IDisposable
    {
        /*NOTES
         * 1.The Cabin Selection is Done by Changing the Selected Draw Number 
         * 2.If the Model Changes the Selected Draw Number must Change First . 
         * 3.The ViewModel Takes Care of Creating the Correct Sides for any of the Respective Different Draws
         * 4.After Changing the DrawNumber we can Change any Property on the Different CabinProperties ViewModels for Primary/Secondary/Tertiary Cabins
         */

        private readonly ILanguageContainerService lc;
        private readonly BronzeUser loggedUser;
        private readonly ILocalStorageService localStorage;
        private readonly BronzeItemsPriceBuilder priceBuilder;
        private readonly ICabinMemoryRepository repo;
        private readonly CabinFactory cabinsFactory;
        private RulesDirector rulesDirector;

        public CabinPropertiesViewModel PrimaryCabin { get; }
        public CabinPropertiesViewModel SecondaryCabin { get; }
        public CabinPropertiesViewModel TertiaryCabin { get; }

        private string notesText = string.Empty;
        /// <summary>
        /// The Notes of the TextField for the Selected Structure
        /// </summary>
        public string NotesText
        {
            get => notesText;
            set
            {
                if (notesText != value)
                {
                    notesText = value;
                    OnPropertyChanged(nameof(NotesText));
                }
            }
        }

        /// <summary>
        /// The Selected Synthesis
        /// </summary>
        public CabinSynthesis Synthesis 
        { 
            get => GetSynthesisWithoutError(); 
        }

        /// <summary>
        /// Creates a Synthesis from the Pricmary Secondary and Tertiary Models , if an Invalid Synthesis is Made it returns an Undefined Synthesis Object
        /// </summary>
        /// <returns></returns>
        private CabinSynthesis GetSynthesisWithoutError()
        {
            try
            {
                var synth = CabinFactory.CreateSynthesis(PrimaryCabin.CabinObject, SecondaryCabin.CabinObject, TertiaryCabin.CabinObject);
                return synth;
            }
            catch (Exception ex) when (ex is InvalidOperationException or InvalidSynthesisDrawsException)
            {
                return CabinSynthesis.Undefined();
            }
        }

        /// <summary>
        /// How Many Sides are Currently Active
        /// </summary>
        public int NumberOfActiveCabinSides
        {
            get
            {
                //For each non Null Side add 1 Side
                //Property Changes when a Model Property is Changed and Gets Handled on the OnPropertyChanged Subscription of Primary/Secondary/Tertiary
                int sidesNo = 0;
                sidesNo = PrimaryCabin?.Model != null ? sidesNo + 1 : sidesNo;
                sidesNo = SecondaryCabin?.Model != null ? sidesNo + 1 : sidesNo;
                sidesNo = TertiaryCabin?.Model != null ? sidesNo + 1 : sidesNo;
                return sidesNo;
            }
        }

        #region 1.Cabin Model Selection Choices

        /// <summary>
        /// The Opening Category Selected by the User 
        /// Null if Category is not Selected Yet
        /// </summary>
        private OpeningCategory? selectedOpeningCategory;
        public OpeningCategory? SelectedOpeningCategory
        {
            get { return selectedOpeningCategory; }
            set
            {
                selectedOpeningCategory = value;
                OnPropertyChanged(nameof(SelectedOpeningCategory));
            }
        }

        /// <summary>
        /// The SlidingType Selected by the User 
        /// Null if this Category is not Selected
        /// </summary>
        private SlidingType? selectedSlidingType;
        public SlidingType? SelectedSlidingType
        {
            get { return selectedSlidingType; }
            set
            {
                selectedSlidingType = value;
                OnPropertyChanged(nameof(SelectedSlidingType));
            }
        }

        /// <summary>
        /// The CabinSeries Selected by the User , Null if there is not yet a Series Selected
        /// </summary>
        private CabinSeries? selectedSeries;
        public CabinSeries? SelectedSeries
        {
            get { return selectedSeries; }
            set
            {
                selectedSeries = value;
                OnPropertyChanged(nameof(SelectedSeries));
            }
        }

        //Changes also the Cabin Properties ViewModels when Changed
        private CabinDrawNumber selectedCabinDraw;
        public CabinDrawNumber SelectedCabinDraw
        {
            get { return selectedCabinDraw; }
            set
            {
                if (value != selectedCabinDraw)
                {
                    selectedCabinDraw = value;
                    SecondaryCabin.IsPartOfDraw = SelectedCabinDraw;
                    SecondaryCabin.Direction = DefaultSecondaryCabinDirection[SelectedCabinDraw];
                    TertiaryCabin.IsPartOfDraw = SelectedCabinDraw;
                    TertiaryCabin.Direction = DefaultTertiaryCabinDirection[SelectedCabinDraw];

                    //Must Run Last -- Otherwise 
                    //The Property Change Events for Default Values of The Primary Cabin
                    //do not get Passed Down to the Secondary and Tertiary Models
                    PrimaryCabin.IsPartOfDraw = SelectedCabinDraw;
                    PrimaryCabin.Direction = DefaultPrimaryCabinDirection[SelectedCabinDraw];

                    //Reset also the Notes text when changing model
                    NotesText = string.Empty;

                    OnPropertyChanged(nameof(SelectedCabinDraw));
                }


            }
        }

        #endregion

        #region 2.Windows Properties
        //The Last shown window
        private StoryWindow previousWindow;
        public StoryWindow PreviousWindow
        {
            get { return previousWindow; }
            set
            {
                previousWindow = value;
                OnPropertyChanged(nameof(PreviousWindow));
            }
        }

        //The Next Window to be Shown OR the One that is Currently Showing
        private StoryWindow currentWindow;
        public StoryWindow CurrentWindow
        {
            get { return currentWindow; }
            set
            {
                if (currentWindow != value)
                {
                    currentWindow = value;
                    OnPropertyChanged(nameof(CurrentWindow));
                }
            }
        }

        public Dictionary<StoryWindow, bool> IsWindowVisible { get; set; }

        #endregion

        public AssembleCabinViewModel(ILanguageContainerService lc,
                                      BronzeUser loggedUser,
                                      ILocalStorageService localStorage,
                                      BronzeItemsPriceBuilder priceBuilder,
                                      ICabinMemoryRepository repo,
                                      CabinFactory cabinsFactory,
                                      CabinValidator validator,
                                      GlassesBuilderDirector glassBuilder)
        {
            this.lc = lc;
            this.loggedUser = loggedUser;
            this.localStorage = localStorage;
            this.priceBuilder = priceBuilder;
            this.repo = repo;
            this.cabinsFactory = cabinsFactory;
            InitilizeWindowsParameters();

            //Initialize Cabin Properties ViewModels
            PrimaryCabin = new(CabinSynthesisModel.Primary, cabinsFactory, repo, validator, glassBuilder);
            SecondaryCabin = new(CabinSynthesisModel.Secondary, cabinsFactory, repo, validator, glassBuilder);
            TertiaryCabin = new(CabinSynthesisModel.Tertiary, cabinsFactory, repo, validator, glassBuilder);
            PrimaryCabin.PropertyChanged += Cabin_PropertyChanged1;
            SecondaryCabin.PropertyChanged += Cabin_PropertyChanged2;
            TertiaryCabin.PropertyChanged += Cabin_PropertyChanged3;
        }

        /// <summary>
        /// Gets Fired whenever a Cabin Property has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cabin_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            //If the Model was Changed Check inform That the Active Number of Cabin Sides might have Changed
            if (e.PropertyName is nameof(CabinPropertiesViewModel.IsPartOfDraw) or null or "")
            {
                OnPropertyChanged(nameof(NumberOfActiveCabinSides));
            }
            if (e.PropertyName is nameof(CabinPropertiesViewModel.InputHeight) or null or "")
            {
                SecondaryCabin.InputHeight = PrimaryCabin.InputHeight;
                TertiaryCabin.InputHeight = PrimaryCabin.InputHeight;
            }
            if (e.PropertyName is nameof(CabinPropertiesViewModel.Thicknesses) or null or "")
            {
                //HACK! :-( to Force 6mm on Flipepr Panel
                if (SecondaryCabin.Model is CabinModelEnum.ModelWFlipper && (PrimaryCabin.Thicknesses is CabinThicknessEnum.Thick6mm or CabinThicknessEnum.Thick8mm))
                {
                    SecondaryCabin.Thicknesses = CabinThicknessEnum.Thick6mm;
                }
                else
                {
                    SecondaryCabin.Thicknesses = PrimaryCabin.Thicknesses;
                }
                TertiaryCabin.Thicknesses = PrimaryCabin.Thicknesses;
            }
            if (e.PropertyName is nameof(CabinPropertiesViewModel.MetalFinish) or null or "")
            {
                SecondaryCabin.MetalFinish = PrimaryCabin.MetalFinish;
                TertiaryCabin.MetalFinish = PrimaryCabin.MetalFinish;
            }
            if (e.PropertyName is nameof(CabinPropertiesViewModel.GlassFinish) or null or "")
            {
                SecondaryCabin.GlassFinish = PrimaryCabin.GlassFinish;
                TertiaryCabin.GlassFinish = PrimaryCabin.GlassFinish;
            }
            //Apply SafeKids Addition or Removal to all Sides of the Cabin
            if (e.PropertyName is nameof(CabinPropertiesViewModel.HasSafeKids))
            {
                SecondaryCabin.HasSafeKids = PrimaryCabin.HasSafeKids;
                TertiaryCabin.HasSafeKids = PrimaryCabin.HasSafeKids;
            }

            //Apply Perimetrical Frame Changes to All Models that can have one.
            if (e.PropertyName is nameof(CabinPropertiesViewModel.HasPerimetricalFrame))
            {
                if (SecondaryCabin is not null && SecondaryCabin.CanHavePerimatricalFrame)
                {
                    SecondaryCabin.HasPerimetricalFrame = PrimaryCabin.HasPerimetricalFrame;
                }
                if (TertiaryCabin is not null && TertiaryCabin.CanHavePerimatricalFrame)
                {
                    TertiaryCabin.HasPerimetricalFrame = PrimaryCabin.HasPerimetricalFrame;
                }
            }

            //Inform the Property has Changed 
            OnPropertyChanged(nameof(PrimaryCabin));
        }

        /// <summary>
        /// Gets Fired whenever a Cabin Property has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cabin_PropertyChanged2(object sender, PropertyChangedEventArgs e)
        {
            //If the Model was Changed Check inform That the Active Number of Cabin Sides might have Changed
            if (e.PropertyName == nameof(CabinPropertiesViewModel.IsPartOfDraw))
            {
                OnPropertyChanged(nameof(NumberOfActiveCabinSides));
            }
            //Inform the Property has Changed 
            OnPropertyChanged(nameof(SecondaryCabin));
        }

        /// <summary>
        /// Gets Fired whenever a Cabin Property has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cabin_PropertyChanged3(object sender, PropertyChangedEventArgs e)
        {
            //If the Model was Changed Check inform That the Active Number of Cabin Sides might have Changed
            if (e.PropertyName == nameof(CabinPropertiesViewModel.IsPartOfDraw))
            {
                OnPropertyChanged(nameof(NumberOfActiveCabinSides));
            }
            //Inform the Property has Changed 
            OnPropertyChanged(nameof(TertiaryCabin));
        }


        #region 1.Reset/Change ViewModel Methods

        public void ResetViewModel()
        {
            SelectedCabinDraw = CabinDrawNumber.None;
            SelectedSeries = null;
            SelectedOpeningCategory = null;
            SelectedSlidingType = null;
            NotesText = string.Empty;

            ShowWindow(StoryWindow.StartWindow, CurrentWindow);
        }

        public void ChangeSynthesisDirection()
        {
            PrimaryCabin.Direction = HelperMethods.ChangeDirection(PrimaryCabin.Direction ?? CabinDirection.Undefined);
            SecondaryCabin.Direction = HelperMethods.ChangeDirection(SecondaryCabin.Direction ?? CabinDirection.Undefined);
            TertiaryCabin.Direction = HelperMethods.ChangeDirection(TertiaryCabin.Direction ?? CabinDirection.Undefined);
        }

        /// <summary>
        /// Changes the Current Synthesis to the one Passed to this Method
        /// </summary>
        /// <param name="synthesis">The Synthesis we need to Pass to the ViewModel</param>
        public void PassSynthesisToViewModel(CabinSynthesis synthesis)
        {
            SelectedCabinDraw = synthesis.DrawNo;
            PrimaryCabin.SetCabin(synthesis.Primary);
            SecondaryCabin.SetCabin(synthesis.Secondary);
            TertiaryCabin.SetCabin(synthesis.Tertiary);
        }

        public void TransformSynthesisToNewDraw(CabinDrawNumber newDraw)
        {
            var synthesis = cabinsFactory.CreateSynthesis(newDraw);
            synthesis.Primary.NominalLength = PrimaryCabin.InputLength ?? 0;
            synthesis.Primary.Height = PrimaryCabin.InputHeight ?? 0;
            synthesis.Primary.GlassFinish = PrimaryCabin.GlassFinish;
            synthesis.Primary.MetalFinish = PrimaryCabin.MetalFinish;
            var hasSafeKids = PrimaryCabin.HasSafeKids;
            var hasBronzeClean = PrimaryCabin.HasBronzeClean;
            var handle = PrimaryCabin.HandleOption;
            if (synthesis.Primary.Parts.HasSpot(PartSpot.Handle1))
            {
                synthesis.Primary.Parts.SetPart(PartSpot.Handle1, handle);
            }
            if (synthesis.Primary.Parts.HasSpot(PartSpot.Handle2))
            {
                synthesis.Primary.Parts.SetPart(PartSpot.Handle2, handle);
            }

            if (synthesis.Secondary is not null)
            {
                synthesis.Secondary.NominalLength = SecondaryCabin.InputLength ?? 0;
                synthesis.Secondary.Height = SecondaryCabin.InputHeight ?? 0;
                synthesis.Secondary.GlassFinish = SecondaryCabin.GlassFinish;
                synthesis.Secondary.MetalFinish = SecondaryCabin.MetalFinish;
                if (synthesis.Secondary.Parts.HasSpot(PartSpot.Handle1))
                {
                    synthesis.Secondary.Parts.SetPart(PartSpot.Handle1, handle);
                }
                if (synthesis.Secondary.Parts.HasSpot(PartSpot.Handle2))
                {
                    synthesis.Secondary.Parts.SetPart(PartSpot.Handle2, handle);
                }
            }
            if (synthesis.Tertiary is not null)
            {
                synthesis.Tertiary.NominalLength = TertiaryCabin.InputLength ?? 0;
                synthesis.Tertiary.Height = TertiaryCabin.InputHeight ?? 0;
                synthesis.Tertiary.GlassFinish = TertiaryCabin.GlassFinish;
                synthesis.Tertiary.MetalFinish = TertiaryCabin.MetalFinish;
            }

            PassSynthesisToViewModel(synthesis);
            PrimaryCabin.HasSafeKids = hasSafeKids;
            PrimaryCabin.HasBronzeClean = hasBronzeClean;
        }

        #endregion

        #region 2.Windows Methods

        private void InitilizeWindowsParameters()
        {
            IsWindowVisible = new();
            foreach (StoryWindow window in Enum.GetValues(typeof(StoryWindow)))
            {
                IsWindowVisible.Add(window, false); //All Windows are Closed
            }

            ShowWindow(StoryWindow.StartWindow);
        }

        /// <summary>
        /// Shows the selected Window 
        /// </summary>
        /// <param name="windowToShow">The Window to Show</param>
        /// <param name="requestWindow">The Window from which the Request is Coming</param>
        public void ShowWindow(StoryWindow windowToShow, StoryWindow windowToClose = StoryWindow.None)
        {
            IsWindowVisible[windowToClose] = false;
            OnPropertyChanged(windowToClose.ToString()); //inform window has Closed

            PreviousWindow = windowToClose; //Save the Last Window Except if it was the ResumeWindow


            IsWindowVisible[windowToShow] = true;
            OnPropertyChanged(windowToShow.ToString()); //inform window has Opened
            CurrentWindow = windowToShow; //Save the Current Window that Opens Except if its the Resume Window
        }

        /// <summary>
        /// Gets the Parent Window which Creates the Child Window
        /// </summary>
        /// <param name="childWindow">The Child Window</param>
        /// <returns>The Parent Window</returns>
        public StoryWindow GetParentWindow(StoryWindow childWindow)
        {
            StoryWindow parentWindow;
            switch (childWindow)
            {
                case StoryWindow.OpeningPrimary:
                    parentWindow = StoryWindow.StartWindow;
                    break;
                case StoryWindow.SlidingOpenings:
                case StoryWindow.FoldingOpenings:
                case StoryWindow.StandardDoorOpenings:
                case StoryWindow.DoorOnPanelOpenings:
                case StoryWindow.FixedPanelOpenings:
                case StoryWindow.ModelsBathtub:
                    parentWindow = StoryWindow.OpeningPrimary;
                    break;
                case StoryWindow.ModelsS:
                case StoryWindow.ModelsSF:
                case StoryWindow.ModelsSFF:
                case StoryWindow.Models4:
                case StoryWindow.Models4F:
                case StoryWindow.Models4FF:
                case StoryWindow.ModelsA:
                case StoryWindow.ModelsAF:
                case StoryWindow.ModelsC:
                case StoryWindow.ModelsCF:
                    parentWindow = StoryWindow.SlidingOpenings;
                    break;
                case StoryWindow.ModelsP44:
                case StoryWindow.ModelsP46:
                case StoryWindow.ModelsP48:
                case StoryWindow.ModelsP45:
                case StoryWindow.ModelsP47:
                    parentWindow = StoryWindow.FoldingOpenings;
                    break;
                case StoryWindow.ModelsB3151:
                case StoryWindow.ModelsB3252:
                case StoryWindow.ModelsB3353:
                case StoryWindow.ModelsB3859:
                case StoryWindow.ModelsB4161:
                    parentWindow = StoryWindow.StandardDoorOpenings;
                    break;
                case StoryWindow.ModelsHB34:
                case StoryWindow.ModelsHB35:
                case StoryWindow.ModelsHB37:
                case StoryWindow.ModelsHB40:
                case StoryWindow.ModelsHB43:
                    parentWindow = StoryWindow.DoorOnPanelOpenings;
                    break;
                case StoryWindow.ModelsW:
                case StoryWindow.Models81:
                case StoryWindow.Models82:
                case StoryWindow.Models84:
                case StoryWindow.Models85:
                case StoryWindow.Models88:
                case StoryWindow.ModelsE:
                    parentWindow = StoryWindow.FixedPanelOpenings;
                    break;
                case StoryWindow.SeriesPrimary:
                    parentWindow = StoryWindow.StartWindow;
                    break;
                case StoryWindow.SeriesB6000:
                case StoryWindow.SeriesFree:
                case StoryWindow.SeriesHotel8000:
                case StoryWindow.SeriesInox304:
                case StoryWindow.SeriesNiagara6000:
                case StoryWindow.SeriesSmart:
                    parentWindow = StoryWindow.SeriesPrimary;
                    break;
                case StoryWindow.CabinPanel:
                    parentWindow = PreviousWindow;
                    break;
                default:
                case StoryWindow.None:
                case StoryWindow.StartWindow:
                    parentWindow = StoryWindow.None;
                    break;
            }
            return parentWindow;
        }
        #endregion

        #region 3.Pricing & Rules

        /// <summary>
        /// Constructs the Products List -- This Method Should Ptopably go Into Helpers and not Inisde here . The PriceTable Component must call it in its Initialization instead of the ViewModel
        /// </summary>
        /// <returns>List of priceables</returns>
        public List<IPriceable> GetProductsList()
        {
            List<IPriceable> list = [];
            List<ICodeable> cabins = [.. Synthesis.GetCabinList()];
            list = priceBuilder.GetPriceables(cabins);
            //Create the Cabin Products
            PricingRulesOptionsCabins options = new()
            {
                UserCombinedDiscountCabins = loggedUser.CombinedDiscountCabin,
                DefaultB6000HandleCode = repo.GetDefault(new(CabinModelEnum.Model9S, CabinDrawNumber.Draw9S, CabinSynthesisModel.Primary), PartSpot.Handle1),
            };
            if (loggedUser.SelectedAppMode is BronzeAppMode.Retail)
            {
                options.WithIncreasePriceRule = true;
                options.PriceIncreaseFactor = loggedUser.SelectedPriceIncreaseFactorCabins;
            }
            RulesDirector rules = new(options);
            rules.ApplyRulesToMultiple(list);
            rulesDirector = rules;
            list.ForEach(priceable => priceable.VatFactor = loggedUser.VatFactor);

            foreach (var item in list)
            {
                //for special dimensions cabins change the code to three digits
                if (item is Priceable<Cabin> cabin)
                {
                    if (item.AppliedRules.Any(r => r.AppliedRuleType == typeof(CabinSpecialDimensionRule)))
                    {
                        cabin.Product.OverrideCode(CodeGenerator.GenerateThreeDigitDimensionsCode(cabin.Product));
                    }
                    //Reset the override if its not special , otherwise it keeps the old code
                    else cabin.Product.OverrideCode(string.Empty);
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the Names of all Available Rules
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllRulesNames()
        {
            return rulesDirector?.GetRuleListNames() ?? new List<string>();
        }

        #endregion

        #region Repository Methods

        #endregion

        #region Z.Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Z1.Dispose
        /// <summary>
        /// Method is not needed for this Service ? Because it adds as Signleton and Maybe Because Services cannot be disposed this way but have to call dispose Explicitly ? I am not Sure
        /// </summary>
        public void Dispose()
        {
            //loggedUser.OnAuthenticationStateChanged -= GetLoggedUserClaims;
            PrimaryCabin.PropertyChanged -= Cabin_PropertyChanged1;
            SecondaryCabin.PropertyChanged -= Cabin_PropertyChanged2;
            TertiaryCabin.PropertyChanged -= Cabin_PropertyChanged3;
        }
        #endregion
    }
}
