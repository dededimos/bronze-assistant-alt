using CommonInterfacesBronze;

namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    public class CustomPriceRule
    {
        public string Id { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        /// <summary>
        /// Smaller Numbers appear First and execute Price Rules First
        /// </summary>
        public int SortNo { get; set; }
        public CustomRuleValueType RuleValueType { get; set; }
        /// <summary>
        /// The Value of the SpecificDiscount or Extra Discount or Net Price or Extra Price
        /// </summary>
        public decimal RuleValue { get; set; }

        public List<PriceRuleCondition> Conditions { get; set; } = new();

        public bool IsApplicable(BathroomAccessory accessory, string? selectedFinishId = null,decimal quantity = 0)
        {
            //return true only when there are conditions and are all applicable
            return Conditions.Any() && Conditions.All(c => c.IsApplicable(accessory, selectedFinishId,quantity));
        }
        public static CustomPriceRule Undefined() => new();
    }
}

