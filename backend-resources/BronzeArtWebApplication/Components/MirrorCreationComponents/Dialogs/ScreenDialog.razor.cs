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
    public partial class ScreenDialog : ComponentBase, IDisposable
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool HasClock { get; set; }
        [Parameter] public EventCallback<bool> HasClockChanged { get; set; }

        [Parameter] public bool HasBluetooth { get; set; }
        [Parameter] public EventCallback<bool> HasBluetoothChanged { get; set; }

        [Parameter] public bool HasDisplay11 { get; set; }
        [Parameter] public EventCallback<bool> HasDisplay11Changed { get; set; }

        [Parameter] public bool HasDisplay11Black { get; set; }
        [Parameter] public EventCallback<bool> HasDisplay11BlackChanged { get; set; }

        [Parameter] public bool HasDisplay19 { get; set; }
        [Parameter] public EventCallback<bool> HasDisplay19Changed { get; set; }

        [Parameter] public bool HasDisplay20 { get; set; }
        [Parameter] public EventCallback<bool> HasDisplay20Changed { get; set; }

        [Parameter] public EventCallback<MirrorLight> OnNextClick { get; set; }

        [Parameter] public EventCallback OnPreviousClick { get; set; }

        [Parameter] public decimal PriceIncreaseFactor { get; set; }
        [Parameter] public List<MirrorOption> SelectableScreenOptions { get; set; } = [];

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

        /// <summary>
        /// Weather the specififed media option is enabled for selection from the User
        /// </summary>
        /// <param name="mediaOption"></param>
        /// <returns></returns>
        private bool IsMediaEnabled(MirrorOption mediaOption)
        {
            return SelectableScreenOptions.Any(o => mediaOption == o);
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
