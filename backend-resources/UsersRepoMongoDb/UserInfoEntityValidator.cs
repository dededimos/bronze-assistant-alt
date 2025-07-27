using FluentValidation;
using MongoDbCommonLibrary.CommonValidators;

namespace UsersRepoMongoDb
{
    public class UserInfoEntityValidator : DbEntityValidator<UserInfoEntity>
    {
        public UserInfoEntityValidator(bool includeIdValidation = true) : base(includeIdValidation)
        {
            RuleFor(e => e.UserName).NotEmpty().WithErrorCode("UserNameEmpty");
            RuleFor(e => e.GraphUserObjectId).NotEmpty().When(e => e.UserName != "Defaults", ApplyConditionTo.CurrentValidator).WithErrorCode("GraphUserObjectIdEmpty");
        }
    }
}
