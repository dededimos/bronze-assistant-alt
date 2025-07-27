using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class Options9A
{
    public long OrderedCabinId { get; set; }

    public long WallAluminiumAl1 { get; set; }

    public long GlassInAluminiumAlst { get; set; }

    public long OverlapEpik { get; set; }

    public long MagnetStripMagn { get; set; }

    public long AngleDistance { get; set; }

    public long HandleDistanceHd { get; set; }

    public long CoverDistanceCd { get; set; }

    public long L0type { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
