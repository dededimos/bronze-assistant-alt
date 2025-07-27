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
    public partial class VariousExtrasDialog : ComponentBase , IDisposable
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool HasLid { get; set; }

        [Parameter] public EventCallback<bool> HasLidChanged { get; set; }

        [Parameter] public bool HasRounding { get; set; }
        [Parameter] public EventCallback<bool> HasRoundingChanged { get; set; }

        [Parameter] public EventCallback<MirrorLight> OnNextClick { get; set; }

        [Parameter] public EventCallback OnPreviousClick { get; set; }

        [Parameter] public decimal PriceIncreaseFactor { get; set; }
        [Parameter] public List<MirrorOption> SelectableExtras { get; set; } = [];

        /// <summary>
        /// Weather the specififed extra option is enabled for selection from the User
        /// </summary>
        /// <param name="extraOption"></param>
        /// <returns></returns>
        private bool IsExtraEnabled(MirrorOption extraOption)
        {
            return SelectableExtras.Any(o => extraOption == o);
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