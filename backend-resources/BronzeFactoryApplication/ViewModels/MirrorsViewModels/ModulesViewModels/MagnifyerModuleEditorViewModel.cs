using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MagnifierModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<MagnifierModuleInfo>
    {
        private readonly IEditModelModalsGenerator editModelsModalGenerator;
        private MagnifierModuleInfo model;

        public double Magnification
        {
            get => model.Magnification;
            set => SetProperty(model.Magnification, value, model, (m, v) => model.Magnification = v);
        }

        public double MinDistanceFromSupport
        {
            get => model.MinDistanceFromSupport;
            set => SetProperty(model.MinDistanceFromSupport, value, model, (m, v) => model.MinDistanceFromSupport = v);
        }

        public double MinDistanceFromSandblast
        {
            get => model.MinDistanceFromSandblast;
            set => SetProperty(model.MinDistanceFromSandblast, value, model, (m, v) => model.MinDistanceFromSandblast = v);
        }

        public double MinDistanceFromOtherModules
        {
            get => model.MinDistanceFromOtherModules;
            set => SetProperty(model.MinDistanceFromOtherModules, value, model, (m, v) => model.MinDistanceFromOtherModules = v);
        }

        public CircleInfo MagnifierDimensions
        {
            get => model.MagnifierDimensions;
            set => SetProperty(model.MagnifierDimensions, value, model, (m, v) => model.MagnifierDimensions = v);
        }

        public CircleInfo VisibleMagnifierDimensions
        {
            get => model.VisibleMagnifierDimensions;
            set => SetProperty(model.VisibleMagnifierDimensions, value, model, (m, v) => model.VisibleMagnifierDimensions = v);
        }

        public MagnifierModuleEditorViewModel(IEditModelModalsGenerator editModelsModalGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.editModelsModalGenerator = editModelsModalGenerator;
        }

        [RelayCommand]
        private void EditMagnifierDimensions(CircleInfo dimensionsToEdit)
        {
            EditModelMessage<CircleInfo> message = new(dimensionsToEdit, this);
            editModelsModalGenerator.OpenEditModal(message);
        }

        public MagnifierModuleInfo CopyPropertiesToModel(MagnifierModuleInfo model)
        {
            model.Magnification = this.Magnification;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.MagnifierDimensions = this.MagnifierDimensions.GetDeepClone();
            model.VisibleMagnifierDimensions = this.VisibleMagnifierDimensions.GetDeepClone();
            return model;
        }

        public MagnifierModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(MagnifierModuleInfo model)
        {
            this.model = model.GetDeepClone();
            SetBaseProperties(model);
            OnPropertyChanged(string.Empty);
        }
    }
}
