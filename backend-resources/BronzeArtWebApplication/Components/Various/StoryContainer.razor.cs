using Microsoft.AspNetCore.Components;

namespace BronzeArtWebApplication.Components.Various
{
    public partial class StoryContainer : ComponentBase
    {
        [Parameter] public RenderFragment TitleContent { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool IsTitleBoxVisible { get; set; } = true;
        [Parameter] public bool IsCloseIconVisible { get; set; } = false;
        [Parameter] public bool IsResetIconVisible { get; set; } = true;

        [Parameter] public string TitleContainerStyle { get; set; } = string.Empty;
        [Parameter] public string StoryContainerStyle { get; set; } = string.Empty;
        [Parameter] public RenderFragment MainContent { get; set; }
        
        [Parameter] public string MainContentStyle { get; set; }
        
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool IsNextButtonVisible { get; set; } = true;

        [Parameter] public bool IsPreviousButtonVisible { get; set; } = true;

        [Parameter] public EventCallback OnPreviousClick { get; set; }
        
        [Parameter] public EventCallback OnNextClick { get; set; }
        [Parameter] public EventCallback OnCloseClick { get; set; }
        [Parameter] public EventCallback OnResetClick { get; set; }
    }
}
