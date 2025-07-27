using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsV
{
    public long OrderedCabinId { get; set; }

    public long OverlapEpik { get; set; }

    public long MagnetStripMagn { get; set; }

    public long MagnetAluminiumAlmag { get; set; }

    public long HandleDistanceHd { get; set; }

    public long CoverDistanceCd { get; set; }

    public long ClosureMagnetOptions { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
