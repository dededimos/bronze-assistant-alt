using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService.SearchOrdersViewSettingsService
{
    public class SearchOrdersViewSettingsProvider : ISearchOrdersViewSettingsProvider
    {
        private readonly SettingsDbContextFactory _dbContextFactory;

        public SearchOrdersViewSettingsProvider(SettingsDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Returns the Currently Selected Settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SearchOrdersViewSettings> GetSettingsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Get the non Default Settings
            var settings = await context.SearchOrdersViewSettingsTable.FirstOrDefaultAsync(s => !s.IsDefault) 
                ?? throw new Exception("Non Default Search-View Settings not Found... this should never Happen...");
            return new SearchOrdersViewSettings(settings);
        }
        /// <summary>
        /// Returns the Default Settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SearchOrdersViewSettings> GetDefaultsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Get the non Default Settings
            var settings = await context.SearchOrdersViewSettingsTable.FirstOrDefaultAsync(s => s.IsDefault) 
                ?? throw new Exception("Default Search-View Settings not Found... this should never Happen...");
            return new SearchOrdersViewSettings(settings);
        }
        public async Task<SearchOrdersViewSettings> RestoreDefaultsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Get the non Default Settings
            var defaults = await context.SearchOrdersViewSettingsTable.FirstOrDefaultAsync(s => s.IsDefault);
            var nonDefaults = await context.SearchOrdersViewSettingsTable.FirstOrDefaultAsync(s => !s.IsDefault);

            if (defaults is null || nonDefaults is null) throw new Exception("Defaults or Non Defaults not found , this should never happen...");
            nonDefaults.CopySettings(defaults);
            await context.SaveChangesAsync();

            return await GetDefaultsAsync();
        }
        public async Task SaveSettingsAsync(SearchOrdersViewSettings settings)
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Get the non Default Settings
            var settingsDTO = await context.SearchOrdersViewSettingsTable.FirstOrDefaultAsync(s => !s.IsDefault) 
                ?? throw new Exception("Current Settings not FOund ... , this should never happen...");
            settingsDTO.CopySettings(settings.ToDto());
            await context.SaveChangesAsync();
            return;
        }
    }

    public partial class SearchOrdersViewSettingsViewModel : BaseViewModel
    {
        private readonly ISearchOrdersViewSettingsProvider settingsProvider;
        private SearchOrdersViewSettings _undoStore = new();
        
        [ObservableProperty]
        private int maxResultsGetSmallOrders;
        [ObservableProperty]
        private bool ignoreCacheGetSmallOrders;
        [ObservableProperty]
        private bool isInitilized;

        public SearchOrdersViewSettingsViewModel(ISearchOrdersViewSettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task InitilizeViewModelAsync()
        {
            if (!IsInitilized)
            {
                _undoStore = await settingsProvider.GetSettingsAsync();
                MaxResultsGetSmallOrders = _undoStore.MaxResultsGetSmallOrders;
            }
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            try
            {
                await settingsProvider.SaveSettingsAsync(this.ToSettings());
                _undoStore = this.ToSettings();
            }
            catch (Exception ex)
            {
                this.CopyFromSettings(_undoStore);
                MessageService.LogAndDisplayException(ex);
            }
        }

        [RelayCommand]
        private async Task RestoreDefaults()
        {
            try
            {
                var defaults = await settingsProvider.RestoreDefaultsAsync();
                this.CopyFromSettings(defaults);
                _undoStore = defaults;
            }
            catch (Exception ex)
            {
                this.CopyFromSettings(_undoStore);
                MessageService.LogAndDisplayException(ex);
            }
            
        }

        private void CopyFromSettings(SearchOrdersViewSettings settings)
        {
            MaxResultsGetSmallOrders = settings.MaxResultsGetSmallOrders;
        }
        private SearchOrdersViewSettings ToSettings()
        {
            return new SearchOrdersViewSettings()
            {
                MaxResultsGetSmallOrders = MaxResultsGetSmallOrders
            };
        }
    }

}
