using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;

namespace DataAccessLib.NoSQLModels;
public class CabinSettingsEntity : DbEntity
{
    public CabinSettings Settings { get; set; }

    public CabinModelEnum Model { get; set; }
    public CabinDrawNumber DrawNumber { get; set; }
    public CabinSynthesisModel SynthesisModel { get; set; }

    public CabinSettingsEntity(CabinSettings settings , CabinModelEnum model , CabinDrawNumber drawNumber,CabinSynthesisModel synthesisModel , string notes)
    {
        Settings = settings;
        Model = model;
        DrawNumber = drawNumber;
        SynthesisModel = synthesisModel;
        Notes = notes;
    }

}
