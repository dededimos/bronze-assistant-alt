using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Components.UniversalComponents
{
    public partial class NewsDialog : ComponentBase
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        private readonly List<(string title, List<string> descriptions)> news = [];

        protected override void OnInitialized()
        {
            news.Add(("NewsTitle212",       news212));
            news.Add(("NewsTitle208",       news208));
            news.Add(("NewsTitle201206", news201206));
            news.Add(("NewsTitle200",       news200));
            base.OnInitialized();
        }

        private readonly List<string> news100 =
        [
            "CabinSectionReady",
            "CapsuleMirrorReady",
            "FramedLightMirrorsWithoutSandblast",
        ];
        private readonly List<string> news101 =
        [
            "EllipseMirrorReady",
            "LinkItemsReady",
            "BetterLooksReady2",
        ];
        private readonly List<string> news102103 =
        [
            "BathtubPanelsReady",
        ];
        private readonly List<string> news104105106 =
        [
            "InvalidFlagBug",
            "CabinPricingComing",
            "AccessoriesComing"
        ];
        private readonly List<string> news107abc =
        [
            "SaveCabinSynthesis",
        ];

        private readonly List<string> news108 =
        [
            "AccessoriesDemoNews"
        ];

        private readonly List<string> news109110 =
        [
            "CabinsPricing",
            "CabinsPricing2",
            "CabinsOptions",
            "CabinsOptions2",
            "BugFixes109",
        ];
        private readonly List<string> news111112 =
        [
            "NotesBoxAdded",
            "CabinPricingFixes",
            "IrregularThicknessPricing",
            "BugFixes111",
        ];
        private readonly List<string> news113 =
        [
            "BugFixes113",
        ];

        private readonly List<string> news200 =
        [
            "A2001",
            "A2002",
            "A2003",
            "A2004",
            "A2005",
            "A2006",
            "A2007",
            "A2008",
            "A2009",
            "A20010",
            "A20011",
            "A20012",
            "A20013",
            "A20014",
        ];
        private readonly List<string> news201206 =
        [
            "A2012061"
        ];
        private readonly List<string> news208 =
        [
            "A2081",
            "A2082",
            "A2083",
            "A2084",
        ];
        private readonly List<string> news212 =
        [
            "A2121",
            "A2122",
            "A2123",
            "A2124",
        ];

    }
}
