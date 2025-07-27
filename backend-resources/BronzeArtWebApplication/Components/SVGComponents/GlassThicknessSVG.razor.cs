using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums;

namespace BronzeArtWebApplication.Components.SVGComponents
{
    public partial class GlassThicknessSVG : ComponentBase
    {
        [Parameter] public CabinThicknessEnum? Thickness { get; set; }
        [Parameter] public string TextColor { get; set; } = "#6F2443"; //<---Default Original Color , AppTertiary -->"#9D080D"
        [Parameter] public string GradientColor { get; set; } = "#A2D9F7";//<---Default Original Color , AppTertiary -->"#9D080D"

    }
}
