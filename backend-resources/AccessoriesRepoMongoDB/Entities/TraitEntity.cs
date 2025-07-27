using BathAccessoriesModelsLibrary;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System.Diagnostics.CodeAnalysis;

namespace AccessoriesRepoMongoDB.Entities
{
    public class TraitEntity : DbEntity, IDeepClonable<TraitEntity>
    {
        /// <summary>
        /// Indicated the Order in which the Trait should Appear , a Lower Number Appears first in results
        /// </summary>
        public int SortNo { get; set; } = 99999;
        public bool IsEnabled { get; set; } = false;
        public TypeOfTrait TraitType { get; set; }
        public string PhotoURL { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public LocalizedString Trait { get; set; } = LocalizedString.Undefined();
        public LocalizedString TraitTooltip { get; set; } = LocalizedString.Undefined();
        /// <summary>
        /// The Trait Groups Ids that this Trait has Assigned to It
        /// </summary>
        public HashSet<string> AssignedGroups { get; set; } = new();

        /// <summary>
        /// Returns a Deep Clone of the Trait Entity
        /// </summary>
        /// <returns></returns>
        public virtual TraitEntity GetDeepClone()
        {
            TraitEntity clone = new();
            this.CopyBaseTraitProperties(clone);
            return clone;
        }

        /// <summary>
        /// Copies the TraitEntities Properties to the passed Clone
        /// </summary>
        /// <param name="clone"></param>
        protected void CopyBaseTraitProperties(TraitEntity clone)
        {
            this.CopyBaseEntityProperties(clone);
            clone.SortNo = SortNo;
            clone.IsEnabled = IsEnabled;
            clone.TraitType = TraitType;
            clone.PhotoURL = PhotoURL;
            clone.Code = Code;
            clone.Trait = this.Trait.GetDeepClone();
            clone.TraitTooltip = this.TraitTooltip.GetDeepClone();
            clone.AssignedGroups = new(this.AssignedGroups);
        }
    }

    public class TraitEntityEqualityComparer : IEqualityComparer<TraitEntity>
    {
        public bool Equals(TraitEntity? x, TraitEntity? y)
        {
            var baseEntityComparer = new DbEntityEqualityComparer();
            var localizedStringComparer = new LocalizedStringEqualityComparer();
            bool areSecondaryTypesSame = true;
            
            // Hack to not make another Trait Entity Comparer
            if (x is PrimaryTypeTraitEntity ptX && y is PrimaryTypeTraitEntity ptY)
            {
                areSecondaryTypesSame = ptX.AllowedSecondaryTypes.SequenceEqual(ptY.AllowedSecondaryTypes);
            }
            else if ((x is PrimaryTypeTraitEntity && y is not PrimaryTypeTraitEntity) || 
                x is not PrimaryTypeTraitEntity && y is PrimaryTypeTraitEntity)
            {
                areSecondaryTypesSame = false;
            }

            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return baseEntityComparer.Equals(x, y) &&
            //The base comparer checks for nullability 
            x!.TraitType == y!.TraitType &&
            x.IsEnabled == y.IsEnabled &&
            x.PhotoURL == y.PhotoURL &&
            x.Code == y.Code &&
            x.SortNo == y.SortNo &&
            x.AssignedGroups.SequenceEqual(y.AssignedGroups) &&
            localizedStringComparer.Equals(x.Trait,y.Trait) &&
            localizedStringComparer.Equals(x.TraitTooltip, y.TraitTooltip) && 
            areSecondaryTypesSame;
        }

        public int GetHashCode([DisallowNull] TraitEntity obj)
        {
            throw new NotSupportedException($"{typeof(DbEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}
