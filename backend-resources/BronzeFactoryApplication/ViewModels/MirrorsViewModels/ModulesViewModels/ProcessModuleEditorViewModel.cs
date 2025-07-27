using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class ProcessModuleEditorViewModel : MirrorModuleInfoBaseViewModel , IEditorViewModel<MirrorProcessModuleInfo>
    {
        public ProcessModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }
        private readonly IEditModelModalsGenerator modalsGenerator;
        private MirrorProcessModuleInfo model;

        public ShapeInfo ProcessShape
        {
            get => model.ProcessShape;
            set => SetProperty(model.ProcessShape, value, model, (m, v) => model.ProcessShape = v);
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

        /// <summary>
        /// The Value that are being shown as supported in the UI
        /// </summary>
        public IEnumerable<ShapeInfoType> AcceptedShapeTypes { get; } 
            = Enum.GetValues(typeof(ShapeInfoType))
                  .Cast<ShapeInfoType>()
                  .Except([ShapeInfoType.CompositeShapeInfo,ShapeInfoType.TwoShapeInfo,ShapeInfoType.LineShapeInfo])
                  .ToArray();

        private ShapeInfoType selectedProcessShape;
        public ShapeInfoType SelectedProcessShape
        {
            get => selectedProcessShape;
            set
            {
                if (SetProperty(ref selectedProcessShape, value, nameof(SelectedProcessShape)))
                {
                    ProcessShape = selectedProcessShape switch
                    {
                        ShapeInfoType.Undefined => ShapeInfo.Undefined(),
                        ShapeInfoType.RectangleShapeInfo => RectangleInfo.ZeroRectangle(),
                        ShapeInfoType.RectangleRingShapeInfo => RectangleRingInfo.RectangleRingZero(),
                        ShapeInfoType.CircleShapeInfo => CircleInfo.ZeroCircle(),
                        ShapeInfoType.CircleRingShapeInfo => CircleRingInfo.ZeroCircleRing(),
                        ShapeInfoType.CapsuleShapeInfo => CapsuleInfo.CapsuleZero(),
                        ShapeInfoType.CapsuleRingShapeInfo => CapsuleRingInfo.CapsuleRingZero(),
                        ShapeInfoType.EllipseShapeInfo => EllipseInfo.ZeroEllipse(),
                        ShapeInfoType.EllipseRingShapeInfo => EllipseRingInfo.ZeroEllipseRing(),
                        ShapeInfoType.EggShapeInfo => EggShapeInfo.ZeroEgg(),
                        ShapeInfoType.EggRingShapeInfo => EggShapeRingInfo.ZeroEggRing(),
                        ShapeInfoType.CircleSegmentShapeInfo => CircleSegmentInfo.CircleSegmentZero(),
                        ShapeInfoType.CircleSegmentRingShapeInfo => CircleSegmentRingInfo.CircleSegmentRingZero(),
                        ShapeInfoType.CircleQuadrantShapeInfo => CircleQuadrantInfo.ZeroQuadrant(),
                        ShapeInfoType.CircleQuadrantRingShapeInfo => CircleQuadrantRingInfo.ZeroQuadrantRing(),
                        ShapeInfoType.RegularPolygonShapeInfo => RegularPolygonInfo.ZeroRegularPolygon(),
                        ShapeInfoType.PolygonShapeInfo => PolygonInfo.ZeroPolygon(),
                        ShapeInfoType.CompositeShapeInfo or
                        ShapeInfoType.LineShapeInfo or 
                        ShapeInfoType.TwoShapeInfo or 
                        _=> throw new NotSupportedException($"{selectedProcessShape} is not a Supported Shape for Mirror Processing"),
                    };
                }
            }
        }


        [RelayCommand]
        private void EditProcess(ShapeInfo shapeToEdit)
        {
            switch (shapeToEdit.ShapeType)
            {
                case ShapeInfoType.Undefined:
                    break;
                case ShapeInfoType.RectangleShapeInfo:
                    EditModelMessage<RectangleInfo> rectMessage = new((RectangleInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(rectMessage);
                    break;
                case ShapeInfoType.RectangleRingShapeInfo:
                    EditModelMessage<RectangleRingInfo> rectRingMessage = new((RectangleRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(rectRingMessage);
                    break;
                case ShapeInfoType.CircleShapeInfo:
                    EditModelMessage<CircleInfo> circleMessage = new((CircleInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(circleMessage);
                    break;
                case ShapeInfoType.CircleRingShapeInfo:
                    EditModelMessage<CircleRingInfo> circleRingMessage = new((CircleRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(circleRingMessage);
                    break;
                case ShapeInfoType.CapsuleShapeInfo:
                    EditModelMessage<CapsuleInfo> capsuleMessage = new((CapsuleInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(capsuleMessage);
                    break;
                case ShapeInfoType.CapsuleRingShapeInfo:
                    EditModelMessage<CapsuleRingInfo> capsuleRingMessage = new((CapsuleRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(capsuleRingMessage);
                    break;
                case ShapeInfoType.EllipseShapeInfo:
                    EditModelMessage<EllipseInfo> ellipseMessage = new((EllipseInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(ellipseMessage);
                    break;
                case ShapeInfoType.EllipseRingShapeInfo:
                    EditModelMessage<EllipseRingInfo> ellipseRingMessage = new((EllipseRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(ellipseRingMessage);
                    break;
                case ShapeInfoType.EggShapeInfo:
                    EditModelMessage<EggShapeInfo> eggMessage = new((EggShapeInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(eggMessage);
                    break;
                case ShapeInfoType.EggRingShapeInfo:
                    EditModelMessage<EggShapeRingInfo> eggRingMessage = new((EggShapeRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(eggRingMessage);
                    break;
                case ShapeInfoType.CircleSegmentShapeInfo:
                    EditModelMessage<CircleSegmentInfo> segmentMessage = new((CircleSegmentInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(segmentMessage);
                    break;
                case ShapeInfoType.CircleSegmentRingShapeInfo:
                    EditModelMessage<CircleSegmentRingInfo> segmentRingMessage = new((CircleSegmentRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(segmentRingMessage);
                    break;
                case ShapeInfoType.CircleQuadrantShapeInfo:
                    EditModelMessage<CircleQuadrantInfo> quadrantMessage = new((CircleQuadrantInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(quadrantMessage);
                    break;
                case ShapeInfoType.CircleQuadrantRingShapeInfo:
                    EditModelMessage<CircleQuadrantRingInfo> quadrantRingMessage = new((CircleQuadrantRingInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(quadrantRingMessage);
                    break;
                case ShapeInfoType.RegularPolygonShapeInfo:
                    EditModelMessage<RegularPolygonInfo> regularPolygonMessage = new((RegularPolygonInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(regularPolygonMessage);
                    break;
                case ShapeInfoType.PolygonShapeInfo:
                    EditModelMessage<PolygonInfo> polygonMessage = new((PolygonInfo)shapeToEdit, this);
                    modalsGenerator.OpenEditModal(polygonMessage);
                    break;
                case ShapeInfoType.LineShapeInfo:
                case ShapeInfoType.TwoShapeInfo:
                case ShapeInfoType.CompositeShapeInfo:
                    throw new NotSupportedException($"{shapeToEdit.ShapeType} is not Supported ...");
            }
        }

        public void SetModel(MirrorProcessModuleInfo model)
        {
            SetBaseProperties(model);

            //First so it does not re-change the selected shape
            SelectedProcessShape = model.ProcessShape.ShapeType;

            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public MirrorProcessModuleInfo CopyPropertiesToModel(MirrorProcessModuleInfo model)
        {
            model.MinDistanceFromSupport = this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast = this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.ProcessShape = this.ProcessShape.GetDeepClone();
            return model;
        }

        public MirrorProcessModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }
    }
}
