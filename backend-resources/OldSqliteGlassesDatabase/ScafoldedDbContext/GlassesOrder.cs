using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class GlassesOrder
{
    public long Id { get; set; }

    public string SupplierOrderNo { get; set; } = null!;

    public string? Notes { get; set; }

    public string? Date { get; set; }

    public virtual ICollection<OrderedCabin> OrderedCabins { get; } = new List<OrderedCabin>();

    public virtual ICollection<OrderedGlass> OrderedGlasses { get; } = new List<OrderedGlass>();
}
