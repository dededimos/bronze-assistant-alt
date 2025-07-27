using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BronzeRulesPricelistLibrary.ConcreteRules.CommonRules;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesAccessories
{
    public class AccessoryPriceRuleException : IPricingRule
    {
        private readonly CustomPriceRule irregularPrice;
        protected string ruleNameOverride = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string RuleName { get => string.IsNullOrWhiteSpace(ruleNameOverride) ? this.GetType().Name : ruleNameOverride; }
        public string RuleApplicationDescription { get; set; } = string.Empty;
        public RuleModification ModificationType { get => GetModificationFromRuleValueType(); }
        private RuleModification GetModificationFromRuleValueType()
        {
            return irregularPrice.RuleValueType switch
            {
                CustomRuleValueType.TotalDiscountType => RuleModification.SetsTotalDiscount,
                CustomRuleValueType.ExtraDiscountType => RuleModification.SetsAdditionalDiscountPercent,
                CustomRuleValueType.NetPriceType => RuleModification.SetsNetPrice,
                CustomRuleValueType.AdditionaPriceValueType => RuleModification.SetsAdditionalPriceValue,
                CustomRuleValueType.NewCataloguePriceType => RuleModification.SetsStartingPrice,
                _ => RuleModification.None,
            };
        }

        public AccessoryPriceRuleException(CustomPriceRule iregularPrice)
        {
            this.irregularPrice = iregularPrice;
        }

        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ApplyRule(IPriceable product)
        {
            ruleNameOverride = irregularPrice.DescriptionInfo.Name;
            try
            {
                if (product is AccessoryPriceable a)
                {
                    switch (irregularPrice.RuleValueType)
                    {
                        case CustomRuleValueType.TotalDiscountType:
                            ApplySingleTotalDiscount(a);
                            break;
                        case CustomRuleValueType.ExtraDiscountType:
                            ApplyExtraDiscount(a);
                            break;
                        case CustomRuleValueType.NetPriceType:
                            ApplyNetPrice(a);
                            break;
                        case CustomRuleValueType.AdditionaPriceValueType:
                            ApplyAdditionaPrice(a);
                            break;
                        case CustomRuleValueType.NewCataloguePriceType:
                            ApplyNewCataloguePrice(a);
                            break;
                        case CustomRuleValueType.Undefined:
                        default:
                            RuleApplicationDescription = "Undefined/NotSupported RuleValueType";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                RuleApplicationDescription = $"Error Executing Rule";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is AccessoryPriceable a)
            {
                return irregularPrice.IsApplicable(a.Product, a.SelectedFinish.Finish.Id,product.Quantity);
            }
            return false;
        }

        private void ApplySingleTotalDiscount(AccessoryPriceable p)
        {
            RemovePreviousDiscounts(p);
            ApplyDiscount(p, irregularPrice.RuleValue);
        }
        private void ApplyExtraDiscount(AccessoryPriceable p)
        {
            if (irregularPrice.RuleValueType == CustomRuleValueType.ExtraDiscountType)
            {
                ApplyDiscount(p, irregularPrice.RuleValue);
            }
        }
        private void ApplyNetPrice(AccessoryPriceable p)
        {
            if (irregularPrice.RuleValueType == CustomRuleValueType.NetPriceType)
            {
                RemovePreviousDiscounts(p);
                RemoveBasePrice(p);

                p.StartingPrice = irregularPrice.RuleValue;
                RuleApplicationDescription = $"+{p.StartingPrice:0.00€}";
            }
        }
        private void ApplyAdditionaPrice(AccessoryPriceable p)
        {
            p.StartingPrice += irregularPrice.RuleValue;
            // Shows + or - in front of the string based on weather the result is positive negative or zero
            RuleApplicationDescription = string.Format("{0:+0.00€;-0.00€;0.00€}", irregularPrice.RuleValue);
        }
        private void ApplyNewCataloguePrice(AccessoryPriceable p)
        {
            RemoveBasePrice(p);
            p.StartingPrice = irregularPrice.RuleValue;
            // Shows + or - in front of the string based on weather the result is positive negative or zero
            RuleApplicationDescription = $"{irregularPrice.RuleValue:0.00€}";
        }

        private static void RemovePreviousDiscounts(AccessoryPriceable p)
        {
            //Find all previous applied Discounts and Delete Them
            p.Discounts.Clear();
            //Must be done as a List , otherwise throws enumeration altered exception
            var appliedDiscounts = p.AppliedRules.Where(ar => ar.AppliedRuleType == typeof(DiscountPriceRule)).ToList();
            foreach (var appliedDiscount in appliedDiscounts)
            {
                p.AppliedRules.Remove(appliedDiscount);
            }
        }
        private void ApplyDiscount(AccessoryPriceable p , decimal discount)
        {
            p.Discounts.Add(discount);
            RuleApplicationDescription = $"-{discount:0.00%}";
        }
        private static void RemoveBasePrice(AccessoryPriceable p)
        {
            p.StartingPrice = 0;
            var basePriceRule = p.AppliedRules.FirstOrDefault(ar=> ar.AppliedRuleType ==typeof(AccessoryBasePriceRule));
            if (basePriceRule != null)
            {
                p.AppliedRules.Remove(basePriceRule);
            }
        }
    }
}
