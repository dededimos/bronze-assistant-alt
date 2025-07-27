using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class Options9F
{
    public long OrderedCabinId { get; set; }

    public long WallAluminiumAl1 { get; set; }

    public long ConnectorAluminiumAlc { get; set; }

    public long GlassInAluminiumAlst { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
