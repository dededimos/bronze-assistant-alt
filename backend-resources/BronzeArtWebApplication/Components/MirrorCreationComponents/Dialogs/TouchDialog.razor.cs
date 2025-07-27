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
    public partial class TouchDialog :ComponentBase , IDisposable
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool HasSwitch { get; set; }
        [Parameter] public EventCallback<bool> HasSwitchChanged { get; set; }

        [Parameter] public bool HasDimmer { get; set; }
        [Parameter] public EventCallback<bool> HasDimmerChanged { get; set; }

        [Parameter] public bool HasSensor { get; set; }
        [Parameter] public EventCallback<bool> HasSensorChanged { get; set; }

        [Parameter] public EventCallback<MirrorLight> OnNextClick { get; set; }

        [Parameter] public EventCallback OnPreviousClick { get; set; }

        [Parameter] public decimal PriceIncreaseFactor { get; set; }
        [Parameter] public List<MirrorOption> SelectableTouchSwitches { get; set; } = [];

        private static decimal ExtraPrice(MirrorOption option)
        {
            if (option == MirrorOption.TouchSwitch)
            {
                return 0.00m;
            }
            else
            {
                MirrorExtra extra = new(option);
                decimal price = extra.GetPrice();
                return price;
            }
        }

        /// <summary>
        /// Weather the specififed touch option is enabled for selection from the User
        /// </summary>
        /// <param name="touchOption"></param>
        /// <returns></returns>
        private bool IsTouchEnabled(MirrorOption touchOption)
        {
            return SelectableTouchSwitches.Any(o => touchOption == o);
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