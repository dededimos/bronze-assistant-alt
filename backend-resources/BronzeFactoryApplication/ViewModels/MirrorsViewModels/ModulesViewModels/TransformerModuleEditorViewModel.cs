using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using Microsoft.EntityFrameworkCore.Metadata;
using MirrorsLib;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class TransformerModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<TransformerModuleInfo>
    {
        public TransformerModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }
        private readonly IEditModelModalsGenerator modalsGenerator;
        protected TransformerModuleInfo model;

        public double Watt
        {
            get => model.Watt;
            set => SetProperty(model.Watt, value, model, (m, v) => model.Watt = v);
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

        public RectangleInfo TransformerDimensions
        {
            get => model.TransformerDimensions;
            set => SetProperty(model.TransformerDimensions, value, model, (m, v) => model.TransformerDimensions = v);
        }

        public IPRating IP
        {
            get => model.IP;
            set => SetProperty(model.IP, value, model, (m, v) => model.IP = v);
        }     
        
        [RelayCommand]
        private void EditDimensions(RectangleInfo dimensionsToEdit)
        {
            EditModelMessage<RectangleInfo> message = new(dimensionsToEdit, this);
            modalsGenerator.OpenEditModal(message);
        }
        [RelayCommand]
        private void EditIPRating(IPRating ipRating)
        {
            EditModelMessage<IPRating> message = new(ipRating, this);
            modalsGenerator.OpenEditModal(message);
        }

        public TransformerModuleInfo CopyPropertiesToModel(TransformerModuleInfo model)
        {
            model.Watt = this.Watt;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.TransformerDimensions = this.TransformerDimensions.GetDeepClone();
            model.IP = this.IP.GetDeepClone();
            return model;
        }

        public TransformerModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(TransformerModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
    }
}
