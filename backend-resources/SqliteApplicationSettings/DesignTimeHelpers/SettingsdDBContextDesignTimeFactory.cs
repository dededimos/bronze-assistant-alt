using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteApplicationSettings.DesignTimeHelpers
{
    /// <summary>
    /// This class helps to Create the DB Context for the Migrations
    /// </summary>
    public class SettingsdDBContextDesignTimeFactory : IDesignTimeDbContextFactory<SettingsDbContext>
    {
        public SettingsDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder().UseSqlite("Data Source=appSettings.db").EnableSensitiveDataLogging().ConfigureWarnings(warnings=> warnings.Log(RelationalEventId.PendingModelChangesWarning)).Options;
            
            return new SettingsDbContext(options);
        }
    }
}
