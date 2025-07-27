using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MirrorsLib;
using MirrorsLib.Services.PositionService;
using MirrorsRepositoryMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorConstraintsEntityEditorViewModel : MongoEntityBaseUndoEditorViewModel<MirrorConstraintsEntity>, IMirrorEntityEditorViewModel<MirrorConstraintsEntity>
    {
        public IEditorViewModel<MirrorConstraints> Constraints { get; }

        public MirrorConstraintsEntityEditorViewModel(
            Func<IEditorViewModel<MirrorConstraints>> constraintsVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityVmFactory)
            : base(baseEntityVmFactory)
        {
            Constraints = constraintsVmFactory.Invoke();
            Constraints.PropertyChanged += Constraints_PropertyChanged;
        }

        private void Constraints_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Constraints));
        }

        public override MirrorConstraintsEntity CopyPropertiesToModel(MirrorConstraintsEntity model)
        {
            base.CopyPropertiesToModel(model);
            Constraints.CopyPropertiesToModel(model.Constraints);
            return model;
        }

        protected override void SetModelWithoutUndoStore(MirrorConstraintsEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            Constraints.SetModel(model.Constraints);
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                Constraints.PropertyChanged -= Constraints_PropertyChanged;
                Constraints.Dispose();
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
