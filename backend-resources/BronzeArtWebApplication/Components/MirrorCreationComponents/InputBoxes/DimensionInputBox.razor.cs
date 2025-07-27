using Microsoft.AspNetCore.Components;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.InputBoxes
{
    public partial class DimensionInputBox : ComponentBase
    {
        [Parameter] public string Label { get; set; }
        [Parameter] public int MinDimension { get; set; }
        [Parameter] public int MaxDimension { get; set; }

        [Parameter] public int? Dimension { get; set; }
        [Parameter] public EventCallback<int?> DimensionChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
    }
}
