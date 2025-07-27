using Microsoft.AspNetCore.Components;

namespace BronzeArtWebApplication.Components.Various
{
    public partial class PanelContainer : ComponentBase
    {
        [Parameter] public RenderFragment TitleContent { get; set; }
        [Parameter] public RenderFragment Content { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public string TitleStyle { get; set; }
    }
}
