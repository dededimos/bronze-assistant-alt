using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsVf
{
    public long OrderedCabinId { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
