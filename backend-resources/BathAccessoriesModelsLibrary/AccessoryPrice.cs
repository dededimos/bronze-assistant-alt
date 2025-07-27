namespace BathAccessoriesModelsLibrary
{
    public class AccessoryPrice
    {
        /// <summary>
        /// If Finish Trait is not null Price will be considered for this specific Finish
        /// </summary>
        public AccessoryTrait? FinishTrait { get; set; }
        /// <summary>
        /// If the Finish Trait is null then the Trait Group will be considered for Price
        /// </summary>
        public AccessoryTraitGroup? FinishTraitGroup { get; set; }
        public AccessoryTrait PriceTrait { get; set; } = AccessoryTrait.Empty(TypeOfTrait.PriceTrait);
        public decimal PriceValue { get; set; }
        public string RefersToName { get => GetRefersToName(); }

        public static AccessoryPrice Undefined() => new();
        public bool IsUndefined { get => FinishTrait == null && FinishTraitGroup == null && PriceValue == 0; }

        private string GetRefersToName()
        {
            if (FinishTrait != null)
            {
                return FinishTrait.Trait;
            }
            else if(FinishTraitGroup != null)
            {
                return FinishTraitGroup.DescriptionInfo.Name;
            }
            else
            {
                return "??PriceName??";
            }
        }
    }
}
