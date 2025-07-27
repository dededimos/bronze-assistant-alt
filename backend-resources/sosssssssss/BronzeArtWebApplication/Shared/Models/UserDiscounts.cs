using BathAccessoriesModelsLibrary.AccessoriesUserOptions;

namespace BronzeArtWebApplication.Shared.Models
{
#nullable enable
    public class UserDiscounts
    {
        public DiscountsStash CabinsDiscounts { get; set; }
        public DiscountsStash MirrorsDiscounts { get; set; }
        public DiscountsStash AccessoriesDiscounts { get; set; }

        public UserDiscounts(BronzeUser user , UserAccessoriesOptions userAccessoriesOptions)
        {
            CabinsDiscounts = new(user.PrimaryDiscountCabin * 0.01m, user.SecondaryDiscountCabin * 0.01m, user.TertiaryDiscountCabin * 0.01m);
            MirrorsDiscounts = new(user.PrimaryDiscount * 0.01m, user.SecondaryDiscount * 0.01m, user.TertiaryDiscount * 0.01m);
            AccessoriesDiscounts = new(userAccessoriesOptions.Discounts.MainDiscountDecimal, userAccessoriesOptions.Discounts.SecondaryDiscountDecimal, userAccessoriesOptions.Discounts.TertiaryDiscountDecimal);
        }
    }

    /// <summary>
    /// An object providing Discounts
    /// </summary>
    public class DiscountsStash
    {
        public decimal PrimaryDecimal { get; set; }
        public decimal SecondaryDecimal { get; set; }
        public decimal TertiaryDecimal { get; set; }

        //Calculated only
        public decimal TotalDecimal { get => 1 - (PrimaryFactor * SecondaryFactor * TertiaryFactor); }
        public decimal TotalPercent { get => TotalDecimal * 100; }
        public decimal PrimaryPercent { get => PrimaryDecimal * 100; }
        public decimal SecondaryPercent { get => SecondaryDecimal * 100; }
        public decimal TertiaryPercent { get => TertiaryDecimal * 100; }
        public decimal TotalFactor { get; set; }
        public decimal PrimaryFactor { get => (1 - PrimaryDecimal); }
        public decimal SecondaryFactor { get => (1 - SecondaryDecimal); }
        public decimal TertiaryFactor { get => (1 - TertiaryDecimal); }

        public DiscountsStash(decimal primaryDecimal = 0, decimal secondaryDecimal = 0, decimal tertiaryDecimal = 0)
        {
            PrimaryDecimal = primaryDecimal;
            SecondaryDecimal = secondaryDecimal;
            TertiaryDecimal = tertiaryDecimal;
        }
    }
}
