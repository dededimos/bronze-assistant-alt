using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.IdGenerators;
using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbCommonLibrary.CommonEntities
{
    /// <summary>
    /// OLD BASE ENTITY
    /// </summary>
    public class DbEntity
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        [BsonIgnoreIfDefault] //When an Id is not Provided its value will be ignored and the system will generate one
        public ObjectId Id { get; set; }
        /// <summary>
        /// The String Representation of the Object Id of the Entity
        /// </summary>
        public string IdAsString { get => Id.ToString(); }
        
        /// <summary>
        /// The Date of Creation , The ObjectId Property of MongoDb Has built-in the Timestamp of the Creation of the Document
        /// </summary>
        public DateTime Created { get => Id.CreationTime.ToLocalTime().Date; } //Stored Dates are all in UTC this is why toLocal
        /// <summary>
        /// Last Modified should be in UTC format when saved (Older Repos have wrongful implementation of this => code for accessories and before)
        /// </summary>
        public DateTime LastModified { get; set; }
        public DateTime LastModifiedLocal { get => LastModified.ToLocalTime(); }
        public string Notes { get; set; } = string.Empty;

        public void CopyBaseEntityProperties(DbEntity entity)
        {
            entity.Id = Id;
            entity.LastModified = LastModified;
            entity.Notes = Notes;
        }
    }

    public class DbEntityEqualityComparer : IEqualityComparer<DbEntity>
    {
        public bool Equals(DbEntity? x, DbEntity? y)
        {
            if (ReferenceEquals(x, y))
            {
                // If x and y are the same instance or they are both null, they are equal
                return true;
            }
            //if one of them is null and ther other is not
            if (x is null || y is null)
            {
                return false;
            }
            // else normal
            return x.Id == y.Id && 
                x.LastModified == y.LastModified &&
                x.Notes == y.Notes;
        }

        public int GetHashCode([DisallowNull] DbEntity obj)
        {
            throw new NotSupportedException($"{typeof(DbEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

}
