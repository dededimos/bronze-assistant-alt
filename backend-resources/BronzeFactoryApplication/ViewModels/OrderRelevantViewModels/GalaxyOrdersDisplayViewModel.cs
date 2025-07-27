using BronzeFactoryApplication.ApplicationServices.MessangerService;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;

public partial class GalaxyOrdersDisplayViewModel : BaseViewModel
{
    private readonly GalaxyOrdersImportService galaxyRepository;
    private readonly IMessenger messenger;
    private readonly ValidatorCabinCode validatorCabinCode;

    public GalaxyOrdersDisplayViewModel(
        GalaxyOrdersImportService galaxyRepository,
        IMessenger messenger,
        ValidatorCabinCode cabinCodeValidator)
    {
        BusyPrompt = "Retrieving...";
        this.galaxyRepository = galaxyRepository;
        this.messenger = messenger;
        this.validatorCabinCode = cabinCodeValidator;
    }

    public int SkipFilter
    {
        get => galaxyRepository.SkipFilterValue;
        set => SetProperty(galaxyRepository.SkipFilterValue, value, galaxyRepository, (repo, newValue) => repo.SkipFilterValue = newValue);
    }
    public int TakeFilter
    {
        get => galaxyRepository.TakeFilterValue;
        set => SetProperty(galaxyRepository.TakeFilterValue, value, galaxyRepository, (repo, newValue) => repo.TakeFilterValue = newValue);
    }
    public DateTime DateGreaterOrEqualFilter
    {
        get => galaxyRepository.DateGreaterOrEqualFilterValue;
        set => SetProperty(galaxyRepository.DateGreaterOrEqualFilterValue, value, galaxyRepository, (repo, newValue) => repo.DateGreaterOrEqualFilterValue = newValue);
    }
    public DateTime DateLessOrEqualFilter
    {
        get => galaxyRepository.DateLessOrEqualFilterValue;
        set => SetProperty(galaxyRepository.DateLessOrEqualFilterValue, value, galaxyRepository, (repo, newValue) => repo.DateLessOrEqualFilterValue = newValue);
    }
    public string LogInCall
    {
        get => galaxyRepository.LogInCall;
        set => SetProperty(galaxyRepository.LogInCall, value, galaxyRepository, (repo, newValue) => repo.LogInCall = newValue);
    }
    public string CallBase
    {
        get => galaxyRepository.CallBase;
        set => SetProperty(galaxyRepository.CallBase, value, galaxyRepository, (repo, newValue) => repo.CallBase = newValue);
    }

    [ObservableProperty]
    private bool includeOnlyCabinOrders = true;
    [ObservableProperty]
    private bool includeOnlyMirrorOrders = true;

    public ObservableCollection<BronzeDocumentViewModel> GalaxyDocuments { get; set; } = [];

    [ObservableProperty]
    private BronzeDocumentViewModel? selectedDocument;

    [RelayCommand]
    private async Task GetOrdersAsync()
    {
        IsBusy = true;
        try
        {
            var result = await galaxyRepository.GetOrdersAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                GalaxyDocuments.Clear();
            });
            // Give time to Ui to Update so it does not block
            await Task.Delay(50);

