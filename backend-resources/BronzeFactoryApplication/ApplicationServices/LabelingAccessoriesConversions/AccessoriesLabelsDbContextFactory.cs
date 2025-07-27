using Microsoft.EntityFrameworkCore.Diagnostics;
using SqliteLabelingDatabase;

namespace BronzeFactoryApplication.ApplicationServices.LabelingAccessoriesConversions
{
    public class AccessoriesLabelsDbContextFactory
    {
        private readonly string _connectionString;
        public AccessoriesLabelsDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public AccessoriesLabelsDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(_connectionString).ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning)).Options;
            return new AccessoriesLabelsDbContext(options);
        }
    }
}
