using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.ViewModels;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components;

public partial class CabinCodeInput : ComponentBase
{
    [Inject] public AssembleCabinViewModel vm { get; set; }
    [Inject] public ISnackbar snackbar { get; set; }
    [Inject] public NavigationManager nm { get; set; }
    [Inject] public CabinFactory factory { get; set; }

    [Parameter] public string Style { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public Variant TextBoxVariant { get; set; }

    private readonly PatternMask mask = new("xxxx-xx-xxxxx") { MaskChars = new[] { new MaskChar('x', @"[0-9a-zA-Z]") } };

    private bool isCodeValid { get => string.IsNullOrWhiteSpace(errorText); }
    private string typedText = "";
    private string errorText = "";


    //NOT USED -- MASK IS OVERRIDDING KEYPRESS HANDLER
    private void OnKeyEventHandler(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            TranslateCode();
        }
    }

    private void TranslateCode()
    {
        CabinCodeTranslator translator = new(factory);
        var result = translator.GenerateCabin(typedText);
        if (result.result.IsValid)
        {
            if (result.cabin is null)
            {
                errorText = lc.Keys["UnrecognizedCabin"];
            }
            else
            {
                errorText = "";
                try
                {
                    CabinSynthesis synthesis = CabinFactory.CreateSynthesis(result.cabin);
                    vm.PassSynthesisToViewModel(synthesis);
                    nm.NavigateTo($"/AssembleCabin/{StoryWindow.CabinPanel}");
                    snackbar.Add(lc.Keys["CabinFromCodeGeneration"], Severity.Success);
                }
                catch (Exception ex)
                {
                    nm.NavigateTo("/AssembleCabin");
                    snackbar.Add(lc.Keys["CodeGenerationFailed"], Severity.Error);
                    Console.WriteLine(ex.Message);
                }
            }
        }
        else
        {
            IEnumerable<string> errorStrings = result.result.Errors.Select(e => lc.Keys[e.ErrorCode]);
            errorText = CommonHelpers.CommonVariousHelpers.GetStringOfList(errorStrings);
            snackbar.Add(lc.Keys["InvalidCabinCode"], Severity.Warning);
        }
    }

}
