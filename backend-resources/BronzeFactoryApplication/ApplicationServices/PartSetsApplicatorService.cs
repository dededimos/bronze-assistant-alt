using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices;

public class PartSetsApplicatorService
{
    private readonly ICabinMemoryRepository repo;
    private readonly ILogger<PartSetsApplicatorService> logger;

    /// <summary>
    /// The Sets must each contain Unique parts Per Cabin Identifier. Otherwise only the first will be used
    /// </summary>
    public Dictionary<CabinIdentifier, List<PartSet>> Sets { get; set; } = new();

    public PartSetsApplicatorService(ICabinMemoryRepository repo , ILogger<PartSetsApplicatorService> logger)
    {
        this.repo = repo;
        this.logger = logger;
    }

    public void InitilizeService()
    {
        Sets = new();
        foreach (var defaultList in repo.DefaultLists)
        {
            var identifier = new CabinIdentifier(defaultList.Key.Item1,defaultList.Key.Item2,defaultList.Key.Item3);
            Sets.Add(identifier, defaultList.Value.ConnectedParts);
        }
        logger.LogInformation("{applicator} has Initilized...", nameof(PartSetsApplicatorService));
    }

    /// <summary>
    /// Changes the Parts of a Parts List according to the Found set matching the Spot and Applied Code
    /// </summary>
    /// <param name="partsList">The Parts List where the change happens</param>
    /// <param name="spotChanged">The Spot the Triggered the Change</param>
    /// <param name="partCodeApplied">The New Code that has already applied to the Spot of the Parts List</param>
    /// <returns>Returns the Applied Set or null if no Set was Applied</returns>
    public IEnumerable<PartSpot> TryApplySetChange(CabinPartsList partsList, PartSpot spotChanged, string partCodeApplied, CabinIdentifier identifier)
    {
        //If the Parts list does not have such a spot do not change anything 
        if (partsList.HasSpot(spotChanged))
        {
            // Find the Set where the Changed Part Resides in 
            var setToApply = FindSetWithPartInSpot(spotChanged, partCodeApplied, identifier);
            //If the Set is found Check all the Spots of the Set and for each one existing in the parts list change the current part => to the set Part
            //Omit the already changed part and parts that are already set in the Spot
            if (setToApply is not null)
            {
                List<PartSpot> spotsChanged = new();
                foreach (var spot in setToApply.SelectionSet.Keys)
                {
                    if (spot != spotChanged 
                     && partsList.HasSpot(spot)
                     && (partsList.GetPartOrNull(spot)?.Code != setToApply.SelectionSet[spot]))
                    {
                        //If the Spot has an Empty Code then set it to empty
                        var newPartToSet = string.IsNullOrWhiteSpace(setToApply.SelectionSet[spot]) 
                            ? null 
                            : repo.GetPart(setToApply.SelectionSet[spot],identifier);
                        partsList.SetPart(spot, newPartToSet );
                        spotsChanged.Add(spot);
                    }
                }
                logger.LogInformation("PartSet has been Applied to : {spots} , FOR : {identifier}",
                    string.Join(',',spotsChanged.Select(spot=>spot.ToString())),
                    identifier.ToString());
                //Return that a spots that parts changed
                return spotsChanged;
            }
        }
        // return that no spots have changed
        logger.LogInformation("No Part Set Was Found to Apply");
        return Enumerable.Empty<PartSpot>();
    }

    /// <summary>
    /// Finds the Set that contains the designated part in the designated Spot
    /// </summary>
    /// <param name="spot">The Spot of the Part</param>
    /// <param name="partCode">The parts code</param>
    /// <returns>The Set or null if there is no set</returns>
    private PartSet? FindSetWithPartInSpot(PartSpot spot, string partCode, CabinIdentifier identifier)
    {
        //Find the Sets matching the Identifier and return the first set that matches the Spot,Code combination
        if(Sets.TryGetValue(identifier, out var partSets))
        {
            return partSets.Where(s => s.IsApplied)
            .FirstOrDefault(s => s.IncludesPartInSpot(spot, partCode));
        }
        return null;
    }

    /// <summary>
    /// If the Applicator Should Apply a Set Change
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="partCodeChanged"></param>
    /// <returns></returns>
    public bool ShouldApplySetChange(CabinIdentifier identifier , string partCodeChanged)
    {
        if (Sets.ContainsKey(identifier))
        {
            if (Sets[identifier].Any(ps=> ps.IncludesPart(partCodeChanged)))
            {
                return true;
            }
        }
        return false;
    }
}
