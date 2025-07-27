using CommonInterfacesBronze;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
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
public class CabinPartEntity : DbEntity ,IDeepClonable<CabinPartEntity>
{
    public CabinPart Part { get; set; }
    public Dictionary<string, string> LanguageDescriptions { get; set; } = [];
    /// <summary>
    /// The Structures where this Part is UsedOn
    /// </summary>
    public HashSet<CabinIdentifierContainer> Uses { get; set; } = [];
    public bool IsUsed { get => Uses.Count != 0; }

    /// <summary>
    /// The Additional Parts per Structure this Part Has
    /// </summary>
    public Dictionary<CabinIdentifier, List<CabinPart>> AdditionalPartsPerStructure { get; set; } = [];

    public CabinPartEntity(CabinPart cabinPart,Dictionary<CabinIdentifier,List<CabinPart>> additionalPartsPerStructure , Dictionary<string,string> languageDescriptions)
    {
        Part = cabinPart;
        AdditionalPartsPerStructure = additionalPartsPerStructure.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(p => p.GetDeepClone()).ToList());
        LanguageDescriptions = new(languageDescriptions);
    }

    public CabinPartEntity GetDeepClone()
    {
        CabinPartEntity clone = new(Part.GetDeepClone(),AdditionalPartsPerStructure, LanguageDescriptions)
        {
            Id = Id,
            LastModified = LastModified,
            Uses = new(Uses),
            Notes = Notes
        };
        return clone;
    }

    /// <summary>
    /// Replaces the Part with the specified Code with a new Part
    /// Replacement goes 2levels deep in additional Parts and 1 level down in Parts per Structure
    /// </summary>
    /// <param name="partToReplaceCode">The Code to replace</param>
    /// <param name="newPart">The New Part</param>
    public void ReplaceAdditionalPart(string partToReplaceCode,CabinPart newPart)
    {
        List<List<CabinPart>> partsLists = [];

        //Add to the Lists the PartsLists from Additional Parts (up to 2 levels down)
        if (Part.AdditionalParts.Count != 0)
        {
            //Combine all the lists of additional Parts up to two levels down
            partsLists.Add(Part.AdditionalParts);
            foreach (var part in Part.AdditionalParts)
            {
                if (part.AdditionalParts.Count != 0)
                {
                    partsLists.Add(part.AdditionalParts);
                    foreach (var part2Level in part.AdditionalParts)
                    {
                        if (part2Level.AdditionalParts.Count != 0)
                        {
                            partsLists.Add(part2Level.AdditionalParts);
                        }
                    }
                }
            }
        }

        //Add also all the Lists for the Parts per Structure
        foreach (var listPerStructure in AdditionalPartsPerStructure.Values)
        {
            partsLists.Add(listPerStructure);
            
            //Add all the Additional Parts for each Part of perStructure List only for 1 Level
            foreach (var addPart in listPerStructure)
            {
                if (addPart.AdditionalParts.Count != 0)
                {
                    partsLists.Add(addPart.AdditionalParts);
                }
            }
        }

        //Change the Part with the new One in each and every one of the lists that the part To Replace is Present
        foreach (var list in partsLists)
        {
            //If the List contains this Code Replace it (by keeping the same Quantity)
            if (list.Any(p => p.Code == partToReplaceCode))
            {
                //Foreach one of them replace the Part with a new one but the Old Quantity
                //Traverse the List backwards otherwise there is an enumeration changed error
                //Count the Total Number -1 to find max index , as long as i >= 0 meaning geater or equal to min index
                for (int i = list.Count -1; i >= 0; i--)
                {
                    //If the item is one which code should be replaced
                    //Clone the new Item apss the old quantity and replace it into the list
                    if (list[i].Code == partToReplaceCode)
                    {
                        var newPartToAdd = newPart.GetDeepClone();
                        newPartToAdd.Quantity = list[i].Quantity;
                        list[i] = newPartToAdd;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Searches up to Two levels down in Additional Parts and 1 Level in Additional Parts per Structure
    /// returns wheather the code being searched Exists
    /// </summary>
    /// <param name="partCodeToSearch">The Code to Search for</param>
    /// <returns></returns>
    public bool HasAdditionalPart(string partCodeToSearch)
    {
        //Search if there is any Code in any of the Parts List of Additional Parts , 2Levels down
        if (Part.AdditionalParts.Count != 0)
        {
            List<CabinPart> allParts = new(Part.AdditionalParts);
            foreach (var part in Part.AdditionalParts)
            {
                if (part.AdditionalParts.Count != 0)
                {
                    allParts.AddRange(part.AdditionalParts);
                    foreach (var part2 in part.AdditionalParts)
                    {
                        if (part2.AdditionalParts.Count != 0)
                        {
                            allParts.AddRange(part2.AdditionalParts);
                        }
                    }
                }
            }

            if (allParts.Any(p => p.Code == partCodeToSearch))
            {
                return true;
            }
        }

        //Search also inside the Additional Parts per Structure and their AdditionalParts as well up to 1 level down
        if (AdditionalPartsPerStructure.Values.Count != 0)
        {
            foreach (var list in AdditionalPartsPerStructure.Values)
            {
                if (list.Any(p=>p.Code == partCodeToSearch))
                {
                    return true;
                }
                foreach (var item in list)
                {
                    if (item.AdditionalParts.Any(p => p.Code == partCodeToSearch)) return true;
                }
            }
        }
        //If nothing of the above is fruitfull return false;
        return false;
    }
}

/// <summary>
/// Mongo DB Does not handle Strcuts unless a Custom Seriliazer is made for them
/// </summary>
public class CabinIdentifierContainer
{
    public CabinModelEnum Model { get; set; }
    public CabinDrawNumber DrawNumber { get; set; }
    public CabinSynthesisModel SynthesisModel { get; set; }
    public CabinIdentifier Identifier { get => new(Model, DrawNumber, SynthesisModel); }

    public CabinIdentifierContainer(CabinModelEnum model , CabinDrawNumber number , CabinSynthesisModel synthesisModel)
    {
        Model = model;
        DrawNumber = number;
        SynthesisModel = synthesisModel;
    }
    public CabinIdentifierContainer(CabinIdentifier identifier)
    {
        Model = identifier.Model;
        DrawNumber = identifier.DrawNumber;
        SynthesisModel = identifier.SynthesisModel;
    }
}




