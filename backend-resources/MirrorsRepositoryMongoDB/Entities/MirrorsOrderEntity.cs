using CommonInterfacesBronze;
using MirrorsLib.MirrorsOrderModels;
using MirrorsLib.Repositories;
using MongoDbCommonLibrary.CommonEntities;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorsOrderEntity : BronzeOrderEntity , IDeepClonable<MirrorsOrderEntity>
    {
        public List<MirrorOrderRowEntity> Rows { get; set; } = [];
        public MirrorsOrderEntity()
        {
            
        }

        public override MirrorsOrderEntity GetDeepClone()
        {
            var clone = (MirrorsOrderEntity)base.GetDeepClone();
            clone.Rows = Rows.GetDeepClonedList();
            return clone;
        }

        public MirrorsOrder ToMirrorsOrder(IMirrorsDataProvider dataProvider)
        {
            MirrorsOrder order = new();
            this.CopyBasePropertiesToOrder(order);
            order.Rows = this.Rows.Select(r => r.ToMirrorOrderRow(dataProvider)).ToList();
            return order;
        }
        public static MirrorsOrderEntity CreateFromModel(MirrorsOrder model)
        {
            MirrorsOrderEntity entity = new();
            entity.CopyFromModelsBaseProperties(model);
            entity.Rows = model.Rows.Select(r=> MirrorOrderRowEntity.CreateFromModel(r)).ToList();
            return entity;
        }
    }
}
