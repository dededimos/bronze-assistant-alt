using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.ExtensionMethods
{
    public static class MongoDbExtensions
    {
        /// <summary>
        /// Converts an Enumerable of <see cref="ObjectId"/> into a <see cref="List{string}"/>
        /// </summary>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        public static List<string> ToListStringIds(this IEnumerable<ObjectId> objectIds)
        {
            return objectIds.Select(oid=> oid.ToString()).ToList();
        }
    }
}
