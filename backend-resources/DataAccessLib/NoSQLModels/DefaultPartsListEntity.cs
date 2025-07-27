using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.NoSQLModels;
public class DefaultPartsListEntity : DbEntity
{
    public DefaultPartsList DefaultPartsList { get; set; }
    public CabinModelEnum Model { get; set; }
    public CabinDrawNumber DrawNumber { get; set; }
    public CabinSynthesisModel SynthesisModel { get; set; }

    public DefaultPartsListEntity(
        DefaultPartsList defaultPartsList,
        CabinModelEnum model,
        CabinDrawNumber drawNumber,
        CabinSynthesisModel synthesisModel)
    {
        DefaultPartsList = defaultPartsList;
        Model = model;
        DrawNumber = drawNumber;
        SynthesisModel = synthesisModel;
    }

    public CabinIdentifier GetIdentifier() { return new CabinIdentifier(Model,DrawNumber, SynthesisModel); }
}

public class CabinPartsListEntity : DbEntity
{
    public Dictionary<PartSpot, CabinPart?> PositionedParts { get; set; } = new();
    public List<CabinPart> GenericParts { get; set; } = new();
    public string TypeDiscriminator { get; set; } = typeof(CabinPartsList).AssemblyQualifiedName ?? "InvalidType";

    public CabinPartsListEntity(CabinPartsList partsList)
    {
        PositionedParts = partsList.GetPositionedParts();
        GenericParts = new(partsList.GenericParts);
        TypeDiscriminator = partsList.GetType().AssemblyQualifiedName ?? "InvalidType";
    }

    public CabinPartsList ToPartsList()
    {
        //Read the Type from the Discriminator
        Type? partsType = Type.GetType(TypeDiscriminator) 
            ?? throw new Exception($"TypeDiscriminator Value: '{TypeDiscriminator}' does not match any known type -- thrown at {nameof(CabinPartsListEntity)}-Method :{nameof(ToPartsList)}");

        //Create an Instance of the Requested Type
        if (Activator.CreateInstance(partsType) is not CabinPartsList parts)
        {
            throw new Exception($"Inconsistent TypeDiscriminator , Expected a derived Type of{nameof(CabinPartsList)} , Actual {TypeDiscriminator}");

        }

        parts.InitilizeParts(PositionedParts, GenericParts);
        return parts;
    }

    private CabinPartsListEntity()
    {

    }

    public static CabinPartsListEntity Empty()
    {
        return new();
    }

}