using CommonInterfacesBronze;

namespace BathAccessoriesModelsLibrary
{
    public class AccessoryTrait : IDeepClonable<AccessoryTrait>
    {
        public const string EmptyAccessoryTraitId = "Empty";

        /// <summary>
        /// The Id of the Trait
        /// </summary>
        public string Id { get; set; } = string.Empty;
        public int SortNo { get; set; } = 99999;
        public TypeOfTrait TraitType { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PhotoURL { get; set; } = string.Empty;
        public string Trait { get; set; } = string.Empty;
        public string TraitTooltip { get; set; } = string.Empty;
        public HashSet<AccessoryTraitGroup> Groups { get; set; } = new();
        public List<AccessoryTrait> SecondaryTypes { get; set; } = new();
        public List<string> SecondaryTypesIds { get; set; } = new();

        public static AccessoryTrait Undefined() => new() { Id = "Undefined", Code = "-", Trait = "-" };
        public static AccessoryTrait Empty(TypeOfTrait type) => new() { Id = EmptyAccessoryTraitId, TraitType = type, SortNo = 99999 };
        public bool IsEmpty { get => Id == EmptyAccessoryTraitId; }

        public AccessoryTrait GetDeepClone()
        {
            var clone = (AccessoryTrait)this.MemberwiseClone();
            clone.Groups = new(this.Groups);
            clone.SecondaryTypes = new(this.SecondaryTypes);
            return clone;
        }

        //The Equals method Here is not Overriden , we want Reference Equality At All Times , All Traits Must be Unique
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

    public class AccessoryTraitDTO : IDeepClonable<AccessoryTraitDTO>
    {
        public string Id { get; set; } = string.Empty;
        public int SortNo { get; set; }
        public TypeOfTrait TraitType { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Trait { get; set; } = string.Empty;
        public string TraitTooltip { get; set; } = string.Empty;
        public HashSet<string> GroupsIds { get; set; } = new();
        public List<string> SecondaryTypes { get; set; } = new();

        public AccessoryTraitDTO GetDeepClone()
        {
            var clone = (AccessoryTraitDTO)this.MemberwiseClone();
            clone.GroupsIds = new(this.GroupsIds);
            clone.SecondaryTypes = new(this.SecondaryTypes);
            return clone;
        }

        /// <summary>
        /// Transforms the DTO Object into an Accessory Trait without including its Secondary Types
        /// </summary>
        /// <param name="traitGroups"></param>
        /// <returns></returns>
        public AccessoryTrait ToAccessoryTraitWOSecondTypes(Dictionary<string, AccessoryTraitGroup> traitGroups)
        {
            AccessoryTrait trait = new()
            {
                Id = this.Id,
                SortNo = this.SortNo,
                TraitType = this.TraitType,
                Code = this.Code,
                PhotoURL = this.PhotoUrl,
                Trait = this.Trait,
                TraitTooltip = this.TraitTooltip,
                SecondaryTypesIds = new(this.SecondaryTypes)
            };
            
            //Find the matching Groups and add Them if they exist
            foreach (var groupId in GroupsIds)
            {
                if (traitGroups.TryGetValue(groupId, out var group))
                {
                    trait.Groups.Add(group);
                }
            }

            return trait;
        }
    }

}
