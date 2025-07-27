using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared
{
    public partial class BlankLayout
    {
        private readonly MudTheme currentTheme  = new()
        {
            PaletteLight = new PaletteLight()
            {
                Black = "#272c34",
                Background = "#FFFFFF",
                Tertiary = "#9D080D"
                //BackgroundGrey = "#27272f",
                //Surface = "#373740",
                //DrawerBackground = "#27272f",
                //DrawerText = "rgba(255,255,255, 0.50)",
                //DrawerIcon = "rgba(255,255,255, 0.50)",
                //AppbarBackground = "#27272f",
                //AppbarText = "rgba(255,255,255, 0.70)",
                //TextPrimary = "rgba(255,255,255, 0.70)",
                //TextSecondary = "rgba(255,255,255, 0.50)",
                //ActionDefault = Colors.BlueGrey.Darken1,
                //ActionDisabled = "rgba(255,255,255, 0.26)",
                //ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                //Divider = "rgba(255,255,255, 0.12)",
                //DividerLight = "rgba(255,255,255, 0.06)",
                //TableLines = "rgba(255,255,255, 0.12)",
                //LinesDefault = "rgba(255,255,255, 0.12)",
                //LinesInputs = "rgba(255,255,255, 0.3)",
                //TextDisabled = "rgba(255,255,255, 0.2)"
            }
        };
    }
}

