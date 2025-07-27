using AccessoriesRepoMongoDB.Entities;
using FluentValidation;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonValidators;
using static AccessoriesRepoMongoDB.Validators.AccessoriesValidationErrorCodes;

namespace AccessoriesRepoMongoDB.Validators
{
    public class BathAccessoryEntityValidator : DescriptiveEntityValidator<BathAccessoryEntity>
    {
        public BathAccessoryEntityValidator(bool includeIdValidation = true) : base(includeIdValidation,false)
        {
            // Code must not be Undefinied
            RuleFor(a => a.Code).NotEmpty().NotEqual("??").WithErrorCode(UndefiniedAccessoryCode);
            RuleFor(a => a.MainCode).NotEmpty().NotEqual(" ").WithErrorCode(UndefiniedAccessoryMainCode);

            // Finish Cannot Be Empty
            RuleFor(a => a.Finish).NotEmpty().WithErrorCode(UndefiniedAccessoryFinish);

            RuleFor(a => a.AvailableFinishes).NotEmpty().WithErrorCode("EmptyAvailableFinishes");
            RuleFor(a => a.AvailableFinishes).Must(l => l.Count == l.Select(af=> af.FinishId).Distinct().Count()).WithErrorCode("DuplicateAvailableFinish");
            RuleForEach(a => a.AvailableFinishes).ChildRules(infoValidator =>
            {
                infoValidator.RuleFor(f => f.FinishId).NotEmpty().WithErrorCode("UndefinedAvailableFinish");
            });
            

            // All Size Variations must not be empty
            RuleForEach(a => a.SizeVariations).ChildRules(sizeVariation =>
            {
                sizeVariation.RuleFor(sv => sv).NotEmpty().WithErrorCode(UndefiniedSizeVariation);
            });
            // If there are Size Variations Size Cannot be Empty
            When(a => a.SizeVariations.Count > 0, () =>
            {
                RuleFor(a => a.Size).NotEmpty().WithErrorCode(UndefiniedSizeWhenThereIsVariation);
            });

            // Primary Types Must not be Empty
            RuleFor(a => a.PrimaryTypes).NotEmpty().WithErrorCode(UndefiniedAccessoryPrimaryTypes);
            // All Primary Types Must be Definied
            RuleForEach(a => a.PrimaryTypes).ChildRules(primaryType =>
            {
                primaryType.RuleFor(pt => pt).NotEmpty().WithErrorCode(UndefiniedAccessoryPrimaryType);
            });

            // Secondary Types Must not be Empty
            RuleFor(a => a.SecondaryTypes).NotEmpty().WithErrorCode(UndefiniedAccessorySecondaryTypes);
            // All Secondary Types Must be Definied
            RuleForEach(a => a.SecondaryTypes).ChildRules(secondaryType =>
            {
                secondaryType.RuleFor(st => st).NotEmpty().WithErrorCode(UndefiniedAccessorySecondaryType);
            });

            // All Categories Must be Definied
            RuleForEach(a => a.Categories).ChildRules(category =>
            {
                category.RuleFor(c => c).NotEmpty().WithErrorCode(UndefiniedAccessoryCategory);
            });

            // Series Cannot be Empty
            RuleFor(a=> a.Series).NotEmpty().WithErrorCode(UndefiniedAccessorySeries);
            // All Series Must be Definied
            RuleForEach(a => a.Series).ChildRules(series =>
            {
                series.RuleFor(s => s).NotEmpty().WithErrorCode(UndefiniedAccessorySeries);
            });

            // All MountingTypes Must be Definied
            RuleForEach(a => a.MountingTypes).ChildRules(mountingType =>
            {
                mountingType.RuleFor(mt => mt).NotEmpty().WithErrorCode(UndefiniedAccessoryMountingType);
            });
            // All Mounting Variations must not be empty
            RuleForEach(a => a.MountingVariations).ChildRules(mountingVariation =>
            {
                mountingVariation.RuleFor(sv => sv).NotEmpty().WithErrorCode(UndefiniedMountingVariation);
            });

            // All Dimensions must not be Empty Traits and Values should be there
            RuleForEach(a => a.Dimensions).ChildRules(dimension =>
            {
                dimension.RuleFor(d => d.Key).NotEmpty().WithErrorCode(UndefiniedDimensionKey);
                dimension.RuleFor(d => d.Value).NotEmpty().WithErrorCode(UndefiniedDimensionValue);
            });

            // All Prices must not be Empty Traits and Values should be there
            RuleForEach(a => a.PricesInfo).ChildRules(priceInfo =>
            {
                priceInfo.RuleFor(p => p.PriceTraitId).NotEmpty().WithErrorCode("PriceInfoWithPriceOfUnidentifiedId");
                priceInfo.RuleFor(p => p).Must(p=> string.IsNullOrWhiteSpace(p.RefersToFinishGroupId) == false  || string.IsNullOrWhiteSpace(p.RefersToFinishId) == false).WithErrorCode("PriceInfoWithUndefinedGroupAndFinish");
            });
        }
    }
}
