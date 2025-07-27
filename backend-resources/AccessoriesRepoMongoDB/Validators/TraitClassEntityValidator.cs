using AccessoriesRepoMongoDB.Entities;
using FluentValidation;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccessoriesRepoMongoDB.Validators.AccessoriesValidationErrorCodes;

namespace AccessoriesRepoMongoDB.Validators
{
    public class TraitClassEntityValidator : DescriptiveEntityValidator<TraitClassEntity>
    {
        public TraitClassEntityValidator(bool includeIdValidation = true) : base(includeIdValidation)
        {
            // Trait Type Cannot be Undefined
            RuleFor(tc => tc.TraitType).NotEmpty().WithErrorCode(UndefinedTraitType);
            RuleForEach(tc => tc.Traits).ChildRules(trait =>
            {
                trait.RuleFor(t => t).NotEqual(ObjectId.Empty).WithErrorCode(UndefiniedTraitIdInListOfTraits);
            });
        }
    }
}
