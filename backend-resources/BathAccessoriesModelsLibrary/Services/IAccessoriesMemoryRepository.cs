using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using CommonHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonHelpers.CommonExtensions;

namespace BathAccessoriesModelsLibrary.Services
{
    /// <summary>
    /// A repository that resides in Memory
    /// </summary>
    public interface IAccessoriesMemoryRepository
    {
        public const string IndexingAccessories = "IndexingAccessories";

        public List<BathroomAccessory> Accessories { get; }
        Dictionary<TypeOfTrait, List<AccessoryTrait>> TraitsByType { get; }
        UserAccessoriesOptions AccessoriesOptions { get; }
        List<UserAccessoriesOptions> AllAccessoriesOptions { get; }
        public bool HasStockInfo { get; }

        /// <summary>
        /// Builds the Repository for a normal or Power User
        /// </summary>
        /// <param name="stash"></param>
        /// <param name="isPowerUsersRepo"></param>
        /// <returns></returns>
        Task BuildRepositoryAsync(AccessoriesDtoStash stash, bool isPowerUsersRepo);

        /// <summary>
        /// Search within the Index of the Repository
        /// </summary>
        /// <param name="searchTerm">The Search Term</param>
        /// <returns>The Items found from the Search</returns>
        Task<IEnumerable<IndexedItem>> SearchIndexAsync(string searchTerm);
        /// <summary>
        /// Search Within the Accessories Codes Index
        /// </summary>
        /// <param name="searchTerm">The Search Term</param>
        /// <returns>The Items found from the Search</returns>
        Task<IEnumerable<BathroomAccessory>> SearchAccessoriesCodesAsync(string searchTerm);

        /// <summary>
        /// An even informing about buiilding phases of the Repository
        /// </summary>
        public event EventHandler<string>? OnRepositoryBuilding;
        public event EventHandler? OnRepositoryCreated;
        public bool IsBuilt { get; }

        /// <summary>
        /// Returns the Name of the TraitClass
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTraitClassName(TypeOfTrait type);
        /// <summary>
        /// Returns the Photo Url of a trait Class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTraitClassPhotoUrl(TypeOfTrait type);
        /// <summary>
        /// Returns a Trait Class based on the Provided Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        AccessoryTraitClass GetTraitClass(TypeOfTrait type);
        /// <summary>
        /// Returns the List of Traits for a Certain Type or Empty
        /// </summary>
        /// <param name="type">The Type of Trait</param>
        /// <returns>The List of Traits</returns>
        List<AccessoryTrait> GetTraitsByType(TypeOfTrait type);
        /// <summary>
        /// Returns the List of Accessories for a Certain Series
        /// </summary>
        /// <param name="seriesCode"></param>
        /// <returns></returns>
        List<BathroomAccessory> GetAccessoriesBySeries(string seriesCode);
        /// <summary>
        /// Returns the List of Accessories for a certain Primary Type
        /// </summary>
        /// <param name="primaryTypeCode"></param>
        /// <returns></returns>
        List<BathroomAccessory> GetAccessoriesByPrimaryType(string primaryTypeCode);
        /// <summary>
        /// Returns the List of Accessories for a certain Finish
        /// </summary>
        /// <param name="finishCode"></param>
        /// <returns></returns>
        List<BathroomAccessory> GetAccessoriesByFinish(string finishCode);
        /// <summary>
        /// Gets an Accessory by its General Code
        /// </summary>
        /// <param name="generalCode">The General Code of the Accessory</param>
        /// <returns></returns>
        BathroomAccessory? GetAccessoryByCode(string generalCode);
        /// <summary>
        /// Gets an Accessory by its Id
        /// </summary>
        /// <param name="id">The id of the Accessory</param>
        /// <returns></returns>
        BathroomAccessory? GetAccessoryById(string id);
        /// <summary>
        /// Get A Trait by its Id
        /// </summary>
        /// <param name="id">The Id of the Trait</param>
        /// <returns></returns>
        AccessoryTrait? GetTraitById(string id);
        /// <summary>
        /// Returns the Stock of an Accessory by its Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        decimal GetAccessoryStock(string code);
    }

