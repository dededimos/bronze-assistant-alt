using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels;
public class CabinPartsList : ICabinParts , IDeepClonable<CabinPartsList>
{
#nullable enable

    /// <summary>
    /// The Generica Parts of a Cabin (Those that are of no importance in Calculations)
    /// </summary>
    public List<CabinPart> GenericParts { get; set; } = new();

    protected Dictionary<PartSpot, CabinPart?> positionedParts = new();
    public Dictionary<PartSpot , CabinPart?> GetPositionedParts()
    {
        return new(positionedParts);
    }

    public bool HasSpot(PartSpot spot)
    {
        return positionedParts.ContainsKey(spot);
    }

    /// <summary>
    /// Returns all the Parts of the First Level (Without including any nested Parts) and Without Cloning
    /// Parts with zero Quantity ARE NOT RETURNED
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CabinPart> GetCabinPartsNotNested()
    {
        List<CabinPart> parts = new();
        parts.AddRange(positionedParts.Values.Where(v => v is not null)!); //They are never null
        parts.AddRange(GenericParts);
        return parts.Where(p => p.Quantity > 0);
    }
    /// <summary>
    /// Returns Clones of ALL Parts including Nested Parts and Appropriate Quantities for the specified Structure
    /// Parts with zero Quantity ARE NOT RETURNED
    /// </summary>
    /// <param name="identifier">The Structures Identifier</param>
    /// <returns></returns>
    public IEnumerable<CabinPart> GetCabinPartsNested(CabinIdentifier identifier)
    {
        List<CabinPart> parts = new();
        //zero qty parts are not returned
        var notNestedParts = GetCabinPartsNotNested().Select(p=>p.GetDeepClone());
        parts.AddRange(notNestedParts);
        foreach (var part in notNestedParts)
        {
            //zero qty parts are not returned
            parts.AddRange(part.GetIncludedPartsWithQty(identifier));
        }
        return parts;
    }

    /// <summary>
    /// Returns a Part or Null , If the Spot does not Exist , if the Spot exists but is Empty , if the returned part is not of the requested Type
    /// </summary>
    /// <typeparam name="T">The Part of the Type</typeparam>
    /// <param name="spot">The Spot of the Part</param>
    /// <returns></returns>
    public T? GetPartOrNull<T>(PartSpot spot) where T : CabinPart
    {
        positionedParts.TryGetValue(spot, out CabinPart? part);
        return part as T;
    }

    /// <summary>
    /// Returns a Part or Null if the Spot does not Exist or if the Spot exists but is Empty
    /// </summary>
    /// <typeparam name="T">The Part of the Type</typeparam>
    /// <param name="spot">The Spot of the Part</param>
    /// <returns></returns>
    public CabinPart? GetPartOrNull(PartSpot spot)
    {
        positionedParts.TryGetValue(spot, out CabinPart? part);
        return part;
    }

    /// <summary>
    /// Set a spot with a specific Part
    /// If the spot is not Valid or If the Part is not Valid this will throw an Exception
    /// </summary>
    /// <param name="spot">The Spot</param>
    /// <param name="part">The Part to place to the Spot</param>
    /// <exception cref="NotSupportedException">When Not Supported Spot or Code</exception>
    /// <returns>The same List for further Sets</returns>
    public CabinPartsList SetPart(PartSpot spot , CabinPart? part)
    {
        if (!HasSpot(spot)) throw new ArgumentException($"Spot Does not Exists for this PartsList {this.GetType().Name}", nameof(spot));
        positionedParts[spot] = part;
        return this;
    }

    /// <summary>
    /// Initilizes the Lists of Positioned and Generic Parts
    /// </summary>
    /// <param name="positionedParts">The Positioned Parts</param>
    /// <param name="genericParts">The Generic Parts</param>
    public void InitilizeParts(Dictionary<PartSpot,CabinPart?> positionedParts,IEnumerable<CabinPart> genericParts)
    {
        //Those Enumerables have to be made like this so that the parts are cloned and are not given around with the same reference
        foreach (var kvp in positionedParts)
        {
            this.positionedParts.Add(kvp.Key, kvp.Value?.GetDeepClone() ?? null);
        }
        foreach (var part in genericParts)
        {
            this.GenericParts.Add(part.GetDeepClone());
        }
    }

    /// <summary>
    /// Returns a DeepClone of the PartsList
    /// </summary>
    /// <returns></returns>
    public virtual CabinPartsList GetDeepClone()
    {
        if (Activator.CreateInstance(this.GetType()) is not CabinPartsList list)
        {
            throw new InvalidOperationException("Unexpected Error , Instance Activator could not Resolve Instance");
        }
        list.InitilizeParts(this.positionedParts, this.GenericParts);
        return list;
    }

    public static CabinPartsList Empty()
    {
        return new CabinPartsList();
    }
}

public enum PartSpot
{
    //DO NOT CHANGE NUMBERING or Text -- ITS STORED IN DATABASE
    Undefined = 0,
    Handle1 = 1,
    Handle2 = 2,
    HorizontalGuideTop = 3,
    HorizontalGuideBottom = 4,
    WallSide1 = 5,
    WallSide2 = 6,
    NonWallSide = 7,
    TopSide = 8,
    BottomSide1 = 9,
    BottomSide2 = 10,
    StepBottomSide = 11,
    WallHinge = 12,
    MiddleHinge = 13,
    PivotHinge = 14,
    CloseProfile = 15,
    CloseStrip = 16,
    SupportBar = 17,
    Angle = 18,

    FixedGlass1SideSealer = 19,
    FixedGlass1BottomSealer = 20,
    FixedGlass2SideSealer = 21,
    FixedGlass2BottomSealer = 22,

    DoorGlass1SideSealer = 23,
    DoorGlass1SideSealerAlt = 24,

    DoorGlass1BottomSealer = 25,

    DoorGlass2SideSealer = 26,
    DoorGlass2SideSealerAlt = 27,

    DoorGlass2BottomSealer = 28,

    Door1FrontSealer = 29,
    Door2FrontSealer = 30,

    HorizontalGuideSealerTop1 = 31,
    HorizontalGuideSealerTop2 = 32,
    HorizontalGuideSealerBottom1 = 33,
    HorizontalGuideSealerBottom2 = 34,

    HorizontalGuideStopper1 = 35,
    HorizontalGuideStopper2 = 36,
    HorizontalGuideStopper3 = 37,
    HorizontalGuideStopper4 = 38,

    TopWheel1 = 39,
    TopWheel2 = 40,
    BottomWheel1 = 41,
    BottomWheel2 = 42,

    DoorStoppersTop1 = 43,
    DoorStoppersTop2 = 44,
    DoorStoppersBottom1 = 45,
    DoorStoppersBottom2 = 46,

    InternalProfileWall1 = 47,
    InternalProfileWall1Alt = 48,
    InternalProfileWall2 = 49,
    InternalProfileWall2Alt = 50,
}

