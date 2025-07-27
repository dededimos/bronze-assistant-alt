using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MongoDatabaseEntityEditorViewModel : BaseViewModel , IEditorViewModel<MongoDatabaseEntity>
    {
        public string Id { get; private set; } = ObjectId.Empty.ToString();
        public DateTime Created { get; private set; } = DateTime.MinValue;
        public DateTime LastModified { get; private set; } = DateTime.MinValue;

        [ObservableProperty]
        private string notes = string.Empty;

        public MongoDatabaseEntity CopyPropertiesToModel(MongoDatabaseEntity model)
        {
            model.Id = Id;
            model.LastModified = LastModified;
            model.Notes = Notes;
            return model;
        }
        public MongoDatabaseEntity GetModel()
        {
            return CopyPropertiesToModel(new());
        }
        public void SetModel(MongoDatabaseEntity model)
        {
            this.Id = model.Id;
            this.LastModified = model.LastModified;
            this.Notes = model.Notes;
            this.Created = model.Created;
        }


    }
}
