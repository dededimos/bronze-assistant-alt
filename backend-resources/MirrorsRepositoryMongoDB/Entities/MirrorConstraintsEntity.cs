using CommonInterfacesBronze;
using MirrorsLib;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorConstraintsEntity : MongoDatabaseEntity , IDeepClonable<MirrorConstraintsEntity> , IEqualityComparerCreator<MirrorConstraintsEntity>
    {
        public MirrorConstraints Constraints { get; set; } = new();

        public override MirrorConstraintsEntity GetDeepClone()
        {
            MirrorConstraintsEntity clone = new();
            CopyBaseEntityProperties(clone);
            clone.Constraints = this.Constraints.GetDeepClone();
            return clone;
        }
        public MirrorConstraints ToConstraints()
        {
            return Constraints.GetDeepClone();
        }

        static IEqualityComparer<MirrorConstraintsEntity> IEqualityComparerCreator<MirrorConstraintsEntity>.GetComparer()
        {
            return new MirrorConstraintsEntityEqualityComparer();
        }
    }

    public class MirrorConstraintsEntityEqualityComparer : IEqualityComparer<MirrorConstraintsEntity>
    {
        public bool Equals(MirrorConstraintsEntity? x, MirrorConstraintsEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            var constraintsEqualityComparer = MirrorConstraints.GetComparer();
            var baseEntityComparer = new DatabaseEntityEqualityComparer();


            return baseEntityComparer.Equals(x, y) &&
                constraintsEqualityComparer.Equals(x.Constraints, y.Constraints);
        }

        public int GetHashCode([DisallowNull] MirrorConstraintsEntity obj)
        {
            throw new NotSupportedException($"{typeof(MirrorConstraintsEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}
