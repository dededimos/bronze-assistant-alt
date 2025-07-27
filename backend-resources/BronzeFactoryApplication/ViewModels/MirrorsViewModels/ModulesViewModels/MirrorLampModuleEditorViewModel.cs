using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MirrorLampModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<MirrorLampModuleInfo>
    {
        public MirrorLampModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }

        private readonly IEditModelModalsGenerator modalsGenerator;
        protected MirrorLampModuleInfo model;

        public double Watt
        {
            get => model.Watt;
            set => SetProperty(model.Watt, value, model, (m, v) => model.Watt = v);
        }

        public double TotalLength
        {
            get => model.TotalLength;
            set => SetProperty(model.TotalLength, value, model, (m, v) => model.TotalLength = v);
        }

        public double TotalHeight
        {
            get => model.TotalHeight;
            set => SetProperty(model.TotalHeight, value, model, (m, v) => model.TotalHeight = v);
        }

        public double TotalDepth
        {
            get => model.TotalDepth;
            set => SetProperty(model.TotalDepth, value, model, (m, v) => model.TotalDepth = v);
        }

        public int Kelvin
        {
            get => model.Kelvin;
            set => SetProperty(model.Kelvin, value, model, (m, v) => model.Kelvin = v);
        }

        public int Lumen
        {
            get => model.Lumen;
            set => SetProperty(model.Lumen, value, model, (m, v) => model.Lumen = v);
        }

        public bool NeedsTouchButton
        {
            get => model.NeedsTouchButton;
            set => SetProperty(model.NeedsTouchButton, value, model, (m, v) => model.NeedsTouchButton = v);
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

        public IPRating IP
        {
            get => model.IP;
            set => SetProperty(model.IP, value, model, (m, v) => model.IP = v);
        }

        public RectangleInfo LampBodyDimensions
        {
            get => model.LampBodyDimensions;
            set => SetProperty(model.LampBodyDimensions, value, model, (m, v) => model.LampBodyDimensions = v);
        }

        public RectangleInfo LampSupportDimensions
        {
            get => model.LampSupportDimensions;
            set => SetProperty(model.LampSupportDimensions, value, model, (m, v) => model.LampSupportDimensions = v);
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

        public MirrorLampModuleInfo CopyPropertiesToModel(MirrorLampModuleInfo model)
        {
            model.Watt = this.Watt;
            model.TotalLength = this.TotalLength;
            model.TotalHeight = this.TotalHeight;
            model.TotalDepth = this.TotalDepth;
            model.Kelvin = this.Kelvin;
            model.Lumen = this.Lumen;
            model.NeedsTouchButton = this.NeedsTouchButton;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.IP = this.IP.GetDeepClone();
            model.LampBodyDimensions = this.LampBodyDimensions.GetDeepClone();
            model.LampSupportDimensions = this.LampSupportDimensions.GetDeepClone();
            return model;
        }

        public MirrorLampModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(MirrorLampModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
    }
}
