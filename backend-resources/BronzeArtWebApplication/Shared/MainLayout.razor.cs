using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using AKSoftware.Localization.MultiLanguages;
using Microsoft.AspNetCore.Components.Web;

namespace BronzeArtWebApplication.Shared
{
    public partial class MainLayout
    {
        private MudTheme currentTheme;
        
        readonly MudTheme lightTheme = new()
        {
            PaletteLight = new PaletteLight()
            {
                Black = "#272c34",
                Background = "#FFFFFF",
                Primary = "#67003C",
                Secondary = "#F50057",
                Tertiary = "#9D080D",
                //WHAT I USE AS BACKGROUND GREY: #E0E0E0
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
            },
            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "350px",
                DefaultBorderRadius = "10px",
            }
        };


        protected override async Task OnInitializedAsync()
        {
            //await Task.Delay(2000);
            currentTheme = lightTheme;
            await base.OnInitializedAsync();
        }

        // Code to Open Close Drawer
        bool _drawerOpen = false;

        /// <summary>
        /// Opens the Drawer Component
        /// </summary>
        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        /// <summary>
        /// Changes the Language Culture EN/GR/IT
        /// </summary>
        private async Task ChangeLanguage(string languageString)
        {
            if (languageContainer.CurrentCulture.Name != languageString)
            {
                //Change the Language Resource
                languageContainer.SetLanguage(System.Globalization.CultureInfo.GetCultureInfo(languageString));
                //Store the Newly selected Language
                await PreserveSelectedCulture(languageString);
                // Navigate to Index and force Reload so the Language Container Refreshes
                navigationManager.NavigateTo("/", true);
            }
        }

        /// <summary>
        /// Stores the Selected Language to LocalStorage
        /// </summary>
        /// <param name="selectedCulture"></param>
        /// <returns></returns>
        async Task PreserveSelectedCulture(string selectedCulture)
        {
            await localStorage.SetItemAsync("BronzeAppLanguage", selectedCulture);
        }
    }
}
