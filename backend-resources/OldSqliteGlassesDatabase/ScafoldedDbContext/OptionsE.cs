using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsE
{
    public long OrderedCabinId { get; set; }

    public long FinalHeightOption { get; set; }

    public long FixingOptions { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
