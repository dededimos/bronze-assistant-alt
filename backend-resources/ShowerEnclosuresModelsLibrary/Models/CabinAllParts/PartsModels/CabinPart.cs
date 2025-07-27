using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
public class CabinPart : IDeepClonable<CabinPart> ,ICodeable
{
    public string Code { get; set; }
    public string Description { get; set; }
    public string PhotoPath { get; set; }
    public virtual CabinPartType Part { get; protected set; } = CabinPartType.GenericPart;
    public MaterialType Material { get; set; }
    /// <summary>
    /// The Additional Parts from which this part Consists Of (ex. Screws , Anchors , Lids e.t.c.)
    /// </summary>
    public List<CabinPart> AdditionalParts { get; set; } = new();
        
    /// <summary>
    /// The Quantity of this Part examined by this Object (Relevant for BOMs or Costing)
    /// </summary>
    public double Quantity { get; set; } = 1;

    public CabinPart()
    {
        
    }
    public CabinPart(CabinPartType partType)
    {
        Part = partType;
    }

    /// <summary>
    /// Returns CLONES OF all the Parts that are Included with this Part for the specified Structure (along with their Quantities)
    /// </summary>
    /// <param name="identifier">The Structures Identifier</param>
    /// <returns>The parts Included along with their Quantities</returns>
    public IEnumerable<CabinPart> GetIncludedPartsWithQty(CabinIdentifier identifier)
    {
        //Create a new List to hold all the Parts to be returned
        List<CabinPart> listOfParts = new();
        
        //First traverse all the Additional Parts and get Clones with the correct quantity
        var additionalPartsWithQty = AdditionalParts.Where(p => p.Quantity > 0).Select(p =>
        {
            var clone = p.GetDeepClone();
            clone.Quantity *= this.Quantity;
            return clone;
        });

        //Add those Parts to the List that should be returned
        listOfParts.AddRange(additionalPartsWithQty);
        
        //Foreach of those parts execute this method  again this will get ALL nested Parts of each Part for infinite levels down the tree.
        listOfParts.AddRange(additionalPartsWithQty.SelectMany(p => p.GetIncludedPartsWithQty(identifier)));

        return listOfParts;
    }

    public virtual CabinPart GetDeepClone()
    {
        var clone = (CabinPart)this.MemberwiseClone();
        clone.AdditionalParts = AdditionalParts.Select(p => p.GetDeepClone()).ToList();
        return clone;
    }

    //public bool Equals(CabinPart other)
    //{
    //    if (Code == other.Code && AdditionalParts.SequenceEqual(other.AdditionalParts))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //public override bool Equals(object obj)
    //{
    //    if (obj == null) return false;
    //    if (obj.GetType() != this.GetType()) return false;
    //    return Equals((CabinPart)obj);
    //}

    //public override int GetHashCode()
    //{
    //    unchecked // OverFlow is fine , just Wrap
    //    {
    //        int hash = 17;
    //        hash = hash * 23 + Code.GetHashCode();
    //        foreach (var part in AdditionalParts)
    //        {
    //            hash = hash * 23 + part.GetHashCode();
    //        }
    //        return hash;
    //    }
    //}
}