    public class AccessoriesMemoryRepository : IAccessoriesMemoryRepository
    {
        private Dictionary<string, IndexedItem> searchIndex = [];
        public Dictionary<string, BathroomAccessory> AccessoriesByCode { get; private set; } = [];
        public Dictionary<string, BathroomAccessory> AccessoriesById { get; private set; } = [];
        public Dictionary<string, List<BathroomAccessory>> AccessoriesBySeriesCode { get; private set; } = [];
        public Dictionary<string, List<BathroomAccessory>> AccessoriesByPrimaryTypeCode { get; private set; } = [];
        public Dictionary<string, List<BathroomAccessory>> AccessoriesByFinishCode { get; private set; } = [];
        public List<BathroomAccessory> Accessories { get; private set; } = [];
        public Dictionary<string, AccessoryTrait> TraitsById { get; private set; } = [];
        public Dictionary<TypeOfTrait, List<AccessoryTrait>> TraitsByType { get; private set; } = [];
        public Dictionary<TypeOfTrait, AccessoryTraitClass> TraitClassesByType { get; private set; } = [];
        public List<AccessoryTraitClass> TraitClasses { get; private set; } = [];
        public Dictionary<string, AccessoryTraitGroup> TraitGroupsById { get; private set; } = [];
        public List<AccessoryTraitGroup> TraitGroups { get; private set; } = [];
        public Dictionary<string, decimal> StockInfo { get; private set; } = [];
        public bool HasStockInfo => StockInfo.Count != 0;
        public UserAccessoriesOptions AccessoriesOptions { get => User.AccessoriesOptions; }
        public List<UserAccessoriesOptions> AllAccessoriesOptions { get; private set; } = [];
        public UserInfo User { get; set; } = UserInfo.Undefined();
        /// <summary>
        /// Weather the Repository has been Built
        /// </summary>
        public bool IsBuilt { get; private set; }

        public event EventHandler<string>? OnRepositoryBuilding;
        public event EventHandler? OnRepositoryCreated;

