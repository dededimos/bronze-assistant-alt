using MongoDB.Driver;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService
{
    public class GlassesStockSettingsProvider : IGlassesStockSettingsProvider
    {
        private readonly SettingsDbContextFactory _dbContextFactory;

        public GlassesStockSettingsProvider(SettingsDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<GlassesStockSettings>> GetDefaultsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var settings = await context.GlassesStockServiceSettingsTable.Where(s => s.IsDefault).Select(dto => new GlassesStockSettings(dto)).ToListAsync();
            if (settings is null)
            {
                throw new Exception("No settings where found for the GlassesStock Service ...");
            }
            return settings;
        }

        public async Task<IEnumerable<GlassesStockSettings>> GetSettingsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Get the non Default Settings
            var settings = await context.GlassesStockServiceSettingsTable.Where(s => !s.IsDefault).Select(dto=> new GlassesStockSettings(dto)).ToListAsync();
            if (settings is null)
            {
                throw new Exception("No settings where found for the GlassesStock Service ...");
            }
            return settings;
        }

        /// <summary>
        /// Restores to Default Settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<GlassesStockSettings>> RestoreDefaultsAsync()
        {
            //Get the Defaults
            var defaults = await GetDefaultsAsync();
            var selectedSettings = await GetSettingsAsync();

            //Store the Defaults as Selected
            var defaultsDtos = defaults.Select(d => d.ToDto());
            //Set them as non Defaults and non Selected and pass the Ids of the selected ones so that they can replace them in the Database
            foreach (var defaultDto in defaultsDtos)
            {
                defaultDto.IsSelected = true;
                defaultDto.IsDefault = false;
                defaultDto.Id = selectedSettings.FirstOrDefault(s => s.ConcernsModel == defaultDto.ConcernsModel)?.Id ?? throw new Exception("A default Setting was not able to match with a current User Setting , Reset to Defaults has Failed");
            }

            using var context = _dbContextFactory.CreateDbContext();
            context.GlassesStockServiceSettingsTable.UpdateRange(defaultsDtos);
            await context.SaveChangesAsync();
            
            //Return the New Restored Settings
            return await GetSettingsAsync();
        }

        /// <summary>
        /// Saves a Setting change to the Database
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task SaveSettingsAsync(GlassesStockSettings setting)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.GlassesStockServiceSettingsTable.Update(setting.ToDto());
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a setting for a certain Model
        /// </summary>
        /// <returns></returns>
        public async Task<GlassesStockSettings> GetSettingAsync(CabinModelEnum model)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var retrievesSetting = await context.GlassesStockServiceSettingsTable.FirstOrDefaultAsync(dto => dto.ConcernsModel == model && !dto.IsDefault)
                ?? throw new Exception($"Glasses Stock Setting for {model} was not found ...");
            return new GlassesStockSettings(retrievesSetting);
        }

    }
}
