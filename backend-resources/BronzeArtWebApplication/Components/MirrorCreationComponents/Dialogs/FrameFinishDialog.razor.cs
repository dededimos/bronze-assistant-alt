using BronzeArtWebApplication.Shared.Models;
using BronzeRulesPricelistLibrary;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Helpers;
using MirrorsModelsLibrary.Models;
using System;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.Dialogs
{
    public partial class FrameFinishDialog : ComponentBase, IDisposable
    {
        //If the Dialog Is Visible Property
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        //The Selected Type Finish from the User
        [Parameter] public SupportFinishType? SelectedFinishType { get; set; }
        [Parameter] public EventCallback<SupportFinishType?> SelectedFinishTypeChanged { get; set; }

        //The Selected Paint Finish
        [Parameter] public SupportPaintFinish? SelectedPaintFinish { get; set; }
        [Parameter] public EventCallback<SupportPaintFinish?> SelectedPaintFinishChanged { get; set; }

        //The Dialog's Actiona Buttons Parameters
        [Parameter] public EventCallback OnPreviousClick { get; set; }
        [Parameter] public EventCallback OnNextClick { get; set; }

        [Parameter] public int? MirrorLength { get; set; }
        [Parameter] public int? MirrorHeight { get; set; }
        [Parameter] public decimal PriceIncreaseFactor { get; set; }

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // If SelectedFinishType is null, set it to Painted and notify the parent
            if (SelectedFinishType == null && IsVisible)
            {
                SelectedFinishType = SupportFinishType.Painted;
                // Notify the parent component about the change
                SelectedFinishTypeChanged.InvokeAsync(SelectedFinishType);
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsPricingVisible))
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Gets the Tooltip Price String of the Painted Frame Finish
        /// </summary>
        /// <param name="paintFinish"></param>
        /// <returns></returns>
        private string GetPaintedFramePrice(SupportPaintFinish paintFinish)
        {
            string priceString;
            if (MirrorHeight is not null && MirrorLength is not null)
            {
                decimal price = 99999;
                if (paintFinish is SupportPaintFinish.Black or SupportPaintFinish.RalColor)
                {
                    price = MirrorsPricelist.GetPaintedBlackRalFramePrice((int)MirrorLength, (int)MirrorHeight, paintFinish) * PriceIncreaseFactor;
                    price += paintFinish is SupportPaintFinish.RalColor ? SupportModel.RalColorAdditionalPrice : 0;
                }
                else 
                {
                    price = MirrorsPricelist.GetSimilarElectroplatedFramePrice((int)MirrorLength, (int)MirrorHeight, paintFinish) * PriceIncreaseFactor;
                }

                    priceString = price.ToString("0.00\u20AC");
            }
            else
            {
                priceString = lc.Keys["DimensionsNotSet"];
            }
            return priceString;
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
