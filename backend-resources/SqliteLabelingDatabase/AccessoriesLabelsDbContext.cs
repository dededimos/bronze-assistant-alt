using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteLabelingDatabase
{
    public class AccessoriesLabelsDbContext : DbContext
    {
        public DbSet<AccessoryLabelDTO> AccessoriesTable { get; set; }


        //Pass the options to the Base Constructor and will create the DB/Migration For us
        public AccessoriesLabelsDbContext(DbContextOptions options) : base(options)
        {

        }
    }
    /// <summary>
    /// This class helps to Create the DB Context for the Migrations
    /// </summary>
    public class SettingsdDBContextDesignTimeFactory : IDesignTimeDbContextFactory<AccessoriesLabelsDbContext>
    {
        public AccessoriesLabelsDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder().UseSqlite("Data Source=Accessories.db").EnableSensitiveDataLogging().ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning)).Options;

            return new AccessoriesLabelsDbContext(options);
        }
    }
}
