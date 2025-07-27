using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbCommonLibrary.CommonInterfaces;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbCommonLibrary.CommonEntities
{
    public class MongoDatabaseEntity : IDatabaseEntity, IEqualityComparerCreator<MongoDatabaseEntity>, IDeepClonable<MongoDatabaseEntity>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("Created")]
        public DateTime Created { get => ObjectId.TryParse(Id, out ObjectId objId) ? objId.CreationTime.ToLocalTime().Date : DateTime.MinValue; }
        private DateTime lastModified;
        public DateTime LastModified { get => lastModified.ToLocalTime().Date; set => lastModified = value; }
        public string Notes { get; set; } = string.Empty;

        public static IEqualityComparer<MongoDatabaseEntity> GetComparer()
        {
            return new DatabaseEntityEqualityComparer();
        }

        public virtual MongoDatabaseEntity GetDeepClone()
        {
            return (MongoDatabaseEntity)MemberwiseClone();
        }

        /// <summary>
        /// Copies the Properties of the Base Entity
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CopyBaseEntityProperties(MongoDatabaseEntity entity)
        {
            entity.Id = Id;
            entity.LastModified = LastModified;
            entity.Notes = Notes;
        }
    }
    public class DatabaseEntityEqualityComparer : IEqualityComparer<IDatabaseEntity>
    {
        public bool Equals(IDatabaseEntity? x, IDatabaseEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Id == y.Id &&
                x.LastModified == y.LastModified &&
                x.Notes == y.Notes;
        }

        public int GetHashCode([DisallowNull] IDatabaseEntity obj)
        {
            throw new NotSupportedException($"{typeof(DatabaseEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}
