using CommonHelpers;
using DataAccessLib.NoSQLModels;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace DataAccessLib;

/// <summary>
/// A Repository Constaining CRUD Operations for Cabin Constraints
/// </summary>
public interface ICabinConstraintsRepository
{
    /// <summary>
    /// Deletes a Constraint from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or failed Operation</returns>
    Task<OperationResult> DeleteConstraintAsync(CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);

    /// <summary>
    /// Returns all Constraints
    /// </summary>
    /// <returns></returns>
    Task<OperationResult<List<CabinConstraintsEntity>>> GetAllConstraintsAsync();

    /// <summary>
    /// Inserts a Constraint into the Database for the Concerning Model-Draw-Synthesis Combination
    /// </summary>
    /// <param name="constraint">The Constraint</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <returns>A Success or Failed Operation</returns>
    Task<OperationResult> InsertNewConstraintAsync(CabinConstraints constraint, CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);
    
    /// <summary>
    /// Updates a Constraint
    /// </summary>
    /// <param name="constraintsToUpdate"></param>
    /// <param name="concerningModel"></param>
    /// <param name="concerningDraw"></param>
    /// <param name="concerningSynthesisModel"></param>
    /// <returns>Success or Failed Operation Result</returns>
    Task<OperationResult> UpdateConstraintAsync(CabinConstraints constraintsToUpdate, CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);

    /// <summary>
    /// Returns All The Constraints in a Dictionary , Keys being the Cabin Identifiers
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<CabinIdentifier, CabinConstraints>> GetConstraintsDictionaryAsync();
}