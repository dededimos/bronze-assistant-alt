using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents
{
    public partial class MirrorPricingTable : ComponentBase , IDisposable
    {
        [Parameter] public List<IPriceable> Priceables { get; set; }
        [Parameter] public bool WithPrintStyle { get; set; } = false;

        /// <summary>
        /// The Class of the Pricing Table Component
        /// </summary>
        [Parameter] public string Class { get; set; }

        private bool readOnly = true;

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsPricingVisible)
                               or nameof(BronzeUser.SelectedAppMode)
                               or nameof(BronzeUser.IsPowerUser))
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Makes the Catalogue Total PriceText StrikenThrough when in Retail Mode
        /// </summary>
        private string CataloguePriceStyle
        {
            get
            {
                string style = "";
                if (user.SelectedAppMode == BronzeAppMode.Retail)
                {
                    style += "text-decoration:line-through;";
                }
                return style;
            }
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
