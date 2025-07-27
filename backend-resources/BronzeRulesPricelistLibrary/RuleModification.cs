namespace BronzeRulesPricelistLibrary
{
    public enum RuleModification
    {
        None = 0,
        SetsStartingPrice = 1,
        SetsAdditionalDiscountPercent = 2,
        SetsAdditionalPricePercent = 3,
        SetsAdditionalDiscountValue = 4,
        SetsAdditionalPriceValue = 5,
        SetsNetPrice = 6,
        SetsTotalDiscount = 7,
    }
}
