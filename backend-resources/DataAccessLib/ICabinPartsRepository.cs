using CommonHelpers;
using DataAccessLib.NoSQLModels;
using MongoDB.Bson;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;

namespace DataAccessLib;
public interface ICabinPartsRepository
{
    /// <summary>
    /// Deletes a Part and Returns the Result of the Operation
    /// </summary>
    /// <param name="partCode">The Parts Code</param>
    /// <returns>The Deletion Result</returns>
    Task<OperationResult> DeletePartAsync(string partCode);
    /// <summary>
    /// Gets all the Parts from the Database
    /// </summary>
    /// <returns>The Result containing the Parts or Failure</returns>
    Task<OperationResult<List<CabinPartEntity>>> GetAllPartsAsync();

    /// <summary>
    /// Returns the List of PART objects with the Description in the Localized Defined Language
    /// If there is no such Language returns the Default Description
    /// </summary>
    /// <param name="isoTwoLetterLanguage">ex. GR,EN,IT</param>
    /// <returns></returns>
    Task<List<CabinPart>> GetAllPartsObjectsAsync(string isoTwoLetterLanguage);

    /// <summary>
    /// Inserts a New Part
    /// </summary>
    /// <param name="part">The Part to Insert</param>
    /// <param name="additionalPartsPerStructure">The Additional Parts of this part according to the Structure it is placed into</param>
    /// <param name="languageDescriptions">The Dictionary of the Various Cultures as Keys and their respective Description translations for the updating part</param>
    /// <param name="identifiers">The Uses of the Part , in which structures is used</param>
    /// <param name="notes">The Notes of this new part entity to be saved</param>
    /// <returns>The Result or Failure</returns>
    Task<OperationResult<ObjectId>> InsertNewPartAsync(
        CabinPart part,
        Dictionary<CabinIdentifier, List<CabinPart>> additionalPartsPerStructure,
        Dictionary<string, string> languageDescriptions,
        string notes,
        HashSet<CabinIdentifier> identifiers);

    /// <summary>
    /// Updates an Existing Part
    /// </summary>
    /// <param name="part">The Part Update</param>
    /// <param name="additionalPartsPerStructure">The Additional Parts of this Part according to the Structure that this Part is placed into</param>
    /// <param name="languageDescriptions">The Dictionary of the Various Cultures as Keys and their respective Description translations for the updating part</param>
    /// <param name="identifiers">The Uses of the Part , in which structures is used</param>
    /// <param name="notes">The Notes for this Part Entity</param>
    /// <param name="session">The Transaction session or Null if this does not run inside the scope of a Transaction </param>
    /// <returns>The Result of the Opeation</returns>
    Task<OperationResult> UpdatePartAsync(
        CabinPart part,
        Dictionary<CabinIdentifier, List<CabinPart>> additionalPartsPerStructure,
        Dictionary<string, string> languageDescriptions,
        HashSet<CabinIdentifier> identifiers,
        string notes,
        IClientSessionHandle? session = null);
    /// <summary>
    /// Updates an Existing Part (Searches by Id)
    /// </summary>
    /// <param name="partEntity">The Part Update</param>
    /// <param name="session">The Transaction session or Null if this does not run inside the scope of a Transaction </param>
    /// <returns>The Result of the Opeation</returns>
    Task<OperationResult> UpdatePartAsync(CabinPartEntity partEntity, IClientSessionHandle? session = null);


}