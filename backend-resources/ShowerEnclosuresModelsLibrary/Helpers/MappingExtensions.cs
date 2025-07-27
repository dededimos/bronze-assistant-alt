using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models;
using System.Runtime.CompilerServices;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    public static class MappingExtensions
    {
#nullable enable
        /// <summary>
        /// Generates a <typeparamref name="T"/> from the specified <paramref name="defaults"/>
        /// </summary>
        /// <typeparam name="T">The Type of the List that will be generated</typeparam>
        /// <param name="defaults">The List Defaults</param>
        /// <param name="identifier">The Identifier of the structure</param>
        /// <param name="r">The Repository from which to generate the Parts</param>
        /// <returns>The Constructed List</returns>
        public static T ToPartsList<T>(this DefaultPartsList defaults,CabinIdentifier identifier, ICabinMemoryRepository r) where T : CabinPartsList, new()
        {
            //Creates a PartSpot Dictioanry by extracting codes from PartSpotDefaults
            Dictionary<PartSpot, CabinPart?> positionedParts = defaults.SpotDefaults.ToDictionary(
                kvp => kvp.Key, //Key Selector Function
                kvp =>
                {
                    if (string.IsNullOrEmpty(kvp.Value.DefaultCode)) return null;
                    else
                    {
                        var partToReturn = r.GetPart(kvp.Value.DefaultCode,identifier);
                        //Copy also the QTY from the Default List !
                        partToReturn.Quantity = kvp.Value.DefaultQuantity;
                        return partToReturn;
                    }
                }
                );

            List<CabinPart> genericParts = new();
            foreach (var genericPartDefault in defaults.GenericParts)
            {
                var partToAdd = r.GetPart(genericPartDefault.PartCode);
                partToAdd.Quantity = genericPartDefault.PartQuantity;
                genericParts.Add(partToAdd);
            }

            T parts = new();
            parts.InitilizeParts(positionedParts, genericParts);
            return parts;
        }

        /// <summary>
        /// Returns the Identifier of this Cabin
        /// </summary>
        /// <param name="cabin">The Cabin</param>
        /// <returns>The Identifier or One that its draw is None</returns>
        public static CabinIdentifier Identifier(this Cabin cabin)
        {
            if(cabin?.Model is null) return new(Enums.CabinModelEnum.Model9S, CabinDrawNumber.None, cabin?.SynthesisModel ?? CabinSynthesisModel.Primary);
            return new(cabin.Model ?? Enums.CabinModelEnum.Model9S, cabin.IsPartOfDraw, cabin.SynthesisModel);
        }

    }
}
