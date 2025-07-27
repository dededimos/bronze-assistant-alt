using FluentValidation;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonValidators;
using CommonHelpers;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorElementTraitEntityValidator : AbstractValidator<MirrorElementTraitEntity>
    {
        public MirrorElementTraitEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetInheritanceValidator(v =>
            {
                v.Add<CustomMirrorTraitEntity>(new CustomMirrorTraitEntityValidator(includeIdValidation));
            });
        }
    }
    public class MirrorElementTraitEntityBaseValidator : MirrorElementEntityBaseValidator<MirrorElementTraitEntity>
    {
        public MirrorElementTraitEntityBaseValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            RuleFor(t => t.TargetTypes).Must(l => l.HasDuplicates() is false).WithErrorCode($"TargetTypesContainDuplicates");
            RuleFor(t => t.TargetElementIds).Must(l => l.HasDuplicates() is false).WithErrorCode("TargetElementIdsContainDuplicates");
        }
    }
    public class CustomMirrorTraitEntityValidator : AbstractValidator<CustomMirrorTraitEntity>
    {
        public CustomMirrorTraitEntityValidator(bool includeIdValidation)
        {
            RuleFor(e => e).SetValidator(new MirrorElementTraitEntityBaseValidator(includeIdValidation));
            RuleFor(e => e.CustomTraitType).SetValidator(new LocalizedStringValidator(true));
        }
    }
}
