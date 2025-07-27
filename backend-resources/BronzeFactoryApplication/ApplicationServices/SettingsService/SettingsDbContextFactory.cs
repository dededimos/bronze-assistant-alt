using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using SqliteApplicationSettings;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService
{
    /// <summary>
    /// A Class that Creates the Context for the SettingsDb , Should be a singleton ? 
    /// </summary>
    public class SettingsDbContextFactory
    {
        private readonly string _connectionString;

        public SettingsDbContextFactory(string connString)
        {
            _connectionString = connString;
        }

        public SettingsDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(_connectionString).ConfigureWarnings(warnings=> warnings.Log(RelationalEventId.PendingModelChangesWarning)).Options;
            return new SettingsDbContext(options);
        }
    }

}
