using AccessoriesRepoMongoDB.Entities;
using FluentValidation;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Validators
{
    public class UserAccessoriesOptionsEntityValidator : DescriptiveEntityValidator<UserAccessoriesOptionsEntity>
    {
        public UserAccessoriesOptionsEntityValidator(bool includeIdValidation = true) : base(includeIdValidation)
        {
            RuleFor(o => o.Name).NotEmpty().WithErrorCode("OptionsNameEmpty");
            RuleFor(o => o.AppearingDimensionsGroup).NotEmpty().WithErrorCode("DimensionsGroupEmpty");
        }
    }
}
