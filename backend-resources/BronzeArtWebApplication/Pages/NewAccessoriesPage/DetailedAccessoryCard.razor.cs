using BathAccessoriesModelsLibrary;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage
{
#nullable enable
    public partial class DetailedAccessoryCard : IDisposable
    {
        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.CodeParamName)]
        public string? Code { get; set; } 

        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.FinishParamName)]
        public string? InitialShownAccessoryFinishCode { get; set; }

        private BathroomAccessory? accessory;
        private string carouselHoveredImage = string.Empty;
        private AccessoryFinish selectedFinish = AccessoryFinish.Empty();
        private string hoveredDimensionPhotoUrl = string.Empty;

        protected override void OnInitialized()
        {
            Basket.PropertyChanged += Basket_PropertyChanged;
        }

        private void Basket_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BasketViewModel.PricesEnabled))
            {
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            if(!string.IsNullOrEmpty(Code)) accessory = repo.GetAccessoryByCode(Code);
            if (accessory is null) return;

            if (string.IsNullOrEmpty(InitialShownAccessoryFinishCode))
            {
                InitialShownAccessoryFinishCode = accessory.BasicFinish.Code;
            }
            selectedFinish = accessory.AvailableFinishes.FirstOrDefault(af => af.Finish.Code == InitialShownAccessoryFinishCode)
                ?? (accessory.AvailableFinishes.FirstOrDefault() ?? AccessoryFinish.Empty());
        }

        /// <summary>
        /// Triggers the Download of a Blob Image
        /// </summary>
        /// <param name="blobNameOrUrl"></param>
        /// <returns></returns>
        private async Task DownloadBlob(string blobNameOrUrl)
        {
            try
            {
                string blobName = photosHelper.RemoveContainerPathFromUrl(blobNameOrUrl);
                var sasUri = await Api.GetAccessoriesBlobDownloadUri(blobName);
                await Js.TriggerFileDownload($"{accessory?.Code ?? "photo"}-{DateTime.Now}.jpg", sasUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            Basket.PropertyChanged -= Basket_PropertyChanged;
            GC.SuppressFinalize(this);
        }
    }
}
