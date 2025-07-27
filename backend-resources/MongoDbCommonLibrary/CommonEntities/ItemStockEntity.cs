using CommonInterfacesBronze;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonEntities
{
    [BsonIgnoreExtraElements]
    public class ItemStockEntity : MongoDatabaseEntity , IDeepClonable<ItemStockEntity>
    {
        public decimal Quantity { get; set; }

        public string Code { get; set; } = string.Empty;

        public override ItemStockEntity GetDeepClone()
        {
            return (ItemStockEntity)base.GetDeepClone();
        }
    }
}
