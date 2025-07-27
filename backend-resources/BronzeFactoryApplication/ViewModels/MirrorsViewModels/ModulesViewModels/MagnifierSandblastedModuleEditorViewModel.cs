using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MagnifierSandblastedModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<MagnifierSandblastedModuleInfo>
    {
        private readonly IEditModelModalsGenerator editModelsModalGenerator;
        protected MagnifierSandblastedModuleInfo model;

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

        public CircleRingInfo SandblastDimensions
        {
            get => model.SandblastDimensions;
            set => SetProperty(model.SandblastDimensions, value, model, (m, v) => model.SandblastDimensions = v);
        }

        public MagnifierSandblastedModuleEditorViewModel(IEditModelModalsGenerator editModelsModalGenerator)
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

        [RelayCommand]
        private void EditSandblastedMagnifierDimensions(CircleRingInfo sandblastToEdit)
        {
            EditModelMessage<CircleRingInfo> message = new(sandblastToEdit, this);
            editModelsModalGenerator.OpenEditModal(message);
        }

        public MagnifierSandblastedModuleInfo CopyPropertiesToModel(MagnifierSandblastedModuleInfo model)
        {
            CopyBasePropertiesToModel(model);
            model.Magnification = this.Magnification;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.MagnifierDimensions = this.MagnifierDimensions.GetDeepClone();
            model.VisibleMagnifierDimensions = this.VisibleMagnifierDimensions.GetDeepClone();
            model.SandblastDimensions = this.SandblastDimensions.GetDeepClone();
            return model;
        }

        public MagnifierSandblastedModuleInfo GetModel()
        {
            return this.model.GetDeepClone();
        }

        public void SetModel(MagnifierSandblastedModuleInfo model)
        {
            SuppressPropertyNotifications();
            this.model = model.GetDeepClone();
            SetBaseProperties(model);
            ResumePropertyNotifications();
            OnPropertyChanged(string.Empty);
        }
    }
}
