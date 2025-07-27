using DrawingLibrary.Models.PresentationOptions;
using Microsoft.EntityFrameworkCore;
using SqliteApplicationSettings.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService
{
    public class GeneralSettingsProvider : IGeneralSettingsProvider
    {
        private readonly SettingsDbContextFactory _dbContextFactory;

        public GeneralSettingsProvider(SettingsDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Sets the Selected Language
        /// </summary>
        /// <param name="selectedLanguage">the Selected Language Identifyer Culture to set , ex. el-GR </param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only Greek and English Are Supported</exception>
        /// <exception cref="Exception">When lang Setting is not Found</exception>
        public async Task SetSelectedLanguage(string selectedLanguage)
        {
            if (selectedLanguage is not "el-GR" and not "en-EN") throw new NotSupportedException("Only Greek(el-GR) and English(en-EN) are supported Selected Languages");
            using var context = _dbContextFactory.CreateDbContext();
            var langSetting = await context.GeneralSettingsTable.FirstOrDefaultAsync(s => s.SettingName == GeneralApplicationSettings.SelectedLanguage.ToString());
            if (langSetting is null)
            {
                throw new Exception("Selected Language Setting Not Found");
            }
            langSetting.SettingValue = selectedLanguage;
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// Returns the Currently Selected Language
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When the Lang Setting is Not Found</exception>
        public async Task<string> GetSelectedLanguage()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var langSetting = await context.GeneralSettingsTable.FirstOrDefaultAsync(s => s.SettingName == GeneralApplicationSettings.SelectedLanguage.ToString());
            if (langSetting is null)
            {
                throw new Exception("Selected Language Setting Not Found");
            }
            return langSetting.SettingValue;
        }

        /// <summary>
        /// Sets the Currently Selected Theme
        /// </summary>
        /// <param name="selectedTheme">The theme to set as Select (ex. Dark)</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only Dark and Light are Supported</exception>
        /// <exception cref="Exception">When the Theme Setting is not Found</exception>
        public async Task SetSelectedTheme(string selectedTheme)
        {
            if (selectedTheme is not "Dark" and not "Light") throw new NotSupportedException("Only Dark and Light are supported Themes");
            using var context = _dbContextFactory.CreateDbContext();
            var themeSetting = await context.GeneralSettingsTable.FirstOrDefaultAsync(s => s.SettingName == GeneralApplicationSettings.SelectedTheme.ToString()) 
                ?? throw new Exception("Selected Theme Setting Not Found");
            themeSetting.SettingValue = selectedTheme;

            DrawingPresentationOptionsGlobal.IsDarkTheme = themeSetting.SettingValue == "Dark";
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// Returns the Currently Selected Theme
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When the Lang Setting is Not Found</exception>
        public async Task<string> GetSelectedTheme()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var themeSetting = await context.GeneralSettingsTable.FirstOrDefaultAsync(s => s.SettingName == GeneralApplicationSettings.SelectedTheme.ToString());
            if (themeSetting is null)
            {
                throw new Exception("Selected Language Setting Not Found");
            }
            return themeSetting.SettingValue;
        }

        /// <summary>
        /// Returns the ApplicationVersion
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetApplicationVersion()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var appVersionSetting = await context.GeneralSettingsTable.FirstOrDefaultAsync(s => s.SettingName == GeneralApplicationSettings.ApplicationVersion.ToString());
            if (appVersionSetting is null)
            {
                return "Version ????";
            }
            return appVersionSetting.SettingValue;
        }
    }
}
