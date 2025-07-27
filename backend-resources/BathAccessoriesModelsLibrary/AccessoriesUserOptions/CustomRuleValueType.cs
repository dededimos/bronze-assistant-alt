namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    public enum CustomRuleValueType
    {
        Undefined = 0 ,
        /// <summary>
        /// All Discounts are not take into account except this one
        /// </summary>
        TotalDiscountType = 1,
        /// <summary>
        /// Adds extra discount on top of the Rest
        /// </summary>
        ExtraDiscountType = 2,
        /// <summary>
        /// Nets the Price , Removes all Discounts
        /// </summary>
        NetPriceType = 3 ,
        /// <summary>
        /// Adds to the Starting Price of the Item
        /// </summary>
        AdditionaPriceValueType = 4 , 
        /// <summary>
        /// Modifies the Default Starting Price of the Item
        /// </summary>
        NewCataloguePriceType = 5,
    }
}

