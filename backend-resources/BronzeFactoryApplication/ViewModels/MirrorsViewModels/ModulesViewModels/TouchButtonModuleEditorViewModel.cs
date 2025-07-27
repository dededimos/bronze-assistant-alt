using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.Interfaces;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class TouchButtonModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<TouchButtonModuleInfo>
    {
        public TouchButtonModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }
        private readonly IEditModelModalsGenerator modalsGenerator;
        protected TouchButtonModuleInfo model;

        public double Watt
        {
            get => model.Watt;
            set => SetProperty(model.Watt, value, model, (m, v) => model.Watt = v);
        }

        public int NumberOfButtons
        {
            get => model.NumberOfButtons;
            set => SetProperty(model.NumberOfButtons, value, model, (m, v) => model.NumberOfButtons = v);
        }

        public double TouchButtonsDistance
        {
            get => model.TouchButtonsDistance;
            set => SetProperty(model.TouchButtonsDistance, value, model, (m, v) => model.TouchButtonsDistance = v);
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

        public double ButtonOffsetXFromRearRectangle
        {
            get => model.ButtonOffsetXFromRearRectangle;
            set => SetProperty(model.ButtonOffsetXFromRearRectangle, value, model, (m, v) => model.ButtonOffsetXFromRearRectangle = v);
        }

        public double ButtonOffsetYFromRearRectangle
        {
            get => model.ButtonOffsetYFromRearRectangle;
            set => SetProperty(model.ButtonOffsetYFromRearRectangle, value, model, (m, v) => model.ButtonOffsetYFromRearRectangle = v);
        }

        public ShapeInfo FrontDimensionsButton
        {
            get => model.FrontDimensionsButton;
            set => SetProperty(model.FrontDimensionsButton, value, model, (m, v) => model.FrontDimensionsButton = v);
        }

        public RectangleInfo RearDimensions
        {
            get => model.RearDimensions;
            set => SetProperty(model.RearDimensions, value, model, (m, v) => model.RearDimensions = v);
        }

        public IPRating IP
        {
            get => model.IP;
            set => SetProperty(model.IP, value, model, (m, v) => model.IP = v);
        }

        public ObservableCollection<string> RegulatedItems { get; } = new();


        /// <summary>
        /// The Accepted Shape Types for a Demister
        /// </summary>
        public IEnumerable<ShapeInfoType> AcceptedShapeTypes { get; } = new List<ShapeInfoType>() { ShapeInfoType.RectangleRingShapeInfo, ShapeInfoType.CircleRingShapeInfo };
        private ShapeInfoType selectedShapeTypeButton = ShapeInfoType.RectangleRingShapeInfo;
        

        /// <summary>
        /// The Selected Shape type of the Demister Pad
        /// </summary>
        public ShapeInfoType SelectedShapeTypeButton
        {
            get => selectedShapeTypeButton;
            set
            {
                if (SetProperty(ref selectedShapeTypeButton, value, nameof(SelectedShapeTypeButton)))
                {
                    var thickness = FrontDimensionsButton is IRingShape ring ? ring.Thickness : 10;
                    FrontDimensionsButton = selectedShapeTypeButton switch
                    {
                        ShapeInfoType.RectangleRingShapeInfo => new RectangleRingInfo(FrontDimensionsButton.GetTotalLength(), FrontDimensionsButton.GetTotalHeight(), thickness),
                        ShapeInfoType.CircleRingShapeInfo => new CircleRingInfo(FrontDimensionsButton.GetTotalLength() / 2d, thickness),
                        _ => throw new NotSupportedException($"{nameof(ShapeInfoType)}:{SelectedShapeTypeButton} is not Supported in a {nameof(TouchButtonModuleInfo)}")
                    };
                }
            }
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
        [RelayCommand]
        private void EditIPRating(IPRating ipRating)
        {
            EditModelMessage<IPRating> message = new(ipRating, this);
            modalsGenerator.OpenEditModal(message);
        }
        [RelayCommand]
        private void EditDimensions(ShapeInfo dimensionsToEdit)
        {
            switch (dimensionsToEdit)
            {
                case RectangleRingInfo rectRing:
                    EditModelMessage<RectangleRingInfo> rectRingMessage = new(rectRing, this);
                    modalsGenerator.OpenEditModal(rectRingMessage);
                    break;
                case RectangleInfo rect:
                    EditModelMessage<RectangleInfo> rectMessage = new(rect, this);
                    modalsGenerator.OpenEditModal(rectMessage);
                    break;
                case CircleRingInfo circleRing:
                    EditModelMessage<CircleRingInfo> circleRingMessage = new(circleRing, this);
                    modalsGenerator.OpenEditModal(circleRingMessage);
                    break;
                default:
                    throw new NotSupportedException($"{nameof(TouchButtonModuleInfo)} does not support shapes other than {ShapeInfoType.RectangleRingShapeInfo} or {ShapeInfoType.CircleRingShapeInfo} or {ShapeInfoType.RectangleShapeInfo}");
            }
        }

        public TouchButtonModuleInfo CopyPropertiesToModel(TouchButtonModuleInfo model)
        {
            model.Watt = this.Watt;
            model.NumberOfButtons = this.NumberOfButtons;
            model.TouchButtonsDistance = this.TouchButtonsDistance;
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.ButtonOffsetXFromRearRectangle = this.ButtonOffsetXFromRearRectangle;
            model.ButtonOffsetYFromRearRectangle = this.ButtonOffsetYFromRearRectangle;
            model.FrontDimensionsButton = this.FrontDimensionsButton.GetDeepClone();
            model.RearDimensions = this.RearDimensions.GetDeepClone();
            model.IP = this.IP.GetDeepClone();
            model.RegulatedItems = new(this.RegulatedItems);
            return model;
        }

        public TouchButtonModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(TouchButtonModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            //When the model is set the Selected Accepted shape type must be set on the backing field and Inform about the change .
            //Otherwise if the Property of the Selected Shape type is changed , the setter will actually change and the Object of the Front Dimensions property
            switch (model.FrontDimensionsButton)
            {
                case RectangleRingInfo:
                    selectedShapeTypeButton = ShapeInfoType.RectangleRingShapeInfo;
                    OnPropertyChanged(nameof(SelectedShapeTypeButton));
                    break;
                case CircleRingInfo:
                    selectedShapeTypeButton = ShapeInfoType.CircleRingShapeInfo;
                    OnPropertyChanged(nameof(SelectedShapeTypeButton));
                    break;
                default:
                    //do nothing
                    break;
            }

            this.RegulatedItems.Clear();
            foreach (var item in model.RegulatedItems)
            {
                this.RegulatedItems.Add(item);
            }
            OnPropertyChanged(string.Empty);
        }
    }
}
