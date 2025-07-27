using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.ThirdStep
{
    public partial class SelectOpeningsModelWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<CabinDrawNumber> OnModelSelection { get; set; }

        /// <summary>
        /// According to the Window that need to be Shown a Model List is Populated on the Initilized Lifecycle Method
        /// </summary>
        [Parameter] public StoryWindow ModelWindowToRender { get; set; }

        //The ModelsList to Populate
        private List<CabinDrawNumber> modelsList;

        protected override void OnInitialized()
        {
            switch (ModelWindowToRender)
            {
                case StoryWindow.ModelsBathtub:
                    modelsList = new() { CabinDrawNumber.DrawNV, CabinDrawNumber.DrawMV2, CabinDrawNumber.DrawNV2, CabinDrawNumber.Draw8W40 };
                    break;
                case StoryWindow.ModelsS:
                    modelsList = new() { CabinDrawNumber.Draw9S, CabinDrawNumber.DrawVS, CabinDrawNumber.DrawWS };
                    break;
                case StoryWindow.ModelsSF:
                    modelsList = new() { CabinDrawNumber.Draw9S9F, CabinDrawNumber.DrawVSVF };
                    break;
                case StoryWindow.ModelsSFF:
                    modelsList = new() { CabinDrawNumber.Draw9S9F9F };
                    break;
                case StoryWindow.Models4:
                    modelsList = new() { CabinDrawNumber.Draw94, CabinDrawNumber.DrawV4 };
                    break;
                case StoryWindow.Models4F:
                    modelsList = new() { CabinDrawNumber.Draw949F, CabinDrawNumber.DrawV4VF };
                    break;
                case StoryWindow.Models4FF:
                    modelsList = new() { CabinDrawNumber.Draw949F9F };
                    break;
                case StoryWindow.ModelsA:
                    modelsList = new() { CabinDrawNumber.Draw9A, CabinDrawNumber.DrawVA };
                    break;
                case StoryWindow.ModelsAF:
                    modelsList = new() { CabinDrawNumber.Draw9A9F };
                    break;
                case StoryWindow.ModelsC:
                    modelsList = new() { CabinDrawNumber.Draw9C };
                    break;
                case StoryWindow.ModelsCF:
                    modelsList = new() { CabinDrawNumber.Draw9C9F };
                    break;
                case StoryWindow.ModelsP44:
                    modelsList = new() { CabinDrawNumber.DrawNP44 };
                    break;
                case StoryWindow.ModelsP46:
                    modelsList = new() { CabinDrawNumber.Draw2CornerNP46 };
                    break;
                case StoryWindow.ModelsP48:
                    modelsList = new() { CabinDrawNumber.Draw2StraightNP48 };
                    break;
                case StoryWindow.ModelsP45:
                    modelsList = new() { CabinDrawNumber.DrawCornerNP6W45};
                    break;
                case StoryWindow.ModelsP47:
                    modelsList = new() { CabinDrawNumber.DrawStraightNP6W47};
                    break;
                case StoryWindow.ModelsB3151:
                    modelsList = new() { CabinDrawNumber.DrawNB31, CabinDrawNumber.DrawDB51 };
                    break;
                case StoryWindow.ModelsB3252:
                    modelsList = new() { CabinDrawNumber.DrawCornerNB6W32,CabinDrawNumber.DrawCornerDB8W52 };
                    break;
                case StoryWindow.ModelsB3353:
                    modelsList = new() { CabinDrawNumber.Draw2CornerNB33, CabinDrawNumber.Draw2CornerDB53 };
                    break;
                case StoryWindow.ModelsB3859:
                    modelsList = new() { CabinDrawNumber.DrawStraightNB6W38, CabinDrawNumber.DrawStraightDB8W59 };
                    break;
                case StoryWindow.ModelsB4161:
                    modelsList = new() { CabinDrawNumber.Draw2StraightNB41, CabinDrawNumber.Draw2StraightDB61 };
                    break;
                case StoryWindow.ModelsHB34:
                    modelsList = new() { CabinDrawNumber.DrawHB34 };
                    break;
                case StoryWindow.ModelsHB35:
                    modelsList = new() { CabinDrawNumber.DrawCornerHB8W35 };
                    break;
                case StoryWindow.ModelsHB37:
                    modelsList = new() { CabinDrawNumber.Draw2CornerHB37 };
                    break;
                case StoryWindow.ModelsHB40:
                    modelsList = new() { CabinDrawNumber.DrawStraightHB8W40 };
                    break;
                case StoryWindow.ModelsHB43:
                    modelsList = new() { CabinDrawNumber.Draw2StraightHB43 };
                    break;
                case StoryWindow.ModelsW:
                    modelsList = new() { CabinDrawNumber.Draw8W };
                    break;
                case StoryWindow.ModelsE:
                    modelsList = new() { CabinDrawNumber.DrawE };
                    break;
                case StoryWindow.Models81:
                    modelsList = new() { CabinDrawNumber.Draw8WFlipper81 };
                    break;
                case StoryWindow.Models82:
                    modelsList = new() { CabinDrawNumber.Draw2Corner8W82 };
                    break;
                case StoryWindow.Models84:
                    modelsList = new() { CabinDrawNumber.Draw1Corner8W84 };
                    break;
                case StoryWindow.Models85:
                    modelsList = new() { CabinDrawNumber.Draw2Straight8W85 };
                    break;
                case StoryWindow.Models88:
                    modelsList = new() { CabinDrawNumber.Draw2CornerStraight8W88 };
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(StoryWindow)} :{ModelWindowToRender} is not Valid Parameter for {nameof(SelectOpeningsModelWindow)} Component");
            }

            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change Name
            if (e.PropertyName == ModelWindowToRender.ToString())
            {
                StateHasChanged();
            }
        }

        private void HandleModelSelection(CabinDrawNumber selectedModel)
        {
            vm.SelectedCabinDraw = selectedModel; //Pass the Selected Model as Primary

            //Go to the Cabin Panel and Close this window.
            vm.ShowWindow(StoryWindow.CabinPanel, ModelWindowToRender);
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
