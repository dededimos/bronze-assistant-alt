using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Models
{
#nullable enable
    public class BronzeUser : IDisposable, INotifyPropertyChanged
    {
        private readonly AuthenticationStateProvider stateProvider;

        private bool isCurrentlyAuthorizing;
        public bool IsCurrentlyAuthorizing
        {
            get => isCurrentlyAuthorizing;
            set
            {
                if (isCurrentlyAuthorizing != value)
                {
                    isCurrentlyAuthorizing = value;
                    OnPropertyChanged(nameof(IsCurrentlyAuthorizing));
                }
            }
        }

        #region 1.Claims

        private decimal primaryDiscount;
        /// <summary>
        /// User Primary Discount in 0.00 Format
        /// </summary>
        public decimal PrimaryDiscount
        {
            get => primaryDiscount;
            set
            {
                if (value != primaryDiscount)
                {
                    primaryDiscount = value;
                    OnPropertyChanged(nameof(PrimaryDiscount));
                }
            }
        }

        private decimal secondaryDiscount;
        /// <summary>
        /// User Secondary Discount in 0.00 Format
        /// </summary>
        public decimal SecondaryDiscount
        {
            get => secondaryDiscount;
            set
            {
                if (value != secondaryDiscount)
                {
                    secondaryDiscount = value;
                    OnPropertyChanged(nameof(SecondaryDiscount));
                }
            }
        }

        private decimal tertiaryDiscount;
        /// <summary>
        /// User Tertiary Discount in 0.00 Format
        /// </summary>
        public decimal TertiaryDiscount
        {
            get => tertiaryDiscount;
            set
            {
                if (value != tertiaryDiscount)
                {
                    tertiaryDiscount = value;
                    OnPropertyChanged(nameof(TertiaryDiscount));
                }
            }
        }

        private decimal retailDiscount;
        /// <summary>
        /// The Applied Retail Discount
        /// </summary>
        public decimal RetailDiscount
        {
            get => retailDiscount;
            set
            {
                if (value != retailDiscount)
                {
                    retailDiscount = value;
                    OnPropertyChanged(nameof(RetailDiscount));
                }
            }
        }

        /// <summary>
        /// The Maximum RetailDiscount a User can Put
        /// </summary>
        private int maximumRetailDiscount = 100;
        public int MaximumRetailDiscount
        {
            get => maximumRetailDiscount;
            set
            {
                if (value != maximumRetailDiscount)
                {
                    maximumRetailDiscount = value;
                    OnPropertyChanged(nameof(MaximumRetailDiscount));
                }
            }
        }

        /// <summary>
        /// The MinimumRetailDiscount a User can Put
        /// </summary>
        private int minimumRetailDiscount = 0;
        public int MinimumRetailDiscount
        {
            get => minimumRetailDiscount;
            set
            {
                if (value != minimumRetailDiscount)
                {
                    minimumRetailDiscount = value;
                    OnPropertyChanged(nameof(MinimumRetailDiscount));
                }
            }
        }

        /// <summary>
        /// Whether the user can Adjust the Discount during Retail Mode
        /// </summary>
        private bool isRetailDiscountAdjustable;
        public bool IsRetailDiscountAdjustable
        {
            get => isRetailDiscountAdjustable;
            set
            {
                if (value != isRetailDiscountAdjustable)
                {
                    isRetailDiscountAdjustable = value;
                    OnPropertyChanged(nameof(IsRetailDiscountAdjustable));
                }
            }
        }

        private string displayName = string.Empty;
        /// <summary>
        /// User Display Name
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (value != displayName)
                {
                    displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        private bool isPowerUser;
        /// <summary>
        /// If User has Power User Privilage
        /// </summary>
        public bool IsPowerUser
        {
            get => isPowerUser;
            set
            {
                if (value != isPowerUser)
                {
                    isPowerUser = value;
                    OnPropertyChanged(nameof(IsPowerUser));
                }
            }
        }

        private bool isRetailFunctionAvailable;
        /// <summary>
        /// Wheather the User can Use the Retail Function
        /// </summary>
        public bool IsRetailFunctionAvailable
        {
            get => isRetailFunctionAvailable;
            set
            {
                if (value != isRetailFunctionAvailable)
                {
                    isRetailFunctionAvailable = value;
                    OnPropertyChanged(nameof(IsRetailFunctionAvailable));
                }
            }
        }

        private bool isWholesaleFunctionAvailable;
        /// <summary>
        /// Wheather the User Can Use the Wholesale Function
        /// </summary>
        public bool IsWholesaleFunctionAvailable
        {
            get => isWholesaleFunctionAvailable;
            set
            {
                if (value != isWholesaleFunctionAvailable)
                {
                    isWholesaleFunctionAvailable = value;
                    OnPropertyChanged(nameof(IsWholesaleFunctionAvailable));
                }
            }
        }

        private bool isRetailDiscountVisible;
        /// <summary>
        /// Weather the DSiscount is Visible during Retail Mode
        /// </summary>
        public bool IsRetailDiscountVisible
        {
            get => isRetailDiscountVisible;
            set
            {
                if (value != isRetailDiscountVisible)
                {
                    isRetailDiscountVisible = value;
                    OnPropertyChanged(nameof(IsRetailDiscountVisible));
                }
            }
        }

        private decimal retailPriceIncreaseFactor;
        /// <summary>
        /// The Default Increase Factor for the Catalogue Price
        /// </summary>
        public decimal RetailPriceIncreaseFactor
        {
            get => retailPriceIncreaseFactor;
            set
            {
                if (value != retailPriceIncreaseFactor)
                {
                    retailPriceIncreaseFactor = value;
                    OnPropertyChanged(nameof(RetailPriceIncreaseFactor));
                }
            }
        }

        private decimal retailPriceIncreaseFactorCabins = 1m;
        public decimal RetailPriceIncreaseFactorCabins
        {
            get => retailPriceIncreaseFactorCabins;
            set
            {
                if (value != retailPriceIncreaseFactorCabins)
                {
                    retailPriceIncreaseFactorCabins = value;
                    OnPropertyChanged(nameof(RetailPriceIncreaseFactorCabins));
                }
            }
        }

        private RetailModeTheme retailTheme;
        /// <summary>
        /// The Retail Theme of the Current User
        /// </summary>
        public RetailModeTheme RetailTheme
        {
            get => retailTheme;
            set
            {
                if (value != retailTheme)
                {
                    retailTheme = value;
                    OnPropertyChanged(nameof(RetailTheme));
                }
            }
        }

        private decimal vatFactor;
        public decimal VatFactor
        {
            get => vatFactor;
            set
            {
                if (value != vatFactor)
                {
                    vatFactor = value;
                    OnPropertyChanged(nameof(VatFactor));
                }
            }
        }

        private string userEmail = string.Empty;
        /// <summary>
        /// The Users Email Adress
        /// </summary>
        public string UserEmail
        {
            get => userEmail;
            set
            {
                if (value != userEmail)
                {
                    userEmail = value;
                    OnPropertyChanged(nameof(UserEmail));
                }
            }
        }

        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            set
            {
                if (value != isAuthenticated)
                {
                    isAuthenticated = value;
                    OnPropertyChanged(nameof(IsAuthenticated));
                }
            }
        }


        #endregion

        #region 1.1 CabinsClaimsOnly
        private decimal primaryDiscountCabin;
        /// <summary>
        /// User Primary Discount in 100.00% Format
        /// </summary>
        public decimal PrimaryDiscountCabin
        {
            get => primaryDiscountCabin;
            set
            {
                if (value != primaryDiscountCabin)
                {
                    primaryDiscountCabin = value;
                    OnPropertyChanged(nameof(PrimaryDiscountCabin));
                }
            }
        }

        private decimal secondaryDiscountCabin;
        /// <summary>
        /// User Secondary Discount in 100.00% Format
        /// </summary>
        public decimal SecondaryDiscountCabin
        {
            get => secondaryDiscountCabin;
            set
            {
                if (value != secondaryDiscountCabin)
                {
                    secondaryDiscountCabin = value;
                    OnPropertyChanged(nameof(SecondaryDiscountCabin));
                }
            }
        }

        private decimal tertiaryDiscountCabin;
        /// <summary>
        /// User Tertiary Discount in 100.00% Format
        /// </summary>
        public decimal TertiaryDiscountCabin
        {
            get => tertiaryDiscountCabin;
            set
            {
                if (value != tertiaryDiscountCabin)
                {
                    tertiaryDiscountCabin = value;
                    OnPropertyChanged(nameof(TertiaryDiscountCabin));
                }
            }
        }
        #endregion

        #region 2.Changable Properties

        private BronzeAppMode selectedAppMode;
        /// <summary>
        /// The Bronze Application Currently Selected Mode
        /// </summary>
        public BronzeAppMode SelectedAppMode
        {
            get => selectedAppMode;
            set
            {
                if (value != selectedAppMode)
                {
                    selectedAppMode = value;
                    SetDefaultUserSelectedProperties();
                    OnPropertyChanged(nameof(SelectedAppMode));
                }
            }
        }

        private RetailModeTheme selectedRetailTheme;
        /// <summary>
        /// The Current Theme Selected
        /// </summary>
        public RetailModeTheme SelectedRetailTheme
        {
            get => selectedRetailTheme;
            set
            {
                if (value != selectedRetailTheme)
                {
                    selectedRetailTheme = value;
                    OnPropertyChanged(nameof(SelectedRetailTheme));
                }
            }
        }

        private decimal selectedPriceIncreaseFactor;
        /// <summary>
        /// The Currently Selected Price increase Factor
        /// </summary>
        public decimal SelectedPriceIncreaseFactor
        {
            get => selectedPriceIncreaseFactor;
            set
            {
                if (value != selectedPriceIncreaseFactor)
                {
                    selectedPriceIncreaseFactor = value;
                    OnPropertyChanged(nameof(SelectedPriceIncreaseFactor));
                }
            }
        }

        private decimal selectedPriceIncreaseFactorCabins;
        /// <summary>
        /// The Currently Selected Price increase Factor
        /// </summary>
        public decimal SelectedPriceIncreaseFactorCabins
        {
            get => selectedPriceIncreaseFactorCabins;
            set
            {
                if (value != selectedPriceIncreaseFactorCabins)
                {
                    selectedPriceIncreaseFactorCabins = value;
                    OnPropertyChanged(nameof(SelectedPriceIncreaseFactorCabins));
                }
            }
        }

        private bool isPricingVisible;
        /// <summary>
        /// Whether the Current User Can See Prices or Not
        /// IsRetail or IsWholeSale Available Properties Define this one
        /// </summary>
        public bool IsPricingVisible
        {
            get => isPricingVisible;
            set
            {
                if (value != isPricingVisible)
                {
                    isPricingVisible = value;
                    OnPropertyChanged(nameof(IsPricingVisible));
                }
            }
        }

        private bool isDiscountVisible;
        /// <summary>
        /// Whether the current USer can See Discount or Not
        /// </summary>
        public bool IsDiscountVisible
        {
            get => isDiscountVisible;
            set
            {
                if (value != isDiscountVisible)
                {
                    isDiscountVisible = value;
                    OnPropertyChanged(nameof(IsDiscountVisible));
                }
            }
        }

        private bool isDiscountAdjustable;
        /// <summary>
        /// Wheather the Discount Can Be Adjusted
        /// </summary>
        public bool IsDiscountAdjustable
        {
            get => isDiscountAdjustable;
            set
            {
                if (value != isDiscountAdjustable)
                {
                    isDiscountAdjustable = value;
                    OnPropertyChanged(nameof(IsDiscountAdjustable));
                }
            }
        }

        private int maximumDiscount = 100; //Default value if Local storage is 0;
        /// <summary>
        /// The Maximum Discount on Retail Mode that can be applied 
        /// </summary>
        public int MaximumDiscount
        {
            get => maximumDiscount;
            set
            {
                if (value != maximumDiscount)
                {
                    maximumDiscount = value;
                    OnPropertyChanged(nameof(MaximumDiscount));
                }
            }
        }

        private int minimumDiscount = 0;
        /// <summary>
        /// The Minimum Discount on Retail Mode that will be applied
        /// </summary>
        public int MinimumDiscount
        {
            get => minimumDiscount;
            set
            {
                if (value != minimumDiscount)
                {
                    minimumDiscount = value;
                    OnPropertyChanged(nameof(MinimumDiscount));
                }
            }
        }


        //Mirror Discounts
        private decimal selectedPrimaryDiscount;
        public decimal SelectedPrimaryDiscount
        {
            get => selectedPrimaryDiscount;
            set
            {
                if (value != selectedPrimaryDiscount)
                {
                    selectedPrimaryDiscount = value;
                    OnPropertyChanged(nameof(SelectedPrimaryDiscount));
                    OnPropertyChanged(nameof(CombinedDiscount));
                }
            }
        }

        private decimal selectedSecondaryDiscount;
        public decimal SelectedSecondaryDiscount
        {
            get => selectedSecondaryDiscount;
            set
            {
                if (value != selectedSecondaryDiscount)
                {
                    selectedSecondaryDiscount = value;
                    OnPropertyChanged(nameof(SelectedSecondaryDiscount));
                    OnPropertyChanged(nameof(CombinedDiscount));
                }
            }
        }

        private decimal selectedTertiaryDiscount;
        public decimal SelectedTertiaryDiscount
        {
            get => selectedTertiaryDiscount;
            set
            {
                if (value != selectedTertiaryDiscount)
                {
                    selectedTertiaryDiscount = value;
                    OnPropertyChanged(nameof(SelectedTertiaryDiscount));
                    OnPropertyChanged(nameof(CombinedDiscount));
                }
            }
        }

        //Cabin Discounts

        private decimal selectedPrimaryDiscountCabin;
        public decimal SelectedPrimaryDiscountCabin
        {
            get => selectedPrimaryDiscountCabin;
            set
            {
                if (value != selectedPrimaryDiscountCabin)
                {
                    selectedPrimaryDiscountCabin = value;
                    OnPropertyChanged(nameof(SelectedPrimaryDiscountCabin));
                    OnPropertyChanged(nameof(CombinedDiscountCabin));
                }
            }
        }

        private decimal selectedSecondaryDiscountCabin;
        public decimal SelectedSecondaryDiscountCabin
        {
            get => selectedSecondaryDiscountCabin;
            set
            {
                if (value != selectedSecondaryDiscountCabin)
                {
                    selectedSecondaryDiscountCabin = value;
                    OnPropertyChanged(nameof(SelectedSecondaryDiscountCabin));
                    OnPropertyChanged(nameof(CombinedDiscountCabin));
                }
            }
        }

        private decimal selectedTertiaryDiscountCabin;
        public decimal SelectedTertiaryDiscountCabin
        {
            get => selectedTertiaryDiscountCabin;
            set
            {
                if (value != selectedTertiaryDiscountCabin)
                {
                    selectedTertiaryDiscountCabin = value;
                    OnPropertyChanged(nameof(SelectedTertiaryDiscountCabin));
                    OnPropertyChanged(nameof(CombinedDiscountCabin));
                }
            }
        }

        private decimal selectedVatFactor;
        public decimal SelectedVatFactor
        {
            get => selectedVatFactor;
            set
            {
                if (value != selectedVatFactor)
                {
                    selectedVatFactor = value;
                    OnPropertyChanged(nameof(SelectedVatFactor));
                }
            }
        }

        /// <summary>
        /// Gets the Combined Discount Mirrors
        /// </summary>
        public decimal CombinedDiscount
        {
            get
            {
                decimal combinedDisc = 100m * (1m - ((1m - SelectedPrimaryDiscount / 100m) * (1m - SelectedSecondaryDiscount / 100m) * (1m - SelectedTertiaryDiscount / 100m)));
                return combinedDisc;
            }
        }

        /// <summary>
        /// Gets the Combined Discount for the Cabins
        /// </summary>
        public decimal CombinedDiscountCabin
        {
            get
            {
                decimal combinedDiscCabin = 100m * (1m - ((1m - SelectedPrimaryDiscountCabin / 100m) * (1m - SelectedSecondaryDiscountCabin / 100m) * (1m - SelectedTertiaryDiscountCabin / 100m)));
                return combinedDiscCabin;
            }
        }

        /// <summary>
        /// The White Label Theme (Default = None Normal App)
        /// </summary>
        private WhiteLabelTheme whiteLabel;

        /// <summary>
        /// If the App is in WhiteLabel State
        /// </summary>
        public bool IsWhiteLabeled
        {
            get
            {
                if (whiteLabel != WhiteLabelTheme.None)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The Base URI of the Applications Current Instance
        /// </summary>
        private string baseURI;

        private string accessToken = string.Empty;
        public string GetAccessToken() => accessToken;

        #endregion

        /// <summary>
        /// Constructor of UserService
        /// </summary>
        /// <param name="stateProvider">The Authentication state Provider from MSAL Service</param>
        /// <param name="baseURI">The Base Uri of the Current application Instance</param>
        public BronzeUser(AuthenticationStateProvider stateProvider, string baseURI)
        {
            Console.WriteLine("Initilized User Service");
            Console.WriteLine($"BaseURI: {baseURI}");
            this.baseURI = baseURI;
            whiteLabel = BronzeAppThemeInfo.WhiteLabelThemeByURI.GetValueOrDefault(baseURI); //Returns the Default White Label.None or the Associated Value if it finds one
            this.stateProvider = stateProvider;
        }

        public async Task InitilizeBronzeUser()
        {
            try
            {
                if (IsWhiteLabeled)
                {
                    SetWhiteLabelUser(whiteLabel);
                    SetDefaultAppMode();
                }
                else
                {
                    IsCurrentlyAuthorizing = true;
                    //Get the Auth State
                    var authState = await stateProvider.GetAuthenticationStateAsync();
                    //Assign the Claims
                    GetAuthenticatedUsersClaims(authState);
                    this.stateProvider.AuthenticationStateChanged += StateProvider_AuthenticationStateChanged;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error Initilizing BronzeUser: {ex.Message}");
            }
            finally
            {
                IsCurrentlyAuthorizing = false;
            }
        }

        private void StateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            _ = HandleAuthStateChangedAsync(task);
        }
        private async Task HandleAuthStateChangedAsync(Task<AuthenticationState> task)
        {
            try
            {
                IsCurrentlyAuthorizing = true;
                var authState = await task;
                GetAuthenticatedUsersClaims(authState);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing authentication change: {ex.Message}");
            }
            finally
            {
                IsCurrentlyAuthorizing = false;
            }
        }


        /// <summary>
        /// Sets the Logged Users Claims when authentication State Has Changed
        /// </summary>
        /// <param name="taskAuthState">The Task Containing ther Authentication State</param>
        public void GetAuthenticatedUsersClaims(AuthenticationState authState)
        {
            Console.WriteLine("Retrieving Claims");
            IsCurrentlyAuthorizing = true;
            //THE WHOLE METHOD NEEDS MASSIVE REFACTORING
            //1.The Claims need to be retrieved only once from authstate 
            //2.

            ////Get the auth State from the Task Result
            //AuthenticationState authState = await taskAuthState;

            #region 1.Set User Discounts 
            #region 1.Primary-Secondary-Tertiary Mirrors
            string? discountClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_DiscountApplied")?.Value;
            if (discountClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(discountClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedDiscount);
                if (isParsed)
                {
                    PrimaryDiscount = parsedDiscount;
                }
                else
                {
                    PrimaryDiscount = 0;
                }
            }
            else
            {
                PrimaryDiscount = 0;
            }

            string? secondaryDiscountClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_SecondaryDiscount")?.Value;
            if (secondaryDiscountClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(secondaryDiscountClaim, out decimal parsedDiscount);
                if (isParsed)
                {
                    SecondaryDiscount = parsedDiscount;
                }
                else
                {
                    SecondaryDiscount = 0;
                }
            }

            string? tertiaryDiscountClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_TertiaryDiscount")?.Value;
            if (tertiaryDiscountClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(tertiaryDiscountClaim, out decimal parsedDiscount);
                if (isParsed)
                {
                    TertiaryDiscount = parsedDiscount;
                }
                else
                {
                    TertiaryDiscount = 0;
                }
            }
            #endregion
            #region 2.Primary-Secondary-Tertiary Cabins
            string? discountCabinClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_DiscountAppliedCabin")?.Value;
            if (discountCabinClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(discountCabinClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedDiscount);
                if (isParsed)
                {
                    PrimaryDiscountCabin = parsedDiscount;
                }
                else
                {
                    PrimaryDiscountCabin = 0;
                }
            }
            else
            {
                PrimaryDiscountCabin = 0;
            }

            string? secondaryDiscountCabinClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_SecondaryDiscountCabin")?.Value;
            if (secondaryDiscountCabinClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(secondaryDiscountCabinClaim, out decimal parsedDiscount);
                if (isParsed)
                {
                    SecondaryDiscountCabin = parsedDiscount;
                }
                else
                {
                    SecondaryDiscountCabin = 0;
                }
            }

            string? tertiaryDiscountCabinClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_TertiaryDiscountCabin")?.Value;
            if (tertiaryDiscountCabinClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(tertiaryDiscountCabinClaim, out decimal parsedDiscount);
                if (isParsed)
                {
                    TertiaryDiscountCabin = parsedDiscount;
                }
                else
                {
                    TertiaryDiscountCabin = 0;
                }
            }
            #endregion
            #region 3.Max-Min Retail Discounts
            string? maxRetailDiscountClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_MaximumRetailDiscount")?.Value;
            if (maxRetailDiscountClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(maxRetailDiscountClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedDiscount);
                if (isParsed)
                {
                    MaximumRetailDiscount = (int)(parsedDiscount * 100);
                }
                else
                {
                    MaximumRetailDiscount = 100;
                }
            }

            string? minRetailDiscountClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_MinimumRetailDiscount")?.Value;
            if (minRetailDiscountClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(minRetailDiscountClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedDiscount);
                if (isParsed)
                {
                    MinimumRetailDiscount = (int)(parsedDiscount * 100);
                }
                else
                {
                    MinimumRetailDiscount = 0;
                }
            }
            #endregion
            #endregion
            #region 2.Set User Name
            string? NameClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "name")?.Value;
            if (NameClaim is not null and not "")
            {
                DisplayName = NameClaim;
            }
            else
            {
                DisplayName = "AuthenticatedNamelessUser";
            }

            #endregion
            #region 3.Set PowerUser or Not
            string? isPowerUser = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_IsPowerUser")?.Value;
            if (bool.TryParse(isPowerUser, out bool isPowerUserParsed))
            {
                if (isPowerUserParsed)
                {
                    IsPowerUser = true;
                }
                else
                {
                    IsPowerUser = false;
                }
            }
            else
            {
                IsPowerUser = false;
            }

            #endregion
            #region 4.Set IsRetailFunctionAvailable
            string? isRetailAvailable = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_IsRetailFunctionAvailable")?.Value;
            if (bool.TryParse(isRetailAvailable, out bool isRetailAvailableParsed))
            {
                if (isRetailAvailableParsed)
                {
                    IsRetailFunctionAvailable = true;
                }
                else
                {
                    IsRetailFunctionAvailable = false;
                }
            }
            else
            {
                IsRetailFunctionAvailable = false;
            }
            #endregion
            #region 5.Set IsWholesaleFunctionAvailable
            string? isWholesaleAvailable = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_IsWholesaleFunctionAvailable")?.Value;
            if (bool.TryParse(isWholesaleAvailable, out bool isWholesaleAvailableParsed))
            {
                if (isWholesaleAvailableParsed)
                {
                    IsWholesaleFunctionAvailable = true;
                }
                else
                {
                    IsWholesaleFunctionAvailable = false;
                }
            }
            else
            {
                IsWholesaleFunctionAvailable = false;
            }
            #endregion
            #region 6.Set IsRetailDiscountAdjustable
            string? isRetailDiscAdjustable = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_IsRetailDiscountAdjustable")?.Value;
            if (bool.TryParse(isRetailDiscAdjustable, out bool isRetailDiscAdjustableParsed))
            {
                if (isRetailDiscAdjustableParsed)
                {
                    IsRetailDiscountAdjustable = true;
                }
                else
                {
                    IsRetailDiscountAdjustable = false;
                }
            }
            else
            {
                IsRetailDiscountAdjustable = false;
            }
            #endregion
            #region 7.Set IsRetailDiscountVisible
            string? isRetailDiscVisible = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_IsRetailDiscountVisible")?.Value;
            if (bool.TryParse(isRetailDiscVisible, out bool isRetailDiscVisibleParsed))
            {
                if (isRetailDiscVisibleParsed)
                {
                    IsRetailDiscountVisible = true;
                }
                else
                {
                    IsRetailDiscountVisible = false;
                }
            }
            else
            {
                IsRetailDiscountVisible = false;
            }
            #endregion
            #region 8.Set RetailPriceIncreaseFactor
            string? increaseFactorClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_RetailPriceIncreaseFactor")?.Value;
            if (increaseFactorClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(increaseFactorClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedIncreaseFactor);
                if (isParsed)
                {
                    RetailPriceIncreaseFactor = parsedIncreaseFactor;
                }
                else
                {
                    RetailPriceIncreaseFactor = 1;
                }
            }
            else
            {
                RetailPriceIncreaseFactor = 1;
            }
            #endregion
            #region 9.Set RetailTheme
            string? retailThemeClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_RetailModeTheme")?.Value;
            if (retailThemeClaim is not null and not "")
            {
                foreach (RetailModeTheme value in Enum.GetValues<RetailModeTheme>())
                {
                    if (value.ToString() == retailThemeClaim)
                    {
                        RetailTheme = value;
                        break; // Exit the Loop we found our Value
                    }
                }
                //If there is no matching value the Retail Theme defaults to "RetailModeTheme.None"
            }
            else
            {
                RetailTheme = RetailModeTheme.None;
            }
            #endregion
            #region 10.SetVatFactor
            string? vatFactorClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "extension_DefaultVatFactor")?.Value;
            if (vatFactorClaim is not null and not "")
            {
                bool isParsed = decimal.TryParse(vatFactorClaim, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out decimal parsedVatFactor);
                if (isParsed)
                {
                    VatFactor = parsedVatFactor;
                }
                else
                {
                    VatFactor = 0;
                }
            }
            else
            {
                VatFactor = 0;
            }
            #endregion
            #region 11.SetUser Email
            string? emailClaim = authState.User.Claims.SingleOrDefault(c => c.Type == "emails")?.Value.Trim('\"', '[', ']');
            if (!string.IsNullOrEmpty(emailClaim))
            {
                UserEmail = emailClaim;
            }
            #endregion

            #region 12.IsAuthenticated

            IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

            #endregion

            //foreach (var item in authState.User.Claims)
            //{
            //    Console.WriteLine($"{item.ToString()}--{item.Value}");
            //}

            ////Notify any Subscribers that the Authentication State Has Changed
            //OnAuthenticationStateChanged?.Invoke();
            SetDefaultAppMode();
            IsCurrentlyAuthorizing = false;

            //TEST TO SEE ALL CLAIMS RETRIEVED
            //foreach (var item in authState.User.Claims)
            //{
            //    Console.WriteLine($"{item.ToString()}{item.Value}");
            //}
            //Console.WriteLine($"IsInROLEAdmin: {authState.User.IsInRole("Admin")}");
            //Console.WriteLine($"IsInROLEClient: {authState.User.IsInRole("Client")}");
        }

        #region 3.Initilization Methods

        /// <summary>
        /// Runs at each Authentication or Application Restart
        /// </summary>
        private void SetDefaultAppMode()
        {
            if (IsWhiteLabeled)
            {
                SelectedAppMode = BronzeAppMode.Retail;
            }
            else
            {
                if ((IsRetailFunctionAvailable && IsWholesaleFunctionAvailable) || IsPowerUser)
                {
                    SelectedAppMode = BronzeAppMode.Wholesale;
                }
                else if (IsRetailFunctionAvailable)
                {
                    SelectedAppMode = BronzeAppMode.Retail;
                }
                else if (IsWholesaleFunctionAvailable)
                {
                    SelectedAppMode = BronzeAppMode.Wholesale;
                }
                else
                {
                    SelectedAppMode = BronzeAppMode.Guest;
                }
            }
        }

        /// <summary>
        /// Sets the Default Properties for the Current Selected Mode
        /// Executes whenever the Selected Mode Changes
        /// </summary>
        private void SetDefaultUserSelectedProperties()
        {
            switch (SelectedAppMode)
            {
                case BronzeAppMode.Retail:
                    SelectedRetailTheme = RetailTheme;
                    SelectedPriceIncreaseFactor = RetailPriceIncreaseFactor;
                    SelectedPriceIncreaseFactorCabins = RetailPriceIncreaseFactorCabins;
                    IsPricingVisible = true;
                    IsDiscountVisible = IsRetailDiscountVisible;
                    IsDiscountAdjustable = IsRetailDiscountAdjustable;
                    MaximumDiscount = MaximumRetailDiscount;
                    MinimumDiscount = MinimumRetailDiscount;
                    SelectedPrimaryDiscount = MinimumRetailDiscount;
                    SelectedSecondaryDiscount = 0;
                    SelectedTertiaryDiscount = 0;
                    SelectedPrimaryDiscountCabin = 0;
                    SelectedSecondaryDiscountCabin = 0;
                    SelectedTertiaryDiscountCabin = 0;
                    SelectedVatFactor = VatFactor;
                    break;
                case BronzeAppMode.Wholesale:
                    SelectedRetailTheme = RetailTheme;
                    SelectedPriceIncreaseFactor = 1;
                    SelectedPriceIncreaseFactorCabins = 1;
                    IsPricingVisible = true;
                    IsDiscountVisible = true;
                    IsDiscountAdjustable = IsPowerUser; //Adjustable only for Power Users
                    MaximumDiscount = 100;
                    MinimumDiscount = 0;
                    SelectedPrimaryDiscount = PrimaryDiscount;
                    SelectedSecondaryDiscount = SecondaryDiscount;
                    SelectedTertiaryDiscount = TertiaryDiscount;
                    SelectedPrimaryDiscountCabin = PrimaryDiscountCabin;
                    SelectedSecondaryDiscountCabin = SecondaryDiscountCabin;
                    SelectedTertiaryDiscountCabin = TertiaryDiscountCabin;
                    //SelectedVatFactor = VatFactor; Irrelevant Vat is not Visible in Wholesale
                    break;
                default:
                case BronzeAppMode.Guest:
                    SelectedRetailTheme = RetailModeTheme.None;
                    SelectedPriceIncreaseFactor = 1;
                    SelectedPriceIncreaseFactorCabins = 1;
                    IsPricingVisible = false;
                    IsDiscountVisible = false;
                    IsDiscountAdjustable = false;
                    MaximumDiscount = 0;
                    MinimumDiscount = 0;
                    SelectedPrimaryDiscount = 0;
                    SelectedSecondaryDiscount = 0;
                    SelectedTertiaryDiscount = 0;
                    SelectedPrimaryDiscountCabin = 0;
                    SelectedSecondaryDiscountCabin = 0;
                    SelectedTertiaryDiscountCabin = 0;
                    MinimumDiscount = 0;
                    MaximumDiscount = 0;
                    //SelectedVatFactor = VatFactor; Irrelevant
                    break;
            }
        }


        /// <summary>
        /// Sets the User Claims for a Provided White Label Theme or None if No White Label is Requested
        /// </summary>
        /// <param name="whiteLabel">The White Label for Which to Set the Theme</param>
        private void SetWhiteLabelUser(WhiteLabelTheme whiteLabel)
        {
            switch (whiteLabel)
            {

                case WhiteLabelTheme.Papapolitis:
                    PrimaryDiscount = 0m;
                    SecondaryDiscount = 0m;
                    TertiaryDiscount = 0m;
                    PrimaryDiscountCabin = 0m;
                    SecondaryDiscountCabin = 0m;
                    TertiaryDiscountCabin = 0m;
                    RetailDiscount = 27;
                    MaximumRetailDiscount = 27;
                    MinimumRetailDiscount = 27;
                    IsRetailDiscountAdjustable = false;
                    DisplayName = "GuestState";
                    IsPowerUser = false;
                    IsRetailFunctionAvailable = true;
                    IsWholesaleFunctionAvailable = false;
                    IsRetailDiscountVisible = true;
                    RetailPriceIncreaseFactor = 1.379m;
                    RetailTheme = RetailModeTheme.Papapolitis;
                    VatFactor = 1.24m;
                    break;
                case WhiteLabelTheme.None:
                case WhiteLabelTheme.Lakiotis:
                    PrimaryDiscount = 0m;
                    SecondaryDiscount = 0m;
                    TertiaryDiscount = 0m;
                    PrimaryDiscountCabin = 0m;
                    SecondaryDiscountCabin = 0m;
                    TertiaryDiscountCabin = 0m;
                    RetailDiscount = 0m;
                    MaximumRetailDiscount = 0;
                    MinimumRetailDiscount = 0;
                    IsRetailDiscountAdjustable = false;
                    DisplayName = "GuestState";
                    IsPowerUser = false;
                    IsRetailFunctionAvailable = true;
                    IsWholesaleFunctionAvailable = false;
                    IsRetailDiscountVisible = false;
                    RetailPriceIncreaseFactor = 1;
                    RetailTheme = RetailModeTheme.Lakiotis;
                    VatFactor = 1.24m;
                    break;
                default:
                    //Do Nothing Do not Set any Values //Simple User Without Privilages
                    break;
            }
        }

        #endregion


        /// <summary>
        /// Event on Authentication State Change
        /// </summary>
        //public event Action OnAuthenticationStateChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When the Service is Disposed Unsubscribe
        /// </summary>
        public void Dispose()
        {
            if (stateProvider != null)
            {
                this.stateProvider.AuthenticationStateChanged -= StateProvider_AuthenticationStateChanged;
            }
            GC.SuppressFinalize(this);
        }
    }
}
