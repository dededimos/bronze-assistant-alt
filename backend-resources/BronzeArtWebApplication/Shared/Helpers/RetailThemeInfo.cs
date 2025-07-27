using BronzeArtWebApplication.Shared.Enums;
using System;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Shared.Helpers
{
    /// <summary>
    /// Contains Methods to Retrieve the Theme Information (ex. Logo Paths - language Container strings e.t.c.)
    /// </summary>
    public static class BronzeAppThemeInfo
    {
        /// <summary>
        /// Returns the Logo of the Selected Theme
        /// </summary>
        /// <param name="theme">The Theme</param>
        /// <returns>The Logo src Path Of the Selected Theme</returns>
        public static string GetRetailThemeLogoPath(RetailModeTheme theme)
        {
            string logoPath = "";

            switch (theme)
            {
                case RetailModeTheme.Papapolitis:
                    //Bronze Logo
                    logoPath = "../Images/Logos/PapapolitisLogo.png";
                    break;
                case RetailModeTheme.None:
                case RetailModeTheme.Bronze:
                case RetailModeTheme.Lakiotis:
                default:
                    //Bronze Logo
                    logoPath = "../Images/Logos/BABlack.png";
                    break;
            }
            return logoPath;
        }

        /// <summary>
        /// The Base URIs that match with a Client Theme // BREAKING CHANGE IF THESE ARE ALTERED
        /// </summary>
        public static readonly Dictionary<string, WhiteLabelTheme> WhiteLabelThemeByURI = new()
        {
            { "https://www.bronzeapp.eu/", WhiteLabelTheme.None },
            { "https://www.bronzeapp.gr/", WhiteLabelTheme.None },
            { "https://ashy-pebble-023850403.azurestaticapps.net/", WhiteLabelTheme.None},
            { "https://localhost:44341/", WhiteLabelTheme.None },
            { "https://localhost:5001/", WhiteLabelTheme.None },
            { "https://papapolitis.bronzeapp.gr/", WhiteLabelTheme.Papapolitis },
            { "https://lakiotis.bronzeapp.gr/", WhiteLabelTheme.Lakiotis },
        };
    }
}
