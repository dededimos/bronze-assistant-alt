using CommonHelpers;
using DataAccessLib.NoSQLModels;
using DataAccessLib.NoSQLModels.Validators;
using MongoDB.Bson;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Models;

namespace DataAccessLib.MongoDBAccess.RepoImplementations;
/// <summary>
/// Instantiates a Mongo Cabin Parts repo
/// Any MongoException is Caught and passed through the Operation Result to be thrown or not by the Consumer
/// </summary>
public class MongoCabinPartsRepository : ICabinPartsRepository
{
    private readonly IMongoCollection<CabinPartEntity> parts;
    private readonly CabinPartEntityValidator newPartValidator = new(true);
    private readonly CabinPartEntityValidator updatedPartValidator = new(false);

    public MongoCabinPartsRepository(IMongoDbCabinsConnection connection)
    {
        parts = connection.PartsCollection;
    }

    /// <summary>
    /// Returns all Parts on the List
    /// </summary>
    /// <returns></returns>
    public async Task<OperationResult<List<CabinPartEntity>>> GetAllPartsAsync()
    {
        try
        {
            //Find all records where the find predicate is true , here means all records
            var results = await parts.FindAsync(_ => true);
            var found = results.ToList();
            return OperationResult<List<CabinPartEntity>>.Success(found);
        }
        catch (Exception ex)
        {
            return OperationResult<List<CabinPartEntity>>.Failure(ex);
        }
    }

    /// <summary>
    /// Returns all the Parts with the Description in the Defined isoFourLetterLanguage
    /// </summary>
    /// <param name="isoFourLetterLanguage"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<CabinPart>> GetAllPartsObjectsAsync(string isoFourLetterLanguage)
    {
        var result = await GetAllPartsAsync();
        if (result.IsSuccessful)
        {
            var partsEntities = result.Result ?? new List<CabinPartEntity>();

            foreach (var partEntity in partsEntities)
            {
                if (partEntity.LanguageDescriptions.TryGetValue(isoFourLetterLanguage,out string? localizedDesc))
                {
                    partEntity.Part.Description = localizedDesc;
                }
            }
            return partsEntities.Select(p => p.Part).ToList();
        }
        else throw new Exception(result.Exception?.Message ?? "An Undefined Error Occured...", result.Exception);
    }

