using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonInterfaces
{
    public interface IDatabaseEntity
    {
        /// <summary>
        /// The Id of the Entity
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The Time of creation of the Entity
        /// </summary>
        public DateTime Created { get; }
        /// <summary>
        /// The Date of the last modification of the Entity
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Notes for the Entity
        /// </summary>
        public string Notes { get; }
    }
}
