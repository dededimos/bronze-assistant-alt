using CommonHelpers;
using DataAccessLib.NoSQLModels;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;

namespace DataAccessLib;
public interface ICabinSettingsRepository
{
    /// <summary>
    /// Deletes a Setting from the Database
    /// </summary>
    /// <param name="concerningModel">The concerning Model</param>
    /// <param name="concerningDraw">The concerning Draw</param>
    /// <param name="concerningSynthesisModel">The concerning Synthesis Model</param>
    /// <returns>A Successfful or Failed Operation</returns>
    Task<OperationResult> DeleteSettingAsync(CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel);
    /// <summary>
    /// Returns all Settings for the available Combinations of Model-Draw-SynthesisModel
    /// </summary>
    /// <returns>The Result if Success otherwise a failed Operation</returns>
    Task<OperationResult<List<CabinSettingsEntity>>> GetAllSettingsAsync();

    /// <summary>
    /// Return a Dictionary of All the Cabin Settings
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<CabinIdentifier, CabinSettings>> GetCabinSettingsDictionaryAsync();

    /// <summary>
    /// Insert a Setting for the Selected Model-Draw-SynthesisModel Combination
    /// </summary>
    /// <param name="setting">The Setting to Insert</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Synthesis Model</param>
    /// <param name="notes">Notes for this settings Entity</param>
    /// <returns>a Successful or Failed Operation</returns>
    Task<OperationResult> InsertNewSettingAsync(CabinSettings setting, CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel,string notes);
    /// <summary>
    /// Updates a Parts List
    /// </summary>
    /// <param name="settingToUpdate">The Updated Setting</param>
    /// <param name="concerningModel">The Concerning Model</param>
    /// <param name="concerningDraw">The Concerning Draw</param>
    /// <param name="concerningSynthesisModel">The Concerning Synthesis Model</param>
    /// <param name="notes">Notes for this settings Entity</param>
    /// <returns>a Successful or Failed Operation </returns>
    Task<OperationResult> UpdateSettingAsync(CabinSettings settingToUpdate, CabinModelEnum concerningModel, CabinDrawNumber concerningDraw, CabinSynthesisModel concerningSynthesisModel,string notes);
}