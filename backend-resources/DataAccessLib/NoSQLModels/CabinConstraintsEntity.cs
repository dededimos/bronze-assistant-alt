using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.NoSQLModels;
public class CabinConstraintsEntity : DbEntity
{
    public CabinConstraints Constraints { get; set; }

    public CabinDrawNumber DrawNumber { get; set; }
    public CabinSynthesisModel SynthesisModel { get; set; }
    public CabinModelEnum Model { get; set; }

    public CabinConstraintsEntity(
        CabinConstraints constraints,
        CabinModelEnum model,
        CabinDrawNumber drawNumber,
        CabinSynthesisModel synthesisModel)
	{
        Constraints = constraints;
        Model = model;
        DrawNumber = drawNumber;
        SynthesisModel = synthesisModel;
    }

    /// <summary>
    /// Returns the Cabin Identifyer for this ConstraintsEntity
    /// </summary>
    /// <returns></returns>
    public CabinIdentifier Identifyer()
    {
        return new(this.Model,this.DrawNumber,this.SynthesisModel);
    }
}
