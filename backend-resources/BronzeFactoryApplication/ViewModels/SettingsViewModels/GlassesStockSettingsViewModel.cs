using BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.SettingsViewModels
{
    public partial class GlassesStockSettingsViewModel : BaseViewModel
    {
        private IGlassesStockSettingsProvider settingsProvider;

        [ObservableProperty]
        private GlassesStockSettings? sessionSettings;

        public GlassesStockSettingsViewModel(IGlassesStockSettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task SetModelSettings(CabinModelEnum? concerningModel)
        {
            if (concerningModel is null)
            {
                SessionSettings = null;
                return;
            }
            // Set the Session Settings to the Same
            var settings = await settingsProvider.GetSettingAsync((CabinModelEnum)concerningModel);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SessionSettings = settings;
            });
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            if (SessionSettings is not null)
            {
                //Save the new Settings and Also Retrieve Again the Saved User Settings to the backing filed
                await settingsProvider.SaveSettingsAsync(SessionSettings);
            }
        }

        [RelayCommand]
        private async Task SetSettingToDefaultAsync()
        {
            if (SessionSettings is not null)
            {
                var defaults = await settingsProvider.GetDefaultsAsync();
                var foundDefault = defaults.FirstOrDefault(d => d.ConcernsModel == SessionSettings.ConcernsModel)
                    ?? throw new Exception($"{SessionSettings.ConcernsModel} was not Found in {nameof(GlassesStockSettings)}");
                SessionSettings.ShouldCompareHeight = foundDefault.ShouldCompareHeight;
                SessionSettings.AllowedHeightDifference = foundDefault.AllowedHeightDifference;
                SessionSettings.ShouldCompareLength = foundDefault.ShouldCompareLength;
                SessionSettings.AllowedLengthDifference = foundDefault.AllowedLengthDifference;
                SessionSettings.ShouldCompareThickness = foundDefault.ShouldCompareThickness;
                SessionSettings.ShouldCompareFinish = foundDefault.ShouldCompareFinish;
            }
        }
    }
}
