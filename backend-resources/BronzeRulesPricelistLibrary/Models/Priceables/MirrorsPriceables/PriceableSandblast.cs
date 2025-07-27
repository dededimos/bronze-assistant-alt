using BronzeRulesPricelistLibrary.ConcreteRules;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables
{
    public class PriceableSandblast : IPriceable
    {
        private readonly MirrorSandblast sandblastType;
        public MirrorSandblast SandblastType { get => sandblastType; }
        public string Code { get => GetCode(); }
        public decimal VatFactor { get; set; }
        public decimal StartingPrice { get; set; }
        public List<decimal> Discounts { get; set; } = new();

        public List<AppliedRule> AppliedRules { get; set; } = new();
        public string ThumbnailPhotoPath { get; set; } = "";
        public List<string> DescriptionKeys { get; set; } = new();
        public decimal Quantity { get; set; } = 1;

        public PriceableSandblast(MirrorSandblast sandblast)
        {
            sandblastType = sandblast;
        }

        private string GetCode()
        {
            string code = sandblastType switch
            {
                MirrorSandblast.H8 =>    "0000-H8-BLAST",
                MirrorSandblast.X6 =>    "0000-X6-BLAST",
                MirrorSandblast.X4 =>    "0000-X4-BLAST",
                MirrorSandblast._6000 => "0000-60-BLAST",
                MirrorSandblast.N6 =>    "0000-N6-BLAST",
                MirrorSandblast.M3 =>    "0000-M3-BLAST",
                _ =>                     "-No Sandblast-",
            };
            return code;
        }

        public Type GetWrappedObjectType()
        {
            return typeof(MirrorSandblast);
        }

        public IPriceable GetNewCopy()
        {
            return new PriceableSandblast(SandblastType) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}
