using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;

/// <summary>
/// Provides the Settings for the GlassesStock Service
/// </summary>
public interface IGlassesStockSettingsProvider
{
    /// <summary>
    /// Gets the Default Settings
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<GlassesStockSettings>> GetDefaultsAsync();
    /// <summary>
    /// Gets the Current Settings
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<GlassesStockSettings>> GetSettingsAsync();
    /// <summary>
    /// Retrieves a setting for a certain Model
    /// </summary>
    /// <returns></returns>
    Task<GlassesStockSettings> GetSettingAsync(CabinModelEnum model);
    /// <summary>
    /// Restores the Current Settings to Defaults and returns them
    /// </summary>
    /// <returns>The Current Settings that have been set to Default</returns>
    Task<IEnumerable<GlassesStockSettings>> RestoreDefaultsAsync();
    /// <summary>
    /// Saves a settings object
    /// </summary>
    /// <param name="setting">The settings to save</param>
    /// <returns></returns>
    Task SaveSettingsAsync(GlassesStockSettings setting);
}
