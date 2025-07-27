using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    public class UserAccessoriesOptions
    {
        public string Id { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public List<AccessoryTrait> AppearingDimensions { get; set; } = new();
        public AccessoryTraitGroup? PricesGroup { get; set; }
        public UserAccessoriesDiscounts Discounts { get; set; } = UserAccessoriesDiscounts.Empty();
        public List<CustomPriceRule> CustomPriceRules { get; set; } = new();

        public static UserAccessoriesOptions Undefined() => new();
    }
    public class UserAccessoriesDiscounts : IDeepClonable<UserAccessoriesDiscounts>
    {
        /// <summary>
        /// The General Discount in the Provided Catalogue Prices (ex. for 40% discount the value is 0.4)
        /// </summary>
        public decimal MainDiscountDecimal { get; set; }
        public decimal MainDiscountPercent { get => MainDiscountDecimal * 100m; }
        /// <summary>
        /// Secondary Discount in 0.00Format (ex. for 40% discount the value is 0.4)
        /// </summary>
        public decimal SecondaryDiscountDecimal { get; set; }
        public decimal SecondaryDiscountPercent { get => SecondaryDiscountDecimal * 100m; }
        /// <summary>
        /// Tertiary Discount in 0.00Format (ex. for 40% discount the value is 0.4)
        /// </summary>
        public decimal TertiaryDiscountDecimal { get; set; }
        public decimal TertiaryDiscountPercent { get => TertiaryDiscountDecimal * 100m; }

        public decimal QuantityDiscPrimary { get; set; }
        public int QuantityDiscQuantityPrimary { get; set; }
        public decimal QuantityDiscSecondary { get; set; }
        public int QuantityDiscQuantitySecondary { get; set; }
        public decimal QuantityDiscTertiary { get; set; }
        public int QuantityDiscQuantityTertiary { get; set; }

        public static UserAccessoriesDiscounts Empty() => new();

        /// <summary>
        /// The Total Discount Factor (The number with which to multiply Initial Price to find the Net Price)
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalBasicDiscountFactor() => (1 - MainDiscountDecimal) * (1 - SecondaryDiscountDecimal) * (1 - TertiaryDiscountDecimal);
        /// <summary>
        /// The Total Discount (The number with which to multiply the Initial Price to find the Total Value of the Discount)
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalBasicDiscountDecimal() => 1 - (1 - MainDiscountDecimal) * (1 - SecondaryDiscountDecimal) * (1 - TertiaryDiscountDecimal);
        /// <summary>
        /// The Total Discount %
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalBasicDiscountPercent() => GetTotalBasicDiscountDecimal() * 100m;
        /// <summary>
        /// The Total Discount in decimal Format ex. 0.5 
        /// The Number with which to multiply Price to find the Total Discounted Value
        /// </summary>
        /// <param name="qty"></param>
        /// <returns></returns>
        public decimal GetTotalDiscount(int qty)
        {
            decimal totalDiscountFactor = GetTotalBasicDiscountFactor();
            if (qty >= QuantityDiscQuantityPrimary) totalDiscountFactor *= 1 - QuantityDiscPrimary;
            if (qty >= QuantityDiscQuantitySecondary) totalDiscountFactor *= 1 - QuantityDiscSecondary;
            if (qty >= QuantityDiscQuantityTertiary) totalDiscountFactor *= 1 - QuantityDiscTertiary;
            return 1 - totalDiscountFactor;
        }


        public UserAccessoriesDiscounts GetDeepClone()
        {
            return (UserAccessoriesDiscounts)MemberwiseClone();
        }
    }
    public class UserAccessoriesOptionsDTO
    {
        public string Id { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public string AppearingDimensionsGroupId { get; set; } = string.Empty;
        public string PricesGroupId { get; set; } = string.Empty;
        public UserAccessoriesDiscounts Discounts { get; set; } = new();
        public List<string> CustomPriceRules { get; set; } = new();

        public static UserAccessoriesOptionsDTO Undefined() => new();

        public UserAccessoriesOptions ToAccessoriesOptions(List<AccessoryTrait> dimensionTraits,Dictionary<string,AccessoryTraitGroup> groups,List<CustomPriceRule> customRules)
        {
            UserAccessoriesOptions options = new()
            {
                Id = Id,
                DescriptionInfo = this.DescriptionInfo.GetDeepClone(),
                AppearingDimensions = dimensionTraits.Where(t=> t.Groups.Any(g=>g.Id == this.AppearingDimensionsGroupId)).ToList(),
                PricesGroup = groups.TryGetValue(this.PricesGroupId,out var group) ? group : null,
                Discounts = this.Discounts.GetDeepClone(),
                CustomPriceRules = customRules.Where(cr=> this.CustomPriceRules.Any(id=> id == cr.Id)).ToList()
            };
            return options;
        }
    }
}

