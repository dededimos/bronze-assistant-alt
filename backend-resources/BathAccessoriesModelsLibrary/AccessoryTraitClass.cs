using CommonInterfacesBronze;
using static BathAccessoriesModelsLibrary.AccessoryTrait;

namespace BathAccessoriesModelsLibrary
{
    public class AccessoryTraitClass : IDeepClonable<AccessoryTraitClass>
    {
        public int SortNo { get; set; }
        public virtual TypeOfTrait TraitType { get; set; }
        public string PhotoURL { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public Dictionary<string,AccessoryTrait> Traits { get; set; } = new();

        public static AccessoryTraitClass Empty(TypeOfTrait type) => new() { TraitType = type };

        public AccessoryTraitClass GetDeepClone()
        {
            var clone = (AccessoryTraitClass)this.MemberwiseClone();
            clone.DescriptionInfo = this.DescriptionInfo.GetDeepClone();
            clone.Traits = this.Traits.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            return clone;
        }
    }

    public class AccessoryTraitClassDTO : IDeepClonable<AccessoryTraitClassDTO>
    {
        public int SortNo { get; set; }
        public TypeOfTrait TraitType { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public List<string> AccessoryTraitsIds { get; set; } = new();

        public AccessoryTraitClassDTO GetDeepClone()
        {
            var clone = (AccessoryTraitClassDTO)this.MemberwiseClone();
            clone.DescriptionInfo = this.DescriptionInfo.GetDeepClone();
            clone.AccessoryTraitsIds = new(this.AccessoryTraitsIds);
            return clone;
        }

        public AccessoryTraitClass ToAccessoryTraitClass(Dictionary<string,AccessoryTrait> traits)
        {
            AccessoryTraitClass tc = new()
            {
                SortNo = this.SortNo,
                TraitType = this.TraitType,
                PhotoURL = this.PhotoUrl,
                DescriptionInfo = this.DescriptionInfo.GetDeepClone(),
            };

            foreach (var id in AccessoryTraitsIds)
            {
                if (traits.TryGetValue(id,out var trait))
                {
                    tc.Traits.Add(trait.Id,trait);
                }
            }

            return tc;
        }
    }
}
