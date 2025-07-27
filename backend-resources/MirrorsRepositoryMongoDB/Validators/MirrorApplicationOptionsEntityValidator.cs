using FluentValidation;
using MirrorsLib;
using MirrorsLib.Validators;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorApplicationOptionsEntityValidator : AbstractValidator<MirrorApplicationOptionsEntity>
    {
        public MirrorApplicationOptionsEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetValidator(new OptionsEntityBaseValidator(includeIdValidation));
            RuleFor(e => e.Options).SetValidator(v => new MirrorApplicationOptionsBaseValidator());
            RuleFor(e => e.OptionsType).Must((entity, type) => type == entity.Options.GetType().Name).WithErrorCode("OptionsTypeMustBeEqualToOptionsTypeName");
        }
    }
}