        /// <summary>
        /// Builds the Repository from the Stash Object
        /// </summary>
        /// <param name="stash"></param>
        public async Task BuildRepositoryAsync(AccessoriesDtoStash stash, bool isPowerUsersRepo)
        {
            #region 1.Build TraitGroups
            TraitGroupsById = stash.TraitGroups.ToDictionary(tg => tg.Id);
            TraitGroups = stash.TraitGroups.OrderBy(tg => tg.SortNo).ToList();
            #endregion

            await Task.Delay(20);

            #region 2. Build Traits By Id
            TraitsById = stash.Traits.Select(dto => dto.ToAccessoryTraitWOSecondTypes(TraitGroupsById))
                                     .ToDictionary(t => t.Id);
            //Add any Secondary Types Now that the Dictionary is Made
            foreach (var trait in TraitsById.Values)
            {
                foreach (var secId in trait.SecondaryTypesIds)
                {
                    if (TraitsById.TryGetValue(secId, out var secondaryType))
                    {
                        trait.SecondaryTypes.Add(secondaryType);
                    }
                }
            }
            #endregion

            await Task.Delay(20);

            #region 3.Build Trait Classes
            // Add Classes
            TraitClassesByType = stash.TraitClasses.Select(tc => tc.ToAccessoryTraitClass(TraitsById)).ToDictionary(tc => tc.TraitType);

            //Build ordered List of Trait Classes
            TraitClasses = TraitClassesByType.Values.OrderBy(tc => tc.SortNo).ToList();
            #endregion

            #region 4.Build Traits By Type
            // Construct the Traits By Type List
            TraitsByType = TraitClassesByType.ToDictionary(
                tc => tc.Key,
                // The Trait Class => Its Traits Dictionary.Values => Ordered By Sort No
                tc => tc.Value.Traits.Values.OrderBy(trait => trait.SortNo).ToList()
                );
            #endregion

            await Task.Delay(20);

            #region 5.Build Accessories and insert Prices
            // Build The Accessories without their Variations
            AccessoriesById = stash.Accessories.ToDictionary(a => a.Id, a =>
            {
                //Get the Prices info for this Accessory and Transform it 
                var pricesInfo = stash.PricesInfo.TryGetValue(a.Id, out var prices) ? prices.Select(p => p.ToAccessoryPrice(TraitsById, TraitGroupsById)).ToList() : new();
                return a.ToBathroomAccessory(TraitsById, pricesInfo);
            });

            AccessoriesByCode = AccessoriesById.ToDictionary(a => a.Value.Code, a => a.Value);
            Accessories = AccessoriesById.Values
                .OrderBy(a => a.Series.SortNo)
                .ThenBy(a => a.PrimaryType.SortNo)
                .ThenBy(a => a.SecondaryType.SortNo)
                .ThenBy(a => a.SortNo)
                .ToList();
            #endregion

            await Task.Delay(20);

            #region 6.Build Accessories per Series/PrimaryType/Finish
            AccessoriesBySeriesCode = [];
            AccessoriesByPrimaryTypeCode = [];
            int count = Accessories.Count;
            double inc = 0;
            foreach (var a in Accessories)
            {
                inc++;
                if (inc % 7 == 0 || inc == count)
                {
                    OnRepositoryBuilding?.Invoke(this, $"{(count == 0 ? 0 : inc / count):0%}");
                    await Task.Delay(1);
                }
                #region Series
                var seriesCode = a.Series.Code;
                //if the series Code is not There add it
                if (!AccessoriesBySeriesCode.ContainsKey(seriesCode))
                {
                    AccessoriesBySeriesCode[seriesCode] = new List<BathroomAccessory>();
                }
                //Add the Accessory
                AccessoriesBySeriesCode[seriesCode].Add(a);

                //Check also Other Series
                foreach (var series in a.OtherSeries)
                {
                    string otherSeriesCode = series.Code;
                    //if the Series Code is not there add it
                    if (!AccessoriesBySeriesCode.ContainsKey(otherSeriesCode))
                    {
                        AccessoriesBySeriesCode[otherSeriesCode] = new List<BathroomAccessory>();
                    }
                    AccessoriesBySeriesCode[otherSeriesCode].Add(a);
                }
                //Sort all Accessories Lists by PrimaryType Otherwise they Delay if every time they are getting sorted
                foreach (var listKey in AccessoriesBySeriesCode.Keys)
                {
                    AccessoriesBySeriesCode[listKey] =
                        AccessoriesBySeriesCode[listKey]
                        .OrderBy(a => a.PrimaryType.SortNo)
                        .ThenBy(a => a.SecondaryType.SortNo)
                        .ThenBy(a => a.SortNo)
                        .ToList();
                }
                #endregion

                #region PrimaryType
                //if the primaryType Code is not There Add it
                var primaryTypeCode = a.PrimaryType.Code;
                if (!AccessoriesByPrimaryTypeCode.ContainsKey(primaryTypeCode))
                {
                    AccessoriesByPrimaryTypeCode[primaryTypeCode] = new List<BathroomAccessory>();
                }
                //Add the Accessory
                AccessoriesByPrimaryTypeCode[primaryTypeCode].Add(a);

                //Check also Other Primary Types
                foreach (var primaryType in a.OtherPrimaryTypes)
                {
                    string otherPrimaryTypeCode = primaryType.Code;
                    //if the Primary Type Code is not there add it
                    if (!AccessoriesByPrimaryTypeCode.ContainsKey(otherPrimaryTypeCode))
                    {
                        AccessoriesByPrimaryTypeCode[otherPrimaryTypeCode] = new List<BathroomAccessory>();
                    }
                    AccessoriesByPrimaryTypeCode[otherPrimaryTypeCode].Add(a);
                }
                //Sort all Accessories Lists by PrimaryType Otherwise they Delay if every time they are getting sorted
                foreach (var listKey in AccessoriesByPrimaryTypeCode.Keys)
                {
                    AccessoriesByPrimaryTypeCode[listKey] =
                        AccessoriesByPrimaryTypeCode[listKey]
                        .OrderBy(a => a.Series.SortNo)
                        .ThenBy(a => a.SecondaryType.SortNo)
                        .ThenBy(a => a.SortNo)
                        .ToList();
                }
                #endregion

                #region Finish
                //Check also Available Finishes
                foreach (var finish in a.AvailableFinishes)
                {
                    string finishCode = finish.Finish.Code;
                    //if the Primary Type Code is not there add it
                    if (!AccessoriesByFinishCode.ContainsKey(finishCode))
                    {
                        AccessoriesByFinishCode[finishCode] = new List<BathroomAccessory>();
                    }
                    AccessoriesByFinishCode[finishCode].Add(a);
                }
                //Sort all Accessories Lists by Finish Otherwise they Delay if every time they are getting sorted
                foreach (var listKey in AccessoriesByFinishCode.Keys)
                {
                    AccessoriesByFinishCode[listKey] =
                        AccessoriesByFinishCode[listKey]
                        .OrderBy(a => a.Series.SortNo)
                        .ThenBy(a => a.PrimaryType.SortNo)
                        .ThenBy(a => a.SecondaryType.SortNo)
                        .ThenBy(a => a.SortNo)
                        .ToList();
                }
                #endregion
            }
            #endregion

            #region 7.Build User
            // Build the Options
            if (stash.Users.Count != 0 && stash.AccessoriesOptions.Count != 0)
            {
                List<AccessoryTrait> dimTraits = TraitsByType.TryGetValue(TypeOfTrait.DimensionTrait, out var dimensions) ? dimensions : new();
                if (stash.AccessoriesOptions.Count > 1 && isPowerUsersRepo)
                {
                    AllAccessoriesOptions = stash.AccessoriesOptions.Select(o => o.ToAccessoriesOptions(dimTraits, TraitGroupsById, stash.CustomPriceRules)).ToList();

                }
                // Find the specific one for this user (or power user)
                var accOptions = AllAccessoriesOptions
                    .FirstOrDefault(o => o.Id == stash.Users.First().AccessoriesOptionsId)
                    //If no matching id is found get the first options and create 
                    ?? stash.AccessoriesOptions.First().ToAccessoriesOptions(dimTraits, TraitGroupsById, stash.CustomPriceRules);
                User = stash.Users.First().ToUserInfo(accOptions);
            }
            else
            {
                User = UserInfo.Undefined();
            }
            #endregion

            #region 8.Build Stock Dictionary
            if (stash.StockInfo.Count != 0)
            {
                StockInfo = stash.StockInfo.ToDictionary(s => s.Key, s => s.Value);
            }
            #endregion

            await BuildIndexAsync();

            IsBuilt = true;
            OnRepositoryBuilding?.Invoke(this, "100% Completed");
            OnRepositoryCreated?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Builds the Index of the Repository
        /// </summary>
        private async Task BuildIndexAsync()
        {
            await Task.Delay(20);
            Dictionary<string, IndexedItem> index = [];

            foreach (var acc in AccessoriesByCode)
            {
                index.Add(acc.Key, new(acc.Value, acc.Key));
            }

            foreach (var trait in TraitsByType.Where(t => t.Key is TypeOfTrait.PrimaryTypeTrait or TypeOfTrait.FinishTrait or TypeOfTrait.SeriesTrait).SelectMany(t => t.Value).ToList())
            {
                int whiteSpaceIncrementor = 0;
                int originalStringLength = trait.Code.Length;
                // Add a space to the key if the key was already there 
                // if there is already a key with a space then add incrementally 1 more white space until the key is added
                while (index.TryAdd(trait.Code.PadRight(originalStringLength + whiteSpaceIncrementor), new(trait, trait.Code)) is false)
                {
                    whiteSpaceIncrementor++;
                }

                //Do the Same for the Addition of the Trait Name
                whiteSpaceIncrementor = 0;
                // Remove Diacritics from the Trait Name to be able to search with and without them
                var traitKey = trait.Trait.RemoveDiacritics().ToUpper();
                originalStringLength = traitKey.Length;
                while (index.TryAdd(traitKey.PadRight(originalStringLength + whiteSpaceIncrementor), new(trait, traitKey)) is false)
                {
                    whiteSpaceIncrementor++;
                }
            }
            searchIndex = index;
        }

        /// <summary>
        /// Searches in the Index
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IndexedItem>> SearchIndexAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<IndexedItem>();
            return await Task.Run(() =>
            {
                // Remove Diacritics from search Text
                var searchText = searchTerm.RemoveDiacritics().ToUpper();

                // Find which keys match ignoring Case 
                var foundKeys = searchIndex.Keys.Where(k => k.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                // Collect the Results in a List
                List<IndexedItem> results = new();
                foreach (var foundKey in foundKeys)
                {
                    results.Add(searchIndex[foundKey]);
                }

                //Normalize any Greek Carachters to Latin and search again now with the Latin Characters
                var normalizedSearchText = searchText.NormalizeGreekToLatin();
                if (searchText != normalizedSearchText)
                {

                    var newFoundKeys = searchIndex.Keys.Where(k => k.Contains(normalizedSearchText, StringComparison.OrdinalIgnoreCase));
                    foreach (var newFoundKey in newFoundKeys)
                    {
                        results.Add(searchIndex[newFoundKey]);
                    }
                }

                //Normalize Inverse Latin to Greek
                var normalizedInverseSearchText = searchText.NormalizeLatinToGreek();
                if (searchText != normalizedInverseSearchText)
                {
                    var newFoundKeys = searchIndex.Keys.Where(k => k.Contains(normalizedInverseSearchText, StringComparison.OrdinalIgnoreCase));
                    foreach (var newFoundKey in newFoundKeys)
                    {
                        results.Add(searchIndex[newFoundKey]);
                    }
                }

                //Distinct by the Hash of the Asscociated item to avoid duplicate Results , Give Traits first in the Results
                return results.DistinctBy(i => i.AssociatedItemHash()).OrderBy(r => r.AssociatedTrait != null ? 0 : 1);
            });
        }

        public List<BathroomAccessory> GetAccessoriesBySeries(string seriesCode)
        {
            _ = AccessoriesBySeriesCode.TryGetValue(seriesCode, out var accessories);
            return accessories ?? new();
        }
        public List<BathroomAccessory> GetAccessoriesByPrimaryType(string primaryTypeCode)
        {
            _ = AccessoriesByPrimaryTypeCode.TryGetValue(primaryTypeCode, out var accessories);
            return accessories ?? new();
        }
        public List<BathroomAccessory> GetAccessoriesByFinish(string finishCode)
        {
            _ = AccessoriesByFinishCode.TryGetValue(finishCode, out var accessories);
            return accessories ?? new();
        }
        public BathroomAccessory? GetAccessoryById(string id)
        {
            _ = AccessoriesById.TryGetValue(id, out BathroomAccessory? accessory);
            return accessory;
        }
        public BathroomAccessory? GetAccessoryByCode(string code)
        {
            _ = AccessoriesByCode.TryGetValue(code, out BathroomAccessory? accessory);
            return accessory;
        }
        public AccessoryTrait? GetTraitById(string id)
        {
            _ = TraitsById.TryGetValue(id, out AccessoryTrait? trait);
            return trait;
        }
        public List<AccessoryTrait> GetTraitsByType(TypeOfTrait type)
        {
            _ = TraitsByType.TryGetValue(type, out List<AccessoryTrait>? traits);
            return traits ?? new();
        }
        public AccessoryTraitClass GetTraitClass(TypeOfTrait type)
        {
            _ = TraitClassesByType.TryGetValue(type, out var traitClass);
            return traitClass ?? AccessoryTraitClass.Empty(type);
        }
        public string GetTraitClassName(TypeOfTrait type)
        {
            _ = TraitClassesByType.TryGetValue(type, out var traitClass);
            return traitClass?.DescriptionInfo.Name ?? "TclassName????";
        }
        public string GetTraitClassPhotoUrl(TypeOfTrait type)
        {
            _ = TraitClassesByType.TryGetValue(type, out var traitClass);
            return traitClass?.PhotoURL ?? "TclassPhotoUrl????";
        }

        /// <summary>
        /// Returns the Accessories where their code contains some part of the search term
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BathroomAccessory>> SearchAccessoriesCodesAsync(string searchTerm)
        {
            return await Task.Run(() =>
            {
                //Search the term , convert the term from greek to latin and vice versa and search again if its not the same add the results . Distinct them in the end
                var found = AccessoriesByCode.Where(kvp => kvp.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Value);
                var searchTermInLatin = searchTerm.NormalizeGreekToLatin(); 
                var searchTermInGreek = searchTerm.NormalizeLatinToGreek(); 

                if (searchTermInLatin != searchTerm)//meaning text is already latin
                {
                    found = found.Concat(AccessoriesByCode.Where(kvp => kvp.Key.Contains(searchTermInLatin, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Value));
                }
                if (searchTermInGreek != searchTerm)// meaning text is already greek
                {
                    found = found.Concat(AccessoriesByCode.Where(kvp => kvp.Key.Contains(searchTermInGreek, StringComparison.OrdinalIgnoreCase)).Select(kvp => kvp.Value));
                }

                return found.Distinct();
            });
        }

        public decimal GetAccessoryStock(string code)
        {
            _ = StockInfo.TryGetValue(code, out decimal stock);
            return stock;
        }
    }

    public class IndexedItem
    {
        public BathroomAccessory? AssociatedAccessory { get; set; }
        public AccessoryTrait? AssociatedTrait { get; set; }

        public string Key { get; set; } = string.Empty;

        public string ItemDescription { get => GetDescription(); }

        /// <summary>
        /// Returns the description of the Indexed Item
        /// </summary>
        /// <returns></returns>
        private string GetDescription()
        {
            if (AssociatedAccessory is not null)
            {
                return AssociatedAccessory.Code;
            }
            else if (AssociatedTrait is not null)
            {
                return AssociatedTrait.Trait;
            }
            else
            {
                return "N/A";
            }
        }

        public IndexedItem(BathroomAccessory accessory, string key)
        {
            AssociatedAccessory = accessory;
            Key = key;
        }
        public IndexedItem(AccessoryTrait trait, string key)
        {
            AssociatedTrait = trait;
            Key = key;
        }
        public IndexedItem(BathroomAccessory accessory, AccessoryTrait trait, string key)
        {
            AssociatedAccessory = accessory;
            AssociatedTrait = trait;
            Key = key;
        }

        /// <summary>
        /// Returns the HashCode of the Asscociated Item , in order to not return duplicate Results for different Keys
        /// </summary>
        /// <returns></returns>
        public int AssociatedItemHash()
        {
            return AssociatedAccessory?.GetHashCode()
                ?? AssociatedTrait?.GetHashCode()
                ?? 0
                ;
        }
    }

}
