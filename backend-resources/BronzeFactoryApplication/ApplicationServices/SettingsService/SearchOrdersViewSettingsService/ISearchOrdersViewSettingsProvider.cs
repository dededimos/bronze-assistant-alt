namespace BronzeFactoryApplication.ApplicationServices.SettingsService.SearchOrdersViewSettingsService
{
    public interface ISearchOrdersViewSettingsProvider
    {
        /// <summary>
        /// Gets the Default Settings
        /// </summary>
        /// <returns></returns>
        Task<SearchOrdersViewSettings> GetDefaultsAsync();
        /// <summary>
        /// Gets the Current Settings
        /// </summary>
        /// <returns></returns>
        Task<SearchOrdersViewSettings> GetSettingsAsync();
        /// <summary>
        /// Restores the Current Settings to Defaults and returns them
        /// </summary>
        /// <returns>The Current Settings that have been set to Default</returns>
        Task<SearchOrdersViewSettings> RestoreDefaultsAsync();
        /// <summary>
        /// Saves a settings object
        /// </summary>
        /// <param name="settings">The settings to save</param>
        /// <returns></returns>
        Task SaveSettingsAsync(SearchOrdersViewSettings settings);
    }
}