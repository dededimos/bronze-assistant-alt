using CommonHelpers;
using DataAccessLib.NoSQLModels;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace DataAccessLib;
public interface ICabinPartsListsRepository
{

    /// <summary>
    /// Returns all the Parts Lists for the available Combinations of Model-Draw-SynthesisModel
    /// </summary>
    /// <returns>The Result if Success otherwise a failed Operation</returns>
    Task<OperationResult<List<DefaultPartsListEntity>>> GetAllPartsListsAsync();

    /// <summary>
    /// Returns a Dictionary of all the Parts Lists
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<CabinIdentifier, DefaultPartsList>> GetPartsListsDictionaryAsync();

    /// <summary>
    /// Deletes a PartsList from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or Failed Operation</returns>
    Task<OperationResult> DeletePartsListAsync(CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);
    /// <summary>
    /// Deletes a PartsList from the Database
    /// </summary>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <returns>a Successful or Failed Operation</returns>
    Task<OperationResult> DeletePartsListAsync(CabinIdentifier identifier);

    /// <summary>
    /// Insert a New Parts List for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="defaultPartsList">The Parts List to Insert</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Synthesis Model</param>
    /// <returns>a Successful or Failed Operation</returns>
    Task<OperationResult> InsertNewPartsListAsync(DefaultPartsList defaultPartsList, CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);
    /// <summary>
    /// Insert a New Parts List for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="defaultPartsList">The Parts List to Insert</param>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <returns>a Successful or Failed Operation</returns>
    Task<OperationResult> InsertNewPartsListAsync(DefaultPartsList defaultPartsList, CabinIdentifier identifier);

    /// <summary>
    /// Updates a Parts List
    /// </summary>
    /// <param name="defaultPartsListToUpdate">The Updated List</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <param name="session">The Session of the Transaction or Null if this does not run inside a transaction</param>
    /// <returns>a Successful or Failed Operation </returns>
    Task<OperationResult> UpdatePartsListAsync(
        DefaultPartsList defaultPartsListToUpdate, 
        CabinModelEnum concerningModel, 
        CabinDrawNumber concerningDraw, 
        CabinSynthesisModel concerningSynthesisModel,
        IClientSessionHandle? session = null);

    /// <summary>
    /// Updates a Parts List
    /// </summary>
    /// <param name="defaultPartsListToUpdate">The Updated List</param>
    /// <param name="identifier">The identifier of the Cabin owning the List</param>
    /// <param name="session">The Session of the Transaction or Null if this does not run inside a transaction</param>
    /// <returns>a Successful or Failed Operation</returns>
    Task<OperationResult> UpdatePartsListAsync(
        DefaultPartsList defaultPartsListToUpdate,
        CabinIdentifier identifier,
        IClientSessionHandle? session = null);
}