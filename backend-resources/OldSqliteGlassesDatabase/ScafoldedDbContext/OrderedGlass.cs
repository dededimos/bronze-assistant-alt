using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OrderedGlass
{
    public long Id { get; set; }

    public long OrderedCabinId { get; set; }

    public long GlassesOrderId { get; set; }

    public int Quantity { get; set; }

    public string? Notes { get; set; }

    public string? OrderNo { get; set; }

    public long GlassDraw { get; set; }

    public long GlassType { get; set; }

    public long GlassThickness { get; set; }

    public long GlassFinish { get; set; }

    public long Height { get; set; }

    public long Length { get; set; }

    public long StepHeight { get; set; }

    public long StepLength { get; set; }

    public long HasArrived { get; set; }

    public byte[]? Cost { get; set; }

    public long? QuantityReceived { get; set; }

    public string? OrderedCabinCode { get; set; }

    public virtual GlassesOrder GlassesOrder { get; set; } = null!;

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;

}
