using BathAccessoriesModelsLibrary;
using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System.Diagnostics.CodeAnalysis;
using static BathAccessoriesModelsLibrary.AccessoryTrait;

namespace AccessoriesRepoMongoDB.Entities
{
    public class TraitClassEntity : DescriptiveEntity, IDeepClonable<TraitClassEntity>
    {
        /// <summary>
        /// Indicated the Order in which the Trait Class should Appear , a Lower Number Appears first in results
        /// </summary>
        public int SortNo { get; set; } = 99999;
        public virtual TypeOfTrait TraitType { get; set; }
        public bool IsEnabled { get; set; }
        public string PhotoURL { get; set; } = string.Empty;
        public HashSet<ObjectId> Traits { get; set; } = new();

        public TraitClassEntity GetDeepClone()
        {
            TraitClassEntity clone = new()
            {
                Id = Id,
                LastModified = LastModified,
                Notes = Notes,
                Name = this.Name.GetDeepClone(),
                Description = this.Description.GetDeepClone(),
                ExtendedDescription = this.ExtendedDescription.GetDeepClone(),
                SortNo = this.SortNo,
                TraitType = this.TraitType,
                IsEnabled = this.IsEnabled,
                PhotoURL = this.PhotoURL,
                Traits = new(this.Traits)
            };
            return clone;
        }
    }

    public class TraitClassEntityEqualityComparer : IEqualityComparer<TraitClassEntity>
    {
        public bool Equals(TraitClassEntity? x, TraitClassEntity? y)
        {
            var descriptiveEntityComparer = new DescriptiveEntityEqualityComparer();

            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return descriptiveEntityComparer.Equals(x, y) &&
            x!.TraitType == y!.TraitType &&
            x.IsEnabled == y.IsEnabled &&
            x.PhotoURL == y.PhotoURL &&
            x.SortNo == y.SortNo &&
            x.Traits.SequenceEqual(y.Traits);
        }

        public int GetHashCode([DisallowNull] TraitClassEntity obj)
        {
            throw new NotSupportedException($"{typeof(TraitClassEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

}
