using FluentValidation;
using MirrorsLib.Validators;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorConstraintsEntityValidator : AbstractValidator<MirrorConstraintsEntity>
    {
        public MirrorConstraintsEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetValidator(new DatabaseEntityValidator(includeIdValidation));
            RuleFor(e => e.Constraints).SetValidator(new MirrorConstraintsValidator());
        }
    }
}
