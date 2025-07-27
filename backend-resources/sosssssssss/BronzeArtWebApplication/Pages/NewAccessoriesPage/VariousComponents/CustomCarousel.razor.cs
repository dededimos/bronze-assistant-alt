
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage.VariousComponents
{
    public partial class CustomCarousel
    {
        /// <summary>
        /// The Urls of the Images of the Carousel
        /// </summary>
        [Parameter]
        public List<string> ImageUrls { get; set; }

        /// <summary>
        /// The Image currently being hovered
        /// </summary>
        [Parameter]
        public string ImageHovered { get; set; }

        [Parameter]
        public EventCallback<string> ImageHoveredChanged { get; set; }

        public async Task OnImageHoveredChanged(string newValue)
        {
            if (ImageHovered != newValue)
            {
                ImageHovered = newValue;
                await ImageHoveredChanged.InvokeAsync(ImageHovered);
            }
        }
    }
}