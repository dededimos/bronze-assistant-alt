using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces
{
    public interface ICabinMemoryRepository
    {
#nullable enable
        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinConstraints> AllConstraints { get; }
        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinSettings> AllSettings { get; }
        Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), DefaultPartsList> DefaultLists { get; }
        /// <summary>
        /// A list of all the Valid Identifiers for this Repo (The Structures Allowed to be Made)
        /// </summary>
        List<CabinIdentifier> ValidIdentifiers { get; }

        /// <summary>
        /// Weather an Identifier is Valid for this Repository
        /// </summary>
        /// <param name="identifier">The Identifier</param>
        /// <returns>True if it is , false if it is not</returns>
        public bool IsValidIdentifier(CabinIdentifier identifier)
        {
            return ValidIdentifiers.Any(i => i == identifier);
        }
        /// <summary>
        /// Get the Models that Match with the Selected Draw Number
        /// </summary>
        /// <param name="draw">The Draw Number</param>
        /// <returns>The List of Models that have a match with the selected Draw , else an Empty Enumerable if there are no Matches</returns>
        public IEnumerable<CabinModelEnum> GetMatchingModels(CabinDrawNumber draw)
        {
            return ValidIdentifiers.Where(i => i.DrawNumber == draw).Select(i => i.Model).Distinct();
        }
        /// <summary>
        /// Get the SynthesisModels that Match with the Selected Draw Number and Model
        /// </summary>
        /// <param name="draw">The Draw Number</param>
        /// <param name="model">The Model</param>
        /// <returns>The List of SynthesisModels that have a match with the selected Draw and Model , else an Empty Enumerable if there are no Matches</returns>
        public IEnumerable<CabinSynthesisModel> GetMatchingSynthesisModels(CabinDrawNumber draw, CabinModelEnum model)
        {
            return ValidIdentifiers.Where(i => i.DrawNumber == draw && i.Model == model).Select(i=> i.SynthesisModel).Distinct();
        }

        /// <summary>
        /// Initilize the Repository
        /// </summary>
        /// <returns>The Result of the Operation</returns>
        public Task<OperationResult> InitilizeRepo(string languageStringDescriptor);
        /// <summary>
        /// Finds the Default Part for the Designated Spot in the Specified sturcute , Assumed the Part is of the definied Type
        /// </summary>
        /// <typeparam name="T">The Part Type</typeparam>
        /// <param name="model">The Structure's Model</param>
        /// <param name="draw">The Structure's Draw</param>
        /// <param name="synthesisModel">The Structure's Synthesis Model</param>
        /// <param name="spot">The Spot for which to find the Default Part</param>
        /// <returns>The Default Part</returns>
        T? GetDefaultPart<T>(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot) where T : CabinPart;
        /// <summary>
        /// Finds the Default Part for the Designated Spot in the Specified sturcute , Assumed the Part is of the definied Type
        /// </summary>
        /// <typeparam name="T">The Part Type</typeparam>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <param name="spot">The Spot for which to find the Default Part</param>
        /// <returns>The Default Part</returns>
        T? GetDefaultPart<T>(CabinIdentifier identifier, PartSpot spot) where T : CabinPart;
        /// <summary>
        /// Finds the Default Part for the Designated Spot in the Specified sturcute
        /// </summary>
        /// <param name="model">The Structure's Model</param>
        /// <param name="draw">The Structure's Draw</param>
        /// <param name="synthesisModel">The Structure's Synthesis Model</param>
        /// <param name="spot">The Spot for which to find the Default Part</param>
        /// <returns>The Default Part</returns>
        CabinPart? GetDefaultPart(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot);
        /// <summary>
        /// Finds the Default Part for the Designated Spot in the Specified sturcute
        /// </summary>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <param name="spot">The Spot for which to find the Default Part</param>
        /// <returns>The Default Part</returns>
        CabinPart? GetDefaultPart(CabinIdentifier identifier, PartSpot spot);
        /// <summary>
        /// Returns the Default Part Code for the Selected Structure  ,in the Designated Spot
        /// </summary>
        /// <param name="model">The Structure's Model</param>
        /// <param name="draw">The Structure's Draw</param>
        /// <param name="synthesisModel">The Structure's Synthesis Model</param>
        /// <param name="spot">The Spot for which to get the Default Code</param>
        /// <returns>The Default Code</returns>
        string GetDefault(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot);
        /// <summary>
        /// Returns the Default Part Code for the Selected Structure  ,in the Designated Spot
        /// </summary>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <param name="spot">The Spot for which to get the Default Part Code</param>
        /// <returns>The Default Code</returns>
        string GetDefault(CabinIdentifier identifier, PartSpot spot);
        /// <summary>
        /// Returns the Codes that are valid for a specific spot in the designated structure
        /// </summary>
        /// <param name="model">The Structure's Model</param>
        /// <param name="draw">The Structures Draw</param>
        /// <param name="synthesisModel">The Structure's Synthesis Model</param>
        /// <param name="spot">The Spot for which to get Valid Codes</param>
        /// <returns>The Valid Codes</returns>
        IEnumerable<string> GetSpotValids(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot);
        /// <summary>
        /// Returns the Codes that are valid for a specific spot in the designated structure
        /// </summary>
        /// <param name="identifier">The Structures Identifier</param>
        /// <param name="spot">The Spot</param>
        /// <returns>The Valid Codes</returns>
        IEnumerable<string> GetSpotValids(CabinIdentifier identifier, PartSpot spot);
        /// <summary>
        /// Returns the Default Part Quantity for the Specified spot in the Specified Structure
        /// </summary>
        /// <param name="spot">The Specified Spot</param>
        /// <param name="identifier">The Structures identifier</param>
        /// <returns>The Quantity or zero if the Spot or Structure does not Exists</returns>
        public double GetSpotDefaultQuantity(PartSpot spot, CabinIdentifier identifier);

        /// <summary>
        /// Returns a Cloned List of All the Parts in the Repository
        /// </summary>
        /// <returns>The List of Parts</returns>
        public List<CabinPart> GetAllParts();
        /// <summary>
        /// Gets all the Parts Along with any Additionals they May have for the specified Identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public List<CabinPart> GetAllParts(CabinIdentifier identifier);
        /// <summary>
        /// Gets All the Original Part Objects from the Repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CabinPart> GetAllPartsOriginal();
        /// <summary>
        /// Gets a Clone of the specified Part by Code
        /// </summary>
        /// <param name="partCode">The Code of the Part</param>
        /// <returns>The Requested Part</returns>
        public CabinPart GetPart(string partCode);
        /// <summary>
        /// Returns the specified Part along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="partCode">The Code of the Part</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public CabinPart GetPart(string partCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original part from the Repository
        /// </summary>
        /// <param name="partCode">The Part's Code</param>
        /// <returns></returns>
        public CabinPart GetPartOriginal(string partCode);

        /// <summary>
        /// Gets a Clone of the specified Profile by Code
        /// </summary>
        /// <param name="profileCode">The Code of the Profile</param>
        /// <returns>The Requested Profile</returns>
        public Profile GetProfile(string profileCode);
        /// <summary>
        /// Returns the specified Profile along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="profileCode">The Code of the Profile</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public Profile GetProfile(string profileCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the original Profile Object from the Repository
        /// </summary>
        /// <param name="profileCode">The Code of the Profile</param>
        /// <returns></returns>
        public Profile GetProfileOriginal(string profileCode);

        /// <summary>
        /// Gets a Clone of the Hinge Part by Code
        /// </summary>
        /// <param name="hingeCode">The Code of the Hinge</param>
        /// <returns>The Requested Hinge</returns>
        public CabinHinge GetHinge(string hingeCode);
        /// <summary>
        /// Returns the specified Hinge along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="hingeCode">The Code of the hinge</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public CabinHinge GetHinge(string hingeCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original Hinge Object from the Repository
        /// </summary>
        /// <param name="hingeCode">The Hinge's Code</param>
        /// <returns></returns>
        public CabinHinge GetHingeOriginal(string hingeCode);
        
        /// <summary>
        /// Gets a Clone of the specified Strip by Code
        /// </summary>
        /// <param name="stripCode">The Code of the Strip</param>
        /// <returns>The Requested Strip</returns>
        public GlassStrip GetStrip(string stripCode);
        /// <summary>
        /// Returns the specified Strip along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="stripCode">The Code of the Strip</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public GlassStrip GetStrip(string stripCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original Strip Object from the Repository
        /// </summary>
        /// <param name="stripCode">The strip's code</param>
        /// <returns></returns>
        public GlassStrip GetStripOriginal(string stripCode);
        /// <summary>
        /// Returns the Strips of a Certain Type for the Specified Structure
        /// </summary>
        /// <param name="stripType">The Type of the Strip</param>
        /// <param name="identifier">The Structures Identifier</param>
        /// <returns></returns>
        public IEnumerable<GlassStrip> GetStrips(CabinStripType stripType, CabinIdentifier identifier);

        /// <summary>
        /// Gets a Clone of the specified Angle by Code
        /// </summary>
        /// <param name="angleCode">The Code of the Angle</param>
        /// <returns>The Requested Angle</returns>
        public CabinAngle GetAngle(string angleCode);
        /// <summary>
        /// Returns the specified Angle along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="angleCode">The Code of the Angle</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public CabinAngle GetAngle(string angleCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original Angle object from the Repository
        /// </summary>
        /// <param name="angleCode">The angle's code</param>
        /// <returns></returns>
        public CabinAngle GetAngleOriginal(string angleCode);

        /// <summary>
        /// Gets a Clone of the specified Support Bar by Code
        /// </summary>
        /// <param name="supportBarCode">The Code of the Support Bar</param>
        /// <returns>The Requested Support Bar</returns>
        public SupportBar GetSupportBar(string supportBarCode);
        /// <summary>
        /// Returns the specified SupportBar along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="supportBarCode">The Code of the SupportBar</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public SupportBar GetSupportBar(string supportBarCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original Support Bar object from the Repository
        /// </summary>
        /// <param name="supportBarCode">The support bars code</param>
        /// <returns></returns>
        public SupportBar GetSupportBarOriginal(string supportBarCode);

        /// <summary>
        /// Gets a Clone of the specified Support by Code
        /// </summary>
        /// <param name="supportCode">The Code of the Support</param>
        /// <returns>The Requested Support</returns>
        public CabinSupport GetSupport(string supportCode);
        /// <summary>
        /// Returns the specified Support along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="supportCode">The Code of the Support</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public CabinSupport GetSupport(string supportCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original support Object from the Repository
        /// </summary>
        /// <param name="supportCode">The Support's Code</param>
        /// <returns></returns>
        public CabinSupport GetSupportOriginal(string supportCode);

        /// <summary>
        /// Gets a Clone of the specified Handle by Code
        /// </summary>
        /// <param name="handleCode">The Code of the Handle</param>
        /// <returns>The Requested Handle</returns>
        public CabinHandle GetHandle(string handleCode);
        /// <summary>
        /// Returns the specified Handle along with all its Additionals for the Specified Structure
        /// </summary>
        /// <param name="handleCode">The Code of the Handle</param>
        /// <param name="identifier">The Structure's Identifier</param>
        /// <returns></returns>
        public CabinHandle GetHandle(string handleCode, CabinIdentifier identifier);
        /// <summary>
        /// Gets the Original Handle Object from the Repository
        /// </summary>
        /// <param name="handleCode">The handle's Code</param>
        /// <returns></returns>
        public CabinHandle GetHandleOriginal(string handleCode);

    }
}