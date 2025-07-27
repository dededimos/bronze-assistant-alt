using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class PriceInfoDTO
    {
        public decimal PriceValue { get; set; }
        public string PriceId { get; set; } = string.Empty;
        public string FinishId { get; set; } = string.Empty;
        public string FinishGroupId { get; set; } = string.Empty;

        public AccessoryPrice ToAccessoryPrice(Dictionary<string,AccessoryTrait> traits,Dictionary<string,AccessoryTraitGroup> groups)
        {
            var priceTrait = traits.TryGetValue(this.PriceId, out AccessoryTrait? foundPrice) ? foundPrice : AccessoryTrait.Empty(TypeOfTrait.PriceTrait);
            var finishTrait = traits.TryGetValue(this.FinishId, out AccessoryTrait? foundFinish) ? foundFinish : null;
            var finishTraitGroup = groups.TryGetValue(this.FinishGroupId, out AccessoryTraitGroup? foundFinishGroup) ? foundFinishGroup : null;
            AccessoryPrice price = new()
            {
                PriceTrait = priceTrait,
                FinishTrait = finishTrait,
                FinishTraitGroup = finishTraitGroup,
                PriceValue = this.PriceValue,
        };
            return price;
        }
    }

}
