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
    public class TraitGroupEntityValidator : DescriptiveEntityValidator<TraitGroupEntity>
    {
        public TraitGroupEntityValidator(bool includeIdValidation = true) : base(includeIdValidation)
        {
            RuleFor(tg => tg.Code)
                .NotEmpty()
                .WithErrorCode("Undefined Trait Group Code");

            RuleFor(tg => tg.PermittedTraitTypes)
                .NotEmpty()
                .WithErrorCode($"{nameof(TraitGroupEntity.PermittedTraitTypes)} cannot be Empty at least one Type Must be included");
        }
    }
}
