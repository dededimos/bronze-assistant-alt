namespace BronzeFactoryApplication.ApplicationServices.SettingsService
{
    public interface IGeneralSettingsProvider
    {
        Task<string> GetSelectedLanguage();
        Task<string> GetSelectedTheme();
        Task SetSelectedLanguage(string selectedLanguage);
        Task SetSelectedTheme(string selectedTheme);
        Task<string> GetApplicationVersion();
    }
}