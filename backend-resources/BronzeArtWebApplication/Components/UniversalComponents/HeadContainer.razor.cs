using Microsoft.AspNetCore.Components;

namespace BronzeArtWebApplication.Components.UniversalComponents
{
    ///DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****
    ///DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****
    ///DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****DEPRECATED****
    /// <summary>
    /// Component Manipulating the Head Content for each Razor Page Seperately
    /// Helps Changing Page-Title , as well as Canonical , Link-rel Alternative e.t.c.
    /// This container should be used once per page to avoid duplicated Values in Pages
    /// </summary>
    public partial class HeadContainer : ComponentBase
    {
        /// <summary>
        /// The Base Uri To be Used , a NavigationManager.BaseURI can be used otherwise defaults to www.bronzeapp.eu
        /// </summary>
        [Parameter] public string BaseUri { get; set; } = "https://www.bronzeapp.eu";

        /// <summary>
        /// The Relative Path of the Page , meaning baseURI'/relativePath'
        /// The Trailing Slash must be Included !
        /// </summary>
        [Parameter] public string RelativePathToBaseUri { get; set; }
        /// <summary>
        /// Wheather to allow indexing to Crawlers
        /// Sets meta tag name = robots content = index,follow or noindex
        /// </summary>
        [Parameter] public bool AllowCrawling { get; set; }

        /// <summary>
        /// The Page Title
        /// </summary>
        [Parameter] public string PageTitle { get; set; }
        /// <summary>
        /// A Description of the Contents for this Page
        /// </summary>
        [Parameter] public string PageDescription { get; set; }

    }
}
