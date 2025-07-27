using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using MementosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Services.SaveToStorageServices
{
#nullable enable
    public class BasketSaveService
    {
        private readonly ILocalStorageService ls;
        private readonly IMessageService ms;
        private readonly ILanguageContainerService lc;
        private readonly BasketViewModel basket;
        public const string savedBasketsKey = "SavedBaskets";

        public event EventHandler? IsBusyChanged;
        public event EventHandler? SavedBasketsChanged;
        public event EventHandler? BasketRestored;
        private void OnSavedBasketsChanged()
        {
            SavedBasketsChanged?.Invoke(this, EventArgs.Empty);
        }


        private List<BasketSave> savedBaskets = new List<BasketSave>();
        /// <summary>
        /// Weather the cached list of saved baskets is dirty
        /// </summary>
        private bool isSavedBasketsDirty = true;

        private bool isBusy;
        public bool IsBusy 
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    IsBusyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public BasketSaveService(
            ILocalStorageService ls,
            BasketViewModel basket,
            IMessageService ms,
            ILanguageContainerService lc)
        {
            this.ls = ls;
            this.basket = basket;
            this.ms = ms;
            this.lc = lc;
        }

        /// <summary>
        /// Gets all the Saved Baskets from Local Storage
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Error on Retrieving</exception>
        public async Task<List<BasketSave>> GetSavedBaskets()
        {
            // If already retrieved no need to do it again
            if (isSavedBasketsDirty)
            {
                try
                {
                    IsBusy = true;
                    savedBaskets = await ls.GetItemAsync<List<BasketSave>>(savedBasketsKey) ?? new();
                    isSavedBasketsDirty = false;
                    OnSavedBasketsChanged();
                    return savedBaskets;
                }
                catch (Exception ex)
                {
                    isSavedBasketsDirty = true;
                    await ms.ErrorAsync(lc.Keys["Info"], lc.Keys["BasketRetrievalError"]);
                    Console.WriteLine(ex.Message);
                    return new();
                }
                finally
                {
                    IsBusy = false;
                }
            }
            else
            {
                return savedBaskets;
            }
        }
        /// <summary>
        /// Saves the current Basket State
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Error While Saving</exception>
        public async Task<bool> SaveBasket()
        {
            var allBaskets = await GetSavedBaskets();

            //if still dirty means there was an error , so return
            if (isSavedBasketsDirty) return false;


            var save = basket.GetTransformation();
            
            if (allBaskets.Any(b=> b.BasketName == save.BasketName))
            {
                //Ask if 
                var msgResult = await ms.QuestionAsync(lc.Keys["Info"], lc.Keys["OverwriteBasket"], lc.Keys[IMessageService.DialogYes], lc.Keys[IMessageService.DialogNo]);
                if (msgResult == MessageResult.Cancel) return false;

                //Replace if user requested it
                var basketWithSameName = allBaskets.First(b => b.BasketName == save.BasketName);
                var indexOfSameNameBasket = allBaskets.IndexOf(basketWithSameName);
                allBaskets[indexOfSameNameBasket] = save;
            }
            else
            {
                allBaskets.Add(save);
            }
                        
            try
            {
                IsBusy = true;
                await ls.SetItemAsync(savedBasketsKey, allBaskets);
                OnSavedBasketsChanged();
                return true;
            }
            catch (Exception ex)
            {
                await ms.ErrorAsync(lc.Keys["Info"], lc.Keys["BasketSaveError"]);
                Console.WriteLine(ex.Message);
                return false;
            }
            finally { IsBusy = false; }
        }
        /// <summary>
        /// Removes a Saved Basket
        /// </summary>
        /// <param name="basketName">The Name of the Basket</param>
        /// <returns></returns>
        public async Task RemoveSavedBasket(string basketName)
        {
            var allBaskets = await GetSavedBaskets();

            //if still dirty means there was an error , so return
            if (isSavedBasketsDirty) return;

            if (allBaskets.Any(b => b.BasketName == basketName))
            {
                //Ask if 
                var msgResult = await ms.QuestionAsync(lc.Keys["Info"], lc.Keys["DeleteBasketQuestion"], lc.Keys[IMessageService.DialogYes], lc.Keys[IMessageService.DialogNo]);
                if (msgResult == MessageResult.Cancel) return;

                //Replace if user requested it
                var basketWithSameName = allBaskets.First(b => b.BasketName == basketName);
                allBaskets.Remove(basketWithSameName);
            }
            else
            {
                await ms.ErrorAsync(lc.Keys["Info"], $"The Specified Basket:'{basketName}' was not Found...");
                return;
            }

            try
            {
                IsBusy = true;
                await ls.SetItemAsync(savedBasketsKey, allBaskets);
                OnSavedBasketsChanged();
            }
            catch (Exception ex)
            {
                await ms.ErrorAsync(lc.Keys["Info"], lc.Keys["BasketSaveError"]);
                Console.WriteLine(ex.Message);
            }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Restores a selected Basket
        /// </summary>
        /// <param name="save"></param>
        public void RestoreSavedBasket(BasketSave save)
        {
            basket.Restore(save);
            BasketRestored?.Invoke(this, EventArgs.Empty);
        }
    }

    public class BasketSave
    {
        public List<BasketItemSave> SavedProducts { get; set; } = new();
        public string BasketName { get; set; } = string.Empty;
        public string BasketNotes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; } 
        public decimal TotalAmounRetail { get; set; }
        /// <summary>
        /// If empty uses Defaults of the current user
        /// </summary>
        public string SelectedAccessoriesOptions { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        /// <summary>
        /// If user is Authed then the Prices saved in products are the ones he chose 
        /// Otherwise all prices saved are zero which means they should be restored to normal discounting if the offer is retrieved when authed.
        /// </summary>
        public bool IsUserAuthenticated { get; set; }
    }

    public class BasketItemSave
    {
        public string PriceableId { get; set; } = string.Empty;
        public string SelectedFinishId { get; set; } = string.Empty;
        public bool IsCodeOverriden { get; set; }
        public string OveriddenCode { get; set; } = string.Empty;
        public string ItemNotes { get; set; } = string.Empty;
        /// <summary>
        /// Needed as when the language changes the saved Description appears as overriden
        /// </summary>
        public bool IsDescriptionOveridden { get; set; }
        public string OveriddenDescription { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal CustomizedPrice { get; set; }
        public decimal PrimaryDiscountPercent { get; set; }
        public decimal SecondaryDiscountPercent { get; set; }
        public decimal TertiaryDiscountPercent { get; set; }
        public decimal AdditionalRulesDiscountPercent { get; set; }
        /// <summary>
        /// Weather the Restoration Process should apply Default Discounting or the Customized ones
        /// </summary>
        public bool AppliesDefaultDiscounting { get; set; }
        /// <summary>
        /// Weather the Restoration Process should apply Default Pricing or the Customized ones
        /// </summary>
        public bool AppliesDefaultStartingPrice { get; set; }
        public bool AreExceptionRulesDisabled { get; set; }

        public decimal RetailCustomizedPrice { get; set; }
        public decimal RetailDiscountPercent { get; set; }
        public bool RetailAppliesDefaultStartingPrice { get; set; }

    }
}
