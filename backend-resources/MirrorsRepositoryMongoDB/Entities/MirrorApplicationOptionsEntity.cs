using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorApplicationOptionsEntity : MongoOptionsEntity, IDeepClonable<MirrorApplicationOptionsEntity>, IEqualityComparerCreator<MirrorApplicationOptionsEntity>
    {
        public MirrorApplicationOptionsBase Options { get; set; } = MirrorApplicationOptionsBase.Empty();


        static IEqualityComparer<MirrorApplicationOptionsEntity> IEqualityComparerCreator<MirrorApplicationOptionsEntity>.GetComparer()
        {
            return new MirrorApplicationOptionsEntityEqualityComparer();
        }

        public override MirrorApplicationOptionsEntity GetDeepClone()
        {
            return (MirrorApplicationOptionsEntity)base.GetDeepClone();
        }
    }

    public class MirrorApplicationOptionsEntityEqualityComparer : IEqualityComparer<MirrorApplicationOptionsEntity>
    {
        private readonly MongoOptionsEntityEqualityComparer baseComparer = new();
        private readonly MirrorApplicationOptionsBaseEqualityComparer mirrorOptionsComparer = new();

        public bool Equals(MirrorApplicationOptionsEntity? x, MirrorApplicationOptionsEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return baseComparer.Equals(x, y)
                && mirrorOptionsComparer.Equals(x.Options,y.Options);
        }

        public int GetHashCode([DisallowNull] MirrorApplicationOptionsEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
