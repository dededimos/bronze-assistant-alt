using Microsoft.EntityFrameworkCore;
using OldSqliteGlassesDatabase.ScafoldedDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldSqliteGlassesDatabase.OldDatabaseConnectionService
{
    [Obsolete("DO NOT USE - DEPRECATED")]
    public class OldOrdersSqliteProvider
    {
        [Obsolete("DO NOT USE - DEPRECATED")]
        public async Task<IEnumerable<GlassesOrder>> RetrieveAllOrders()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=GlassesOrderDatabase.db;").Options;
            using var context = new GlassesOrderDatabaseContext(options);
            return await context.GlassesOrders.ToListAsync();
        }
        [Obsolete("DO NOT USE - DEPRECATED")]
        public async Task<IEnumerable<OrderedCabin>> RetrieveAllCabins()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=GlassesOrderDatabase.db;").Options;
            using var context = new GlassesOrderDatabaseContext(options);
            return await context.OrderedCabins.ToListAsync();
        }
        [Obsolete("DO NOT USE - DEPRECATED")]
        public async Task<IEnumerable<OrderedGlass>> RetrieveAllGlasses()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=GlassesOrderDatabase.db;").Options;
            using var context = new GlassesOrderDatabaseContext();
            return await context.OrderedGlasses.ToListAsync();
        }
    }
}
