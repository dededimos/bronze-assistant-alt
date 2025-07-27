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
public class MongoCabinSettingsRepository : ICabinSettingsRepository
{
    private readonly IMongoCollection<CabinSettingsEntity> settings;

    public MongoCabinSettingsRepository(IMongoDbCabinsConnection connection)
    {
        settings = connection.SettingsCollection;
    }

    /// <summary>
    /// Returns all Settings for the available Combinations of Model-Draw-SynthesisModel
    /// </summary>
    /// <returns>The Result if Success otherwise a failed Operation</returns>
    public async Task<OperationResult<List<CabinSettingsEntity>>> GetAllSettingsAsync()
    {
        try
        {
            //Find all records where the find predicate is true , here means all records
            var results = await settings.FindAsync(_ => true);
            var found = results.ToList();
            return OperationResult<List<CabinSettingsEntity>>.Success(found);
        }
        catch (Exception ex)
        {
            return OperationResult<List<CabinSettingsEntity>>.Failure(ex);
        }
    }

    /// <summary>
    /// Returns the Dictionary of all the Cabin Settings
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Dictionary<CabinIdentifier, CabinSettings>> GetCabinSettingsDictionaryAsync()
    {
        var result = await GetAllSettingsAsync();
        if (result.IsSuccessful)
        {
            var modifiedRes = result.Result?.ToDictionary(e => new CabinIdentifier(e.Model, e.DrawNumber, e.SynthesisModel), e => e.Settings);
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
    /// <param name="settingToUpdate">The Updated Setting</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <param name="notes">Notes for this settings Entity</param>
    /// <returns>a Successful or Failed Operation </returns>
    public async Task<OperationResult> UpdateSettingAsync(
        CabinSettings settingToUpdate,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel,
        string notes)
    {
        try
        {
            //Construct the new Entity
            CabinSettingsEntity entity = new(settingToUpdate, concerningModel, concerningDraw, concerningSynthesisModel,notes)
            { LastModified = DateTime.Now };

            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinSettingsEntity>.Filter.Eq("Model", entity.Model)
                & Builders<CabinSettingsEntity>.Filter.Eq("DrawNumber", entity.DrawNumber)
                & Builders<CabinSettingsEntity>.Filter.Eq("SynthesisModel", entity.SynthesisModel);

            //Replace the found in the database
            //Do not Insert a new one if the part is not there (More like an Update)
            FindOneAndReplaceOptions<CabinSettingsEntity, CabinSettingsEntity> options = new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
            var result = await settings.FindOneAndReplaceAsync(filter, entity, options);
            
            //If returns the Before Document it has succeded in replacing , other wise it did not find one to replace
            return result is not null
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Update , Setting was not Found for :{entity.Model}-{entity.DrawNumber}-{entity.SynthesisModel}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Insert a Setting for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="setting">The Setting to Insert</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Synthesis Model</param>
    /// <param name="notes">Notes for this settings Entity</param>
    /// <returns>a Successful or Failed Operation</returns>
    public async Task<OperationResult> InsertNewSettingAsync(
        CabinSettings setting,
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel,
        string notes)
    {
        try
        {
            //Create the Filter to Find the Item in the Database
            var filter = Builders<CabinSettingsEntity>.Filter.Eq("Model", concerningModel)
                & Builders<CabinSettingsEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<CabinSettingsEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await settings.FindAsync(filter);

            //Fail if Exists
            if (result.Any())
            {
                return OperationResult.Failure($"Could not Insert , a Setting Already exists for :{concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
            }
            //Else Insert
            CabinSettingsEntity entity = new(setting, concerningModel, concerningDraw, concerningSynthesisModel,notes);
            await settings.InsertOneAsync(entity);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

    /// <summary>
    /// Deletes a Setting from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or Failed Operation</returns>
    public async Task<OperationResult> DeleteSettingAsync(
        CabinModelEnum concerningModel,
        CabinDrawNumber concerningDraw,
        CabinSynthesisModel concerningSynthesisModel)
    {
        try
        {
            var filter = Builders<CabinSettingsEntity>.Filter.Eq("Model", concerningModel)
                & Builders<CabinSettingsEntity>.Filter.Eq("DrawNumber", concerningDraw)
                & Builders<CabinSettingsEntity>.Filter.Eq("SynthesisModel", concerningSynthesisModel);
            var result = await settings.DeleteOneAsync(filter);
            return result.IsAcknowledged
                ? OperationResult.Success()
                : OperationResult.Failure($"Could not Delete , Setting was not Found for : {concerningModel}-{concerningDraw}-{concerningSynthesisModel}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex);
        }
    }

}