    /// <summary>
    /// Updates the Values of an Existing Part (Without Updating its Object Id)
    /// </summary>
    /// <param name="part">The Part to Update</param>
    /// <param name="additionalPartsPerStructure">The Additional Parts of this part according to the Structure it is placed into</param>
    /// <param name="languageDescriptions">The Dictionary of the Various Cultures as Keys and their respective Description translations for the updating part</param>
    /// <param name="session">The Transaction session or Null if this does not run inside the scope of a Transaction </param>
    /// <param name="notes">The Notes for this Part Entity</param>
    /// <returns>Success or Failed Operation Result</returns>
    public async Task<OperationResult> UpdatePartAsync(
        CabinPart part,
        Dictionary<CabinIdentifier,List<CabinPart>> additionalPartsPerStructure,
        Dictionary<string, string> languageDescriptions,
        HashSet<CabinIdentifier> identifiers,
        string notes,
        IClientSessionHandle? session = null)
    {
        try
        {
            //Construct the new Entity
            CabinPartEntity entity = new(part, additionalPartsPerStructure, languageDescriptions)
            {
                LastModified = DateTime.Now,
                Uses = new(identifiers.Select(i => new CabinIdentifierContainer(i))),
                Notes = notes
            };
            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinPartEntity>.Filter.Eq("Part.Code", entity.Part.Code);
            //Replace the found in the database
            //Do not Insert a new one if the part is not there (More like an Update)
            FindOneAndReplaceOptions<CabinPartEntity, CabinPartEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            CabinPartEntity? result;
            if (session is null)
            {
                result = await parts.FindOneAndReplaceAsync(filter, entity, options);
            }
            else
            {
                result = await parts.FindOneAndReplaceAsync(session, filter, entity, options);
            }
                        
            return result is not null
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Update , Part with Code:{entity.Part.Code} not Found");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Updates the Values of an Existing Part (searching by ID)
    /// </summary>
    /// <param name="partEntity">The PartEntity to Update</param>
    /// <param name="session">The Transaction session or Null if this does not run inside the scope of a Transaction </param>
    /// <returns>Success or Failed Operation Result</returns>
    public async Task<OperationResult> UpdatePartAsync(
        CabinPartEntity partEntity,
        IClientSessionHandle? session = null)
    {
        try
        {
            //Construct the new Entity
            partEntity.LastModified = DateTime.Now;
            var valResult = updatedPartValidator.Validate(partEntity);
            if (!valResult.IsValid) return OperationResult.Failure(string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode)));

            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinPartEntity>.Filter.Eq("Id", partEntity.Id);

            //Replace the found in the database
            //Do not Insert a new one if the part is not there
            FindOneAndReplaceOptions<CabinPartEntity, CabinPartEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            CabinPartEntity? result;
            if (session is null)
            {
                result = await parts.FindOneAndReplaceAsync(filter, partEntity, options);
            }
            else
            {
                result = await parts.FindOneAndReplaceAsync(session, filter, partEntity, options);
            }

            return result is not null
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Update , Part({partEntity.Part.Code}) with Id:{partEntity.Id} not Found");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Inserts a New Part if it does not exist and returns the inserted Object Id
    /// </summary>
    /// <param name="part">The Part to Insert</param>
    /// <param name="additionalPartsPerStructure">The Additional Parts of this part according to the Structure it is placed into</param>
    /// <param name="languageDescriptions">The Language Descriptions for the Part</param>
    /// <param name="notes">The Notes for this part Entity</param>
    /// <param name="identifiers">The Uses Identifiers</param>
    /// <returns>The Object Id of the Inserted Part or a Failure (exception or Message)</returns>
    public async Task<OperationResult<ObjectId>> InsertNewPartAsync(
        CabinPart part ,
        Dictionary<CabinIdentifier,List<CabinPart>> additionalPartsPerStructure,
        Dictionary<string,string> languageDescriptions,
        string notes,
        HashSet<CabinIdentifier> identifiers)
    {
        try
        {
            var filter = Builders<CabinPartEntity>.Filter.Eq("Part.Code", part.Code);
            var result = await parts.FindAsync(filter);

            //Fail if Exists
            if (result.Any())
            {
                return OperationResult<ObjectId>.Failure($"Could not Insert , a Part with Code:{part.Code} Already Exists");
            }
            //Else Insert
            CabinPartEntity entity = new(part, additionalPartsPerStructure, languageDescriptions) 
            { 
                LastModified = DateTime.Now,
                Uses = new(identifiers.Select(i => new CabinIdentifierContainer(i))),
                Notes = notes
            };
            var valResult = newPartValidator.Validate(entity);
            if (valResult.IsValid)
            {
                await parts.InsertOneAsync(entity);
                var findResult = await parts.FindAsync(filter); //will throw if it does not find it
                return OperationResult<ObjectId>.Success(findResult.Single().Id);
            }
            else
            {
                return OperationResult<ObjectId>.Failure(string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode)));
            }
        }
        catch (Exception ex)
        {
            return OperationResult<ObjectId>.Failure(ex);
        }
    }

    /// <summary>
    /// Deletes a Part from the Database
    /// </summary>
    /// <param name="partCode">The Parts Code</param>
    /// <returns>A successful or Not Succesful Result</returns>
    public async Task<OperationResult> DeletePartAsync(string partCode)
    {
        try
        {
            var filter = Builders<CabinPartEntity>.Filter.Eq("Part.Code", partCode);
            var result = await parts.DeleteOneAsync(filter);
            return result.DeletedCount > 0
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Delete , Part with Code: {partCode} was not Found");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }


    //For test Purposes Only
    /// <summary>
    /// Retursn the List of Keys with its Values Represented in an IEnumerable<string>
    /// </summary>
    /// <returns>the List of strings of the Indexes</returns>
    private IEnumerable<string> GetPartsCollectionIndexesDescriptions()
    {
        List<string> indexes = new();

        using (var cursor = parts.Indexes.List())
        {
            var list = cursor.ToList();
            foreach (var index in list)
            {
                foreach (var key in index)
                {
                    indexes.Add($"Key - {key.Name} : {key.Value}");
                }
            }
        }
        return indexes;
    }
}
