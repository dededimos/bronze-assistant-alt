using BronzeRulesPricelistLibrary.ConcreteRules;
using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    public class PriceableWFrame : IPriceable
    {
        public CabinFinishEnum FrameFinish { get; }
        public string Code { get => GetCode(); }
        public decimal VatFactor { get; set; }
        public decimal StartingPrice { get; set; }
        public List<decimal> Discounts { get; set; } = new();
        public List<AppliedRule> AppliedRules { get; set; } = new();
        public string ThumbnailPhotoPath { get; set; } = string.Empty;
        public List<string> DescriptionKeys { get; set; } = new();
        public decimal Quantity { get; set; } = 1;

        public PriceableWFrame(CabinFinishEnum finish)
        {
            FrameFinish = finish;
        }

        public Type GetWrappedObjectType()
        {
            return typeof(PriceableWFrame);
        }

        private string GetCode()
        {
            return FrameFinish switch
            {
                CabinFinishEnum.Polished => "0050-10-FRAME",
                CabinFinishEnum.Brushed => "0050-11-FRAME",
                CabinFinishEnum.BlackMat => "0050-MM-FRAME",
                CabinFinishEnum.WhiteMat => "0050-AA-FRAME",
                CabinFinishEnum.Bronze => "0050-40-FRAME",
                CabinFinishEnum.BrushedGold => "0050-21-FRAME",
                CabinFinishEnum.Gold => "0050-20-FRAME",
                CabinFinishEnum.Copper => "0050-50-FRAME",
                _ => "N/A",
            };
        }

        public IPriceable GetNewCopy()
        {
            return new PriceableWFrame(FrameFinish) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}
