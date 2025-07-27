using BronzeArtWebApplication.Pages.CabinsPage.Components.Dialogs;
using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.InputBoxes
{
    public partial class DiscountInputBox : ComponentBase , IDisposable
    {
        //Visibility Rules :
        //1.Extra Discounts Visible Only on Wholesale
        //2.Max Extra Discounts 100 Min 0
        //3.General Discount Visible On WholeSale and based on User Property for Retail
        //4.General Discount Adjustable Wholesale:Only For Power Users , Retail: Only when not White Labeled
        //5.Simple Guests nor see or adjust discount
        //6.Max General Discount : 0-100 for wholesale -- Based on Option for Retail

        [Parameter] public decimal CombinedDiscount { get; set; }
        [Parameter] public EventCallback<decimal> CombinedDiscountChanged { get; set; }

        private async Task SetCombinedDiscountParameter(decimal newCombinedDiscount)
        {
            if (CombinedDiscount != newCombinedDiscount)
            {
                CombinedDiscount = newCombinedDiscount;
                PrimaryDiscount = newCombinedDiscount;
                SecondaryDiscount = 0;
                TertiaryDiscount = 0;
                await PrimaryDiscountChanged.InvokeAsync(newCombinedDiscount);
                await SecondaryDiscountChanged.InvokeAsync(0);
                await TertiaryDiscountChanged.InvokeAsync(0);
                await CombinedDiscountChanged.InvokeAsync(newCombinedDiscount);
            }
        }

        [Parameter] public decimal PrimaryDiscount { get; set; }
        [Parameter] public EventCallback<decimal> PrimaryDiscountChanged { get; set; }
        private async Task SetPrimaryDiscountParameter(decimal newPrimaryDiscount)
        {
            if (PrimaryDiscount != newPrimaryDiscount)
            {
                PrimaryDiscount = newPrimaryDiscount;
                CalculateCombinedDiscount();
                await PrimaryDiscountChanged.InvokeAsync(newPrimaryDiscount);
            }
        }

        [Parameter] public decimal SecondaryDiscount { get; set; }
        [Parameter] public EventCallback<decimal> SecondaryDiscountChanged { get; set; }
        private async Task SetSecondaryDiscountParameter(decimal newSecondaryDiscount)
        {
            if (SecondaryDiscount != newSecondaryDiscount)
            {
                SecondaryDiscount = newSecondaryDiscount;
                CalculateCombinedDiscount();
                await SecondaryDiscountChanged.InvokeAsync(newSecondaryDiscount);
            }
        }
        
        [Parameter] public decimal TertiaryDiscount { get; set; }
        [Parameter] public EventCallback<decimal> TertiaryDiscountChanged { get; set; }
        private async Task SetTertiaryDiscountParameter(decimal newTertiaryDiscount)
        {
            if (TertiaryDiscount != newTertiaryDiscount)
            {
                TertiaryDiscount = newTertiaryDiscount;
                CalculateCombinedDiscount();
                await TertiaryDiscountChanged.InvokeAsync(newTertiaryDiscount);
            }
        }

        /// <summary>
        /// Wheather the Extra Discount Boxes are Visible
        /// </summary>
        [Parameter] public bool? AreExtraDiscountsVisible { get; set; }
        [Parameter] public bool AreDiscountBoxesHorizontal { get; set; }
        /// <summary>
        /// Weather there are Labels in the Discount Boxes , If false Boxes will be stripped of Labaling and a General One Will be present on top of it
        /// </summary>
        [Parameter] public bool AreLabelsOnControl { get; set; }

        [Parameter] public int MarginDiv { get; set; } = 2;
        [Parameter] public int PaddingDiv { get; set; } = 2;
        /// <summary>
        /// If set to true Min and Max Discounts in the Discount box are 0 and 100 Respectivly , regardless of User Settings
        /// </summary>
        [Parameter] public bool IsMinMaxDiscountDisabled { get; set; } = false;

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        protected override void OnParametersSet()
        {
            //To Always show the correct discount
            CalculateCombinedDiscount();
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.SelectedAppMode)
                               or nameof(BronzeUser.IsDiscountVisible)
                               or nameof(BronzeUser.MaximumDiscount)
                               or nameof(BronzeUser.IsDiscountAdjustable)
                               or nameof(BronzeUser.MaximumDiscount)
                               or nameof(BronzeUser.MinimumDiscount))
            {
                StateHasChanged();
            }
        }


        /// <summary>
        /// Calculates the Combined Discount by the Primary/Secondary/Tertiary Discounts
        /// </summary>
        private void CalculateCombinedDiscount()
        {
            var combinedDiscount = 100 * (1m - (1 - (PrimaryDiscount) / 100m) * (1 - (SecondaryDiscount) / 100m) * (1 - (TertiaryDiscount) / 100m));
            if(combinedDiscount != CombinedDiscount)
            {
                CombinedDiscount = combinedDiscount;
                CombinedDiscountChanged.InvokeAsync(combinedDiscount);
            }
        }

        /// <summary>
        /// Checks Wheather the Disccount box is Disabled for Interaction
        /// </summary>
        /// <returns></returns>
        private bool IsDiscountDisabled()
        {
            //Return the oposite of Adjustable
            return !user.IsDiscountAdjustable;
        }

        /// <summary>
        /// Checks weather to show the Extra Discount Boxes (Only for Wholesale Mode)
        /// </summary>
        /// <returns></returns>
        private bool AreExtraDiscountBoxesVisible()
        {
            if (AreExtraDiscountsVisible != null)
            {
                return (bool)AreExtraDiscountsVisible;
            }
            else
            {
                return user.SelectedAppMode switch
                {
                    Shared.Enums.BronzeAppMode.Wholesale => true,
                    _ => false,
                };
            }
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
