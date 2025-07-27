using CommonInterfacesBronze;
using FluentValidation;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDbCommonLibrary.CommonValidators.CommonEntitiesValidationErrorCodes;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class DbEntityValidator<T> : AbstractValidator<T> where T : DbEntity
    {
        public DbEntityValidator(bool includeIdValidation = true)
        {
            if (includeIdValidation)
            {
                RuleFor(e => e.Id).NotEqual(ObjectId.Empty).WithErrorCode(InvalidObjectId);
            }
        }
        /// <summary>
        /// Used in a Must Block to indicate that All Locales should exists
        /// When ther are any Locales at all
        /// </summary>
        /// <param name="localizedValues"></param>
        /// <returns></returns>
        protected bool LocalizedValuesMustHaveAllLocales(Dictionary<string, string> localizedValues)
        {
            return
                    localizedValues.ContainsKey(LocalizedString.GREEKIDENTIFIER) &&
                    localizedValues.ContainsKey(LocalizedString.ENGLISHIDENTIFIER) &&
                    localizedValues.ContainsKey(LocalizedString.ITALIANIDENTIFIER) &&
                    localizedValues.Values.All(v => string.IsNullOrEmpty(v) is false);
        }
    }
}
