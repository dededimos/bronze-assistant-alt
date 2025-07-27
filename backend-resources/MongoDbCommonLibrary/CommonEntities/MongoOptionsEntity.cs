using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonEntities
{
    public interface IOptionsEntity : IDatabaseEntity
    {
        string OptionsType { get; }
        BronzeApplicationType ConcerningApplication { get; }
    }

    public class MongoOptionsEntity : MongoDatabaseEntity, IOptionsEntity, IDeepClonable<MongoOptionsEntity>, IEqualityComparerCreator<MongoOptionsEntity>
    {
        public string OptionsType { get; set; } = string.Empty;
        public BronzeApplicationType ConcerningApplication { get; set; }

        static IEqualityComparer<MongoOptionsEntity> IEqualityComparerCreator<MongoOptionsEntity>.GetComparer()
        {
            return new MongoOptionsEntityEqualityComparer();
        }

        public override MongoOptionsEntity GetDeepClone()
        {
            return (MongoOptionsEntity)base.GetDeepClone();
        }
    }

    public enum BronzeApplicationType
    {
        UnspecifiedApplication = 0,
        FactoryAppApplication = 1,
        WebAppApplication = 2,
        BothFactoryAndWebAppApplication = 3,
    }

    public class MongoOptionsEntityEqualityComparer : IEqualityComparer<MongoOptionsEntity>
    {
        private readonly DatabaseEntityEqualityComparer baseComparer = new();

        public bool Equals(MongoOptionsEntity? x, MongoOptionsEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return baseComparer.Equals(x, y)
                && x.OptionsType == y.OptionsType
                && x.ConcerningApplication == y.ConcerningApplication;
        }

        public int GetHashCode([DisallowNull] MongoOptionsEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
