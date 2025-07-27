using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Validators
{
    public static class AccessoriesValidationErrorCodes
    {
        public const string UndefinedTraitType = "UndefinedTraitType";
        public const string UndefinedFinishCode = "UndefinedFinishCode";
        public const string UndefinedTraitDefaultValue = "UndefinedTraitDefaultValue";
        public const string AllLocalesTrait = "AllLocalesTrait";
        public const string UndefiniedTraitTooltipDefaultValue = "TraitTooltipDefaultCannotBeEmptyWhenLocalesPresent";
        public const string AllOrNoneLocalesTraitTooltip = "AllOrNoneLocalesTraitTooltip";
        public const string UndefinedAllowedSecondaryTypes = "UndefinedAllowedSecondaryTypes";
        public const string UndefiniedSecondaryType = "UndefiniedSecondaryType";

        public const string UndefiniedAccessoryCode = "UndefiniedAccessoryCode";
        public const string UndefiniedAccessoryMainCode = "UndefiniedAccessoryMainCode";
        public const string UndefiniedAccessoryFinish = "UndefiniedAccessoryFinish";
        public const string UndefiniedAccessoryPrimaryTypes = "UndefiniedAccessoryPrimaryTypes";
        public const string UndefiniedAccessoryPrimaryType = "UndefiniedAccessoryPrimaryType";
        public const string UndefiniedAccessorySecondaryTypes = "UndefiniedAccessorySecondaryTypes";
        public const string UndefiniedAccessorySecondaryType = "UndefiniedAccessorySecondaryType";
        public const string UndefiniedAccessoryCategory = "UndefiniedAccessoryCategory";
        public const string UndefiniedAccessoryMountingType = "UndefiniedAccessoryMountingType";
        public const string UndefiniedAccessorySeries = "UndefiniedAccessorySeries";
        public const string UndefiniedDimensionValue = "UndefiniedDimensionValue";
        public const string UndefiniedDimensionKey = "UndefiniedDimensionKey";
        public const string UndefiniedPriceValue = "UndefiniedPriceValue";
        public const string UndefiniedSizeVariation = "UndefiniedSizeVariation";
        public const string UndefiniedSizeWhenThereIsVariation = "UndefiniedSizeWhenThereIsVariation";
        public const string UndefiniedMountingVariation = "UndefiniedMountingVariation";

        public const string UndefiniedTraitIdInListOfTraits = "UndefiniedTraitIdInListOfTraits";
    }
}
