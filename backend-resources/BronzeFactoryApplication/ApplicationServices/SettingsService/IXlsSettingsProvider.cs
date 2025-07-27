using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService
{
    public interface IXlsSettingsProvider
    {
        /// <summary>
        /// Creates new Settings 
        /// </summary>
        /// <param name="settings">The New Settings to Create</param>
        /// <returns>The Inserted Id</returns>
        Task<int> AddNewXlsSettingsAsync(XlsSettingsGlasses settings);
        /// <summary>
        /// Deletes a Setting by Id
        /// </summary>
        /// <param name="id">The Id of the setting to Delete</param>
        /// <returns></returns>
        /// <exception cref="Exception">When no setting is found with that Id</exception>
        Task DeleteXlsSettingsAsync(int id);
        /// <summary>
        /// Deletes a Setting By Name
        /// </summary>
        /// <param name="settingsName">The Name of the Setting</param>
        /// <returns></returns>
        /// <exception cref="Exception">When no Setting is Found with that Name</exception>
        Task DeleteXlsSettingsAsync(string settingsName);
        /// <summary>
        /// Gets All the Names of the Available Settings or throws if nothing is found
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When no settings are found</exception>
        Task<IEnumerable<string>> GetAvailableSettingsNamesAsync();
        /// <summary>
        /// Returns all Xls Settings , or throws if not Found
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When not Found</exception>
        Task<IEnumerable<XlsSettingsGlasses>> GetXlsGlassesSettingsAsync();
        /// <summary>
        /// Returns a Setting by Name
        /// </summary>
        /// <param name="settingsName">The Name of the Setting</param>
        /// <returns></returns>
        /// <exception cref="Exception">When nothing is found</exception>
        Task<XlsSettingsGlasses> GetXlsSettingAsync(string settingsName);
        /// <summary>
        /// Returns the Selected Setting or the Default Setting if no Selected Setting is Found , or throws if nothing of the two works
        /// </summary>
        /// <returns></returns>
        Task<XlsSettingsGlasses> GetSelectedSettingsAsync();
        /// <summary>
        /// Updates an Existing Setting
        /// </summary>
        /// <param name="settingsToUpdate">The Setting to Update</param>
        /// <returns></returns>
        Task UpdateXlsSettingsAsync(XlsSettingsGlasses settingsToUpdate);
        /// <summary>
        /// Selects a Setting
        /// </summary>
        /// <param name="settingsName">The Name of the Setting to Select</param>
        /// <returns></returns>
        Task SelectSettingAsync(string settingsName);
        /// <summary>
        /// Returns the Default Settings
        /// </summary>
        /// <returns></returns>
        Task<XlsSettingsGlasses> GetDefaultSettings();
    }
}