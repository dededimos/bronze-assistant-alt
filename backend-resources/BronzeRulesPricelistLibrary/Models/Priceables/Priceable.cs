using BronzeRulesPricelistLibrary.ConcreteRules;
using CommonInterfacesBronze;

namespace BronzeRulesPricelistLibrary.Models.Priceables
{
    public abstract class Priceable<T> : IPriceable<T> where T : ICodeable
    {
        public T Product { get; }

        public virtual string Code { get => Product.Code; }
        public decimal VatFactor { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal Quantity { get; set; } = 1;
        public List<decimal> Discounts { get; set; } = new();
        public List<AppliedRule> AppliedRules { get; set; } = new();
        public virtual string ThumbnailPhotoPath { get; set; } = string.Empty;
        public List<string> DescriptionKeys { get; set; } = new();

        public Priceable(T product)
        {
            Product = product;
        }

        public Type GetWrappedObjectType()
        {
            return Product.GetType();
        }

        public abstract IPriceable GetNewCopy();
    }
}
