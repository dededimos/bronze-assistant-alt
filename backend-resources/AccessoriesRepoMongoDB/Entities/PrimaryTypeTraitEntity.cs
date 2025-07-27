using AccessoriesRepoMongoDB.Entities;
using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AccessoriesRepoMongoDB.Entities
{
    public class PrimaryTypeTraitEntity : TraitEntity
    {
        public List<ObjectId> AllowedSecondaryTypes { get; set; } = new();

        public override PrimaryTypeTraitEntity GetDeepClone()
        {
            PrimaryTypeTraitEntity clone = new();
            CopyBaseTraitProperties(clone);
            clone.AllowedSecondaryTypes = new(this.AllowedSecondaryTypes);
            return clone;
        }
    }

    public class PrimaryTypeTraitEntityEqualityComparer : IEqualityComparer<PrimaryTypeTraitEntity>
    {
        public bool Equals(PrimaryTypeTraitEntity? x, PrimaryTypeTraitEntity? y)
        {
            var traitEntityComparer = new TraitEntityEqualityComparer();

            return traitEntityComparer.Equals(x, y) &&
                //nullity excluded from baseEntity Comparer
                x!.AllowedSecondaryTypes.SequenceEqual(y!.AllowedSecondaryTypes);
        }

        public int GetHashCode([DisallowNull] PrimaryTypeTraitEntity obj)
        {
            throw new NotSupportedException($"{typeof(PrimaryTypeTraitEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}
