using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoMirror;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.Dialogs
{
    public partial class FogDialog : ComponentBase, IDisposable
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public bool HasFog16 { get; set; }
        [Parameter] public EventCallback<bool> HasFog16Changed { get; set; }

        [Parameter] public bool HasFog24 { get; set; }
        [Parameter] public EventCallback<bool> HasFog24Changed { get; set; }

        [Parameter] public bool HasFog55 { get; set; }
        [Parameter] public EventCallback<bool> HasFog55Changed { get; set; }

        [Parameter] public bool HasFogSwitch { get; set; }
        [Parameter] public EventCallback<bool> HasFogSwitchChanged { get; set; }

        [Parameter] public bool HasEcoTouch { get; set; }
        [Parameter] public EventCallback<bool> HasEcoTouchChanged { get; set; }

        [Parameter] public bool IsFogSwitchOptionDisabled { get; set; }

        [Parameter] public EventCallback<MirrorLight> OnNextClick { get; set; }
        [Parameter] public EventCallback OnPreviousClick { get; set; }

        [Parameter] public MirrorLight? Light { get; set; }

        [Parameter] public decimal PriceIncreaseFactor { get; set; }

        [Parameter] public List<MirrorOption> SelectableFogOptions { get; set; } = [];
        [Parameter] public List<MirrorOption> SelectableFogSwitchOptions { get; set; } = [];

        private string SelectedFogSwitchString 
        {
            get
            {
                if (HasFogSwitch)
                {
                    return MirrorOption.TouchSwitchFog.ToString();
                }
                else if (HasEcoTouch)
                {
                    return MirrorOption.EcoTouch.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value == MirrorOption.TouchSwitchFog.ToString())
                {
                    HasFogSwitch = true;
                    HasEcoTouch = false;
                    HasFogSwitchChanged.InvokeAsync(true);
                    HasEcoTouchChanged.InvokeAsync(false);
                }
                else if (value == MirrorOption.EcoTouch.ToString())
                {
                    HasEcoTouch = true;
                    HasFogSwitch = false;
                    HasFogSwitchChanged.InvokeAsync(false);
                    HasEcoTouchChanged.InvokeAsync(true);
                }
                else
                {
                    HasFogSwitch = false;
                    HasEcoTouch = false;
                    HasFogSwitchChanged.InvokeAsync(false);
                    HasEcoTouchChanged.InvokeAsync(false);
                }
                
            }
        }

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        protected override void OnParametersSet()
        {
            //Needed so when Component is Rendered to Change State of ComboBox According to the Provided Parameters
            CheckFogOption();
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsPricingVisible))
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Weather the specififed fog option is enabled for selection from the User
        /// </summary>
        /// <param name="fogOption"></param>
        /// <returns></returns>
        private bool IsFogEnabled(MirrorOption fogOption)
        {
            return SelectableFogOptions.Any(o => fogOption == o);
        }

        /// <summary>
        /// Determines the Touch Switch Fog ComboBox and Option Logic
        /// Turns the Button Off if all Fogs are not selected - otherwise turns it on
        /// Method Runs inside the Setters of the HasFog Parameters
        /// Removes Fog Switch if all Fogs are not Selected
        /// </summary>
        private void CheckFogOption()
        {
            if (Light == MirrorLight.WithoutLight)
            {
                HasFogSwitch = false;
                HasEcoTouch = false;
                IsFogSwitchOptionDisabled = true;
            }
            else if (!HasFog16 && !HasFog24 && !HasFog55)
            {
                HasFogSwitch = false;
                HasEcoTouch = false;
                IsFogSwitchOptionDisabled = true;
            }
            else
            {
                IsFogSwitchOptionDisabled = SelectableFogSwitchOptions.Count != 2;
                if (!HasFogSwitch && !HasEcoTouch) HasFogSwitch = true;
            }
            //To Inform consumers of ChangedCallback about changes when they are done at the Begining of opening the Dialog

            //CANNOT BE INSIDE HERE OTHERWISE IT CAUSED REGRESSION BUGS (INFINATE LOOPS WITH ON PARAMETERS SET)
            //HasFogSwitchChanged.InvokeAsync(HasFogSwitch);
            //HasEcoTouchChanged.InvokeAsync(HasEcoTouch);
        }

        private void SetHasFog16(bool val)
        {
            HasFog16Changed.InvokeAsync(val);
            CheckFogOption();
            HasFogSwitchChanged.InvokeAsync(HasFogSwitch);
            HasEcoTouchChanged.InvokeAsync(HasEcoTouch);
        }
        private void SetHasFog24(bool val)
        {
            HasFog24Changed.InvokeAsync(val);
            CheckFogOption();
            HasFogSwitchChanged.InvokeAsync(HasFogSwitch);
            HasEcoTouchChanged.InvokeAsync(HasEcoTouch);
        }
        private void SetHasFog55(bool val)
        {
            HasFog55Changed.InvokeAsync(val);
            CheckFogOption();
            HasFogSwitchChanged.InvokeAsync(HasFogSwitch);
            HasEcoTouchChanged.InvokeAsync(HasEcoTouch);
        }

        /// <summary>
        /// Returns the Description of the Select Item for the Fog Switch or the Eco Touch
        /// </summary>
        /// <param name="hasFogTouchElseEcoTouch">True for Fog Switch - False for Eco Switch</param>
        /// <returns></returns>
        private string FogTouchSelectItemDescription(string selectedOptionString)
        {
            MirrorOption? option = Enum.TryParse<MirrorOption>(selectedOptionString, out MirrorOption parsedValue) ? parsedValue : null;

            string selectItemCaption = option is not null ? languageContainer.Keys[StaticInfoMirror.MirrorOptionsDescKey[(MirrorOption)option]] : "--------------";

            //When User Logged in Show Price on Select Item of Fog Switch
            if (user.IsPricingVisible && option is not null)
            {
                selectItemCaption += $" ({MirrorExtra.GetOptionPrice((MirrorOption)option) * PriceIncreaseFactor:0.00€})";
            }
            return selectItemCaption;

        }
        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
