using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoMirror;
using MudBlazor;
using BronzeArtWebApplication.Shared.Enums;
using System;
using BronzeArtWebApplication.Shared.Helpers;
using System.Linq;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.AssembleMirrorComponents
{
    public partial class MirrorPropertiesButtonPanel : ComponentBase
    {
        [Parameter] public Color ButtonIconColor { get; set; }
        [Parameter] public Variant OptionButtonVariant { get; set; }

        [Parameter] public EventCallback<MirrorDialog> OnOptionButtonClick { get; set; }
        [Parameter] public EventCallback ResetButtonClick { get; set; }

        #region 1.Button Captions GetMethods
        private string ShapeButtonCaption()
        {
            string caption = languageContainer.Keys["DialogShapeTitle"];
            if (Vm.Shape != null)
            {
                caption = $"{caption} - {languageContainer.Keys[MirrorShapeDescKey[(MirrorShape)Vm.Shape]]}";
            }
            return caption;
        }
        private string LightButtonCaption()
        {
            string caption = languageContainer.Keys["DialogLightTitle"];
            if (Vm.Light != null)
            {
                caption = $"{caption} - {languageContainer.Keys[MirrorLightDescKey[(MirrorLight)Vm.Light]]}";
            }
            return caption;
        }
        private string SupportButtonCaption()
        {
            string caption = languageContainer.Keys["DialogSupportsTitle"];
            if (Vm.Support != null)
            {
                caption = $"{caption} - {languageContainer.Keys[MirrorSupportDescKey[(MirrorSupport)Vm.Support]]}";
            }
            return caption;
        }
        private string FrameFinishButtonCaption()
        {
            string caption = languageContainer.Keys["DialogFrameFinishTitle"];
            if (Vm.Support == MirrorSupport.Frame)
            {
                if (Vm.FinishType == SupportFinishType.Painted && Vm.PaintFinish is not null)
                {
                    caption = $"{caption} - {languageContainer.Keys[SupportPaintFinishDescKey[(SupportPaintFinish)Vm.PaintFinish]]} {languageContainer.Keys["Anodized"]}";
                }
                else if (Vm.FinishType == SupportFinishType.Electroplated && Vm.ElectroplatedFinish is not null)
                {
                    caption = $"{caption} - {languageContainer.Keys[SupportElectroplatedFinishDescKey[(SupportElectroplatedFinish)Vm.ElectroplatedFinish]]} {languageContainer.Keys["Electroplated"]}";
                }
            }
            return caption;
        }
        private string DimensionsButtonCaption()
        {
            string caption = languageContainer.Keys["DialogDimensionsTitle"];
            if (Vm.Shape is null) return $"{caption} - ????";

            if (Vm.Shape is not MirrorShape.Circular)
            {
                if (Vm.Height > 0 && Vm.Length > 0)
                {
                    caption = $"{caption} - {Vm.Length}x{Vm.Height}{languageContainer.Keys["cm"]}";
                }
                else
                {
                    caption = $"{caption} - ????";
                }
            }
            else 
            {
                if (Vm.Diameter > 0)
                {
                    caption = $"{caption} - Φ{Vm.Diameter}{languageContainer.Keys["cm"]}";
                }
                else
                {
                    caption = $"{caption} - ????";
                }
            }
            return caption;
        }
        private string SandblastButtonCaption()
        {
            string caption = languageContainer.Keys["DialogSandblastTitle"];
            if (Vm.Sandblast != null && Vm.Shape != null)
            {
                caption = $"{caption} - {languageContainer.Keys[MirrorSandblastDescKey[(MirrorSandblast)Vm.Sandblast]]}";
            }
            return caption;
        }
        private string SwitchButtonCaption()
        {
            string caption = languageContainer.Keys["DialogTouchTitle"];
            return caption;
        }
        #endregion

        #region 2.Disabled/Enabled Button Methods
        private bool IsEnabledLightButton()
        {
            if (Vm.Shape != null && Vm.Series is not MirrorSeries.ES and not MirrorSeries.EL)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledSupportButton()
        {
            if (Vm.IsDimensionsChosen && StaticInfoMirror.GetSelectableSupports(Vm.Series).Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledFrameFinishButton()
        {
            if (Vm.Support is MirrorSupport.Frame 
                && Vm.Shape is MirrorShape.Rectangular or MirrorShape.Circular 
                && Vm.Series is not MirrorSeries.P8 and not MirrorSeries.P9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledDimensionsButton()
        {
            if (Vm.Shape != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledSandblastButton()
        {
            if (Vm.Light != null &&
                Vm.Shape != null &&
                Vm.Support != null && 
                StaticInfoMirror.GetSelectableSandblasts(Vm.Series).Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledSwitchButton()
        {
            if (Vm.Light != null &&
                Vm.Light != MirrorLight.WithoutLight &&
                Vm.Sandblast != null
                && (StaticInfoMirror.GetSelectableTouchOptions(Vm.Series).Count > 0 
                    || Vm.GetMirrorObject().HasExtrasAny(MirrorOption.TouchSwitch,MirrorOption.DimmerSwitch,MirrorOption.SensorSwitch)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledDemisterButton()
        {
            if (Vm.Shape != null 
                && Vm.Sandblast != null 
                && Vm.Light != null 
                && Vm.Support != null
                && (StaticInfoMirror.GetSelectableFogOptions(Vm.Series).Count > 0 
                        || Vm.GetMirrorObject().HasExtrasAny(MirrorOption.Fog16W , MirrorOption.Fog24W , MirrorOption.Fog55W))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledMagnifyerButton()
        {
            if (Vm.Shape != null
                && Vm.Sandblast != null
                && Vm.Light != null
                && Vm.Support != null
                && (StaticInfoMirror.GetSelectableMagnifierOptions(Vm.Series).Count > 0
                        || Vm.GetMirrorObject().HasExtrasAny(MirrorOption.Zoom, MirrorOption.ZoomLed, MirrorOption.ZoomLedTouch))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledMediaButton()
        {
            if (Vm.Shape != null
                && Vm.Sandblast != null
                && Vm.Light != null
                && Vm.Support != null
                && (StaticInfoMirror.GetSelectableMediaOptions(Vm.Series).Count > 0
                        || Vm.GetMirrorObject().HasExtrasAny(MirrorOption.Clock, MirrorOption.BlueTooth, MirrorOption.DisplayRadio, MirrorOption.DisplayRadioBlack , MirrorOption.Display19))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsEnabledVariousButton()
        {
            if (Vm.Shape != null
                && Vm.Sandblast != null
                && Vm.Light != null
                && Vm.Support != null
                && (StaticInfoMirror.GetSelectableExtraOptions(Vm.Series).Count > 0
                        || Vm.GetMirrorObject().HasExtrasAny(MirrorOption.IPLid, MirrorOption.RoundedCorners))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
