using SqliteApplicationSettings.DTOs;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService.SearchOrdersViewSettingsService
{
    public partial class SearchOrdersViewSettings : ObservableObject
    {
        /// <summary>
        /// The Maximum Results when retrievin the Small Orders Result
        /// </summary>
        [ObservableProperty]
        private int maxResultsGetSmallOrders;

        public SearchOrdersViewSettings()
        {

        }
        public SearchOrdersViewSettings(SearchOrdersViewSettingsDTO dto)
        {
            this.MaxResultsGetSmallOrders = dto.MaxResultsGetSmallOrders;
        }

        public SearchOrdersViewSettingsDTO ToDto()
        {
            return new SearchOrdersViewSettingsDTO()
            {
                MaxResultsGetSmallOrders = MaxResultsGetSmallOrders
            };
        }
    }

}
