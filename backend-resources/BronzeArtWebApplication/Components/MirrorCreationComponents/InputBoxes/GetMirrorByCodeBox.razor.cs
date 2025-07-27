using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.InputBoxes
{
    public partial class GetMirrorByCodeBox : ComponentBase
    {
        [Parameter] public string Style { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public Variant TextBoxVariant { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public EventCallback<Mirror> OnEnterKeyPressed { get; set; }
        public string ErrorTextMessage { get; set; }
        public string UserText { get; set; } = "";

        /// <summary>
        /// Determines whether the TextField Appears in a Wrong State (Typed Text is Wrong)
        /// </summary>
        public bool IsTypedTextWrong { get; set; }

        /// <summary>
        /// Provides a Mirror from a Text String Representing the SKU code of the Mirror
        /// </summary>
        /// <param name="text">The Code Text</param>
        /// <returns>A Mirror represented by the text parameter</returns>
        private Mirror CodeTextToMirror(string text)
        {
            if (text.Length < 4)
            {
                return null;
            }

            Mirror mirror;

            //Check if Written Code Exists in the Ready Mirrors
            mirror = MirrorsStaticData.CatalogueMirrors.FirstOrDefault(m => m.Code == text);
            if (mirror is not null)
            {
                return mirror; //If Found return it else proceed to next Step
            }


            //CUSTOM MIRROR
            mirror = new();
            switch (text[..4])
            {
                case "60H7":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60H8":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.H8;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60X6":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.X6;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60X4":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.X4;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "6000":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast._6000;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60M3":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.M3;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60N9":
                    mirror.Shape = MirrorShape.Circular;
                    mirror.Sandblast = MirrorSandblast.N9;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60N6":
                    mirror.Shape = MirrorShape.Circular;
                    mirror.Sandblast = MirrorSandblast.N6;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60NC":
                    mirror.Shape = MirrorShape.Capsule;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60NL":
                    mirror.Shape = MirrorShape.Ellipse;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60IM":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Perimetrical;
                    mirror.Lighting.Light = MirrorLight.Daylight;
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.TouchSwitch));
                    break;
                case "60IA":
                    mirror.Shape = MirrorShape.Rectangular;
                    mirror.Support.SupportType = MirrorSupport.Double;
                    mirror.Lighting.Light = MirrorLight.WithoutLight;
                    mirror.Sandblast = MirrorSandblast.H7;
                    break;
                case "60IC":
                    mirror.Shape = MirrorShape.Capsule;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Double;
                    mirror.Lighting.Light = MirrorLight.WithoutLight;
                    break;
                case "60IL":
                    mirror.Shape = MirrorShape.Ellipse;
                    mirror.Sandblast = MirrorSandblast.H7;
                    mirror.Support.SupportType = MirrorSupport.Double;
                    mirror.Lighting.Light = MirrorLight.WithoutLight;
                    break;
                case "60NA":
                    mirror.Shape = MirrorShape.Circular;
                    mirror.Sandblast = MirrorSandblast.N9;
                    mirror.Support.SupportType = MirrorSupport.Double;
                    mirror.Lighting.Light = MirrorLight.WithoutLight;
                    break;
                default: //If none of the above is found return Null
                    return null;
            }

            if (UserText.Length >= 5 && UserText.Substring(4, 1) == "-")
            {
                //if there is 3digit Length/Diameter
                if (UserText.Length >= 8 && int.TryParse(UserText.Substring(5, 3), out int length3))
                {
                    if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                    {
                        mirror.Length = length3;
                    }
                    else if (mirror.Shape is MirrorShape.Circular)
                    {
                        mirror.Diameter = length3;
                    }

                    // Continue to Height
                    if (UserText.Length >= 12 && UserText.Substring(8, 1) == "-" && int.TryParse(UserText.Substring(9, 3), out int height3))
                    {
                        if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                        {
                            mirror.Height = height3;
                        }
                    }
                    else if (UserText.Length >= 11 && UserText.Substring(8, 1) == "-" && int.TryParse(UserText.Substring(9, 2), out int height2))
                    {
                        if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                        {
                            mirror.Height = height2;
                        }
                    }
                }
                //if there is 2 digit Length
                else if (UserText.Length >= 7 && int.TryParse(UserText.Substring(5, 2), out int length2))
                {
                    if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                    {
                        mirror.Length = length2;
                    }
                    else if (mirror.Shape is MirrorShape.Circular)
                    {
                        mirror.Diameter = length2;
                    }

                    // Continue to Height
                    if (UserText.Length >= 11 && UserText.Substring(7, 1) == "-" && int.TryParse(UserText.Substring(8, 3), out int height3))
                    {
                        if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                        {
                            mirror.Height = height3;
                        }
                    }
                    else if (UserText.Length >= 10 && UserText.Substring(7, 1) == "-" && int.TryParse(UserText.Substring(8, 2), out int height2))
                    {
                        if (mirror.Shape is MirrorShape.Rectangular or MirrorShape.Capsule or MirrorShape.Ellipse)
                        {
                            mirror.Height = height2;
                        }
                    }
                }
            }

            return mirror;
        }

        /// <summary>
        /// Validates The Typed Text
        /// </summary>
        private void ValidateUserText()
        {
            if (UserText.Length < 4 && UserText.Length > 0)
            {
                IsTypedTextWrong = true;
                ErrorTextMessage = lc.Keys["InvalidLengthInput"];
                return;
            }
            else if (UserText.Substring(0, 4) is not
                "60H7" and not "60H8" and not "60X6" and not
                "60X4" and not "6000" and not "60M3" and not
                "60N9" and not "60N7" and not "60N6" and not
                "60IM" and not "60IA" and not "60NA" and not 
                "60A9" and not "60NC" and not "60IC" and not
                "60NL" and not "60IL")
            {
                IsTypedTextWrong = true;
                ErrorTextMessage = lc.Keys["InvalidCodeInput"];
                return;
            }
            else
            {
                IsTypedTextWrong = false;
                ErrorTextMessage = string.Empty;
            }
        }
    }
}
