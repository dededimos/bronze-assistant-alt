using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels
{
    public partial class DescriptiveEntityViewModel : BaseViewModel, IEditorViewModel<DescriptiveEntity>
    {
        [ObservableProperty]
        private DbEntityViewModel baseEntity;

        [ObservableProperty]
        private LocalizedStringViewModel name;
        [ObservableProperty]
        private LocalizedStringViewModel description;
        [ObservableProperty]
        private LocalizedStringViewModel extendedDescription;

        public DescriptiveEntityViewModel(Func<DbEntityViewModel> baseEntityVmFactory , Func<LocalizedStringViewModel> localizedStringVmFactory)
        {
            BaseEntity = baseEntityVmFactory.Invoke();
            Name = localizedStringVmFactory.Invoke();
            Description = localizedStringVmFactory.Invoke();
            ExtendedDescription = localizedStringVmFactory.Invoke();
        }

        public void SetModel(DescriptiveEntity descriptiveEntity)
        {
            BaseEntity.SetModel(descriptiveEntity);
            Name.SetModel(descriptiveEntity.Name);
            Description.SetModel(descriptiveEntity.Description);
            ExtendedDescription.SetModel(descriptiveEntity.ExtendedDescription);
        }

        public DescriptiveEntity CopyPropertiesToModel(DescriptiveEntity model)
        {
            BaseEntity.CopyPropertiesToModel(model);
            model.Name = this.Name.GetModel();
            model.Description = this.Description.GetModel();
            model.ExtendedDescription = this.ExtendedDescription.GetModel();
            return model;
        }

        public DescriptiveEntity GetModel()
        {
            var model = new DescriptiveEntity();
            return this.CopyPropertiesToModel(model);
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
