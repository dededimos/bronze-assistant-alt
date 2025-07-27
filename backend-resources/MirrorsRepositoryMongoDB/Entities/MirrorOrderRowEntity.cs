using CommonInterfacesBronze;
using MirrorsLib.MirrorsOrderModels;
using MirrorsLib.Repositories;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorOrderRowEntity : BronzeOrderRowEntity , IDeepClonable<MirrorOrderRowEntity>
    {
        public MirrorSynthesisEntity? RowItem { get; set; }

        public override MirrorOrderRowEntity GetDeepClone()
        {
            var clone = (MirrorOrderRowEntity)base.GetDeepClone();
            clone.RowItem = this.RowItem?.GetDeepClone();
            return clone;
        }

        public MirrorOrderRow ToMirrorOrderRow(IMirrorsDataProvider dataProvider)
        {
            MirrorOrderRow row = new();
            this.CopyBasePropertiesToRow(row);
            row.RowItem = this.RowItem?.ToMirrorSynthesis(dataProvider);
            return row;
        }
        public static MirrorOrderRowEntity CreateFromModel(MirrorOrderRow model)
        {
            MirrorOrderRowEntity entity = new();
            entity.CopyFromModelsBaseProperties(model);
            entity.RowItem = model.RowItem is not null ? MirrorSynthesisEntity.CreateFromModel(model.RowItem) : null;
            return entity;
        }
    }
}
