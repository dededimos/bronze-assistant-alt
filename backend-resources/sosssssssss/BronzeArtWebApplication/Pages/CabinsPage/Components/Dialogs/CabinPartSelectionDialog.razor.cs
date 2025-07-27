using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.Dialogs;

public partial class CabinPartSelectionDialog<T> : ComponentBase
    where T : CabinPart
{
    /// <summary>
    /// The Available parts for Selection (Including the One Already Selected)
    /// </summary>
    [Parameter]
    public IEnumerable<T> PartsSelectionList { get; set; } = new List<T>();

    /// <summary>
    /// The Selected Part
    /// </summary>
    [Parameter]
    public T SelectedPart { get; set; }

    [Parameter] public bool IsVisible { get; set; }

    private bool _isVisible;
    /// <summary>
    /// Parameters must be auto properties , in order to run further events on the setter a new helper property is made
    /// </summary>
    public bool IsVisibleHelper
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                IsVisibleChanged.InvokeAsync(_isVisible);
                if (value == false)
                {
                    //Trigger onClosing Event
                    OnClosingDialog.InvokeAsync();
                }
            }
        }
    }

    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public EventCallback OnClosingDialog { get; set; }

    /// <summary>
    /// Event Callback of the Selected Part
    /// </summary>
    [Parameter] public EventCallback<T> OnPartSelection { get; set; }

    /// <summary>
    /// Select the Part OnClick of the Div
    /// </summary>
    /// <param name="part">The Selected Part</param>
    private void SelectPart(T part)
    {
        //If the Selected Part is different than the one clicked , trigger the Selection
        if (part is not null && part.Code != SelectedPart?.Code)
        {
            SelectedPart = part;
            OnPartSelection.InvokeAsync(part);
        }
        else
        {
            //Deselection
            SelectedPart = default;
            OnPartSelection.InvokeAsync(default);
        }
    }

    private static string GetDimensionsString(CabinPart part)
    {
        switch (part.Part)
        {
            case CabinPartType.Handle:
                var handle = part as CabinHandle ?? throw new InvalidOperationException($"{part.Part}: is not a Handle - Cannot get Dimensions");
                if (handle.IsCircularHandle)
                {
                    return $"Φ {handle.HandleLengthToGlass}mm";
                }
                else
                {
                    return $"{handle.HandleLengthToGlass}x{handle.HandleHeightToGlass}mm";
                }
            case CabinPartType.Hinge:
                var hinge = part as CabinHinge ?? throw new InvalidOperationException($"{part.Part}: is not a Hinge - Cannot get Dimensions");
                return $"{hinge.LengthView}x{hinge.LengthView}mm";
            case CabinPartType.ProfileHinge:
            case CabinPartType.MagnetProfile:
            case CabinPartType.Profile:
                var profHinge = part as Profile ?? throw new InvalidOperationException($"{part.Part}: is not a Profile - Cannot get Dimensions");
                return $"{profHinge.ThicknessView}mm";
            case CabinPartType.FloorStopperW:
                var stop = part as FloorStopperW ?? throw new InvalidOperationException($"{part.Part}: is not a FloorStopper - Cannot get Dimensions");
                return $"{stop.LengthView}x{stop.HeightView}mm";
            case CabinPartType.BarSupport:
                var bar = part as SupportBar ?? throw new InvalidOperationException($"{part.Part}: is not a SupportBar - Cannot get Dimensions");
                return "Not Implemented";
            case CabinPartType.SmallSupport:
                var support = part as CabinSupport ?? throw new InvalidOperationException($"{part.Part}: is not a Support - Cannot get Dimensions");
                return $"{support.LengthView}x{support.HeightView}mm";
            case CabinPartType.Strip:
            case CabinPartType.AnglePart:
            case CabinPartType.Undefined:
            default:
                return "";
        }
    }

    protected override Task OnParametersSetAsync()
    {
        //Set the Helper property whenever the Parameter Changes or is Different
        if (IsVisibleHelper != IsVisible)
        {
            IsVisibleHelper = IsVisible;
        }
        return base.OnParametersSetAsync();
    }

}
