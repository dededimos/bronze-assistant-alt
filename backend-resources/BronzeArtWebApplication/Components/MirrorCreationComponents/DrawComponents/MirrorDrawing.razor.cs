using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.DrawsBuilder.Models;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using SVGDrawingLibrary.Models;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.DrawComponents
{
    public partial class MirrorDrawing : ComponentBase
    {
        [Parameter] public MirrorDrawSide Draw { get; set; }
        [Parameter] public MirrorDrawContainer ContainerBox { get; set; }
        [Parameter] public DrawShape ExtrasBoundary { get; set; }
        [Parameter] public DrawShape SandblastBoundary { get; set; }
        [Parameter] public DrawShape SupportsBoundary { get; set; }

        //Options
        [Parameter] public bool ShowLabeledExtras { get; set; }
        [Parameter] public bool ShowGlassDimensions { get; set; }
        [Parameter] public bool ShowDrawBoundaries { get; set; }
    }
}
