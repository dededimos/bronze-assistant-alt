using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsW
{
    public long OrderedCabinId { get; set; }

    public long WallAluminiumAl1 { get; set; }

    public long GlassInAluminiumAlst { get; set; }

    public long FinalHeightOption { get; set; }

    public long FixingOptions { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
