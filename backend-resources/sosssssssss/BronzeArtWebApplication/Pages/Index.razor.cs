using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Services;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.DrawsBuilder.Models;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using MudBlazor;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages
{
    public partial class Index
    {
        public const string MachineRegistryStorageKey = "MachReg";

        [Inject] public BronzeUser user { get; set; }
        [Inject] public IDialogService ds { get; set; }
        [Inject] public ILocalStorageService StorageService { get; set; }
        [Inject] public IMessageService MessageService { get; set; }

        private void SelectCabin()
        {
            navManager.NavigateTo("/AssembleCabin");
        }

        private void SelectMirror()
        {
            navManager.NavigateTo("/AssembleMirror");
        }

        private void SelectAccessories()
        {
            navManager.NavigateTo("/Accessories");
            //if (user.IsPowerUser)
            //{
            //    navManager.NavigateTo("/AccessoriesSelection");
            //}
            //else
            //{
            //    ds.ShowMessageBox(lc.Keys["UnderConstruction"], lc.Keys["SectionStillUnderConstruction"], yesText: "Ok");
            //}
        }

        protected override void OnInitialized()
        {
            //Navigate Directly to Mirror For WhiteLabeled URIs
            if (user.IsWhiteLabeled && user.RetailTheme != RetailModeTheme.Lakiotis)
            {
                SelectMirror();
            }
            base.OnInitialized();
        }

        private async Task PromptToRegisterMachine()
        {
            // check if this machine is already registered in the current browser
            var isAlreadyRegistered = await StorageService.ContainKeyAsync(MachineRegistryStorageKey);
            if (isAlreadyRegistered)
            {
                var result = await MessageService.QuestionAsync(lc.Keys["Info"], "Machine is Already Registered , Continuing will Scrap the Old Registry, Continue?", lc.Keys[IMessageService.DialogOk], lc.Keys[IMessageService.DialogCancel]);
                if (result == MessageResult.Cancel)
                {
                    return;
                }
            }
            var input = await MessageService.OpenInputStringDialogAsync(lc.Keys["RegisterMachine"], lc.Keys["MachineAddress"], string.Empty);
            if (!string.IsNullOrWhiteSpace(input) && Guid.TryParse(input,out _) is false)
            {
                await MessageService.ErrorAsync("Invalid Machine Code", "The Entered Machine Code is not Valid");
                return;
            }
            try
            {
                //User string is empty and machine was registered
                if (string.IsNullOrWhiteSpace(input) && isAlreadyRegistered)
                {
                    await StorageService.RemoveItemAsync(MachineRegistryStorageKey);
                    await MessageService.InfoAsync(lc.Keys["Info"], "Registry Has been Cleaned");
                }
                //user string is empty and nothing was previously registered
                else if(!isAlreadyRegistered && string.IsNullOrWhiteSpace(input))
                {
                    return;
                }
                //user input string is valid and dont care what was previsouly
                else
                {
                    await StorageService.SetItemAsync(MachineRegistryStorageKey, input);
                    await MessageService.InfoAsync(lc.Keys["Info"], "Machine Registered Successfully");
                }
            }
            catch (Exception ex)
            {
                await MessageService.ErrorAsync("Registration Failed",ex.Message);
            }
        }

    }
}
