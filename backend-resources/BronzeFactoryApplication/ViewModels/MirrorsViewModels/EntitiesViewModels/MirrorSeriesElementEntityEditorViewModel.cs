using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Office2010.Excel;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;
using MongoDB.Bson;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorSeriesElementEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorSeriesElementEntity>, IMirrorEntityEditorViewModel<MirrorSeriesElementEntity>
    {
        public MirrorSeriesElementEntityEditorViewModel(
            Func<IEditorViewModel<MirrorSeriesInfo>> seriesInfoVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IEditModelModalsGenerator editModalsGenerator, IMirrorsDataProvider dataProvider) : base(baseEntityFactory, editModalsGenerator)
        {
            SeriesInfo = seriesInfoVmFactory.Invoke();
            SeriesInfo.PropertyChanged += SeriesInfo_PropertyChanged;
            this.dataProvider = dataProvider;
        }

        public IEditorViewModel<MirrorSeriesInfo> SeriesInfo { get; set; }

        public override MirrorSeriesElementEntity CopyPropertiesToModel(MirrorSeriesElementEntity model)
        {
            base.CopyPropertiesToModel(model);
            var seriesInfo = SeriesInfo.GetModel();
            model.IsCustomizedMirrorSeries = seriesInfo.IsCustomizedMirrorsSeries;
            model.AllowsTransitionToCustomizedMirror = seriesInfo.AllowsTransitionToCustomizedMirror;
            model.Constraints = seriesInfo.Constraints.GetDeepClone();
            model.StandardMirrors = seriesInfo.StandardMirrors.Select(m => 
            {
                var entity = MirrorSynthesisEntity.CreateFromModel(m);
                entity.SeriesReferenceId = this.BaseEntity.Id;
                return entity;
            }).ToList();
            model.CustomizationTriggers = new(seriesInfo.CustomizationTriggers);
            //SeriesInfo.CopyPropertiesToModel(model.SeriesInfo); WILL NOT WORK HERE DIFFERENT TYPES ...

            //Must give an Id to all standard Mirrors if they are not set one (example : when they are new)
            foreach (var mirror in model.StandardMirrors)
            {
                if(ObjectId.TryParse(mirror.Id, out ObjectId objId) is false || string.IsNullOrEmpty(mirror.Id))
                {
                    mirror.Id = ObjectId.GenerateNewId().ToString();
                }

            }
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorSeriesElementEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            SeriesInfo.SetModel(model.ToSeries(dataProvider).SeriesInfo);
        }


        private void SeriesInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SeriesInfo));
        }
        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        private readonly IMirrorsDataProvider dataProvider;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                SeriesInfo.PropertyChanged -= SeriesInfo_PropertyChanged;
                SeriesInfo.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
