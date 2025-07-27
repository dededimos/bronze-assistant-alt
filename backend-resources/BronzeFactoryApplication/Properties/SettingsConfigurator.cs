using BronzeFactoryApplication.ApplicationServices;
using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.SettingsService;
using ClosedXML.Excel;
using HandyControl.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BronzeFactoryApplication.Properties
{
    public class SettingsConfigurator : ISettingsConfigurator
    {
        public const string ISOGreek = "el-GR";
        public const string ISOEnglish = "en-EN";
        private readonly IGeneralSettingsProvider generalSettingsProvider;

        public SettingsConfigurator(IGeneralSettingsProvider generalSettingsProvider)
        {
            this.generalSettingsProvider = generalSettingsProvider;
        }

        public async Task ConfigureUserSettings()
        {
            await SetLanguage();
            await SetStartupTheme();
        }

        /// <summary>
        /// Sets the Selected Language from User Settings
        /// </summary>
        public async Task SetLanguage()
        {
            try
            {
                //Set Lanugage Setting
                string selectedLanguage = await generalSettingsProvider.GetSelectedLanguage();
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(selectedLanguage);

                //Greek Dictionary is By Default Compiled within the Merged Dictionaries
                //If the Selected Dictionary is Different than Greek then Remove from merged Dictioanries the Greek and Add the Selected
                if (selectedLanguage != ISOGreek)
                {
                    //Find the LanguageDictionary
                    ResourceDictionary? greekDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(md => md.Contains("TwoLetterIsoLanguageIdentifier"));
                    Application.Current.Resources.MergedDictionaries.Remove(greekDictionary);
                    //Add the Resource Dictionary of the Selected Culture
                    ResourceDictionary newDict = new()
                    {
                        Source = new Uri($"Resources/Languages/Language.{selectedLanguage}.xaml", UriKind.Relative)
                    };
                    Application.Current.Resources.MergedDictionaries.Add(newDict);
                }
            }
            catch (Exception ex)
            {
                string defaultLanguage = ISOGreek;
                Log.Error(ex, "Failed To Set Language from Settings - Default Language Set Instead: {defaultLanguage}",defaultLanguage);
            }
        }
        /// <summary>
        /// Sets the Selected Theme from User Settings
        /// </summary>
        public async Task SetStartupTheme()
        {
            try
            {
                //Set the Custom theme Dictionaries
                PresetManager.Current.ColorPreset = new PresetManager.Preset { ColorPreset = @"Preset\MyTheme", AssemblyName = Assembly.GetExecutingAssembly().GetName().Name };
                
                // Find Selected theme from Settings and Set the Current Dictionary to This Theme
                string selectedTheme = await generalSettingsProvider.GetSelectedTheme();
                if (selectedTheme is not "Dark" and not "Light") throw new NotSupportedException($"'{selectedTheme}' Is not Supported .Only 'Dark' and 'Light' Themes are Supported");
                if (string.IsNullOrWhiteSpace(selectedTheme)) throw new ArgumentNullException(nameof(selectedTheme), $"Selected theme Option has not Been Set");

                #region OLD DEPRECATED CUSTOM DICTIONARIES
                //ResourceDictionary selectedDictionary =
                //    GetResourceDictionaryFromFile(selectedTheme, $"Resources/ThemeDictionaries/Custom{selectedTheme}Dictionary.xaml");

                //ResourceDictionary currentThemeDictionary = GetCurrentThemeDictionaryAtRuntime();

                ////Set all the Current Theme Values to those of the Selected Dictionary
                //foreach (var key in selectedDictionary.Keys)
                //{
                //    currentThemeDictionary[key] = selectedDictionary[key];
                //}

                ////Set the HandyControls Accent Color to the Accent Color of the Selected Theme
                //ThemeManager.Current.AccentColor = (SolidColorBrush)selectedDictionary["CAccentColor"]; 
                #endregion

                //Set also the HandyControls theme to our Custom Theme
                if (selectedTheme is "Light")
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                }
                else
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                }
            }
            catch (Exception ex)
            {
                string defaultTheme = "Dark";
                MessageService.Warnings.SettingsThemeNotFoundDefaultSet();
                Log.Error(ex, "Failed To Set Theme FromSettings - Default Theme Set Instead: {defaultTheme}", defaultTheme);
            }
        }

        #region OLD DEPRECATED CUSTOM DICTIONARIES METHODS
        /// <summary>
        /// Returns a Resource Dictionary Retrieved from a File Location
        /// </summary>
        /// <param name="dictionarySettingName">The Name of the Dictionary (Used only in Exception Message)</param>
        /// <param name="relativeURI">The Uri of the Dictionary - Relative to the File of the Application</param>
        /// <returns>The Requested Dicionary</returns>
        /// <exception cref="Exception">When the Dictionary is Not Found</exception>
        private ResourceDictionary GetResourceDictionaryFromFile(string dictionarySettingName, string relativeURI)
        {
            ResourceDictionary dictionary = new()
            {
                Source = new Uri(relativeURI, UriKind.Relative)
            };
            if (dictionary is null) throw new Exception($"{dictionarySettingName} Dictionary file was Not Found at : {relativeURI}");
            return dictionary;
        }

        /// <summary>
        /// Returns the Current Theme Dictionary
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private ResourceDictionary GetCurrentThemeDictionaryAtRuntime()
        {
            //Get the Current Theme Dictionary
            // All theme Dictionaries have a Key for their Name , Only one is injected to the Application , so we take this one
            ResourceDictionary? currentTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(md => md.Contains("ThemeDictionaryName"));
            if (currentTheme is null) throw new Exception(@"Current Theme Dictionary Was Not Found at Runtime Merged Dictionaries");
            return currentTheme;
        } 
        #endregion

    }
}
