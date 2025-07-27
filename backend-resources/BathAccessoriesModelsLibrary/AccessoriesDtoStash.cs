using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class AccessoriesDtoStash
    {
        public const string DefaultAccOptionsName = "Defaults";

        /// <summary>
        /// The List of Bathroom Accessories as DTOs
        /// </summary>
        public List<BathroomAccessoryDTO> Accessories { get; set; } = [];
        /// <summary>
        /// The List of Traits as DTOs
        /// </summary>
        public List<AccessoryTraitDTO> Traits { get; set; } = [];
        /// <summary>
        /// The List of TraitClasses as DTOs
        /// </summary>
        public List<AccessoryTraitClassDTO> TraitClasses { get; set; } = [];
        /// <summary>
        /// The List of TraitGroups
        /// </summary>
        public List<AccessoryTraitGroup> TraitGroups { get; set; } = [];
        
        /// <summary>
        /// A Dictionary of BathAccessories Ids => matching their PriceInfo Lists
        /// </summary>
        public Dictionary<string, List<PriceInfoDTO>> PricesInfo { get; set; } = [];
        /// <summary>
        /// The List of Custom Price Rules
        /// </summary>
        public List<CustomPriceRule> CustomPriceRules { get; set; } = [];
        /// <summary>
        /// The List of UserAccessories Options as DTOs
        /// </summary>
        public List<UserAccessoriesOptionsDTO> AccessoriesOptions { get; set; } = [];
        
        /// <summary>
        /// The List of Users as DTOs
        /// </summary>
        public List<UserInfoDTO> Users { get; set; } = [];

        /// <summary>
        /// The List of Stock Information per Accessory Code
        /// </summary>
        public Dictionary<string, decimal> StockInfo { get; set; } = [];

        /// <summary>
        /// Any Message need to be Passed with the Stash Object
        /// </summary>
        public string TagMessage { get; set; } = string.Empty;


        /// <summary>
        /// Creates a new Reference of this Stash without sensitive information (Users,PriceInfo,AccessoriesOptions,Price Rules)
        /// </summary>
        /// <returns></returns>
        private AccessoriesDtoStash GetStashNonSensitive()
        {
            AccessoriesDtoStash stash = new()
            {
                Accessories = this.Accessories,
                Traits = this.Traits,
                TraitClasses = this.TraitClasses,
                TraitGroups = this.TraitGroups,
                //Do not include Price Info
                //Do not Include PriceRules
                //Do not Include AccessoriesOptions
                //Do not Include Users
                //Do not Include StockInfo
                TagMessage = this.TagMessage,
            };
            return stash;
        }



        /// <summary>
        /// Returns the Stash for a specific User, Includes Only information eligible for the provided User
        /// If its a Power User => Returns all Rules , and All AccOptions for the Specified PriceTrait Group that this Power User Has (Greek or Italian Catalogue)
        /// </summary>
        /// <param name="userInfo">The User's Info</param>
        /// <returns></returns>
        public AccessoriesDtoStash GetUsersStash(UserInfoDTO userInfo, bool isPowerUser, string usersRegisteredMachine = "")
        {
            //Get the Stash Without Prices and Options
            var newStash = GetStashNonSensitive();

            //Finding the Options gives us
            //1. PriceGroupId that is needed to generate Prices
            //2. The Exception Price Rules for the specific User
            var registeredMachine = userInfo.RM;
            var nonSensitiveInfo = userInfo.GetNonSensitiveUserInfo();
            
            //Expose Sensitive Info (Custom Price Rules and ALL Prices Info only in the users Registered Machine AND ONLY WHEN THE RM is not empty)
            if (isPowerUser && !string.IsNullOrWhiteSpace(registeredMachine) && usersRegisteredMachine == registeredMachine)
            {
                //For Power Users add all Rules and All Accessories Options where the Trait Group is the Same with their UserOptions
                newStash.AccessoriesOptions = this.AccessoriesOptions;
                newStash.CustomPriceRules = this.CustomPriceRules;
                newStash.PricesInfo = this.PricesInfo;
                newStash.StockInfo = this.StockInfo;

                //Do not expose the RM in the Client
                newStash.Users.Add(nonSensitiveInfo);
                return newStash;
            }
            else
            {
                //Find the Accessories Options Eligible for this User if there are not any put defaults , if there are no defaults just put an empty Options
                var accOptions = AccessoriesOptions.FirstOrDefault(o => o.Id == userInfo.AccessoriesOptionsId)
                    ?? AccessoriesOptions.FirstOrDefault(o => o.DescriptionInfo.Name == DefaultAccOptionsName)
                        ?? UserAccessoriesOptionsDTO.Undefined();

                //For the Specific user add only his Own Options
                newStash.AccessoriesOptions.Add(accOptions);
                //For the specific User Get Only the needed Rules
                //Get Only the Rules which have an id that is included in the found Acc Options
                var customPriceRules = CustomPriceRules.Where(pr => accOptions.CustomPriceRules.Any(id => id == pr.Id));
                newStash.CustomPriceRules = customPriceRules.ToList();

                // Keep Only the PriceInfo Objects that their PriceId is a PriceTrait which includes the PricesGroupId of the AccessoriesOptions
                // 1.Find the PriceTraits that have this kind of group , if there are none then there will be no prices
                var priceTraitsWithPriceGroupId = string.IsNullOrEmpty(accOptions.PricesGroupId)
                    ? Enumerable.Empty<string>()
                    : Traits.Where(t => t.TraitType == TypeOfTrait.PriceTrait && t.GroupsIds.Any(id => id == accOptions.PricesGroupId)).Select(priceTrait => priceTrait.Id);

                // 2.Keep The Prices Info that have a Price Id that matches one of the found PriceIds (which in turn have the priceGroup of the accOptions Assigned)
                var pricesInfoToKeep = PricesInfo.ToDictionary(
                    kvp => kvp.Key, //All Accessories Ids as Keys
                    kvp => kvp.Value.Where(priceInfo => priceTraitsWithPriceGroupId.Any(id => id == priceInfo.PriceId)).ToList()); //Keep only PriceInfo that match the PriceId
                newStash.PricesInfo = pricesInfoToKeep;
            }

            //Do not expose the RM in the Client
            newStash.Users.Add(nonSensitiveInfo);

            return newStash;
        }

    }
}
