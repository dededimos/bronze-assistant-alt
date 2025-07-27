using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;

public interface ICabinParts
{
    /// <summary>
    /// Returns all the Parts of the First Level (Without including any nested Parts) and Without Cloning
    /// Parts with zero Quantity ARE NOT RETURNED
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CabinPart> GetCabinPartsNotNested();
    /// <summary>
    /// Returns Clones of ALL Parts including Nested Parts and Appropriate Quantities for the specified Structure
    /// Parts with zero Quantity ARE NOT RETURNED
    /// </summary>
    /// <param name="identifier">The Structures Identifier</param>
    /// <returns></returns>
    public IEnumerable<CabinPart> GetCabinPartsNested(CabinIdentifier identifier);
}
