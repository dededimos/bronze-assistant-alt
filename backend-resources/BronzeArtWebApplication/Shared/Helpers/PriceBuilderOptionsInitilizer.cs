using BronzeRulesPricelistLibrary.Builders;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Text;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoMirror;

namespace BronzeArtWebApplication.Shared.Helpers
{
    /// <summary>
    /// Initilizes the Options for the Price Builder
    /// The Developer can Set here the Photo Paths and Descriptions for all BronzeProducts in the PricingLibrary
    /// This way the Pricing Library is decoupled from the LocalizationLanguage & Photo Paths of each Application its used on
    /// </summary>
    public static class PriceBuilderOptionsInitilizer
    {
        /// <summary>
        /// Sets the Options Description and PhotoPath Functions to be Used by the Builder
        /// </summary>
        /// <returns>Builder Options</returns>
        public static BronzeItemsPriceBuilderOptions InitilizeBuilderOptions()
        {
            BronzeItemsPriceBuilderOptions options = new();
            options.CabinDescFunc = cabin => SetCabinDescKeys(cabin);
            options.Cabin9CDescFunc = (cabin1, cabin2) => SetCabin9CDescKeys(cabin1, cabin2);
            options.CabinExtraDescFunc = extra => SetCabinExtraDescKeys(extra);

            options.CabinPhotoPath = cabin => SetCabinSketchPhotoPath(cabin);
            options.CabinExtraPhotoPath = extra => SetCabinExtraPhotoPath(extra);
            options.CabinPartDescFunc = part => SetCabinPartDescKeys(part);

            options.MirrorDescFunc = mirror => SetMirrorDescKeys(mirror);
            options.MirrorSandblastDescFunc = sandblast => SetSandblastDescKeys(sandblast);
            options.MirrorLightDescFunc = light => SetLightDescKeys(light);
            options.MirrorSupportDescFunc = support => SetSupportDescKeys(support);
            options.MirrorExtraDescFunc = extra => SetMirrorExtraDescKeys(extra);

            return options;
        }

        #region 1.Cabin Set PhotoPath-Description Functions

