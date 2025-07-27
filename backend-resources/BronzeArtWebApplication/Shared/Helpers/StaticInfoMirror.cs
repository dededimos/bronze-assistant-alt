using BronzeArtWebApplication.Shared.Enums;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Helpers
{
    public static class StaticInfoMirror
    {

        /// <summary>
        /// The Sandblasts List of the Rectangular Mirrors
        /// </summary>
        public static List<MirrorSandblast> RectangularSandblasts { get; } =
        [
            MirrorSandblast.H7,
            MirrorSandblast.H8,
            MirrorSandblast.X6,
            MirrorSandblast.X4,
            MirrorSandblast._6000,
            MirrorSandblast.M3
        ];

        /// <summary>
        /// The Sandblasts List of the Circular Mirror
        /// </summary>
        public static List<MirrorSandblast> CircularSandblasts { get; } =
        [
            MirrorSandblast.N6,
            MirrorSandblast.N9
        ];

        /// <summary>
        /// The Available Simple Lights
        /// </summary>
        public static List<MirrorLight> CommonMirrorLights { get; } =
        [
            MirrorLight.Daylight,
            MirrorLight.Warm,
            MirrorLight.Cold,
            MirrorLight.Warm_Cold,
            MirrorLight.Warm_Cold_Day,
            MirrorLight.WithoutLight,
        ];

        /// <summary>
        /// The Available Dotless Lights
        /// </summary>
        public static List<MirrorLight> DotlessMirrorLights { get; } = 
        [
            MirrorLight.Day_COB,
            MirrorLight.Warm_COB,
            MirrorLight.Cold_COB,
            MirrorLight.Warm_Cold_Day_COB,
        ];

        /// <summary>
        /// The Available light for the Premium Mirrors
        /// </summary>
        public static List<MirrorLight> PremiumMirrorLights { get; } =
        [
            MirrorLight.Warm_16W,
            MirrorLight.Day_16W,
            MirrorLight.Warm_Cold_Day_16W,
        ];

        /// <summary>
        /// Returns the Selectable Lights for the specified Mirror Series
        /// (Can return lights not suitable for the sereis but that will change it to another one , for example No Light to Light from A9 Mirror)
        /// <para>Returns Primary Lights for the Common Selectable Lights</para>
        /// <para>Returns Secondary for the not so Common Lights</para>
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static (List<MirrorLight> primary,List<MirrorLight> secondary) GetSelectableLights(MirrorSeries? series)
        {
            List<MirrorLight> primary = [];
            List<MirrorLight> secondary = [];

            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    //Even Not Lighted Mirrors can be changed into lighted ones
                case MirrorSeries.A9:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                    primary = CommonMirrorLights;
                    secondary = [.. DotlessMirrorLights, .. PremiumMirrorLights];
                    break;
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                    primary = CommonMirrorLights;
                    break;
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                    primary = PremiumMirrorLights;
                    break;
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                    primary = [MirrorLight.Daylight];
                    break;
                default:
                case MirrorSeries.Custom:
                case MirrorSeries.M8:
                    break;
            }

            return (primary, secondary);
        }

        /// <summary>
        /// Returns the Selectable Supports for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorSupport> GetSelectableSupports(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.A9:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                    return [MirrorSupport.Perimetrical, MirrorSupport.Frame];
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorSupport.Perimetrical];
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                    return [MirrorSupport.Frame];
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                    return [MirrorSupport.Double,MirrorSupport.FrontSupports, MirrorSupport.Frame, MirrorSupport.Without];
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                    return [MirrorSupport.Double, MirrorSupport.Without];
                case MirrorSeries.Custom:
                case MirrorSeries.M8:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Retusn the Selectable Sandblasts for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorSandblast> GetSelectableSandblasts(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.IM:
                    return RectangularSandblasts;
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.N9Custom:
                    return [MirrorSandblast.N6, MirrorSandblast.N9];
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                case MirrorSeries.P8:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.R7:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                    return [MirrorSandblast.H7];
                case MirrorSeries.A9:
                case MirrorSeries.R9:
                case MirrorSeries.P9:
                    return [MirrorSandblast.N6];
                case MirrorSeries.Custom:
                case MirrorSeries.M8:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Returns the Selectable Touch Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableTouchOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.ND:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorOption.TouchSwitch, MirrorOption.DimmerSwitch, MirrorOption.SensorSwitch];
                //Cannot choose any touch Options from here
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                    return [];
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Returns the Selectable Fog Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableFogOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorOption.Fog16W, MirrorOption.Fog24W, MirrorOption.Fog55W];
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                case MirrorSeries.NS:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Returns the Selectable Fog Switch Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableFogSwitchOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorOption.TouchSwitchFog,MirrorOption.EcoTouch];
                //Have built in Fog with eco switch cannot have other fog options
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.NS:
                //Premium Mirrors Cannot have Fog Options unfortunately...
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Returns the Selectable Magnifier Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableMagnifierOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorOption.Zoom, MirrorOption.ZoomLed, MirrorOption.ZoomLedTouch];
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                    return [];
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                    return [MirrorOption.Zoom];
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }

        /// <summary>
        /// Returns the Selectable Media Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableMediaOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.IM:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                    return [MirrorOption.Clock, MirrorOption.BlueTooth, MirrorOption.DisplayRadio,MirrorOption.DisplayRadioBlack,MirrorOption.Display19];
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                case MirrorSeries.IA:
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }
        /// <summary>
        /// Returns the Selectable Extra Options for the specified Mirror Series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static List<MirrorOption> GetSelectableExtraOptions(MirrorSeries? series)
        {
            switch (series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.IM:
                    return [MirrorOption.IPLid, MirrorOption.RoundedCorners];
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.N9Custom:
                case MirrorSeries.NCCustom:
                case MirrorSeries.NLCustom:
                case MirrorSeries.P8:
                case MirrorSeries.P9:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.R7:
                case MirrorSeries.R9:
                    return [];
                case MirrorSeries.IA:
                    return [MirrorOption.RoundedCorners];
                case MirrorSeries.NA:
                case MirrorSeries.IC:
                case MirrorSeries.IL:
                case MirrorSeries.A9:
                case MirrorSeries.M8:
                case MirrorSeries.Custom:
                default:
                    return [];
            }
        }

        #region A.Image Paths

        public const string H8FramedMirror = "/Images/MirrorsImages/MirrorModelsImages/H8FramedMirror.jpg";
        public const string H7FramedMirror = "/Images/MirrorsImages/MirrorModelsImages/H7FramedMirror.jpg";

        /// <summary>
        /// The Image Path for the Sandblasts Designs
        /// </summary>
        public static readonly Dictionary<MirrorSandblast, string> SandblastImagePaths = new()
        {
            { MirrorSandblast.H7, "/Images/MirrorsImages/MirrorModelsImages/60H7Horizontal.jpg" },
            { MirrorSandblast.H8, "/Images/MirrorsImages/MirrorModelsImages/60H8Vertical.jpg" },
            { MirrorSandblast.M3, "/Images/MirrorsImages/MirrorModelsImages/60M3.jpg" },
            { MirrorSandblast.N6, "/Images/MirrorsImages/MirrorModelsImages/60N6.jpg" },
            { MirrorSandblast.N9, "/Images/MirrorsImages/MirrorModelsImages/60N9.jpg" },
            { MirrorSandblast.X4, "/Images/MirrorsImages/MirrorModelsImages/60X4Horizontal.jpg" },
            { MirrorSandblast.X6, "/Images/MirrorsImages/MirrorModelsImages/60X6Horizontal.jpg" },
            { MirrorSandblast._6000, "/Images/MirrorsImages/MirrorModelsImages/6000Vertical.jpg" }
        };

        /// <summary>
        /// The Image Path for Each Series
        /// </summary>
        public static readonly Dictionary<MirrorSeries, string> SeriesImagePaths = new()
        {
            { MirrorSeries.Custom,  "/Images/Various/NoImageAvailable.jpg"},
            { MirrorSeries.H7,      "/Images/MirrorsImages/MirrorModelsImages/60H7Horizontal.jpg" },
            { MirrorSeries.H8,      "/Images/MirrorsImages/MirrorModelsImages/60H8Horizontal.jpg" },
            { MirrorSeries.X6,      "/Images/MirrorsImages/MirrorModelsImages/60X6Horizontal.jpg" },
            { MirrorSeries.X4,      "/Images/MirrorsImages/MirrorModelsImages/60X4Horizontal.jpg" },
            { MirrorSeries._6000,   "/Images/MirrorsImages/MirrorModelsImages/6000Horizontal.jpg"},
            { MirrorSeries.M3,      "/Images/MirrorsImages/MirrorModelsImages/60M3.jpg" },
            { MirrorSeries.N9,      "/Images/MirrorsImages/MirrorModelsImages/60N9.jpg" },
            { MirrorSeries.N6,      "/Images/MirrorsImages/MirrorModelsImages/60N6.jpg" },
            { MirrorSeries.N7,      "/Images/MirrorsImages/MirrorModelsImages/60N7.jpg" },
            { MirrorSeries.A7,      "/Images/MirrorsImages/MirrorModelsImages/60A7.jpg" },
            { MirrorSeries.A9,      "/Images/MirrorsImages/MirrorModelsImages/60A9.png" },
            { MirrorSeries.M8,      "/Images/MirrorsImages/MirrorModelsImages/TEST.jpg" },
            { MirrorSeries.ND,      "/Images/MirrorsImages/MirrorModelsImages/60ND.jpg" },
            { MirrorSeries.NC,      "/Images/MirrorsImages/MirrorModelsImages/60NCHorizontal.png" },
            { MirrorSeries.NL,      "/Images/MirrorsImages/MirrorModelsImages/60NL.jpg" },
            { MirrorSeries.P8,      "/Images/MirrorsImages/MirrorModelsImages/60P8.jpg" },
            { MirrorSeries.P9,      "/Images/MirrorsImages/MirrorModelsImages/60P9.jpg" },
            { MirrorSeries.R7,      "/Images/MirrorsImages/MirrorModelsImages/60R7Horizontal.jpg" },
            { MirrorSeries.R9,      "/Images/MirrorsImages/MirrorModelsImages/60R9.jpg" },
            { MirrorSeries.NS,      "/Images/MirrorsImages/MirrorModelsImages/60NS.jpg" },
            { MirrorSeries.N1,      "/Images/MirrorsImages/MirrorModelsImages/60N1.jpg" },
            { MirrorSeries.N2,      "/Images/MirrorsImages/MirrorModelsImages/60N2.jpg" },
            { MirrorSeries.EL,      "/Images/MirrorsImages/MirrorModelsImages/60ELHorizontal.jpg" },
            { MirrorSeries.ES,      "/Images/MirrorsImages/MirrorModelsImages/60ESHorizontal.jpg" },

            { MirrorSeries.IM,      "/Images/MirrorsImages/MirrorModelsImages/60H7Horizontal.jpg"},
            { MirrorSeries.IA,      "/Images/MirrorsImages/SimpleRectangleMirror.jpg"},
            { MirrorSeries.N9Custom,"/Images/MirrorsImages/MirrorModelsImages/60N9.jpg"},
            { MirrorSeries.NA      ,"/Images/MirrorsImages/SimpleCircularMirror.jpg"},
            { MirrorSeries.NCCustom,"/Images/MirrorsImages/MirrorModelsImages/60NCHorizontal.png"},
            { MirrorSeries.IC      ,"/Images/MirrorsImages/MirrorModelsImages/60IC.jpg"},
            { MirrorSeries.NLCustom,"/Images/MirrorsImages/MirrorModelsImages/60NL.jpg"},
            { MirrorSeries.IL      ,"/Images/MirrorsImages/MirrorModelsImages/60IL.jpg"},
        };
        /// <summary>
        /// The Image Path for each Series Mirror but with Vertical Dimension Bigger than Horizontal
        /// </summary>
        public static readonly Dictionary<MirrorSeries, string> SeriesImagePathsVertical = new()
        {
            { MirrorSeries.Custom,  "/Images/Various/NoImageAvailable.jpg"},
            { MirrorSeries.H7,      "/Images/MirrorsImages/MirrorModelsImages/60H7Vertical.jpg" },
            { MirrorSeries.H8,      "/Images/MirrorsImages/MirrorModelsImages/60H8Vertical.jpg" },
            { MirrorSeries.X6,      "/Images/MirrorsImages/MirrorModelsImages/60X6Vertical.jpg" },
            { MirrorSeries.X4,      "/Images/MirrorsImages/MirrorModelsImages/60X4Vertical.jpg" },
            { MirrorSeries._6000,   "/Images/MirrorsImages/MirrorModelsImages/6000Vertical.jpg"},
            { MirrorSeries.M3,      "/Images/MirrorsImages/MirrorModelsImages/60M3.jpg" },
            { MirrorSeries.N9,      "/Images/MirrorsImages/MirrorModelsImages/60N9.jpg" },
            { MirrorSeries.N6,      "/Images/MirrorsImages/MirrorModelsImages/60N6.jpg" },
            { MirrorSeries.N7,      "/Images/MirrorsImages/MirrorModelsImages/60N7.jpg" },
            { MirrorSeries.A7,      "/Images/MirrorsImages/MirrorModelsImages/60A7.jpg" },
            { MirrorSeries.A9,      "/Images/MirrorsImages/MirrorModelsImages/60A9.png" },
            { MirrorSeries.M8,      "/Images/MirrorsImages/MirrorModelsImages/TEST.jpg" },
            { MirrorSeries.ND,      "/Images/MirrorsImages/MirrorModelsImages/60ND.jpg" },
            { MirrorSeries.NC,      "/Images/MirrorsImages/MirrorModelsImages/60NCVertical.jpg" },
            { MirrorSeries.NL,      "/Images/MirrorsImages/MirrorModelsImages/60NL.jpg" },
            { MirrorSeries.P8,      "/Images/MirrorsImages/MirrorModelsImages/60P8.jpg" },
            { MirrorSeries.P9,      "/Images/MirrorsImages/MirrorModelsImages/60P9.jpg" },
            { MirrorSeries.R7,      "/Images/MirrorsImages/MirrorModelsImages/60R7Vertical.jpg" },
            { MirrorSeries.R9,      "/Images/MirrorsImages/MirrorModelsImages/60R9.jpg" },
            { MirrorSeries.NS,      "/Images/MirrorsImages/MirrorModelsImages/60NS.jpg" },
            { MirrorSeries.N1,      "/Images/MirrorsImages/MirrorModelsImages/60N1.jpg" },
            { MirrorSeries.N2,      "/Images/MirrorsImages/MirrorModelsImages/60N2.jpg" },
            { MirrorSeries.EL,      "/Images/MirrorsImages/MirrorModelsImages/60ELVertical.jpg" },
            { MirrorSeries.ES,      "/Images/MirrorsImages/MirrorModelsImages/60ESVertical.jpg" },

            { MirrorSeries.IM,      "/Images/MirrorsImages/MirrorModelsImages/60H7Horizontal.jpg"},
            { MirrorSeries.IA,      "/Images/MirrorsImages/SimpleRectangleMirror.jpg"},
            { MirrorSeries.N9Custom,"/Images/MirrorsImages/MirrorModelsImages/60N9.jpg"},
            { MirrorSeries.NA      ,"/Images/MirrorsImages/SimpleCircularMirror.jpg"},
            { MirrorSeries.NCCustom,"/Images/MirrorsImages/MirrorModelsImages/60NCVertical.jpg"},
            { MirrorSeries.IC      ,"/Images/MirrorsImages/MirrorModelsImages/60IC.jpg"},
            { MirrorSeries.NLCustom,"/Images/MirrorsImages/MirrorModelsImages/60NL.jpg"},
            { MirrorSeries.IL      ,"/Images/MirrorsImages/MirrorModelsImages/60IL.jpg"},
        };


        /// <summary>
        /// The Image Path for the Light Option of the Mirror
        /// </summary>
        public static readonly Dictionary<MirrorLight, string> LightImagePaths = new()
        {
            { MirrorLight.WithoutLight,     "/Images/MirrorsImages/OptionsIcons/No Light.png" },
            { MirrorLight.Cold,             "/Images/MirrorsImages/OptionsIcons/Cold Light.png" },
            { MirrorLight.Warm,             "/Images/MirrorsImages/OptionsIcons/Warm Light.png" },
            { MirrorLight.Daylight,         "/Images/MirrorsImages/OptionsIcons/DayLight.png" },
            { MirrorLight.Warm_Cold,        "/Images/MirrorsImages/OptionsIcons/Warm-Cold Light.png" },
            { MirrorLight.Warm_Cold_Day,    "/Images/MirrorsImages/OptionsIcons/Warm Cold Day Light.png" },
            { MirrorLight.Day_COB,          "/Images/MirrorsImages/OptionsIcons/DayDotless.png" },
            { MirrorLight.Warm_COB,         "/Images/MirrorsImages/OptionsIcons/WarmDotless.png" },
            { MirrorLight.Cold_COB,         "/Images/MirrorsImages/OptionsIcons/ColdDotless.png" },
            { MirrorLight.Warm_Cold_Day_COB,"/Images/MirrorsImages/OptionsIcons/WarmColdDayDotless.png" },
            { MirrorLight.Day_16W,          "/Images/MirrorsImages/OptionsIcons/Day16W.png" },
            { MirrorLight.Warm_16W,         "/Images/MirrorsImages/OptionsIcons/Warm16W.png" },
            { MirrorLight.Cold_16W,         "/Images/MirrorsImages/OptionsIcons/Cold16W.png" },
            { MirrorLight.Warm_Cold_Day_16W,"/Images/MirrorsImages/OptionsIcons/WarmColdDay16W.png" },
        };

        /// <summary>
        /// ImagePaths of Mirror Options
        /// </summary>
        public static readonly Dictionary<MirrorOption, string> OptionsImagePaths = new()
        {
            { MirrorOption.BlueTooth, "/Images/MirrorsImages/OptionsIcons/Bluetooth.png" },
            { MirrorOption.Clock, "/Images/MirrorsImages/OptionsIcons/Clock.png" },
            { MirrorOption.DimmerSwitch, "/Images/MirrorsImages/OptionsIcons/Dimmer.png" },
            { MirrorOption.Display19, "/Images/MirrorsImages/OptionsIcons/Meteo 19.png" },
            { MirrorOption.Display20, "/Images/MirrorsImages/OptionsIcons/Meteo 20.png" },
            { MirrorOption.DisplayRadio, "/Images/MirrorsImages/OptionsIcons/Screen No11.png" },
            { MirrorOption.Fog16W, "/Images/MirrorsImages/OptionsIcons/AntiFog16W.png" },
            { MirrorOption.Fog24W, "/Images/MirrorsImages/OptionsIcons/AntiFog24W.png" },
            { MirrorOption.Fog55W, "/Images/MirrorsImages/OptionsIcons/AntiFog55W.png" },
            { MirrorOption.IPLid, "/Images/MirrorsImages/OptionsIcons/IP LID.png" },
            { MirrorOption.RoundedCorners, "/Images/MirrorsImages/OptionsIcons/Round Edges.png" },
            { MirrorOption.SensorSwitch, "/Images/MirrorsImages/OptionsIcons/Sensor.png" },
            { MirrorOption.TouchSwitch, "/Images/MirrorsImages/OptionsIcons/TouchButton.png" },
            { MirrorOption.TouchSwitchFog, "/Images/MirrorsImages/OptionsIcons/FOGTOUCH.png" },
            { MirrorOption.Zoom, "/Images/MirrorsImages/OptionsIcons/Zoom x3.png" },
            { MirrorOption.ZoomLed, "/Images/MirrorsImages/OptionsIcons/Zoom x3 & LED.png" },
            { MirrorOption.ZoomLedTouch, "/Images/MirrorsImages/OptionsIcons/Zoom x3 & LED Touch.png" },
            { MirrorOption.BackLightSealedChannel,"/Images/MirrorsImages/OptionsIcons/BackLightChannel.png" },
            { MirrorOption.FrontLightSealedChannel,"/Images/MirrorsImages/OptionsIcons/FrontLightChannel.png" },
            { MirrorOption.SmartAntiFog,"/Images/MirrorsImages/OptionsIcons/SmartAntiFog.png" },
            { MirrorOption.SingleTopLightedPlexiglass,"/Images/MirrorsImages/OptionsIcons/LightedSinglePlexi.png" },
            { MirrorOption.DoubleTopBottomLightedPlexiglass,"/Images/MirrorsImages/OptionsIcons/DoubleLightedPlexi.png" },
            { MirrorOption.EcoTouch,"/Images/MirrorsImages/OptionsIcons/EcoTouch.png" },
            { MirrorOption.DisplayRadioBlack,"/Images/MirrorsImages/OptionsIcons/Screen11b.png" },
            { MirrorOption.Lamp,"/Images/MirrorsImages/OptionsIcons/Lamp.png" },
            { MirrorOption.LightAluminumChannel,"/Images/MirrorsImages/OptionsIcons/AluminumChannel.png" },
        };

        ///// <summary>
        ///// ImagePaths of Mirror Shapes
        ///// </summary>
        //public static Dictionary<MirrorShape, string> ShapeImagePaths { get; } = new()
        //{
        //    { MirrorShape.Rectangular, "/Images/MirrorsImages/AllRectangularMirrors2.jpg" },
        //    { MirrorShape.Circular, "/Images/MirrorsImages/AllCircularMirrors.jpg" },
        //    { MirrorShape.Capsule, "/Images/MirrorsImages/AllCapsuleMirrors.jpg" },
        //    { MirrorShape.Ellipse, "/Images/MirrorsImages/AllEllipseMirrors.jpg" },
        //    { MirrorShape.Special, "" }
        //};

        /// <summary>
        /// ImagePaths of MirrorSupports Based on Shape
        /// </summary>
        public static readonly Dictionary<(MirrorSupport,MirrorShape), string> SupportImagePaths = new()
        {
            { (MirrorSupport.Double,MirrorShape.Rectangular), "/Images/MirrorsImages/SupportsImages/SquareDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.Rectangular), "/Images/MirrorsImages/SupportsImages/FramedRectangularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.Rectangular), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.Rectangular), "/Images/MirrorsImages/SupportsImages/SquarePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.Rectangular), "/Images/MirrorsImages/SupportsImages/SquareWithoutSupport.png" },
            { (MirrorSupport.Double, MirrorShape.Circular), "/Images/MirrorsImages/SupportsImages/CircleDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.Circular), "/Images/MirrorsImages/SupportsImages/FramedCircularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.Circular), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.Circular), "/Images/MirrorsImages/SupportsImages/CirclePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.Circular), "/Images/MirrorsImages/SupportsImages/CircleWithoutSupport.png" },
            { (MirrorSupport.Double, MirrorShape.Capsule), "/Images/MirrorsImages/SupportsImages/CapsuleDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.Capsule), "/Images/MirrorsImages/SupportsImages/FramedCircularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.Capsule), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.Capsule), "/Images/MirrorsImages/SupportsImages/CapsulePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.Capsule), "/Images/MirrorsImages/SupportsImages/ElipseWithoutSupport.png" },
            { (MirrorSupport.Double, MirrorShape.Ellipse), "/Images/MirrorsImages/SupportsImages/ElipseDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.Ellipse), "/Images/MirrorsImages/SupportsImages/FramedCircularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.Ellipse), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.Ellipse), "/Images/MirrorsImages/SupportsImages/ElipsePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.Ellipse), "/Images/MirrorsImages/SupportsImages/ElipseWithoutSupport.png" },
            { (MirrorSupport.Double,MirrorShape.StoneNS), "/Images/MirrorsImages/SupportsImages/SquareDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.StoneNS), "/Images/MirrorsImages/SupportsImages/FramedRectangularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.StoneNS), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.StoneNS), "/Images/MirrorsImages/SupportsImages/SquarePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.StoneNS), "/Images/MirrorsImages/SupportsImages/SquareWithoutSupport.png" },
            { (MirrorSupport.Double,MirrorShape.PebbleND), "/Images/MirrorsImages/SupportsImages/SquareDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.PebbleND), "/Images/MirrorsImages/SupportsImages/FramedRectangularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.PebbleND), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.PebbleND), "/Images/MirrorsImages/SupportsImages/SquarePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.PebbleND), "/Images/MirrorsImages/SupportsImages/SquareWithoutSupport.png" },
            { (MirrorSupport.Double,MirrorShape.CircleSegment), "/Images/MirrorsImages/SupportsImages/SquareDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.CircleSegment), "/Images/MirrorsImages/SupportsImages/FramedRectangularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.CircleSegment), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.CircleSegment), "/Images/MirrorsImages/SupportsImages/SquarePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.CircleSegment), "/Images/MirrorsImages/SupportsImages/SquareWithoutSupport.png" },
            { (MirrorSupport.Double,MirrorShape.CircleSegment2), "/Images/MirrorsImages/SupportsImages/SquareDoubleSupport.png" },
            { (MirrorSupport.Frame, MirrorShape.CircleSegment2), "/Images/MirrorsImages/SupportsImages/FramedRectangularMirror.png" },
            { (MirrorSupport.FrontSupports, MirrorShape.CircleSegment2), "/Images/MirrorsImages/SupportsImages/FrontSupportsMirror.png" },
            { (MirrorSupport.Perimetrical, MirrorShape.CircleSegment2), "/Images/MirrorsImages/SupportsImages/SquarePerimetricalSupport.png" },
            { (MirrorSupport.Without, MirrorShape.CircleSegment2), "/Images/MirrorsImages/SupportsImages/SquareWithoutSupport.png" },
        };

        /// <summary>
        /// ImagePaths of Paint Finishes
        /// </summary>
        public static readonly Dictionary<SupportPaintFinish, string> PaintFinishPaths = new()
        {
            { SupportPaintFinish.Black, "/Images/Finishes/BlackMatAnodized.png" },
            { SupportPaintFinish.BronzeMat, "/Images/Finishes/BronzeMatAnodized.png" },
            { SupportPaintFinish.ChromeMat, "/Images/Finishes/ChromeMatAnodized.png" },
            { SupportPaintFinish.CopperMat, "/Images/Finishes/CopperMatAnodized.png" },
            { SupportPaintFinish.GoldMat, "/Images/Finishes/GoldMatAnodized.png" },
            { SupportPaintFinish.GraphiteMat, "/Images/Finishes/GraphiteMatAnodized.png" },
            { SupportPaintFinish.RalColor, "/Images/Finishes/RalPainting.png" },
            { SupportPaintFinish.Silver, "/Images/Finishes/SilverAnodized.png" },
        };

        /// <summary>
        /// ImagePaths of Electroplated Finishes
        /// </summary>
        public static readonly Dictionary<SupportElectroplatedFinish, string> ElectroplatingFinishPaths = new()
        {
            { SupportElectroplatedFinish.BronzeBrushed, "/Images/Finishes/BronzeElectroplated.png" },
            { SupportElectroplatedFinish.BronzeMirror, "/Images/Finishes/BronzeElectroplated.png" },
            { SupportElectroplatedFinish.CopperBrushed, "/Images/Finishes/CopperElectroplated.png" },
            { SupportElectroplatedFinish.CopperMirror, "/Images/Finishes/CopperElectroplated.png" },
            { SupportElectroplatedFinish.GoldSimilarBrushed, "/Images/Finishes/SimilarGoldElectroplated.png" },
            { SupportElectroplatedFinish.GoldSimilarMirror, "/Images/Finishes/SimilarGoldElectroplated.png" },
            { SupportElectroplatedFinish.GraphiteBrushed, "/Images/Finishes/GraphiteElectroplated.png" },
            { SupportElectroplatedFinish.GraphiteMirror, "/Images/Finishes/GraphiteElectroplated.png" },
            { SupportElectroplatedFinish.NickelBrushed, "/Images/Finishes/NickelElectroplated.png" },
            { SupportElectroplatedFinish.NickelMirror, "/Images/Finishes/NickelElectroplated.png" },
            { SupportElectroplatedFinish.RealGoldBrushed, "/Images/Finishes/RealGoldElectroplated.png" },
            { SupportElectroplatedFinish.RealGoldMirror, "/Images/Finishes/RealGoldElectroplated.png" },
            { SupportElectroplatedFinish.RoseGoldMirror, "/Images/Finishes/RoseGoldElectroplated.png" },
            { SupportElectroplatedFinish.RoseGoldBrushed, "/Images/Finishes/RoseGoldElectroplated.png" },
        };

        #endregion

        #region Z. Enum Descriptions Language Keys

        /// <summary>
        /// Maps the AssembleMirrorViewModelState Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<BronzeAppMode, string> BronzeAppModeDescKey = new()
        {
            { BronzeAppMode.Guest, "GuestState" },
            { BronzeAppMode.Retail, "RetailState" },
            { BronzeAppMode.Wholesale, "WholesaleState" }
        };

        /// <summary>
        /// Maps the MirrorShape Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorShape, string> MirrorShapeDescKey = new()
        {
            { MirrorShape.Rectangular,      "RectangularMirror"},
            { MirrorShape.Circular,         "CircularMirror"},
            { MirrorShape.Capsule,          "CapsuleMirror"},
            { MirrorShape.Ellipse,          "EllipseMirror"},
            { MirrorShape.Special,          "SpecialMirror"},
            { MirrorShape.StoneNS,          "StoneNSMirror"},
            { MirrorShape.PebbleND,         "PebbleNDMirror"},
            { MirrorShape.CircleSegment,    "CircleSegmentMirror"},
            { MirrorShape.CircleSegment2,   "CircleSegment2Mirror"},
        };

        /// <summary>
        /// Maps the MirrorLight Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorLight, string> MirrorLightDescKey = new()
        {
            { MirrorLight.WithoutLight,         "WithoutLight"},
            { MirrorLight.Cold ,                "Cold"},
            { MirrorLight.Warm ,                "Warm"},
            { MirrorLight.Daylight,             "DayLight"},
            { MirrorLight.Warm_Cold,            "WarmCold"},
            { MirrorLight.Warm_Cold_Day,        "WarmColdDay"},
            { MirrorLight.Day_COB,              "DayCOB" },
            { MirrorLight.Warm_COB,             "WarmCOB" },
            { MirrorLight.Cold_COB,             "ColdCOB" },
            { MirrorLight.Warm_Cold_Day_COB,    "WarmColdDayCOB" },
            { MirrorLight.Day_16W,              "Day16W" },
            { MirrorLight.Warm_16W,             "Warm16W" },
            { MirrorLight.Cold_16W,             "Cold16W" },
            { MirrorLight.Warm_Cold_Day_16W,    "WarmColdDay16W" },
        };

        /// <summary>
        /// Maps the MirrorLight Enum Value to the Language Key FULL description
        /// </summary>
        public static readonly Dictionary<MirrorLight, string> MirrorLightFullDescKey = new()
        {
            { MirrorLight.WithoutLight,         "WithoutLightFullDescription" },
            { MirrorLight.Cold,                 "ColdFullDescription" },
            { MirrorLight.Warm,                 "WarmFullDescription" },
            { MirrorLight.Daylight,             "DayLightFullDescription" },
            { MirrorLight.Warm_Cold,            "WarmColdFullDescription" },
            { MirrorLight.Warm_Cold_Day,        "WarmColdDayFullDescription" },
            { MirrorLight.Day_COB,              "DayCOBFullDescription" },
            { MirrorLight.Warm_COB,             "WarmCOBFullDescription" },
            { MirrorLight.Cold_COB,             "ColdCOBFullDescription" },
            { MirrorLight.Warm_Cold_Day_COB,    "WarmColdDayCOBFullDescription" },
            { MirrorLight.Day_16W,              "Day16WFullDescription" },
            { MirrorLight.Warm_16W,             "Warm16WFullDescription" },
            { MirrorLight.Cold_16W,             "Cold16WFullDescription" },
            { MirrorLight.Warm_Cold_Day_16W,    "WarmColdDay16WFullDescription" },
        };

        /// <summary>
        /// Maps the MirrorSandblast Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorSandblast, string> MirrorSandblastDescKey = new()
        {
            { MirrorSandblast.H7 , "SandblastH7" },
            { MirrorSandblast.H8 , "SandblastH8" },
            { MirrorSandblast.M3 , "SandblastM3" },
            { MirrorSandblast.X6 , "SandblastX6" },
            { MirrorSandblast.X4 , "SandblastX4" },
            { MirrorSandblast._6000 , "Sandblast6000" },
            { MirrorSandblast.N9 , "SandblastN9" },
            { MirrorSandblast.N6 , "SandblastN6" }

        };

        /// <summary>
        /// Maps the MirrorSandblast Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorSeries, string> MirrorSeriesDescKey = new()
        {
            { MirrorSeries.Custom,  "SeriesCustom"},
            { MirrorSeries.H7,      "SeriesH7" },
            { MirrorSeries.H8,      "SeriesH8" },
            { MirrorSeries.X6,      "SeriesX6" },
            { MirrorSeries.X4,      "SeriesX4" },
            { MirrorSeries._6000,   "Series6000" },
            { MirrorSeries.M3,      "SeriesM3" },
            { MirrorSeries.N9,      "SeriesN9" },
            { MirrorSeries.N6,      "SeriesN6" },
            { MirrorSeries.N7,      "SeriesN7" },
            { MirrorSeries.A7,      "SeriesA7" },
            { MirrorSeries.A9,      "SeriesA9" },
            { MirrorSeries.M8,      "SeriesM8" },
            { MirrorSeries.ND,      "SeriesND" },
            { MirrorSeries.NC,      "SeriesNC" },
            { MirrorSeries.NL,      "SeriesNL" },
            { MirrorSeries.P8,      "SeriesP8" },
            { MirrorSeries.P9,      "SeriesP9" },
            { MirrorSeries.R7,      "SeriesR7" },
            { MirrorSeries.R9,      "SeriesR9" },
            { MirrorSeries.NS,      "SeriesNS" },
            { MirrorSeries.N1,      "SeriesN1" },
            { MirrorSeries.N2,      "SeriesN2" },
            { MirrorSeries.EL,      "SeriesEL" },
            { MirrorSeries.ES,      "SeriesES" },

            { MirrorSeries.IM,      "SeriesIM" },
            { MirrorSeries.IA,      "SeriesIA" },
            { MirrorSeries.N9Custom,"SeriesN9Custom"},
            { MirrorSeries.NA,      "SeriesNA" },
            { MirrorSeries.NCCustom,"SeriesNCCustom"},
            { MirrorSeries.IC,      "SeriesIC" },
            { MirrorSeries.NLCustom,"SeriesNLCustom"},
            { MirrorSeries.IL,      "SeriesIL" },

        };

        /// <summary>
        /// Maps the MirrorOptions Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorOption, string> MirrorOptionsDescKey = new()
        {
            { MirrorOption.BlueTooth, "OptionBlueTooth" },
            { MirrorOption.Clock, "OptionsClock" },
            { MirrorOption.DimmerSwitch, "OptionsDimmer" },
            { MirrorOption.Display19, "OptionsDisplay19" },
            { MirrorOption.Display20, "OptionsDisplay20" },
            { MirrorOption.DisplayRadio, "OptionsDisplay11" },
            { MirrorOption.Fog16W, "OptionsFog16W" },
            { MirrorOption.Fog24W, "OptionsFog24W" },
            { MirrorOption.Fog55W, "OptionsFog55W" },
            { MirrorOption.IPLid, "OptionsIPLid" },
            { MirrorOption.RoundedCorners, "OptionsRoundedCorners" },
            { MirrorOption.SensorSwitch, "OptionsSensor" },
            { MirrorOption.TouchSwitch, "OptionsTouch" },
            { MirrorOption.TouchSwitchFog, "OptionsTouchFog" },
            { MirrorOption.Zoom, "OptionsZoom" },
            { MirrorOption.ZoomLed, "OptionsZoomLed" },
            { MirrorOption.ZoomLedTouch, "OptionsZoomLedTouch" },
            { MirrorOption.BackLightSealedChannel, "OptionsBackLightSealedChannel" },
            { MirrorOption.FrontLightSealedChannel, "OptionsFrontLightSealedChannel" },
            { MirrorOption.SmartAntiFog, "OptionsSmartAntiFog" },
            { MirrorOption.SingleTopLightedPlexiglass, "OptionsSingleTopLightedPlexiglass" },
            { MirrorOption.DoubleTopBottomLightedPlexiglass, "OptionsDoubleTopBottomLightedPlexiglass" },
            { MirrorOption.EcoTouch, "OptionsEcoTouch" },
            { MirrorOption.DisplayRadioBlack, "OptionsScreen11b" },
            { MirrorOption.Lamp, "OptionsLamp" },
            { MirrorOption.LightAluminumChannel, "OptionsLightAluminumChannel" },
        };

        /// <summary>
        /// Maps the MirrorSupports Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<MirrorSupport, string> MirrorSupportDescKey = new()
        {
            { MirrorSupport.Double, "DoubleSupport" },
            { MirrorSupport.Frame, "FrameSupport" },
            { MirrorSupport.FrontSupports, "FrontSmallSupport" },
            { MirrorSupport.Perimetrical, "PerimetricalSupport" },
            { MirrorSupport.Without, "WithoutSupport" }
        };

        /// <summary>
        /// Maps the SupportFinish Type Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<SupportFinishType, string> SupportFinishTypeDescKey = new()
        {
            { SupportFinishType.Electroplated,"ElectroplatedFinish"},
            { SupportFinishType.Painted,"PaintedFinish"},
            { SupportFinishType.Simple,"SimpleFinish"}
        };

        /// <summary>
        /// Maps the Supports' Paint Finish Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<SupportPaintFinish, string> SupportPaintFinishDescKey = new()
        {
            { SupportPaintFinish.Black,         "BlackAnodized"     },
            { SupportPaintFinish.BronzeMat,     "BronzeAnodized"    },
            { SupportPaintFinish.ChromeMat,     "ChromeAnodized"    },
            { SupportPaintFinish.CopperMat,     "CopperAnodized"    },
            { SupportPaintFinish.GoldMat,       "GoldAnodized"      },
            { SupportPaintFinish.GraphiteMat,   "GraphiteAnodized"  },
            { SupportPaintFinish.RalColor,      "RalColor"          },
            { SupportPaintFinish.Silver,        "SilverAnodized"    }
        };

        /// <summary>
        /// Maps the Supports' Electrtoplated Finish Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<SupportElectroplatedFinish, string> SupportElectroplatedFinishDescKey = new()
        {
            { SupportElectroplatedFinish.BronzeBrushed,         "BronzeBrushedElectroplated"         },
            { SupportElectroplatedFinish.BronzeMirror,          "BronzeMirrorElectroplated"          },
            { SupportElectroplatedFinish.CopperBrushed,         "CopperBrushedElectroplated"         },
            { SupportElectroplatedFinish.CopperMirror,          "CopperMirrorElectroplated"          },
            { SupportElectroplatedFinish.GoldSimilarBrushed,    "SimilarGoldBrushedElectroplated"    },
            { SupportElectroplatedFinish.GoldSimilarMirror,     "SimilarGoldMirrorElectroplated"     },
            { SupportElectroplatedFinish.GraphiteBrushed,       "GraphiteBrushedElectroplated"       },
            { SupportElectroplatedFinish.GraphiteMirror,        "GraphiteMirrorElectroplated"        },
            { SupportElectroplatedFinish.NickelBrushed,         "NickelBrushedElectroplated"         },
            { SupportElectroplatedFinish.NickelMirror,          "NickelMirrorElectroplated"          },
            { SupportElectroplatedFinish.RealGoldBrushed,       "RealGoldBrushedElectroplated"       },
            { SupportElectroplatedFinish.RealGoldMirror,        "RealGoldMirrorElectroplated"        },
            { SupportElectroplatedFinish.RoseGoldMirror,        "RoseGoldMirrorElectroplated"        },
            { SupportElectroplatedFinish.RoseGoldBrushed,       "RoseGoldBrushedElectroplated"       },

        };

        /// <summary>
        /// Maps the FogTouchOption Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<bool, string> FogTouchOptionsDescKey = new()
        {
            { true, "WithAutonomusTouch" },
            { false, "WithAutonomusTouchEco" }
        };

        #endregion
    }
}
