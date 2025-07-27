using BathAccessoriesModelsLibrary;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Entities
{
    /// <summary>
    /// A group shared among Traits
    /// </summary>
    public class TraitGroupEntity : DescriptiveEntity, IDeepClonable<TraitGroupEntity>
    {
        /// <summary>
        /// The Trait Types that are permitted to use this Group
        /// </summary>
        public HashSet<TypeOfTrait> PermittedTraitTypes { get; set; } = new();
        public string Code { get; set; } = string.Empty;
        public int SortNo { get; set; } = 99999;
        public bool IsEnabled { get; set; }

        public TraitGroupEntity GetDeepClone()
        {
            TraitGroupEntity clone = new()
            {
                Id = Id,
                LastModified = LastModified,
                Notes = Notes,
                Name = this.Name.GetDeepClone(),
                Description = this.Description.GetDeepClone(),
                ExtendedDescription = this.ExtendedDescription.GetDeepClone(),
                SortNo = SortNo,
                Code = Code,
                PermittedTraitTypes = new(this.PermittedTraitTypes),
                IsEnabled = IsEnabled
            };
            return clone;
        }
    }

    public class TraitGroupEntityEqualityComparer : IEqualityComparer<TraitGroupEntity>
    {
        public bool Equals(TraitGroupEntity? x, TraitGroupEntity? y)
        {
            var descriptiveEntityComparer = new DescriptiveEntityEqualityComparer();

            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return descriptiveEntityComparer.Equals(x, y) &&
                x.Code == y.Code &&
                x.SortNo == y.SortNo &&
                x.IsEnabled == y.IsEnabled &&
                x.PermittedTraitTypes.SequenceEqual(y.PermittedTraitTypes);
        }

        public int GetHashCode([DisallowNull] TraitGroupEntity obj)
        {
            throw new NotSupportedException($"{typeof(TraitGroupEntityEqualityComparer).Name} does not Support a Get HashCode Implementation");
        }
    }

}
