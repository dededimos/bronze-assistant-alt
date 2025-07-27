using BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BronzeFactoryApplication.Helpers
{
    public static class ApplicationExtensionHelpers
    {
        /// <summary>
        /// Trys to Find the specified key and Translate it into the curtrently selected Language
        /// </summary>
        /// <param name="key">The Key</param>
        /// <returns>The translation or Key Not Found String</returns>
        public static string TryTranslateKey(this string key)
        {
            return App.Current.TryFindResource(key)?.ToString()
                        ?? $"Key:({key}) not Found";
        }

        /// <summary>
        /// Tryes to Find the specified key and Translate it into the current language , if no Translation is found it returns the key
        /// </summary>
        /// <param name="key">the key of the Translation</param>
        /// <returns></returns>
        public static string TryTranslateKeyWithoutError(this string key)
        {
            return App.Current.TryFindResource(key)?.ToString() ?? key;
        }

        /// <summary>
        /// Returns The Clone of this Part with Modified Quantity 
        /// Equal to the Default Quantity of the Specified Spot for the Specified Structure
        /// </summary>
        /// <param name="part">The Part to Modify</param>
        /// <param name="spot">The Spot of the Part</param>
        /// <param name="identifier">The Structures Identifier</param>
        /// <param name="repo">The Repository Holding the Parts information</param>
        /// <returns>The Part Clone with the Modified Quantity</returns>
        public static T GetCloneWithSpotDefaultQuantity<T>(this T part, PartSpot spot,CabinIdentifier identifier, ICabinMemoryRepository repo)
            where T : CabinPart
        {
            var partClone = part.GetDeepClone();
            partClone.Quantity = repo.GetSpotDefaultQuantity(spot, identifier);
            return (T)partClone; //always a Type of T ,GetDeepClone is overriden always returning the correct type.
        }

    }
}
