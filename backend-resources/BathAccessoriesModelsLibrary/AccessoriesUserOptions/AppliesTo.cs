namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    public enum AppliesTo
    {
        /// <summary>
        /// Does not Apply
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// Applies to All Accessories
        /// </summary>
        AllAccessories = 1,
        /// <summary>
        /// Specific Accessory
        /// </summary>
        AccessorySpecific = 2,
        /// <summary>
        /// All Except this Accessory
        /// </summary>
        ExceptAccessorySpecific = 3,
        /// <summary>
        /// Accessories with the Specified Finish
        /// </summary>
        FinishSpecific = 4,
        /// <summary>
        /// All Finishes Except Specific Finish
        /// </summary>
        ExceptFinishSpecific = 5,
        /// <summary>
        /// Accessories with the Selected Finish having this Finish Trait Group
        /// </summary>
        FinishTraitGroupSpecific = 6,
        /// <summary>
        /// All Finish Groups Except Specific Finish Group
        /// </summary>
        ExceptFinishTraitGroupSpecific = 7,
        /// <summary>
        /// Accessories with this Series as MAIN
        /// </summary>
        SeriesMainSpecific = 8,
        /// <summary>
        /// All Series Except Specific Series as Main
        /// </summary>
        ExceptSeriesMainSpecific = 9,
        /// <summary>
        /// Accessories with a series in the Specified Trait Group as their MAIN
        /// </summary>
        SeriesMainTraitGroupSpecific = 10,
        /// <summary>
        /// All Series Trait Groups except Accessories with the Specific Series Trait Group as its main
        /// </summary>
        ExceptSeriesMainTraitGroupSpecific = 11,
        /// <summary>
        /// Accessories with a series in the Specified Trait Group as their Main or Not
        /// </summary>
        OtherSeriesTraitGroupSpecific = 12,
        /// <summary>
        /// All Series Trait Groups except Accessories with the Speicific Series Trait Group in any of their Series
        /// </summary>
        ExceptOtherSeriesTraitGroupSpecific = 13,
        /// <summary>
        /// Accessories with this Series as Main Or Not
        /// </summary>
        OtherSeriesSpecific = 14,
        /// <summary>
        /// All Accessories except those with that specific Series at its Main or Other
        /// </summary>
        ExceptOtherSeriesSpecific = 15,
        /// <summary>
        /// Accessories with the specified Size
        /// </summary>
        SizeSpecific = 16,
        /// <summary>
        /// Accessories that do not have the specified Size
        /// </summary>
        ExceptSizeSpecific = 17,
        /// <summary>
        /// Accessories with the specified Shape
        /// </summary>
        ShapeSpecific = 18,
        /// <summary>
        /// Accessories without the specified Shape
        /// </summary>
        ExceptShapeSpecific = 19,
        /// <summary>
        /// Accessories with the specified Material
        /// </summary>
        MaterialSpecific = 20,
        /// <summary>
        /// Accessories Without the Specified Material
        /// </summary>
        ExceptMaterialSpecific = 21,
        /// <summary>
        /// Accessories with the specified PrimaryType as their First Type
        /// </summary>
        PrimaryTypeMainSpecific = 22,
        /// <summary>
        /// Accessories Without the Specified Groups as their Main
        /// </summary>
        ExceptPrimaryTypeMainSpecific = 23,
        /// <summary>
        /// Accessories with the specified PrimaryType as one of their PrimaryTypes
        /// </summary>
        OtherPrimaryTypeSpecific = 24,
        /// <summary>
        /// Accessories without the specified PrimaryType as their Main or Other
        /// </summary>
        ExceptOtherPrimaryTypeSpecific = 25,
        /// <summary>
        /// Accessories with the specified SecondaryType as their First Type
        /// </summary>
        SecondaryTypeMainSpecific = 26,
        /// <summary>
        /// Accessories without the specified Secondary Type as their Main
        /// </summary>
        ExceptSecondaryTypeMainSpecific = 27,
        /// <summary>
        /// Accessories with the specified SecondaryType as one of their SecondaryTypes
        /// </summary>
        OtherSecondaryTypeSpecific = 28,
        /// <summary>
        /// Accessories without the specified secondary Type as their main or Other 
        /// </summary>
        ExceptOtherSecondaryTypeSpecific = 29,
        /// <summary>
        /// Accessories of the specified Category
        /// </summary>
        CategorySpecific = 30,
        /// <summary>
        /// Accessories without the specified Category
        /// </summary>
        ExceptCategorySpecific = 31,
        /// <summary>
        /// Accessories of the Specified Mounting
        /// </summary>
        MountingSpecific = 32,
        /// <summary>
        /// Accessories without the specified Mounting Type
        /// </summary>
        ExceptMountingSpecific = 33,
        /// <summary>
        /// Accessories that have the Specified Price Trait
        /// </summary>
        PriceTraitSpecific = 34,
        /// <summary>
        /// Accessories without the specified Price Trait
        /// </summary>
        ExceptPriceTraitSpecific = 35,
        /// <summary>
        /// Accessories that have the Specified Price Trait in the specified Trait Group
        /// </summary>
        PriceTraitGroupSpecific = 36,
        /// <summary>
        /// Accessories without the specified PriceTraitGroup
        /// </summary>
        ExceptPriceTraitGroupSpecific = 37,
        /// <summary>
        /// Applies for a certain quantity
        /// </summary>
        ItemQuantity = 38,
        /// <summary>
        /// Does not Apply for the specified quantity condition
        /// </summary>
        ExceptItemQuantity = 39,
    }
}

