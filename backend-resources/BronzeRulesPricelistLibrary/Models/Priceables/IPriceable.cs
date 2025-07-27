using BronzeRulesPricelistLibrary.ConcreteRules;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables
{
    public interface IPriceable<T> :IPriceable
        where T : ICodeable
    {
        /// <summary>
        /// The Product represented by this priceable
        /// </summary>
        T Product { get; }
    }

    /// <summary>
    /// An object that has Pricing
    /// </summary>
    public interface IPriceable
    {
        public string Code { get; }
        /// <summary>
        /// The Vat Factor , Factor Format >1,<2  (Ex - 1.24)
        /// </summary>
        public decimal VatFactor { get; set; }
        /// <summary>
        /// The Starting Price (Catalogue Price)
        /// </summary>
        public decimal StartingPrice { get; set; }
        /// <summary>
        /// The Product Quantity of this Priceable
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// The Applied Discounts (Multiplicative) , Discount Format < 1
        /// </summary>
        public List<decimal> Discounts { get; set; }
        /// <summary>
        /// The Rules Applied to this Item
        /// </summary>
        public List<AppliedRule> AppliedRules { get; set; }
        /// <summary>
        /// The Path of the Photo to appear on the Pricing Table /Invoice e.t.c.
        /// </summary>
        public string ThumbnailPhotoPath { get; set; }
        /// <summary>
        /// The Keys of the Items Description , or The Description Strings
        /// </summary>
        public List<string> DescriptionKeys { get; set; }

        /// <summary>
        /// Gets the Total Applied Multiplicative Discount Factor , (Actually 1-Discount)
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetTotalDiscountFactor()
        {
            //The First seed value is 1 so the first accumulation is 1 , if there are no elements it returns one 
            decimal totalDiscountFactor = Discounts?.Aggregate(1m, (accumulator, discountValue) => accumulator * (1 - discountValue)) ?? 1;
            return totalDiscountFactor;
        }

        /// <summary>
        /// Returns the Total Discount Percent
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetTotalDiscountPercent()
        {
            return (1 - GetTotalDiscountFactor()) * 100;
        }

        /// <summary>
        /// Returns the Starting Price with Vat Included
        /// </summary>
        /// <returns>The VatIncluded Starting Price</returns>
        public virtual decimal GetStartingPriceWithVat()
        {
            decimal price = StartingPrice * VatFactor;
            return price;
        }

        /// <summary>
        /// Returns the Net Price of the Item (After All Discounts Have Been Applied
        /// </summary>
        /// <returns>The Net Price Decimal</returns>
        public virtual decimal GetNetPrice()
        {
            decimal netPrice = StartingPrice * GetTotalDiscountFactor();
            return netPrice;
        }

        /// <summary>
        /// Gets the Final Price of the Item (Net Price along with Vat)
        /// </summary>
        /// <returns>The Final Price - Vat Included</returns>
        public virtual decimal GetNetPriceWithVat()
        {
            decimal finalPrice = GetNetPrice() * VatFactor;
            return finalPrice;
        }

        public virtual decimal GetVatOnly()
        {
            return GetNetPriceWithVat() - GetNetPrice();
        }
        public virtual decimal GetTotalQuantityVatOnly()
        {
            return GetTotalQuantityNetPriceWithVat() - GetTotalQuantityNetPrice();
        }

        /// <summary>
        /// Returns the Total Net Price for the Specified Item Quantity
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetTotalQuantityNetPrice()
        {
            return GetNetPrice() * Quantity;
        }
        /// <summary>
        /// Returns the Total Net Price with VAT for the Specified Item Quantity
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetTotalQuantityNetPriceWithVat()
        {
            return GetTotalQuantityNetPrice() * VatFactor;
        }

        /// <summary>
        /// Returns the Type of the Wrapped Object (The one Reflecting the Price)
        /// </summary>
        /// <returns>A Type</returns>
        public Type GetWrappedObjectType();

        /// <summary>
        /// Clears all the Applies rules and their Effects (Starting Price = 0 , Applied Rules Cleared , Discounts Cleared)
        /// </summary>
        public void ClearAppliedRules()
        {
            AppliedRules?.Clear();
            Discounts?.Clear();
            StartingPrice = 0;
        }

        /// <summary>
        /// Weather the Priceable has a Certain Modification
        /// </summary>
        /// <param name="modificationType"></param>
        /// <returns></returns>
        public bool HasAppliedModification(RuleModification modificationType)
        {
            return AppliedRules.Any(r => r.ModificationType == modificationType);
        }

        /// <summary>
        /// Returns the Applied Rules that Modified the Discount Percentage of the Priceable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AppliedRule> GetAppliedPercentRulesOnly()
        {
            return AppliedRules.Where(r => r.ModificationType 
            is RuleModification.SetsTotalDiscount 
            or RuleModification.SetsAdditionalDiscountPercent
            or RuleModification.SetsAdditionalPricePercent
            );
        }
        /// <summary>
        /// Returns the Applied Rules that Modified the Price Value of the Priceable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AppliedRule> GetAppliedValueRulesOnly()
        {
            return AppliedRules.Where(r => r.ModificationType 
            is RuleModification.SetsAdditionalPriceValue 
            or RuleModification.SetsAdditionalDiscountValue
            or RuleModification.SetsStartingPrice
            or RuleModification.SetsNetPrice
            );
        }

        /// <summary>
        /// Returns a new Copy (Fresh Discounts and Starting Price) for the same Product at the Same Quantity and Vat as this one
        /// </summary>
        /// <returns></returns>
        public IPriceable GetNewCopy();
    }
}
