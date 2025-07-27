using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class ScreenModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<ScreenModuleInfo>
    {
        public ScreenModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }
        private readonly IEditModelModalsGenerator modalsGenerator;
        protected ScreenModuleInfo model;

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

        public int NumberOfButtons
        {
            get => model.NumberOfButtons;
            set => SetProperty(model.NumberOfButtons, value, model, (m, v) => model.NumberOfButtons = v);
        }

        public bool HasIntegratedBluetooth
        {
            get => model.HasIntegratedBluetooth;
            set => SetProperty(model.HasIntegratedBluetooth, value, model, (m, v) => model.HasIntegratedBluetooth = v);
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

        public RectangleInfo FrontDimensions
        {
            get => model.FrontDimensions;
            set => SetProperty(model.FrontDimensions, value, model, (m, v) => model.FrontDimensions = v);
        }

        public RectangleInfo RearDimensions
        {
            get => model.RearDimensions;
            set => SetProperty(model.RearDimensions, value, model, (m, v) => model.RearDimensions = v);
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

        public ObservableCollection<string> RegulatedItems { get; } = [];

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

        [RelayCommand]
        private void AddRegulatedItem(string item)
        {
            RegulatedItems.Add(item);
            model.RegulatedItems.Add(item);
            OnPropertyChanged(nameof(RegulatedItems));
        }
        [RelayCommand]
        private void RemoveRegulatedItem(string item)
        {
            RegulatedItems.Remove(item);
            model.RegulatedItems.Remove(item);
            OnPropertyChanged(nameof(RegulatedItems));
        }


        public ScreenModuleInfo CopyPropertiesToModel(ScreenModuleInfo model)
        {
            model.Watt = this.Watt;
            model.NumberOfButtons = this.NumberOfButtons;
            model.HasIntegratedBluetooth = this.HasIntegratedBluetooth;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.FrontDimensions = this.FrontDimensions.GetDeepClone();
            model.RearDimensions = this.RearDimensions.GetDeepClone();
            model.Speaker1Dimensions = this.Speaker1Dimensions.GetDeepClone();
            model.Speaker2Dimensions = this.Speaker2Dimensions.GetDeepClone();
            model.IP = this.IP.GetDeepClone();
            model.RegulatedItems = new(this.RegulatedItems);
            return model;
        }

        public ScreenModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(ScreenModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            
            this.RegulatedItems.Clear();
            foreach (var item in model.RegulatedItems)
            {
                this.RegulatedItems.Add(item);
            }
            OnPropertyChanged(string.Empty);
        }
    }
}
