using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class Options9B
{
    public long OrderedCabinId { get; set; }

    public long HingeType { get; set; }

    public long GlassGapAer { get; set; }

    public long WallAluminiumAl1 { get; set; }

    public long WallAluminiumAl2 { get; set; }

    public long MagnetAluminiumThickAl3 { get; set; }

    public long MagnetStripMagn { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
