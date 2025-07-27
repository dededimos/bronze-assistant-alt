using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class AccessoryTraitGroup : IDeepClonable<AccessoryTraitGroup>
    {
        public string Id { get; set; } = string.Empty;
        public int SortNo { get; set; }
        public string Code { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();

        public AccessoryTraitGroup GetDeepClone()
        {
            var clone = (AccessoryTraitGroup)MemberwiseClone();
            clone.DescriptionInfo = this.DescriptionInfo.GetDeepClone();
            return clone;
        }

        public static AccessoryTraitGroup Empty() => new();

        //The Equals method Here is not Overriden , we want Reference Equality At All Times , All TraitGroups Must be Unique
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            // Simply Check for the Id
            return Id.GetHashCode();
        }
    }
}
