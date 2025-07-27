using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Entities
{
    /// <summary>
    /// Represents a CustomPriceRule Object , used to execute Custom Pricing Rules on Accessories
    /// </summary>
    public class CustomPriceRuleEntity : DescriptiveEntity , IDeepClonable<CustomPriceRuleEntity>
    {
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        /// Used in how the Rule is Getting Executed . Smaller Numbers Execute Before everything else. Tied Rules Execute with random order
        /// </summary>
        public int SortNo { get; set; } = 99999;
        /// <summary>
        /// The Value of the Rule is always a decimal , but represents different Kinds of Properties (Extra Discounts ,Total Discount , Net Price e.t.c.)
        /// </summary>
        public CustomRuleValueType RuleValueType { get; set; }
        public string RuleValueDescription { get => GetRuleValueDescription(); }
        public decimal RuleValue { get; set; }
        /// <summary>
        /// The Conditions under which the rule is getting executed
        /// </summary>
        public List<PriceRuleCondition> Conditions { get; set; } = new();

        public CustomPriceRuleEntity GetDeepClone()
        {
            var clone = (CustomPriceRuleEntity)this.MemberwiseClone();
            clone.Name = this.Name.GetDeepClone();
            clone.Description = this.Description.GetDeepClone();
            clone.ExtendedDescription = this.ExtendedDescription.GetDeepClone();
            clone.Conditions = new(this.Conditions.Select(c => c.GetDeepClone()));
            return clone;
        }

        private string GetRuleValueDescription()
        {
            return RuleValueType switch
            {
                CustomRuleValueType.Undefined => "UndefinedRuleValue",
                CustomRuleValueType.TotalDiscountType => $"-{RuleValue * 100}% Total Disc",
                CustomRuleValueType.ExtraDiscountType => $"-{RuleValue * 100}% Disc",
                CustomRuleValueType.NetPriceType => $"{RuleValue:0.00€} Net",
                _ => "NotSupportedRuleValue",
            };
        }
    }

    public class CustomPriceRuleEntityComparer : IEqualityComparer<CustomPriceRuleEntity>
    {
        public bool Equals(CustomPriceRuleEntity? x, CustomPriceRuleEntity? y)
        {
            var descriptiveEntityComparer = new DescriptiveEntityEqualityComparer();
            var conditionEqualityComparer = new PriceRuleConditionEqualityComparer();

            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return descriptiveEntityComparer.Equals(x, y) &&
            x!.IsEnabled == y!.IsEnabled &&
            x.SortNo == y.SortNo &&
            x.RuleValueType == y.RuleValueType &&
            x.RuleValue == y.RuleValue &&
            x.Conditions.SequenceEqual(y.Conditions,conditionEqualityComparer);
        }

        public int GetHashCode([DisallowNull] CustomPriceRuleEntity obj)
        {
            throw new NotSupportedException($"{typeof(CustomPriceRuleEntityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}
