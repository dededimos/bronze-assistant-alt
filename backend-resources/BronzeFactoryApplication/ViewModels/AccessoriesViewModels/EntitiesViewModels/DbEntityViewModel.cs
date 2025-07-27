using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels
{
    public partial class DbEntityViewModel : BaseViewModel, IEditorViewModel<DbEntity>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IdAsString))]
        private ObjectId id;

        public string IdAsString { get => Id.ToString(); }
        public DateTime Created { get => Id.CreationTime.Date.ToLocalTime().Date; }

        [ObservableProperty]
        private DateTime lastModified;
        [ObservableProperty]
        private string notes = string.Empty;

        public DbEntityViewModel()
        {
            
        }

        /// <summary>
        /// Sets the Properties of the current state of the Model , to the ViewModel
        /// </summary>
        /// <param name="model">The Current State of the Model</param>
        public void SetModel(DbEntity model)
        {
            Id = model.Id;
            LastModified = model.LastModified;
            Notes = model.Notes;
        }

        public DbEntity GetModel()
        {
            DbEntity model = new()
            {
                Id = Id,
                LastModified = LastModified,
                Notes = Notes
            };
            return model;
        }

        public DbEntity CopyPropertiesToModel(DbEntity model)
        {
            model.Id = Id;
            model.LastModified = LastModified;
            model.Notes = Notes;
            return model;
        }
    }
}