        /// <summary>
        /// Returns the List of Descriptions for a Cabin
        /// </summary>
        /// <param name="cabin">The Cabin</param>
        /// <returns>The Description Keys/Strings </returns>
        private static List<string> SetCabinDescKeys(Cabin cabin)
        {
            List<string> keys = new();
            string series = CabinSeriesDescKey[cabin.Series];
            string model = cabin.Model != null ? CabinModelEnumDescKey[(CabinModelEnum)cabin.Model] : "N/A Model";
            string metalFinish = cabin.MetalFinish != null ? CabinFinishEnumDescKey[(CabinFinishEnum)cabin.MetalFinish] : "N/A Cabin Finish";
            string thickness = cabin.Thicknesses != null ? CabinThicknessesEnumDescKey[(CabinThicknessEnum)cabin.Thicknesses] : "N/A Thickness";
            string glassFinish = cabin.GlassFinish != null ? GlassFinishEnumDescKey[(GlassFinishEnum)cabin.GlassFinish] : "N/A Glass Finish";
            string dimensions = $"{cabin.NominalLength}x{cabin.Height}mm";
            string direction = CabinDirectionDescKey[cabin.Direction];
            keys.Add(series);
            keys.Add(model);
            keys.Add(metalFinish);
            keys.Add(thickness);
            keys.Add(glassFinish);
            if (cabin.Model != CabinModelEnum.Model9C) { keys.Add(dimensions); }; //Otherwise the Transforming method must Provide this DescKey
            return keys;
        }
        /// <summary>
        /// Returns the List of Descriptions for a 9C Combined Cabin
        /// </summary>
        /// <param name="cabin1">The First Part</param>
        /// <param name="cabin2">The Second Part</param>
        /// <returns></returns>
        private static List<string> SetCabin9CDescKeys(Cabin9C cabin1, Cabin9C cabin2)
        {
            List<string> keys = SetCabinDescKeys(cabin1);
            keys.Add(($"{cabin1.NominalLength}x{cabin2.NominalLength}x{cabin1.Height}mm"));
            return keys;
        }
        /// <summary>
        /// Returns the List of Descriptions for a CabinExtra
        /// </summary>
        /// <param name="cabin">The Cabin</param>
        /// <returns>The Description Keys/Strings </returns>
        private static List<string> SetCabinExtraDescKeys(CabinExtra extra)
        {
            List<string> descriptions = new();

            switch (extra.ExtraType)
            {
                //If its A step add also its size
                case CabinExtraType.StepCut:
                    StepCut step = (StepCut)extra;
                    descriptions.Add("StepCut");
                    descriptions.Add($"{step.StepLength} x (h){step.StepHeight}mm");
                    break;
                //Otherwise Add only the DescriptionKey for the Extra
                case CabinExtraType.BronzeClean:
                case CabinExtraType.SafeKids:
                    descriptions.Add(CabinExtraDescKey[extra.ExtraType]);
                    break;
            }

            return descriptions;
        }
        /// <summary>
        /// Returns the Sketch-Photo Path for a Cabin
        /// </summary>
        /// <param name="cabin">The Cabin Piece</param>
        /// <returns>the strin path</returns>
        private static string SetCabinSketchPhotoPath(Cabin cabin)
        {
            string path;
            if (cabin is Cabin9C)
            {
                //9C Sketch is without Sides , takes directly the Whole Sketch without marking specific Sides
                path = cabin.IsPartOfDraw is CabinDrawNumber.Draw9C ?
                    "../Images/CabinImages/AllCabinDrawSketches/Semicircular.png" :
                    (cabin.IsPartOfDraw is CabinDrawNumber.Draw9C9F ?
                        "../Images/CabinImages/AllCabinDrawSketches/SideSketches/CFPrimarySecondary.png" : 
                        "");
            }
            else
            {
                //Get the Path from the Dictionary of Sketch Paths by Defining the Draw Number and synthesis Model Number
                path = CabinDrawNumberSideImagePath[(cabin.IsPartOfDraw, cabin.SynthesisModel)];
            }
            return path;
        }
        /// <summary>
        /// Returns the Image path for a Cabin Extra
        /// </summary>
        /// <param name="extra">The Cabin Extra</param>
        /// <returns>the string path</returns>
        private static string SetCabinExtraPhotoPath(CabinExtra extra)
        {
            string path = CabinExtraImagePath[extra.ExtraType];
            return path;
        }

        private static List<string> SetCabinPartDescKeys(CabinPart part)
        {
            List<string> keys = new();
            if (part is CabinHandle handle)
            {
                keys.Add(CabinHandleTypeDescKey[handle.HandleType]);
                if (handle.IsCircularHandle)
                {
                    keys.Add($"Φ{handle.HandleLengthToGlass}");
                }
                else
                {
                    keys.Add($"{handle.HandleLengthToGlass}x{handle.HandleHeightToGlass}mm");
                }
            }
            else if (part is GlassStrip strip)
            {
                keys.Add(CabinStripTypeDescKey[strip.StripType]);
            }
            else
            {
                keys.Add(GetCabinPartTypeDescKey(part.Part));
            }
            keys.Add(MaterialTypeDescKey[part.Material]);
                        
            return keys;

        }

        #endregion

        #region 2.Mirror Set PhotoPath-Description Functions

        /// <summary>
        /// Returns the Description Keys for a Mirror
        /// </summary>
        /// <param name="mirror">The Mirror</param>
        /// <returns>a List of Description Keys that form the Description of the Mirror</returns>
        private static List<string> SetMirrorDescKeys(Mirror mirror)
        {
            List<string> keys = new();
            
            string shape = mirror.Shape?.ToString() ?? "----";
            string light = mirror.HasLight() ? "WithLight" : "WithoutLight";

            StringBuilder builder = new();
            switch (mirror.Shape)
            {
                case MirrorShape.Rectangular:
                case MirrorShape.Capsule:
                case MirrorShape.Ellipse:
                case MirrorShape.Special:
                case MirrorShape.StoneNS:
                case MirrorShape.PebbleND:
                case MirrorShape.CircleSegment:
                case MirrorShape.CircleSegment2:
                    builder.Append(mirror.Length).Append('x').Append(mirror.Height).Append("cm");
                    break;
                case MirrorShape.Circular:
                    builder.Append('Φ').Append(mirror.Diameter).Append("cm");
                    break;
                case null:
                    break;
                default:
                    builder.Append("----");
                    break;
            }
            string dimensions = builder.ToString();

            keys.Add(shape);
            keys.Add("Mirror");
            keys.Add(light);
            keys.Add(dimensions);
            return keys;
        }

