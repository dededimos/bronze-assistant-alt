using BronzeArtWebApplication.Shared.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.Dialogs
{
    public partial class MagnifyerDialog : ComponentBase, IDisposable
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool HasMagnifyer { get; set; }
        [Parameter] public EventCallback<bool> HasMagnifyerChanged { get; set; }

        [Parameter] public bool HasMagnifyerLed { get; set; }
        [Parameter] public EventCallback<bool> HasMagnifyerLedChanged { get; set; }

        [Parameter] public bool HasMagnifyerLedTouch { get; set; }
        [Parameter] public EventCallback<bool> HasMagnifyerLedTouchChanged { get; set; }

        [Parameter] public EventCallback<MirrorLight> OnNextClick { get; set; }
        [Parameter] public EventCallback OnPreviousClick { get; set; }

        [Parameter] public decimal PriceIncreaseFactor { get; set; }
        [Parameter] public List<MirrorOption> SelectableMagnifyerOptions { get; set; } = [];

        /// <summary>
        /// Weather the specififed magnifier option is enabled for selection from the User
        /// </summary>
        /// <param name="magnifierOption"></param>
        /// <returns></returns>
        private bool IsMagnifierEnabled(MirrorOption magnifierOption)
        {
            return SelectableMagnifyerOptions.Any(o => magnifierOption == o);
        }

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsPricingVisible))
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
