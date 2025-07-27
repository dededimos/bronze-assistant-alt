using CommonHelpers;
using DataAccessLib.NoSQLModels;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.MongoDBAccess.RepoImplementations;
public class MongoCabinConstraintsRepository : ICabinConstraintsRepository
{
    private readonly IMongoCollection<CabinConstraintsEntity> constraints;

    public MongoCabinConstraintsRepository(IMongoDbCabinsConnection connection)
    {
        constraints = connection.ConstraintsCollection;
    }

    /// <summary>
    /// Returns all Constraints , A failed Operation here will always contain an Exception
    /// </summary>
    /// <returns></returns>
    public async Task<OperationResult<List<CabinConstraintsEntity>>> GetAllConstraintsAsync()
    {
        try
        {
            //Find all records where the find predicate is true , here means all records
            var results = await constraints.FindAsync(_ => true);
            var found = results.ToList();
            return OperationResult<List<CabinConstraintsEntity>>.Success(found);
        }
        catch (Exception ex)
        {
            return OperationResult<List<CabinConstraintsEntity>>.Failure(ex).SetFailureContext($"Get All ConstraintsAsync Failed...");
        }
    }

    /// <summary>
    /// Returns All The Cabin Constraints in a Dictionary , Key Being a Cabin Identifier
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<CabinIdentifier, CabinConstraints>> GetConstraintsDictionaryAsync()
    {
        var result = await GetAllConstraintsAsync();

        if (result.IsSuccessful)
        {
            var modifiedRes = result.Result?.ToDictionary(e => new CabinIdentifier(e.Model, e.DrawNumber, e.SynthesisModel), e => e.Constraints);
            if (modifiedRes is not null)
            {
                return modifiedRes;
            }
            else return new();
        }
        else throw new Exception(result.Exception?.Message ?? "An Undefined Error Occured...",result.Exception);

    }

    /// <summary>
    /// Updates a Constraint
    /// </summary>
    /// <param name="constraintsToUpdate"></param>
    /// <param name="concerningModel"></param>
    /// <param name="concerningDraw"></param>
    /// <param name="concerningSynthesisModel"></param>
    /// <returns>Success or Failed Operation Result</returns>
    public async Task<OperationResult> UpdateConstraintAsync(
        CabinConstraints constraintsToUpdate,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            //Construct the new Entity
            CabinConstraintsEntity entity = new(constraintsToUpdate, concerningModel, concerningDraw, concerningSynthesisModel)
            { LastModified = DateTime.Now };

            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinConstraintsEntity>.Filter.Eq("Model", entity.Model)
                & Builders<CabinConstraintsEntity>.Filter.Eq("DrawNumber", entity.DrawNumber)
                & Builders<CabinConstraintsEntity>.Filter.Eq("SynthesisModel", entity.SynthesisModel);

            //Replace the found in the database
            //Do not Insert a new one if the part is not there (More like an Update)
            FindOneAndReplaceOptions<CabinConstraintsEntity, CabinConstraintsEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
            var result = await constraints.FindOneAndReplaceAsync(filter, entity, options);

            //If returns the Before Document it has succeded in replacing , other wise it did not find one to replace

            return result is not null
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Update , Constraint was not Found for :{entity.Model}-{entity.DrawNumber}-{entity.SynthesisModel}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Inserts a Constraint into the Database for the Concerning Model-Draw-Synthesis Combination
    /// </summary>
    /// <param name="constraint">The Constraint</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <returns>A Success or Failed Operation</returns>
    public async Task<OperationResult> InsertNewConstraintAsync(
        CabinConstraints constraint,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinConstraintsEntity>.Filter.Eq("Model", concerningModel)
                & Builders<CabinConstraintsEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<CabinConstraintsEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await constraints.FindAsync(filter);

            //Fail if Exists
            if (result.Any())
            {
                return OperationResult.Failure($"Could not Insert , a Constraint Already exists for :{concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
            }
            //Else Insert
            CabinConstraintsEntity entity = new(constraint, concerningModel, concerningDraw, concerningSynthesisModel);
            await constraints.InsertOneAsync(entity);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Deletes a Constraint from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or failed Operation</returns>
    public async Task<OperationResult> DeleteConstraintAsync(
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            var filter = Builders<CabinConstraintsEntity>.Filter.Eq("Model", concerningModel)
                & Builders<CabinConstraintsEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<CabinConstraintsEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await constraints.DeleteOneAsync(filter);
            return result.DeletedCount > 1
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Delete , Constraint was not Found for : {concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
        }
        catch (Exception ex)
        {
            //if the isAknowledged is false it will throw an exception . We will not know if the item was deleted or Not
            return OperationResult.Failure(ex);
        }
    }
}
