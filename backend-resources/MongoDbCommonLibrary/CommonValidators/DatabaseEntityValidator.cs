using FluentValidation;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonInterfaces;
using static MongoDbCommonLibrary.CommonValidators.CommonEntitiesValidationErrorCodes;

namespace MongoDbCommonLibrary.CommonValidators
{
    public class DatabaseEntityValidator : AbstractValidator<IDatabaseEntity>
    {
        public DatabaseEntityValidator(bool includeIdValidation = true)
        {
            if (includeIdValidation)
            {
                RuleFor(e => e.Id).Must(id=> ObjectId.TryParse(id, out _)).NotEqual(ObjectId.Empty.ToString()).WithErrorCode(InvalidObjectId);
            }
        }
    }
}
