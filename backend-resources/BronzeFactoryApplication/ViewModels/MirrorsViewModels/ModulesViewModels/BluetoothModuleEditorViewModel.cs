using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
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
    public partial class BluetoothModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<BluetoothModuleInfo>
    {
        public BluetoothModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }

        private readonly IEditModelModalsGenerator modalsGenerator;
        private BluetoothModuleInfo model;

        public double Watt 
        {
            get => model.Watt;
            set => SetProperty(model.Watt, value, model, (m, v) => model.Watt = v);
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

        public RectangleInfo Speaker1Dimensions
        {
            get => model.Speaker1Dimensions;
            set => SetProperty(model.Speaker1Dimensions, value, model, (m, v) => model.Speaker1Dimensions = v);
        }

        public RectangleInfo Speaker2Dimensions
        {
            get => model.Speaker2Dimensions;
            set => SetProperty(model.Speaker2Dimensions, value, model, (m, v) => model.Speaker2Dimensions = v);
        }

        public IPRating IP
        {
            get => model.IP;
            set => SetProperty(model.IP, value, model, (m, v) => model.IP = v);
        }

        [RelayCommand]
        private void EditSpeakerDimensions(RectangleInfo dimensionsToEdit)
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

        public BluetoothModuleInfo CopyPropertiesToModel(BluetoothModuleInfo model)
        {
            model.Watt = this.Watt;
            model.NeedsTouchButton = this.NeedsTouchButton;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.Speaker1Dimensions = this.Speaker1Dimensions.GetDeepClone();
            model.Speaker2Dimensions = this.Speaker2Dimensions.GetDeepClone();
            model.IP = this.IP.GetDeepClone();
            return model;
        }

        public BluetoothModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(BluetoothModuleInfo model)
        {
            this.model = model.GetDeepClone();
            SetBaseProperties(model);
            OnPropertyChanged(string.Empty);
        }
    }
}