        /// <summary>
        /// Returns the Description Keys for the Sandblast of a Mirror
        /// </summary>
        /// <param name="sandblast"></param>
        /// <returns></returns>
        private static List<string> SetSandblastDescKeys(MirrorSandblast sandblast)
        {
            List<string> keys = new();
            string prefix;
            if (sandblast is not MirrorSandblast.H7 and not MirrorSandblast.N9)
            {
                prefix = "WithSandblast";
                string suffix = MirrorSandblastDescKey[sandblast];
                keys.Add(prefix);
                keys.Add(suffix);
            }
            else
            {
                prefix = "WithoutSandblast";
                keys.Add(prefix);
            }

            return keys;
        }

        /// <summary>
        /// Returns the Description Key for Lighting
        /// </summary>
        /// <param name="lighting"></param>
        /// <returns></returns>
        private static List<string> SetLightDescKeys(LightingModel lighting)
        {
            List<string> keys = [];
            MirrorLight light = lighting.Light ?? MirrorLight.WithoutLight;
            string key = MirrorLightFullDescKey[light];
            keys.Add(key);
            return keys;
        }

        /// <summary>
        /// Returns the Description Keys for Support
        /// </summary>
        /// <param name="support">The SupportModel</param>
        /// <returns>The Description Keys that form the Description of a Support</returns>
        private static List<string> SetSupportDescKeys(SupportModel support) 
        {
            List<string> keys = [];
            MirrorSupport supportType = support.SupportType ?? MirrorSupport.Without;
            string prefix = MirrorSupportDescKey[supportType];
            string suffix1 = "";
            string suffix2 = "";
            keys.Add(prefix);

            if (support.SupportType == MirrorSupport.Frame)
            {
                if (support.FinishType == SupportFinishType.Painted)
                {
                    if (support.PaintFinish is not null)
                    {
                        suffix1 = SupportPaintFinishDescKey[(SupportPaintFinish)support.PaintFinish];
                        suffix2 = "Anodized";
                        keys.Add(suffix1);
                        keys.Add(suffix2);
                    }
                    else
                    {
                        suffix1 = "AnodizeNotDefined";
                        keys.Add(suffix1);
                    }
                }
                else if (support.FinishType == SupportFinishType.Electroplated)
                {
                    if (support.ElectroplatedFinish is not null)
                    {
                        suffix1 = SupportElectroplatedFinishDescKey[(SupportElectroplatedFinish)support.ElectroplatedFinish];
                        suffix2 = "Electroplated";
                        keys.Add(suffix1);
                        keys.Add(suffix2);
                    }
                    else
                    {
                        suffix1 = "ElectroplatingNotDefined";
                        keys.Add(suffix1);
                    }
                }
            }
            return keys;
        }
        
        /// <summary>
        /// Returns the Description Keys for a MirrorExtra
        /// </summary>
        /// <param name="extra">The Mirror Extra</param>
        /// <returns>The Description Key for a mirror Extra</returns>
        private static List<string> SetMirrorExtraDescKeys(MirrorExtra extra)
        {
            List<string> keys = [];
            string key = MirrorOptionsDescKey[extra.Option];
            keys.Add(key);
            if (extra.Option is MirrorOption.Fog16W){keys.Add("29x29cm");}
            if (extra.Option is MirrorOption.Fog24W){keys.Add("29x41cm");}
            if (extra.Option is MirrorOption.Fog55W){keys.Add("51x51cm");}
            return keys;
        }

        #endregion


    }
}
