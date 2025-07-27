using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OrderedCabin
{
    public long Id { get; set; }

    public long GlassesOrderId { get; set; }

    public long Model { get; set; }

    public long? CabinFinish { get; set; }

    public long CabinThicknesses { get; set; }

    public long GlassFinish { get; set; }

    public long LengthMin { get; set; }

    public long Height { get; set; }

    public long StepLength { get; set; }

    public long StepHeight { get; set; }

    public long TolleranceMinus { get; set; }

    public long TollerancePlus { get; set; }

    public string OrderNo { get; set; } = null!;

    public string? Date { get; set; }

    public string? Notes { get; set; }

    public long Quantity { get; set; }

    public string? Code { get; set; }

    public virtual GlassesOrder GlassesOrder { get; set; } = null!;

    public virtual ICollection<OrderedGlass> OrderedGlasses { get; } = new List<OrderedGlass>();
}
