using DataAccessLib.NoSQLModels;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models;

namespace DataAccessLib.MongoDBAccess.RepoImplementations;
public class MongoCabinPartsListsRepository : ICabinPartsListsRepository
{
    private readonly IMongoCollection<DefaultPartsListEntity> partsLists;
    
    public MongoCabinPartsListsRepository(IMongoDbCabinsConnection connection)
    {
        partsLists = connection.PartsListsCollection;
    }

    /// <summary>
    /// Returns all the Parts Lists for the available Combinations of Model-Draw-SynthesisModel
    /// </summary>
    /// <returns>The Result if Success otherwise a failed Operation</returns>
    public async Task<OperationResult<List<DefaultPartsListEntity>>> GetAllPartsListsAsync()
    {
        try
        {
            //Find all records where the find predicate is true , here means all records
            var results = await partsLists.FindAsync(_ => true);
            var found = results.ToList();
            return OperationResult<List<DefaultPartsListEntity>>.Success(found);
        }
        catch (Exception ex)
        {
            return OperationResult<List<DefaultPartsListEntity>>.Failure(ex);
        }
    }

    public async Task<Dictionary<CabinIdentifier,DefaultPartsList>> GetPartsListsDictionaryAsync()
    {
        var result = await GetAllPartsListsAsync();
        if (result.IsSuccessful)
        {
            var modifiedRes = result.Result?.ToDictionary(e => new CabinIdentifier(e.Model, e.DrawNumber, e.SynthesisModel), e => e.DefaultPartsList);
            if (modifiedRes is not null)
            {
                return modifiedRes;
            }
            else return new();
        }
        else throw new Exception(result.Exception?.Message ?? "An Undefined Error Occured...", result.Exception);
    }

    /// <summary>
    /// Updates a Parts List
    /// </summary>
    /// <param name="defaultPartsListToUpdate">The Updated List</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <param name="session">The Session of the Transaction or Null if this does not run inside a transaction</param>
    /// <returns>a Successful or Failed Operation </returns>
    public async Task<OperationResult> UpdatePartsListAsync(
        DefaultPartsList defaultPartsListToUpdate,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel,
        IClientSessionHandle? session = null)
    {
        try
        {
            //Construct the new Entity
            DefaultPartsListEntity entity = new(defaultPartsListToUpdate, concerningModel, concerningDraw, concerningSynthesisModel)
            { LastModified = DateTime.Now };

            //Create the Filter to Find the Item in the Database
            var filter = Builders<DefaultPartsListEntity>.Filter.Eq("Model", entity.Model)
                & Builders<DefaultPartsListEntity>.Filter.Eq("DrawNumber", entity.DrawNumber)
                & Builders<DefaultPartsListEntity>.Filter.Eq("SynthesisModel", entity.SynthesisModel);

            DefaultPartsListEntity? result;
            if (session is null)
            {
                //Replace the found in the database
                //Do not Insert a new one if the part is not there (More like an Update)
                FindOneAndReplaceOptions<DefaultPartsListEntity, DefaultPartsListEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
                result = await partsLists.FindOneAndReplaceAsync(filter, entity, options);
            }
            else
            {
                //Replace the found in the database
                //Do not Insert a new one if the part is not there (More like an Update)
                FindOneAndReplaceOptions<DefaultPartsListEntity, DefaultPartsListEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
                result = await partsLists.FindOneAndReplaceAsync(session,filter, entity, options);
            }

            //If returns the Before Document it has succeded in replacing , other wise it did not find one to replace
            return result is not null
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Update , Default-Parts-List was not Found for :{entity.Model}-{entity.DrawNumber}-{entity.SynthesisModel}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Updates a Parts List
    /// </summary>
    /// <param name="defaultPartsListToUpdate">The Updated List</param>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <param name="session">The Session of the Transaction or Null if this does not run inside a transaction</param>
    /// <returns>a Successful or Failed Operation</returns>
    public async Task<OperationResult> UpdatePartsListAsync(
        DefaultPartsList defaultPartsListToUpdate,
        CabinIdentifier identifier,
        IClientSessionHandle? session = null)
    {
        return await UpdatePartsListAsync(
            defaultPartsListToUpdate, 
            identifier.Model, 
            identifier.DrawNumber, 
            identifier.SynthesisModel, 
            session);
    }

    /// <summary>
    /// Insert a New Parts List for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="defaultPartsList">The Parts List to Insert</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Synthesis Model</param>
    /// <returns>a Successful or Failed Operation</returns>
    public async Task<OperationResult> InsertNewPartsListAsync(
        DefaultPartsList defaultPartsList,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            //Create the Filter to Find the Item in the Database
            var filter = Builders<DefaultPartsListEntity>.Filter.Eq("Model", concerningModel)
                & Builders<DefaultPartsListEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<DefaultPartsListEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await partsLists.FindAsync(filter);

            //Fail if Exists
            if (result.Any())
            {
                return OperationResult.Failure($"Could not Insert , a Default-Parts-List Already exists for :{concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
            }
            
            //Else Insert
            DefaultPartsListEntity entity = new(defaultPartsList, concerningModel, concerningDraw, concerningSynthesisModel);
            await partsLists.InsertOneAsync(entity);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Insert a New Parts List for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="defaultPartsList">The Parts List to Insert</param>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <returns>a Successful or Failed Operation</returns>
    public async Task<OperationResult> InsertNewPartsListAsync(DefaultPartsList defaultPartsList,CabinIdentifier identifier)
    {
        return await InsertNewPartsListAsync(
            defaultPartsList,
            identifier.Model,
            identifier.DrawNumber,
            identifier.SynthesisModel);
    }

    /// <summary>
    /// Deletes a PartsList from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or Failed Operation</returns>
    public async Task<OperationResult> DeletePartsListAsync(
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            var filter = Builders<DefaultPartsListEntity>.Filter.Eq("Model", concerningModel)
                & Builders<DefaultPartsListEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<DefaultPartsListEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await partsLists.DeleteOneAsync(filter);
            return result.IsAcknowledged
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Delete , Parts-List was not Found for : {concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Deletes a PartsList from the Database
    /// </summary>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <returns>a Successful or Failed Operation</returns>
    public async Task<OperationResult> DeletePartsListAsync(CabinIdentifier identifier)
    {
        return await DeletePartsListAsync(identifier.Model,identifier.DrawNumber,identifier.SynthesisModel);
    }
}
