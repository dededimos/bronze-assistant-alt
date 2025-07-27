using AccessoriesRepoMongoDB.Entities;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels.HelperTraitValueViewModels
{
    public class PriceInfoViewModel : ObservableObject
    {
        public decimal PriceValue { get; set; }
        public TraitEntity PriceTrait { get; set; }
        public TraitGroupEntity? ReferingFinishGroup { get; set; }
        public TraitEntity? ReferingFinishTrait { get; set; }
        public LocalizedString PriceInfoRefersToName { get => GetRefersToName(); }

        private LocalizedString GetRefersToName()
        {
            if (ReferingFinishTrait != null)
            {
                return ReferingFinishTrait.Trait;
            }
            else if(ReferingFinishGroup != null)
            {
                return ReferingFinishGroup.Name;
            }
            else
            {
                return new LocalizedString("Deprecated", new() { { LocalizedString.GREEKIDENTIFIER,"Κατηργημένο"},{ LocalizedString.ENGLISHIDENTIFIER,"Deprecated"},{ LocalizedString.ITALIANIDENTIFIER,"Deprecato"} });
            }
        }

        public PriceInfoViewModel(TraitEntity priceTrait,
                                  decimal priceValue,
                                  TraitEntity? referingFinishTrait,
                                  TraitGroupEntity? referingFinishGroupEntity)
        {
            PriceTrait = priceTrait;
            ReferingFinishGroup = referingFinishGroupEntity;
            ReferingFinishTrait = referingFinishTrait;
            PriceValue = priceValue;
        }

        public PriceInfo GetPriceInfoObject()
        {
            PriceInfo pi = new()
            {
                PriceValue = this.PriceValue,
                PriceTraitId = this.PriceTrait.IdAsString,
                RefersToFinishId = this.ReferingFinishTrait?.IdAsString ?? "",
                RefersToFinishGroupId = this.ReferingFinishGroup?.IdAsString ?? ""
            };
            return pi;
        }


    }
}
