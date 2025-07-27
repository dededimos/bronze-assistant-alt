using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
/// <summary>
/// This Class should be deprecated and integrate into Cabin Supports , it is of no Use Alone 
/// </summary>
public class FloorStopperW : CabinSupport
{
    public override CabinPartType Part { get => CabinPartType.FloorStopperW; }

    /// <summary>
    /// The Length view that is outside of the Glass
    /// THIS PROPERTY MUST CHANGE TO BE USED IN ALL SUPPORTS
    /// </summary>
    public int OutOfGlassLength { get; set; }
    
    public override FloorStopperW GetDeepClone()
    {
        return (FloorStopperW)base.GetDeepClone();
    }

    //public override int GetHashCode()
    //{
    //    return HashCode.Combine(base.GetHashCode(), OutOfGlassLength);
    //}

    //public override bool Equals(object obj)
    //{
    //    if (obj is FloorStopperW otherSupport)
    //    {
    //        return base.Equals(otherSupport) &&
    //            OutOfGlassLength == otherSupport.OutOfGlassLength;
    //    }
    //    return false;
    //}
}
