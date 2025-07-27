using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System;

namespace BronzeArtWebApplication.Components.Various.VariousDialogs
{
    public partial class BronzeAppModeOptionsDialog : ComponentBase
    {
        /// <summary>
        /// Controls Wheather the Dialog Is Visible
        /// </summary>
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public EventCallback OnSaveAndCloseClick { get; set; }
        [Parameter] public EventCallback OnCloseWithoutSaveClick { get; set; }

        /// <summary>
        /// The Multiplier of the Catalogue Prices
        /// </summary>
        [Parameter] public decimal RetailPriceIncreaseFactor { get; set; }
        [Parameter] public EventCallback<decimal> RetailPriceIncreaseFactorChanged { get; set; }

        /// <summary>
        /// Wheather the Price Increase Factor Can Be Edited
        /// </summary>
        [Parameter] public bool IsPriceIncreaseFactorEditable { get; set; }

        [Parameter] public int MaxRetailDiscount { get; set; }
        [Parameter] public int MinRetailDiscount { get; set; }

        /// <summary>
        /// Wheather the Retail theme can be Edited
        /// </summary>
        [Parameter] public bool IsRetailThemeEditable { get; set; }
        [Parameter] public BronzeAppMode CurrentMode { get; set; }

        [Parameter] public RetailModeTheme RetailTheme { get; set; }
        [Parameter] public EventCallback<RetailModeTheme> RetailThemeChanged { get; set; }

        ///// <summary>
        ///// Save Options and Close the Dialog
        ///// </summary>
        //private async void SaveAndCloseAsync()
        //{
        //    //await localStorage.SetItemAsync("MaxRetailDiscount", MaxRetailDiscount);
        //    IsVisible = false;
        //}

        /// <summary>
        /// Close the Dialog
        /// </summary>
        private void CloseWithoutSave()
        {
            IsVisible = false;
        }


    }
}