            foreach (var item in result)
            {
                if (!IncludeOnlyCabinOrders || item.Rows.Any(r => IsPassingCabinFilter(r)))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GalaxyDocuments.Add(new(item, validatorCabinCode));
                    });
                }

                // Give time to Ui to Update so it does not block
                await Task.Delay(20);
            }
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ResetCallsToDefaultValues()
    {
        galaxyRepository.SetToDefaultConfigurationValues();
        IncludeOnlyCabinOrders = true;
        IncludeOnlyMirrorOrders = true;
        OnPropertyChanged("");
    }

    [RelayCommand]
    private void ClearCache()
    {
        if (galaxyRepository.IsAnyDataCached)
        {
            galaxyRepository.ClearCache();
            MessageService.Info($"Results Stored in Memory have been Cleared.{Environment.NewLine}The Next Call Will Retrieve All Results from the Server and not Memory", "Cache Cleared");
        }
        else
        {
            MessageService.Info("Memory is already Clear. There are not cached Results", "Cache is Empty");
        }
    }

    /// <summary>
    /// Sets the DatePickers to the Selected Filter values
    /// </summary>
    /// <param name="days"></param>
    [RelayCommand]
    private void SetDateFilter(string days)
    {
        DateLessOrEqualFilter = DateTime.Now;
        DateGreaterOrEqualFilter = DateLessOrEqualFilter.AddDays(-int.Parse(days));
    }

    /// <summary>
    /// Moves a Product Row Upper in the Index of the Rows List
    /// </summary>
    /// <param name="row"></param>
    [RelayCommand]
    private void MoveRowUp(BronzeProductRowViewModel row)
    {
        if (SelectedDocument != null)
        {
            var indexOfRow = SelectedDocument.Rows.IndexOf(row);
            if (indexOfRow != 0)
            {
                SelectedDocument.Rows.Move(indexOfRow, indexOfRow - 1);
            }
        }
    }

    [RelayCommand]
    private void MoveRowDown(BronzeProductRowViewModel row)
    {
        if (SelectedDocument != null)
        {
            var indexOfRow = SelectedDocument.Rows.IndexOf(row);
            var maxIndex = SelectedDocument.Rows.Count - 1;
            if (indexOfRow < maxIndex)
            {
                SelectedDocument.Rows.Move(indexOfRow, indexOfRow + 1);
            }
        }
    }

    /// <summary>
    /// Imports a Single Code as a Cabin
    /// </summary>
    /// <param name="row">The Product Row from which the Code is extracted</param>
    [RelayCommand]
    private void ImportCode(BronzeProductRowViewModel row)
    {
        if (validatorCabinCode.Validate(row.Code).IsValid)
        {
            if (SelectedDocument is null) throw new InvalidOperationException("Selected Document was Null.... Unexpected");
            messenger.Send(new ImportCabinsMessage(
                DeviseMessageArgumentFromRow(row),
                (null,""),
                (null,""),
                SelectedDocument.DocumentSeriesNumber));
        }
        else
        {
            MessageService.Warning("lngItemIsNotCabin".TryTranslateKey(), "lngFailure".TryTranslateKey());
        }
    }

    /// <summary>
    /// Imports all the Codes from the selected ProductRows in the Current Document
    /// </summary>
    /// <param name="document">The Document</param>
    [RelayCommand]
    private void ImportCodes(BronzeDocumentViewModel document)
    {
        var selectedRows = document.Rows.Where(r => r.IsSelected);
        var selectedRowsNumber = selectedRows.Count();
        if (selectedRowsNumber > 3)
        {
            MessageService.Warning("lngCabinImportMoreThan3".TryTranslateKey(), "lngFailure".TryTranslateKey());
            return;
        }
        else if (selectedRowsNumber < 1)
        {
            MessageService.Warning("lngCabinImport0".TryTranslateKey(), "lngFailure".TryTranslateKey());
            return;
        }

        var argPrimary = DeviseMessageArgumentFromRow(selectedRows.First());
        var argSecondary = selectedRowsNumber >= 2 ? DeviseMessageArgumentFromRow(selectedRows.Skip(1).First()) : (null,"");
        var argTertiary = selectedRowsNumber >= 3 ? DeviseMessageArgumentFromRow(selectedRows.Skip(2).First()) : (null, "");
        if (SelectedDocument is null) throw new InvalidOperationException("Selected Document was Null.... Unexpected");
        messenger.Send(new ImportCabinsMessage(argPrimary, argSecondary, argTertiary,SelectedDocument.DocumentSeriesNumber));

        //Reset Selection of Rows
        foreach (var row in selectedRows)
        {
            row.IsSelected = false;
        }
    }

    private bool IsPassingCabinFilter(BronzeProductRow row)
    {
        return validatorCabinCode.Validate(row.Code).IsValid;
    }

    /// <summary>
    /// Devises a Message Import Cabin Argument from a Product Row
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    private (CabinSettings? settings, string code) DeviseMessageArgumentFromRow(BronzeProductRowViewModel row)
    {
        CabinSettings? sett = null;
        string code = "";
        if (row.HasCharachteristics)
        {
            sett = ExtractSettingsFromCharachteristicsRow(row);
        }
        else
        {
            code = row.Code;
        }
        return (sett, code);
    }

    /// <summary>
    /// Extracts the Cabin Settings from a Product Row
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private CabinSettings ExtractSettingsFromCharachteristicsRow(BronzeProductRowViewModel row)
    {
        var code = row.Code.ToLower();
        var modelCode = code.Length >= 2 ? code[..2] : "";
        char thickCode = code.Length >= 3 ? code[2] : ' '; //Thickness in general for the rest
        char wthickCode = code.Length >= 2 ? code[0] : ' '; //thickness only for W model
        var glassMetalCode = row.Charachteristic2.Length >= 2 ? row.Charachteristic2[^2..] : "";
        char metalFinishCode = glassMetalCode.Length >= 2 ? glassMetalCode.ToLower()[0] : ' ';
        char glassFinishCode = glassMetalCode.Length >= 2 ? glassMetalCode.ToLower()[1] : ' ';

        #region FIND MODEL
        //Find Model
        CabinModelEnum model = modelCode switch
        {
            "9a" => CabinModelEnum.Model9A,
            "9s" => CabinModelEnum.Model9S,
            "94" => CabinModelEnum.Model94,
            "9f" => CabinModelEnum.Model9F,
            "9b" => CabinModelEnum.Model9B,
            "8w" or "6w" => CabinModelEnum.ModelW,
            "hb" => CabinModelEnum.ModelHB,
            "np" => CabinModelEnum.ModelNP,
            "vs" => CabinModelEnum.ModelVS,
            "vf" => CabinModelEnum.ModelVF,
            "v4" => CabinModelEnum.ModelV4,
            "va" => CabinModelEnum.ModelVA,
            "ws" => CabinModelEnum.ModelWS,
            "db" => CabinModelEnum.ModelDB,
            "nb" => CabinModelEnum.ModelNB,
            "qp" => CabinModelEnum.ModelQP,
            "qb" => CabinModelEnum.ModelQB,
            //Rest cases never have charachteristics
            _ => throw new InvalidOperationException("Could not Extract Model"),
        };
        #endregion

        #region FIND THICKNESS

        CabinThicknessEnum? thickness;
        //Find Thickness
#pragma warning disable IDE0066 // Convert switch statement to expression
        switch (model)
        {
            case CabinModelEnum.Model9A:
            case CabinModelEnum.Model9S:
            case CabinModelEnum.Model94:
            case CabinModelEnum.Model9F:
                thickness = thickCode is '8' ? CabinThicknessEnum.Thick8mm : CabinThicknessEnum.Thick6mm;
                break;
            case CabinModelEnum.Model9B:
            case CabinModelEnum.ModelNP:
            case CabinModelEnum.ModelNB:
            case CabinModelEnum.ModelQP:
            case CabinModelEnum.ModelQB:
                thickness = CabinThicknessEnum.Thick6mm;
                break;
            case CabinModelEnum.ModelW:
                thickness = wthickCode is '6' ? CabinThicknessEnum.Thick6mm : (wthickCode is '0' ? CabinThicknessEnum.Thick10mm : CabinThicknessEnum.Thick8mm);
                break;
            case CabinModelEnum.ModelHB:
            case CabinModelEnum.ModelVS:
            case CabinModelEnum.ModelVF:
            case CabinModelEnum.ModelV4:
            case CabinModelEnum.ModelVA:
            case CabinModelEnum.ModelDB:
                thickness = CabinThicknessEnum.Thick8mm;
                break;
            case CabinModelEnum.ModelWS:
                thickness = CabinThicknessEnum.Thick8mm10mm;
                break;
            case CabinModelEnum.ModelE:
            case CabinModelEnum.ModelWFlipper:
            case CabinModelEnum.ModelNV:
            case CabinModelEnum.ModelMV2:
            case CabinModelEnum.ModelNV2:
            case CabinModelEnum.Model6WA:
            case CabinModelEnum.Model9C:
            case CabinModelEnum.Model8W40:
            default:
                thickness = null;
                break;
        }
#pragma warning restore IDE0066 // Convert switch statement to expression

        #endregion

        #region FIND METAL FINISH
        CabinFinishEnum? metalFinish = metalFinishCode switch
        {
            '1' => (CabinFinishEnum?)CabinFinishEnum.Polished,
            '4' => (CabinFinishEnum?)CabinFinishEnum.Bronze,
            'm' or 'μ' => (CabinFinishEnum?)CabinFinishEnum.BlackMat,
            '2' => (CabinFinishEnum?)CabinFinishEnum.BrushedGold,
            _ => null,
        };
        #endregion

        #region FIND GLASS FINISH
        GlassFinishEnum? glassFinish = glassFinishCode switch
        {
            '0' => (GlassFinishEnum?)GlassFinishEnum.Transparent,
            's' or 'σ' or 'ς' => (GlassFinishEnum?)GlassFinishEnum.Serigraphy,
            'f' or 'φ' => (GlassFinishEnum?)GlassFinishEnum.Frosted,
            'a' or 'α' => (GlassFinishEnum?)GlassFinishEnum.Satin,
            'm' or 'μ' => (GlassFinishEnum?)GlassFinishEnum.Fume,
            _ => null,
        };
        #endregion

        //Find Length and Height
        decimal length = decimal.TryParse(row.Charachteristic1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal c1) ? c1*10 : 0;
        decimal height = decimal.TryParse(row.Charachteristic3, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal c3) ? c3*10 : 0;

        return CabinSettings.Create(
            model, metalFinish ?? CabinFinishEnum.NotSet,
            thickness ?? CabinThicknessEnum.NotSet,
            glassFinish ?? GlassFinishEnum.GlassFinishNotSet,
            Convert.ToInt32(height),
            Convert.ToInt32(length));
    }

}
