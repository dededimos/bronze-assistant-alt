using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsV4
{
    public long OrderedCabinId { get; set; }

    public long OverlapEpik { get; set; }

    public long MagnetStripMagn { get; set; }

    public long HandleDistanceHd { get; set; }

    public long CoverDistanceCd { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
