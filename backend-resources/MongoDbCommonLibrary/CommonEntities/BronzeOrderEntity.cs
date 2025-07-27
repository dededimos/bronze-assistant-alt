using CommonInterfacesBronze;
using CommonOrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonEntities
{
    public class BronzeOrderEntity : MongoDatabaseEntity
    {
        public string OrderNo { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = [];

        public override BronzeOrderEntity GetDeepClone()
        {
            var clone = (BronzeOrderEntity)base.GetDeepClone();
            clone.Metadata = new(this.Metadata);
            return clone;
        }

        public BronzeOrderBase<TRow, TItem> CopyBasePropertiesToOrder<TRow, TItem>(BronzeOrderBase<TRow, TItem> order)
            where TRow : BronzeOrderRowBase<TItem>, IDeepClonable<TRow>
            where TItem : class, ICodeable, IDeepClonable<TItem>
        {
            order.Id = this.Id;
            order.Created = this.Created;
            order.LastModified = this.LastModified;
            order.Notes = this.Notes;
            order.OrderNo = this.OrderNo;
            order.Metadata = new(this.Metadata);
            return order;
        }
        /// <summary>
        /// Copies the Base Model properties to this Entity
        /// </summary>
        /// <typeparam name="TRow"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="baseModel"></param>
        public void CopyFromModelsBaseProperties<TRow, TItem>(BronzeOrderBase<TRow,TItem> baseModel)
            where TRow : BronzeOrderRowBase<TItem>, IDeepClonable<TRow>
            where TItem : class, ICodeable, IDeepClonable<TItem>
        {
            this.Id = baseModel.Id;
            this.LastModified = baseModel.LastModified;
            this.Notes = baseModel.Notes;
            this.OrderNo = baseModel.OrderNo;
            this.Metadata = new(baseModel.Metadata);
            this.Status = baseModel.Status;
        }
    }
}
