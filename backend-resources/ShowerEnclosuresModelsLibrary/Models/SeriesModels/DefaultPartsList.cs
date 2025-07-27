using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels
{
    public class DefaultPartsList : IDeepClonable<DefaultPartsList>
    {
#nullable enable
        public Dictionary<PartSpot, PartSpotDefaults> SpotDefaults { get; set; } = new();
        public List<GenericPartDefaults> GenericParts { get; set; } = new();

        /// <summary>
        /// The List of Part Sets Applied for this Default List
        /// </summary>
        public List<PartSet> ConnectedParts { get; set; } = new();

        public DefaultPartsList()
        {

        }

        /// <summary>
        /// Returns a Spots Default Quantity or zero if the spot is Not Found
        /// </summary>
        /// <param name="spot">The Spot of which we need the default Quantity</param>
        /// <returns>The Default Quantity</returns>
        public double GetSpotDefaultQuantity(PartSpot spot)
        {
            if(SpotDefaults.TryGetValue(spot, out PartSpotDefaults? defaults) && defaults is not null)
            {
                return defaults.DefaultQuantity;
            }
            return 0;
        }

        /// <summary>
        /// Returns all the Codes invloved in this Default List in a Single Collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetUsedCodes()
        {
            return GenericParts.Select(p=> p.PartCode)
                .Concat(SpotDefaults.Values.SelectMany(d => d.ValidCodes))
                .Concat(ConnectedParts.SelectMany(ps=>ps.GetUsedCodes()))
                .Distinct();
        }

        /// <summary>
        /// Returns wheather a certain Spot is Valid for these Defaults
        /// </summary>
        /// <param name="spot">The Spot to Check</param>
        /// <returns></returns>
        public bool IsValidSpot(PartSpot spot)
        {
            return SpotDefaults.ContainsKey(spot);
        }

        /// <summary>
        /// Replaces an Existing Code with a New one . (Only if the Code is present otherwise does nothing)
        /// </summary>
        /// <param name="codeToReplace">The Code to Replace</param>
        /// <param name="newCode">The new Code</param>
        /// <returns>True if something was replaced false if nothing was replaced</returns>
        public bool ReplaceCodeWithNew(string codeToReplace, CabinPart newPart)
        {
            bool hasReplaced = false;
            foreach (var spotDefault in SpotDefaults.Values)
            {
                if(spotDefault.ReplaceCodeWithNew(codeToReplace, newPart.Code)) hasReplaced = true;
            }
            foreach (var part in GenericParts)
            {
                if (part.PartCode == codeToReplace)
                {
                    part.PartCode = newPart.Code;
                    hasReplaced = true; 
                }
            }
            foreach (var connectedSet in ConnectedParts)
            {
                if (connectedSet.ReplaceCodeIfExists(codeToReplace, newPart.Code)) 
                {
                    hasReplaced = true;
                };
            }
            return hasReplaced;
        }

        //*****************************************************************************************************
        //**********MAYBE THESE SHOULD GO ON A STATIC METHOD AND ACCEPT A DEFAULT LIST ALSO AS ARGUMENT********
        /// <summary>
        /// Throws not Supported Exception when the Spot is not Valid for this List
        /// </summary>
        /// <param name="spot">The Spot to Check for Validity</param>
        /// <exception cref="NotSupportedException">When not Valid Spot</exception>
        protected void ThrowIfNotSupportedSpot(PartSpot spot)
        {
            //Throw if defaults do not contain the requested Spot
            if (!SpotDefaults.ContainsKey(spot))
                throw new NotSupportedException($"These Defaults do not Support Spot:{spot}");
        }

        /// <summary>
        /// Throws an Exception if the Spot is not Supported , or if the Code of the Part is Not Valid for the Selected Spot
        /// Always checks first if the Spot is Valid
        /// </summary>
        /// <param name="spot">The Spot of the Part</param>
        /// <param name="code">The Code of the Part</param>
        /// <exception cref="NotSupportedException">When Not Supported Spot or Code</exception>
        protected void ThrowIfNotSupportedSpotOrCode(PartSpot spot, string code)
        {
            ThrowIfNotSupportedSpot(spot);
            var spotDefaults = SpotDefaults[spot];

            if (!spotDefaults.IsPartCodeValid(code))
            {
                StringBuilder builder = new();
                builder
                    .Append(this.GetType().Name)
                    .Append(" does not Support ")
                    .Append("Part:").Append(string.IsNullOrWhiteSpace(code) ? "--Empty--" : code)
                    .Append(" in Spot ").Append(spot)
                    .Append(Environment.NewLine)
                    .Append("Valid Values where :")
                    .Append(Environment.NewLine)
                    .Append(string.Join(Environment.NewLine, spotDefaults.ValidCodes));
                if (spotDefaults.IsOptional) builder.Append("or EMPTY CODE as this Spot is Optional");

                throw new NotSupportedException(builder.ToString());
            }
        }
        //**********MAYBE THESE SHOULD GO ON A STATIC METHOD AND ACCEPT A DEFAULT LIST ALSO AS ARGUMENT********
        //*****************************************************************************************************

        /// <summary>
        /// Returns a DeepClone for this Default List
        /// </summary>
        /// <returns></returns>
        public DefaultPartsList GetDeepClone()
        {
            DefaultPartsList list = new();
            foreach (var kvp in SpotDefaults)
            {
                list.SpotDefaults.Add(kvp.Key, kvp.Value.GetDeepClone());
            }
            list.GenericParts = new(this.GenericParts.Select(d=>d.GetDeepClone()));
            list.ConnectedParts = new(ConnectedParts.Select(c => c.GetDeepClone()));
            return list;
        }
    }

    /// <summary>
    /// Constains the Defaults for a Certain Part Spot
    /// Default Part Code as Well As The Valid Codes for this Spot
    /// </summary>
    public class PartSpotDefaults : IDeepClonable<PartSpotDefaults>
    {
        /// <summary>
        /// From the Provided Parts List , Return only the Parts that are Valid for the Selected Spot
        /// </summary>
        /// <param name="spot">The Spot for Which to Pick the Parts</param>
        /// <param name="parts">The Parts</param>
        /// <returns>The Filtered List</returns>
        public static IEnumerable<CabinPart> FilterOnlyValidParts(PartSpot spot, IEnumerable<CabinPart> parts)
        {
            return spot switch
            {
                PartSpot.Handle1 or PartSpot.Handle2 =>
                parts.Where(p => p is CabinHandle)
                     .OrderBy(p => p.Code),

                PartSpot.HorizontalGuideTop or PartSpot.HorizontalGuideBottom =>
                parts.Where(p => p is Profile profile && profile.ProfileType is CabinProfileType.GuideProfile or CabinProfileType.HorizontalBarProfile)
                     .OrderBy(p => p.Code),

                PartSpot.WallSide1 or PartSpot.WallSide2 =>
                parts.Where(p => (p is Profile profile && (profile.ProfileType is CabinProfileType.WallProfile or CabinProfileType.GlassProfile or CabinProfileType.ConnectorProfile)
                                                       || (p.Part == CabinPartType.SmallSupport)))
                     .OrderBy(p => p.Code),

                PartSpot.NonWallSide =>
                parts.Where(p => p.Part == CabinPartType.SmallSupport
                                        || (p is Profile profile && (profile.ProfileType is CabinProfileType.ConnectorProfile or CabinProfileType.WallProfile or CabinProfileType.BottomGlassProfile or CabinProfileType.GlassProfile)))
                     .OrderBy(p => p.Code),

                PartSpot.TopSide =>
                parts.Where(p => p is Profile profile && profile.ProfileType is CabinProfileType.WallProfile or CabinProfileType.BottomGlassProfile or CabinProfileType.GlassProfile)
                     .OrderBy(p => p.Code),

                PartSpot.BottomSide1 or PartSpot.BottomSide2 =>
                parts.Where(p => (p is Profile profile && profile.ProfileType is CabinProfileType.WallProfile or CabinProfileType.BottomGlassProfile or CabinProfileType.GlassProfile)
                              || (p is CabinSupport))
                     .OrderBy(p => p.Code),

                PartSpot.StepBottomSide =>
                parts.Where(p => p is Profile profile && profile.ProfileType is CabinProfileType.BottomGlassProfile or CabinProfileType.WallProfile)
                     .OrderBy(p => p.Code),

                PartSpot.WallHinge => parts.Where(p => p is HingeDB or ProfileHinge).OrderBy(p => p.Code),

                PartSpot.MiddleHinge => parts.Where(p => p is GlassToGlassHinge or ProfileHinge).OrderBy(p => p.Code),

                PartSpot.PivotHinge => parts.Where(p => p is Hinge9B).OrderBy(p => p.Code),

                PartSpot.CloseProfile =>
                parts.Where(p => p is Profile profile && profile.ProfileType == CabinProfileType.MagnetProfile)
                     .OrderBy(p => p.Code),

                PartSpot.CloseStrip => parts.Where(p => p is GlassStrip).OrderBy(p => p.Code),

                PartSpot.SupportBar => parts.Where(p => p is SupportBar).OrderBy(p => p.Code),

                PartSpot.Angle => parts.Where(p => p.Part == CabinPartType.AnglePart).OrderBy(p => p.Code),

                PartSpot.FixedGlass1SideSealer 
                or PartSpot.FixedGlass1BottomSealer
                or PartSpot.FixedGlass2SideSealer
                or PartSpot.FixedGlass2BottomSealer
                or PartSpot.DoorGlass1SideSealer
                or PartSpot.DoorGlass1SideSealer
                or PartSpot.DoorGlass1BottomSealer
                or PartSpot.DoorGlass2SideSealer
                or PartSpot.DoorGlass2SideSealer
                or PartSpot.DoorGlass2BottomSealer => 
                parts.Where(p=> p.Part == CabinPartType.Strip || (p is Profile prof && prof.ProfileType == CabinProfileType.WaterSealerProfile)).OrderBy(p=>p.Code),
                
                PartSpot.Door1FrontSealer 
                or PartSpot.Door2FrontSealer => 
                parts.OfType<Profile>().Where(p=> p.ProfileType == CabinProfileType.WaterSealerProfile).OrderBy(p=>p.Code),

                PartSpot.HorizontalGuideSealerTop1
                or PartSpot.HorizontalGuideSealerTop2
                or PartSpot.HorizontalGuideSealerBottom1
                or PartSpot.HorizontalGuideSealerBottom2 => 
                parts.OfType<GlassStrip>().Where(s=> s.StripType == CabinStripType.GenericPolycarbonic).OrderBy(p=>p.Code),
                PartSpot.InternalProfileWall1 
                or PartSpot.InternalProfileWall2 => 
                parts.OfType<Profile>().Where(p=> p.ProfileType == CabinProfileType.GlassProfile),
                
                PartSpot.TopWheel1 or PartSpot.TopWheel2 or PartSpot.BottomWheel1 or PartSpot.BottomWheel2 =>
                parts.Where(p=> p.Part == CabinPartType.Wheel),

                _ => parts.OrderBy(p => p.Code),
            };
        }

        public PartSpot Spot { get; set; }

        /// <summary>
        /// The Valid Part Codes for a particular Spot , If the Default Code is not empty it is also included in this List
        /// </summary>
        public List<string> ValidCodes { get; set; } = new();
        /// <summary>
        /// The Default Part Code for a Particular Spot
        /// </summary>
        public string DefaultCode { get; set; } = string.Empty;
        /// <summary>
        /// The Default Quantity of the Involved Part
        /// </summary>
        public double DefaultQuantity { get; set; } = 1;

        /// <summary>
        /// Weather this is an Optional Spot for the List it exists in
        /// </summary>
        public bool IsOptional { get; set; }

        public PartSpotDefaults()
        {

        }
        /// <summary>
        /// Creates Defaults for a Specific Spot with a Default Code and optional Selectable Values
        /// </summary>
        /// <param name="spot">The Spot these default are for</param>
        /// <param name="isOptional">Weather this is an Optional Spot</param>
        /// <param name="defaultCode">The Default Code of the Part for this Spot , when no Part is Default an EmptyString </param>
        /// <param name="validCodes">if omitted only the default Code will be Added</param>
        public PartSpotDefaults(PartSpot spot, bool isOptional, string defaultCode,double defaultQuantity,params string[] validCodes)
        {
            Spot = spot;
            DefaultCode = defaultCode;
            DefaultQuantity = defaultQuantity;
            IsOptional = isOptional;
            ValidCodes = string.IsNullOrWhiteSpace(defaultCode) ? validCodes.Distinct().ToList() : validCodes.Append(defaultCode).Distinct().ToList();
        }

        /// <summary>
        /// Creates Defaults for a Specific Spot with a Default Code and optional Selectable Values
        /// </summary>
        /// <param name="spot">The Spot these default are for</param>
        /// <param name="isOptional">Weather this is an Optional Spot</param>
        /// <param name="defaultCode">The Default Code of the Part for this Spot , when no Part is Default an EmptyString </param>
        /// <param name="validCodes">if omitted only the default code will be added</param>
        public PartSpotDefaults(PartSpot spot,bool isOptional, string defaultCode,double defaultQuantity,IEnumerable<string> validCodes)
        {
            Spot = spot;
            IsOptional = isOptional;
            DefaultCode = defaultCode;
            DefaultQuantity = defaultQuantity;
            ValidCodes = string.IsNullOrWhiteSpace(defaultCode) ? validCodes.Distinct().ToList() : validCodes.Append(defaultCode).Distinct().ToList();
        }

        /// <summary>
        /// Creates a Copy of the specified Defaults
        /// </summary>
        /// <param name="spotDefaults">The Defaults to copy</param>
        public PartSpotDefaults(PartSpotDefaults spotDefaults)
        {
            DefaultCode = spotDefaults.DefaultCode;
            DefaultQuantity = spotDefaults.DefaultQuantity;
            IsOptional= spotDefaults.IsOptional;
            ValidCodes= spotDefaults.ValidCodes.Any() ? new(spotDefaults.ValidCodes) : new();//copies it to a new list all items inside are strings so the new copy is deep
            Spot = spotDefaults.Spot;
        }

        /// <summary>
        /// Weather this partCode is acceptable for this Spot
        /// Will return false if the code is not included in the Valid Codes for the Spot. or if the Code is empty and the Spot is not Optional
        /// </summary>
        /// <param name="partCode">The part of the Code or Empty Indicating no part at this spot</param>
        /// <returns></returns>
        public bool IsPartCodeValid(string partCode)
        {
            if (ValidCodes.Any(c=> c == partCode))
            {
                //Return that code is Valid when it exists within the Valid Codes
                return true;
            }
            else
            {
                //If it is not included in the Valid Codes , return that is ok only if its optional and the part code is empty (meaning no selection)
                return (IsOptional && string.IsNullOrEmpty(partCode));
            }
        }

        /// <summary>
        /// Returns a Clone of this SpotDefaults
        /// </summary>
        /// <returns></returns>
        public PartSpotDefaults GetDeepClone()
        {
            return new PartSpotDefaults(this);
        }

        /// <summary>
        /// Replaces an Existing Code with a New one . (Only if the Code is present otherwise does nothing)
        /// </summary>
        /// <param name="codeToReplace">The Code to Replace</param>
        /// <param name="newCode">The new Code</param>
        /// <returns>True if something was replaced false if nothing was replaced</returns>
        public bool ReplaceCodeWithNew(string codeToReplace , string newCode)
        {
            bool hasReplaced = false;
            if (ValidCodes.Any(c=> c == codeToReplace))
            {
                int index = ValidCodes.IndexOf(codeToReplace);
                ValidCodes[index] = newCode;
                hasReplaced = true;
            }
            if (DefaultCode == codeToReplace)
            {
                DefaultCode = newCode;
                hasReplaced = true;
            }
            return hasReplaced;
        }
    }

    /// <summary>
    /// A Set of Parts that Change together when certain Conditions are met
    /// Conditions : Set IsApplied = true and Identifier Exists in ValidIdentifiers of Set
    /// </summary>
    public class PartSet : IDeepClonable<PartSet>
    {
        public string SetName { get; set; } = "????";

        /// <summary>
        /// A Dictionary of Positions matching to Part Codes . When one of the Codes is Used
        /// The Rest Spots should also change at the Cabins matching this Default List
        /// </summary>
        public Dictionary<PartSpot, string> SelectionSet { get; set; } = new();

        /// <summary>
        /// Weather this Set Application Should be Applied or Ignored
        /// </summary>
        public bool IsApplied { get; set; }
        /// <summary>
        /// Weather application of this set should be Ignored
        /// </summary>
        public bool IsIgnored { get => !IsApplied; }

        /// <summary>
        /// Weather this Part Set includes the designated Part in the designated Spot
        /// </summary>
        /// <param name="spot">The Spot to check</param>
        /// <param name="partCode">The Part Code to find</param>
        /// <returns>True if the Part is included in this Part Set , otherwise false</returns>
        public bool IncludesPartInSpot(PartSpot spot, string partCode)
        {
            return SelectionSet.TryGetValue(spot, out string? code) && code == partCode;
        }
        /// <summary>
        /// Weather this Parts Set Includes a Part that has the Specified Code
        /// </summary>
        /// <param name="partCode">The Code to Search</param>
        /// <returns>True if it does , otherwise false</returns>
        public bool IncludesPart(string partCode)
        {
            return SelectionSet.Values.Any(code => code == partCode);
        }

        /// <summary>
        /// Replaces all Occurances of a given code with a new one
        /// </summary>
        /// <param name="codeToReplace">The Code to replace</param>
        /// <param name="newCode">The new Code</param>
        /// <returns></returns>
        public bool ReplaceCodeIfExists(string codeToReplace,string newCode)
        {
            bool hasReplaced = false;
            if (this.IncludesPart(codeToReplace))
            {
                foreach (var spot in SelectionSet.Keys)
                {
                    if (SelectionSet[spot] == codeToReplace)
                    {
                        SelectionSet[spot] = newCode;
                        hasReplaced = true;
                    }
                }
            }
            return hasReplaced;
        }

        /// <summary>
        /// Returns all the Codes being used by this Part Set
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetUsedCodes()
        {
            return SelectionSet.Values;
        }

        public PartSet GetDeepClone()
        {
            PartSet clone = new()
            {
                SelectionSet = new(this.SelectionSet),
                IsApplied = this.IsApplied,
                SetName = this.SetName
            };
            return clone;
        }
    }

    /// <summary>
    /// A Small Object containing the Default Quantity and Code of a Generic Part
    /// </summary>
    public class GenericPartDefaults : IDeepClonable<GenericPartDefaults>
    {
        public string PartCode { get; set; } = string.Empty;
        public double PartQuantity { get; set; } = 1;

        public GenericPartDefaults()
        {
            
        }
        public GenericPartDefaults(string partCode , double partQuantity)
        {
            this.PartCode = partCode;
            this.PartQuantity = partQuantity;
        }

        public GenericPartDefaults GetDeepClone()
        {
            return new(PartCode,PartQuantity);
        }
    }

}
