using BronzeFactoryApplication.ApplicationServices.MessangerService;
using DataAccessLib.NoSQLModels;
using HandyControl.Properties.Langs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;

public partial class EditPartViewModel : BaseViewModel
{
    public List<string> AllowedCultures { get; } = new() { "el-GR", "en-EN", "it-IT" };
    public string GetTranslation(string cultureString)
    {
        return languageDescriptions.TryGetValue(cultureString, out string? value) ? value : Description;
    }
    public int SelectedLangIndex { get => -1; }
    private readonly Dictionary<string, string> languageDescriptions = new();
    private readonly HashSet<CabinIdentifier> uses = new();

    public ObjectId? Id { get; init; }

    /// <summary>
    /// Weather the Edited Part Has Uses (If it does ,some things are not editable)
    /// </summary>
    public bool HasUses { get => uses.Any(); }

    [ObservableProperty]
    private string code = string.Empty;
    [ObservableProperty]
    private string description = string.Empty;
    [ObservableProperty]
    private string photoPath = string.Empty;
    [ObservableProperty]
    private CabinPartType partType = CabinPartType.GenericPart;
    [ObservableProperty]
    private MaterialType material;

    [ObservableProperty]
    private string newLanguageCultureKey = "el-GR";
    [ObservableProperty]
    private string newLanguageTranslation = string.Empty;
    [ObservableProperty]
    private List<CabinPart> additionalParts = new();
    [ObservableProperty]
    private Dictionary<CabinIdentifier, List<CabinPart>> additionalPartsPerStructure = new();
    [ObservableProperty]
    private string notes = string.Empty;

    public List<TranslationKeyValuePair> LanguageTranslations { get => languageDescriptions.Select(kv => new TranslationKeyValuePair() { CultureKey = kv.Key,Translation = kv.Value}).ToList(); }

    private bool isEdit;
    public bool IsEdit 
    { 
        get => isEdit; 
        private set
        {
            if (isEdit != value)
            {
                isEdit = value;
                OnPropertyChanged(nameof(IsEdit));
            }
        }
    }

    /// <summary>
    /// Changes the Mode of the ViewModel to Edit (after the part has been initially saved)
    /// </summary>
    public void ChangeToEditMode()
    {
        IsEdit = true;
        OnPropertyChanged(nameof(IsEdit));
    }


    /// <summary>
    /// To Create a new Part
    /// </summary>
    public EditPartViewModel(CabinPartType partType)
    {
        this.PartType = partType;
        IsEdit = false;
        //No need for Additional parts list is empty when new part is created
    }

    /// <summary>
    /// To Edit an Existing Part-Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="isEdit">Weather this is an Edit of an existing Entity</param>
    public EditPartViewModel(CabinPartEntity entity , bool isEdit = true)
    {
        Id = isEdit ? entity.Id : default; //Put a Default id if this is for a new Item
        this.Code = entity.Part.Code;
        this.Description = entity.Part.Description;
        this.PhotoPath = entity.Part.PhotoPath;
        this.PartType = entity.Part.Part;
        this.Material = entity.Part.Material;
        this.AdditionalParts = entity.Part.AdditionalParts.Select(p => p.GetDeepClone()).ToList();
        this.AdditionalPartsPerStructure = entity.AdditionalPartsPerStructure.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(p => p.GetDeepClone()).ToList());

        languageDescriptions = new(entity.LanguageDescriptions);
        uses = isEdit ? new(entity.Uses.Select(u=> u.Identifier)) : new(); //Put empty used if its new
        IsEdit = isEdit;
        Notes = new(entity.Notes);
    }

    public virtual CabinPart GetPart()
    {
        CabinPart part = new(this.PartType);
        ExtractPropertiesForBasePart(part);
        return part;
    }
    public virtual CabinPartEntity GetPartEntity()
    {
        CabinPartEntity newPart = new(GetPart(),AdditionalPartsPerStructure, languageDescriptions)
        {
            Id = this.Id ?? default,
            Notes = Notes
        };
        return newPart;
    }

    /// <summary>
    /// Passes the Properties of the BaseClass <see cref="EditPartViewModel"/> to the <see cref="CabinPart"/> argument
    /// </summary>
    /// <param name="part">The Part in which to Pass the base class properties</param>
    protected void ExtractPropertiesForBasePart(CabinPart part)
    {
        part.Code = Code;
        part.Description = Description;
        part.PhotoPath = PhotoPath;
        part.Material = Material;
        part.AdditionalParts = AdditionalParts.Select(p => p.GetDeepClone()).ToList();
    }

    [RelayCommand]
    private void AddOrEditTranslation()
    {
        if (AllowedCultures.Any(c=>c == NewLanguageCultureKey)
            && string.IsNullOrEmpty(NewLanguageTranslation) is false)
        {
            languageDescriptions.Remove(NewLanguageCultureKey);
            languageDescriptions.Add(NewLanguageCultureKey, NewLanguageTranslation);
            NewLanguageTranslation = string.Empty;
            OnPropertyChanged(nameof(LanguageTranslations));
        }
    }
    [RelayCommand]
    private void RemoveLanguageTranslation(string cultureKey)
    {
        string msg = $"{"lngRemoveTranslation".TryTranslateKey()}{Environment.NewLine}{cultureKey}";
        var result = MessageService.Question( msg, "lngRemove".TryTranslateKey(), "lngOk".TryTranslateKey(), "lngCancel".TryTranslateKey());
        if (result is MessageBoxResult.Cancel)
        {
            return;
        }

        if (languageDescriptions.Remove(cultureKey)) 
        {
            OnPropertyChanged(nameof(LanguageTranslations));
            MessageService.Info("lngTranslationRemoveSuccess".TryTranslateKey(), "lngSuccess".TryTranslateKey());
        }
        else
        {
            MessageService.Warning("lngTranslationRemoveFailure".TryTranslateKey(), "lngFailure".TryTranslateKey());
        }
    }
}

public class TranslationKeyValuePair
{
    public string CultureKey { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
}
