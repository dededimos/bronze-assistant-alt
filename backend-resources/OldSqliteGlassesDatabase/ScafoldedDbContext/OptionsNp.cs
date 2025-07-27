using System;
using System.Collections.Generic;

namespace OldSqliteGlassesDatabase.ScafoldedDbContext;

public partial class OptionsNp
{
    public long OrderedCabinId { get; set; }

    public long WallAluminiumAl1 { get; set; }

    public long GlassInAluminiumAlst { get; set; }

    public long GlassGapAer { get; set; }

    public long ConnectorAluminiumAlc { get; set; }

    public long MagnetStripMagn { get; set; }

    public long MagnetAluminiumAlmag { get; set; }

    public long Draw { get; set; }

    public long ClosureMagnetOptions { get; set; }

    public long HingeType { get; set; }

    public long ExtraPanelMinLength { get; set; }

    public long ExtraPanelStepLength { get; set; }

    public long ExtraPanelStepHeight { get; set; }

    public virtual OrderedCabin OrderedCabin { get; set; } = null!;
}
