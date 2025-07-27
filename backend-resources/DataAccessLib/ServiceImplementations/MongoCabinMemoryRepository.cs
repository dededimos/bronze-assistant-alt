using CommonHelpers;
using DataAccessLib.NoSQLModels;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.ServiceImplementations;
public class MongoCabinMemoryRepository : ICabinMemoryRepository
{

    private readonly ICabinPartsRepository partsRepo;
    private readonly ICabinConstraintsRepository constraintsRepo;
    private readonly ICabinPartsListsRepository partsListsRepo;
    private readonly ICabinSettingsRepository settingsRepo;

    private readonly Dictionary<string, CabinPart> AllParts = new();
    private readonly Dictionary<string, Profile> AllProfiles = new();
    private readonly Dictionary<string, CabinHinge> AllHinges = new();
    private readonly Dictionary<string, GlassStrip> AllStrips = new();
    private readonly Dictionary<string, CabinAngle> AllAngles = new();
    private readonly Dictionary<string, SupportBar> AllSupportBars = new();
    private readonly Dictionary<string, CabinSupport> AllSupports = new();
    private readonly Dictionary<string, CabinHandle> AllHandles = new();
    public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinSettings> AllSettings { get; private set; } = new();
    public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinConstraints> AllConstraints { get; private set; } = new();
    public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), DefaultPartsList> DefaultLists { get; private set; } = new();
    
    /// <summary>
    /// Holds all the Additional Parts per Structure for all the Parts
    /// </summary>
    public Dictionary<string, Dictionary<CabinIdentifier, List<CabinPart>>> AdditionalPartsLists { get; private set; } = new();

    /// <summary>
    /// Adds any Structure Additional Parts to the List of ExtraParts of a CabinPart
    /// </summary>
    /// <param name="part">The Part of which the extra parts will be filled</param>
    /// <param name="identifier">The Identifier of the structure</param>
    private void AddAdditionalStructureSpecificParts(CabinPart part,CabinIdentifier identifier)
    {
        //Search if the Provided Code Exists in the Additional PartsList of Codes
        if (AdditionalPartsLists.TryGetValue(part.Code,out Dictionary<CabinIdentifier,List<CabinPart>>? partsDict) && partsDict is not null)
        {
            //Search for the List of Additional Parts for this Particular Structure
            if (partsDict.TryGetValue(identifier,out List<CabinPart>? addParts) && addParts is not null)
            {
                //If it exists Add the Additional Parts to the list
                foreach (var addPart in addParts)
                {
                    part.AdditionalParts.Add(addPart.GetDeepClone());
                }
            }
        }
    }

    public List<CabinIdentifier> ValidIdentifiers { get; private set; } = new();

    public MongoCabinMemoryRepository(
        ICabinPartsRepository partsRepo,
        ICabinConstraintsRepository constraintsRepo,
        ICabinPartsListsRepository partsListsRepo,
        ICabinSettingsRepository settingsRepo)
    {
        this.partsRepo = partsRepo;
        this.constraintsRepo = constraintsRepo;
        this.partsListsRepo = partsListsRepo;
        this.settingsRepo = settingsRepo;
    }

    /// <summary>
    /// Initilizes the Repo according to the provided language Descriptor (ex. el-Gr)
    /// If there is no such descriptor the parts initilize with their default Description
    /// </summary>
    /// <param name="languageStringDescriptor"></param>
    /// <returns></returns>
    public async Task<OperationResult> InitilizeRepo(string languageStringDescriptor)
    {
        #region 1.Initilize Parts

        var operationParts = await partsRepo.GetAllPartsAsync();
        if (operationParts.IsSuccessful)
        {
            AllParts.Clear();
            AdditionalPartsLists.Clear();
            foreach (var partEntity in operationParts.Result!)
            {
                var part = partEntity.Part;
                if (partEntity.LanguageDescriptions.TryGetValue(languageStringDescriptor, out string? description))
                    part.Description = description;
                //Add the Part
                AllParts.Add(partEntity.Part.Code, part);

                //Add the Additional Parts List
                AdditionalPartsLists.Add(partEntity.Part.Code, partEntity.AdditionalPartsPerStructure);
            }
        }
        else
        {
            return OperationResult.Failure($"Failed to Retrieve Parts from Database with Message:{Environment.NewLine}{operationParts.FailureMessage}");
        }

        //Segragate The Parts into Categories
        AllProfiles.Clear();
        AllStrips.Clear();
        AllHinges.Clear();
        AllAngles.Clear();
        AllSupports.Clear();
        AllSupportBars.Clear();
        AllHandles.Clear();
        AllParts.Values.ToList().ForEach(v =>
        {
            switch (v)
            {
                case Profile p:
                    AllProfiles.Add(p.Code, p);
                    break;
                case GlassStrip strip:
                    AllStrips.Add(strip.Code, strip);
                    break;
                case CabinHinge hinge:
                    AllHinges.Add(hinge.Code, hinge);
                    break;
                case CabinAngle angle:
                    AllAngles.Add(angle.Code, angle);
                    break;
                case CabinSupport support:
                    AllSupports.Add(support.Code, support);
                    break;
                case SupportBar supportBar:
                    AllSupportBars.Add(supportBar.Code, supportBar);
                    break;
                case CabinHandle handle:
                    AllHandles.Add(handle.Code, handle);
                    break;
                default:
                    break;
            }
        });
        //Fix all the Descriptions of the Inner Parts
        FixInnerPartsDescriptions();

        #endregion

        #region 2. Initilize Constraints

        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinConstraints> constraintsDict = new();
        var operationConstraints = await constraintsRepo.GetAllConstraintsAsync();
        if (operationConstraints.IsSuccessful)
        {
            foreach (var constraintsEntity in operationConstraints.Result!)
            {
                constraintsDict.Add((constraintsEntity.Model, constraintsEntity.DrawNumber, constraintsEntity.SynthesisModel), constraintsEntity.Constraints);
            }
        }
        else
        {
            return OperationResult.Failure($"Failed to Retrieve Constraints from Database with Message:{Environment.NewLine}{operationConstraints.FailureMessage}");
        }

        AllConstraints = constraintsDict;

        #endregion

        #region 3. Initilize Settings

        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinSettings> settingsDict = new();
        var operationSettings = await settingsRepo.GetAllSettingsAsync();
        if (operationSettings.IsSuccessful)
        {
            foreach (var settingsEntity in operationSettings.Result!)
            {
                settingsDict.Add((settingsEntity.Model, settingsEntity.DrawNumber, settingsEntity.SynthesisModel), settingsEntity.Settings);
            }
        }
        else
        {
            return OperationResult.Failure($"Failed to Retrieve Settings from Database with Message:{Environment.NewLine}{operationSettings.FailureMessage}");
        }

        AllSettings = settingsDict;

        #endregion

        #region 4.Initilize DefaultLists
        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), DefaultPartsList> partsListDict = new();
        var operationPartsList = await partsListsRepo.GetAllPartsListsAsync();
        if (operationPartsList.IsSuccessful)
        {
            foreach (var partListEntity in operationPartsList.Result!)
            {
                partsListDict.Add((partListEntity.Model, partListEntity.DrawNumber, partListEntity.SynthesisModel), partListEntity.DefaultPartsList);
            }
        }
        else
        {
            return OperationResult.Failure($"Failed to Retrieve PartsLists from Database with Message:{Environment.NewLine}{operationPartsList.FailureMessage}");
        }
        DefaultLists = partsListDict;
        #endregion

        //Set all the Valid Identifiers (Select all keys from one of the Dictionaries and make new Identifiers of each key)
        ValidIdentifiers = AllSettings.Keys.Select(k => new CabinIdentifier(k.Item1, k.Item2, k.Item3)).ToList();

        return OperationResult.Success();
    }

    /// <summary>
    /// Fixes the Descriptions of the Inner Parts so that they much the Requested Language
    /// </summary>
    private void FixInnerPartsDescriptions()
    {
        //Gather in one Single List all the Inner Parts and Parts per Structure
        var allAdditionalParts = AllParts.Values.SelectMany(p => p.AdditionalParts);
        var allAdditionalPartsOfAdditionalParts = allAdditionalParts.SelectMany(p=> p.AdditionalParts);
        var allStructureSpecificParts = AdditionalPartsLists.Values.SelectMany(dict => dict.Values.SelectMany(p => p));
        var allStructureSpecificPartsAdditionals = allStructureSpecificParts.SelectMany(p => p.AdditionalParts);

        List<CabinPart> allPartsNestedOrNot = new();
        allPartsNestedOrNot.AddRange(allAdditionalParts);
        allPartsNestedOrNot.AddRange(allAdditionalPartsOfAdditionalParts);
        allPartsNestedOrNot.AddRange(allStructureSpecificParts);
        allPartsNestedOrNot.AddRange(allStructureSpecificPartsAdditionals);

        //Find the Equivalent Part in the 'AllParts' list which has the Descriptions already Made
        foreach (var part in allPartsNestedOrNot)
        {
            if (AllParts.TryGetValue(part.Code, out CabinPart? partWithFixedDescription) && part.Description != partWithFixedDescription.Description)
            {
                part.Description = partWithFixedDescription.Description;
            }
        }
    }

    public List<CabinPart> GetAllParts()
    {
        return AllParts.Values.Select(p => p.GetDeepClone()).ToList();
    }
    /// <summary>
    /// Gets all the Parts Along with any Additionals they May have for the specified Identifier
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<CabinPart> GetAllParts(CabinIdentifier identifier)
    {
        return GetAllParts().Select(p=>
        {
            AddAdditionalStructureSpecificParts(p,identifier);
            return p;
        }).ToList();
    }
    public IEnumerable<CabinPart> GetAllPartsOriginal()
    {
        return AllParts.Values;
    }
    /// <summary>
    /// Returns a Clone of the Original Part of the Repository
    /// </summary>
    /// <param name="partCode">The Code of the Part</param>
    /// <returns>The Part Clone</returns>
    public CabinPart GetPart(string partCode)
    {
        return GetPartOriginal(partCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Part along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="partCode">The Code of the Part</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public CabinPart GetPart(string partCode, CabinIdentifier identifier) 
    {
        var part = GetPart(partCode);
        AddAdditionalStructureSpecificParts(part,identifier);
        return part;
    }
    public CabinPart GetPartOriginal(string partCode)
    {
        return AllParts.TryGetValue(partCode, out CabinPart? part)
            ? (part is not null ? part : throw new InvalidOperationException($"{partCode} returned a Null Part"))
            : throw new ArgumentOutOfRangeException($"Requested {partCode} was not found in the Repository");
    }

    /// <summary>
    /// Returns an Angle Clone from the Original Angle of the Repostitory
    /// </summary>
    /// <param name="angleCode">The Code of the Angle</param>
    /// <returns>The Angle Clone</returns>
    public CabinAngle GetAngle(string angleCode)
    {
        return GetAngleOriginal(angleCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Angle along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="angleCode">The Code of the Angle</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public CabinAngle GetAngle(string angleCode, CabinIdentifier identifier)
    {
        var angle = GetAngle(angleCode);
        AddAdditionalStructureSpecificParts(angle,identifier);
        return angle;
    }
    public CabinAngle GetAngleOriginal(string angleCode)
    {
        return AllAngles.TryGetValue(angleCode, out CabinAngle? angle)
            ? (angle is not null ? angle : throw new InvalidOperationException($"{angleCode} returned a Null Angle"))
            : throw new ArgumentOutOfRangeException($"Requested {angleCode} was not found in the Repository-Angles");
    }

    public CabinHinge GetHinge(string hingeCode)
    {
        return GetHingeOriginal(hingeCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Hinge along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="hingeCode">The Code of the hinge</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public CabinHinge GetHinge(string hingeCode ,CabinIdentifier identifier)
    {
        var hinge = GetHinge(hingeCode);
        AddAdditionalStructureSpecificParts(hinge, identifier);
        return hinge;
    }
    public CabinHinge GetHingeOriginal(string hingeCode)
    {
        return AllHinges.TryGetValue(hingeCode, out CabinHinge? hinge)
            ? (hinge is not null ? hinge : throw new InvalidOperationException($"{hingeCode} returned a Null Hinge"))
            : throw new ArgumentOutOfRangeException($"Requested {hingeCode} was not found in the Repository-Hinges");
    }
    
    public Profile GetProfile(string profileCode)
    {
        return GetProfileOriginal(profileCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Profile along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="profileCode">The Code of the Profile</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public Profile GetProfile(string profileCode,CabinIdentifier identifier)
    {
        var profile = GetProfile(profileCode);
        AddAdditionalStructureSpecificParts(profile, identifier);
        return profile;
    }
    public Profile GetProfileOriginal(string profileCode)
    {
        return AllProfiles.TryGetValue(profileCode, out Profile? profile)
            ? (profile is not null ? profile : throw new InvalidOperationException($"{profileCode} returned a Null Profile"))
            : throw new ArgumentOutOfRangeException($"Requested {profileCode} was not found in the Repository-Profiles");
    }

    public GlassStrip GetStrip(string stripCode)
    {
        return GetStripOriginal(stripCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Strip along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="stripCode">The Code of the Strip</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public GlassStrip GetStrip(string stripCode, CabinIdentifier identifier)
    {
        var strip = GetStrip(stripCode);
        AddAdditionalStructureSpecificParts(strip, identifier);
        return strip;
    }
    public GlassStrip GetStripOriginal(string stripCode)
    {
        return AllStrips.TryGetValue(stripCode, out GlassStrip? strip)
            ? (strip is not null ? strip : throw new InvalidOperationException($"{stripCode} returned a Null Strip"))
            : throw new ArgumentOutOfRangeException($"Requested {stripCode} was not found in the Repository-Glass Strips");
    }
    /// <summary>
    /// Returns the Strips of a Certain Type for the Specified Structure
    /// </summary>
    /// <param name="stripType">The Type of the Strip</param>
    /// <param name="identifier">The Structures Identifier</param>
    /// <returns></returns>
    public IEnumerable<GlassStrip> GetStrips(CabinStripType stripType, CabinIdentifier identifier)
    {
        return AllStrips.Values.Where(s => s.StripType == stripType).Select(s =>
        {
            var strip = s.GetDeepClone();
            AddAdditionalStructureSpecificParts(strip, identifier);
            return strip;
        });
    }

    public CabinSupport GetSupport(string supportCode)
    {
        return GetSupportOriginal(supportCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Support along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="supportCode">The Code of the Support</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public CabinSupport GetSupport(string supportCode,CabinIdentifier identifier)
    {
        var support = GetSupport(supportCode);
        AddAdditionalStructureSpecificParts(support, identifier);
        return support;
    }
    public CabinSupport GetSupportOriginal(string supportCode)
    {
        return AllSupports.TryGetValue(supportCode, out CabinSupport? support)
            ? (support is not null ? support : throw new InvalidOperationException($"{supportCode} returned a Null Support"))
            : throw new ArgumentOutOfRangeException($"Requested {supportCode} was not found in the Repository-Supports");
    }

    public SupportBar GetSupportBar(string supportBarCode)
    {
        return GetSupportBarOriginal(supportBarCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified SupportBar along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="supportBarCode">The Code of the SupportBar</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public SupportBar GetSupportBar(string supportBarCode,CabinIdentifier identifier)
    {
        var supportBar = GetSupportBar(supportBarCode);
        AddAdditionalStructureSpecificParts(supportBar, identifier);
        return supportBar;
    }
    public SupportBar GetSupportBarOriginal(string supportBarCode)
    {
        return AllSupportBars.TryGetValue(supportBarCode, out SupportBar? supportBar)
            ? (supportBar is not null ? supportBar : throw new InvalidOperationException($"{supportBarCode} returned a Null Support Bar"))
            : throw new ArgumentOutOfRangeException($"Requested {supportBarCode} was not found in the Repository-SupportBars");
    }

    /// <summary>
    /// Gets A Clone of the Original Handle of the Repository
    /// </summary>
    /// <param name="handleCode"></param>
    /// <returns></returns>
    public CabinHandle GetHandle(string handleCode)
    {
        return GetHandleOriginal(handleCode).GetDeepClone();
    }
    /// <summary>
    /// Returns the specified Handle along with all its Additionals for the Specified Structure
    /// </summary>
    /// <param name="handleCode">The Code of the Handle</param>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <returns></returns>
    public CabinHandle GetHandle(string handleCode,CabinIdentifier identifier)
    {
        var handle = GetHandle(handleCode);
        AddAdditionalStructureSpecificParts(handle,identifier);
        return handle;
    }
    /// <summary>
    /// Returns the Original Handle of the Repository
    /// </summary>
    /// <param name="handleCode"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CabinHandle GetHandleOriginal(string handleCode)
    {
        return AllHandles.TryGetValue(handleCode, out CabinHandle? handle)
            ? (handle is not null ? handle : throw new InvalidOperationException($"{handleCode} returned a Null Handle"))
            : throw new ArgumentOutOfRangeException($"Requested {handleCode} was not found in the Repository-Handles");
    }

    /// <summary>
    /// Finds the Default Part for the Designated Spot in the Specified sturcute
    /// </summary>
    /// <param name="model">The Structure's Model</param>
    /// <param name="draw">The Structure's Draw</param>
    /// <param name="synthesisModel">The Structure's Synthesis Model</param>
    /// <param name="spot">The Spot for which to find the Default Part</param>
    /// <returns>The Default Part (with any Extras for the Specified Structure)</returns>
    public CabinPart? GetDefaultPart(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
    {
        //Get the DefaultsList for the Part
        DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
        if (defaults is null) return null;

        //Get the Code from the Defaults
        defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
        if (spotDefaults is null || string.IsNullOrEmpty(spotDefaults.DefaultCode)) return null;

        var partToReturn = GetPart(spotDefaults.DefaultCode,new(model,draw,synthesisModel));
        if (partToReturn is not null) partToReturn.Quantity = spotDefaults.DefaultQuantity;

        return partToReturn;
    }
    /// <summary>
    /// Finds the Default Part for the Designated Spot in the Specified sturcute , Assumed the Part is of the definied Type
    /// </summary>
    /// <typeparam name="T">The Part Type</typeparam>
    /// <param name="model">The Structure's Model</param>
    /// <param name="draw">The Structure's Draw</param>
    /// <param name="synthesisModel">The Structure's Synthesis Model</param>
    /// <param name="spot">The Spot for which to find the Default Part</param>
    /// <returns>The Default Part</returns>
    public T? GetDefaultPart<T>(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot) where T : CabinPart
    {
        return GetDefaultPart(model, draw, synthesisModel, spot) as T;
    }
    /// <summary>
    /// Finds the Default Part for the Designated Spot in the Specified sturcute , Assumed the Part is of the definied Type
    /// </summary>
    /// <typeparam name="T">The Part Type</typeparam>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <param name="spot">The Spot for which to find the Default Part</param>
    /// <returns>The Default Part</returns>
    public T? GetDefaultPart<T>(CabinIdentifier identifier, PartSpot spot) where T : CabinPart
    {
        return GetDefaultPart(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot) as T;
    }
    /// <summary>
    /// Finds the Default Part for the Designated Spot in the Specified sturcute
    /// </summary>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <param name="spot">The Spot for which to find the Default Part</param>
    /// <returns>The Default Part</returns>
    public CabinPart? GetDefaultPart(CabinIdentifier identifier, PartSpot spot)
    {
        return GetDefaultPart(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot);
    }
    /// <summary>
    /// Returns the Default Part Code for the Selected Structure  ,in the Designated Spot
    /// </summary>
    /// <param name="model">The Structure's Model</param>
    /// <param name="draw">The Structure's Draw</param>
    /// <param name="synthesisModel">The Structure's Synthesis Model</param>
    /// <param name="spot">The Spot for which to get the Default Code</param>
    /// <returns>The Default Code</returns>
    public string GetDefault(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
    {
        //Get the DefaultsList for the Part
        DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
        if (defaults is null) return "";

        //Get the Code from the Defaults
        defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
        if (spotDefaults is null || string.IsNullOrEmpty(spotDefaults.DefaultCode)) return "";

        return spotDefaults.DefaultCode;
    }
    /// <summary>
    /// Returns the Default Part Code for the Selected Structure  ,in the Designated Spot
    /// </summary>
    /// <param name="identifier">The Structure's Identifier</param>
    /// <param name="spot">The Spot for which to get the Default Part Code</param>
    /// <returns>The Default Code</returns>
    public string GetDefault(CabinIdentifier identifier, PartSpot spot)
    {
        return GetDefault(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot);
    }
    /// <summary>
    /// Returns the Codes that are valid for a specific spot in the designated structure
    /// </summary>
    /// <param name="model">The Structure's Model</param>
    /// <param name="draw">The Structures Draw</param>
    /// <param name="synthesisModel">The Structure's Synthesis Model</param>
    /// <param name="spot">The Spot for which to get Valid Codes</param>
    /// <returns>The Valid Codes</returns>
    public IEnumerable<string> GetSpotValids(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
    {
        //Get the DefaultsList for the Part
        DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
        if (defaults is null) return Enumerable.Empty<string>();

        //Get the Code from the Defaults
        defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
        if (spotDefaults is null) return Enumerable.Empty<string>();

        return spotDefaults.ValidCodes;
    }
    /// <summary>
    /// Returns the Codes that are valid for a specific spot in the designated structure
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="spot"></param>
    /// <returns></returns>
    public IEnumerable<string> GetSpotValids(CabinIdentifier identifier, PartSpot spot)
    {
        return GetSpotValids(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot);
    }

    /// <summary>
    /// Returns the Default Part Quantity for the Specified spot in the Specified Structure
    /// </summary>
    /// <param name="spot">The Specified Spot</param>
    /// <param name="identifier">The Structures identifier</param>
    /// <returns>The Quantity or zero if the Spot or Structure does not Exists</returns>
    public double GetSpotDefaultQuantity(PartSpot spot,CabinIdentifier identifier)
    {
        if (DefaultLists.TryGetValue((identifier.Model,identifier.DrawNumber,identifier.SynthesisModel),out DefaultPartsList? defaults) && defaults is not null)
        {
            return defaults.GetSpotDefaultQuantity(spot);
        }
        return 0;
    }

}
