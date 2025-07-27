using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
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
    public partial class ResistancePadModuleEditorViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<ResistancePadModuleInfo>
    {
        public ResistancePadModuleEditorViewModel(IEditModelModalsGenerator modalsGenerator)
        {
            model = new();
            SetBaseProperties(model);
            this.modalsGenerator = modalsGenerator;
        }
        private readonly IEditModelModalsGenerator modalsGenerator;
        protected ResistancePadModuleInfo model;

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

        public IPRating IP
        {
            get => model.IP;
            set => SetProperty(model.IP, value, model, (m, v) => model.IP = v);
        }

        public ShapeInfo DemisterDimensions
        {
            get => model.DemisterDimensions;
            set => SetProperty(model.DemisterDimensions, value, model, (m, v) => model.DemisterDimensions = v);
        }

        /// <summary>
        /// The Accepted Shape Types for a Demister
        /// </summary>
        public IEnumerable<ShapeInfoType> AcceptedShapeTypes { get; } = [ShapeInfoType.RectangleShapeInfo, ShapeInfoType.CircleShapeInfo];
        private ShapeInfoType selectedDemisterShapeType = ShapeInfoType.RectangleShapeInfo;
        /// <summary>
        /// The Selected Shape type of the Demister Pad
        /// </summary>
        public ShapeInfoType SelectedDemisterShapeType 
        {
            get => selectedDemisterShapeType;
            set
            {
                if (SetProperty(ref selectedDemisterShapeType,value,nameof(SelectedDemisterShapeType)))
                {
                    DemisterDimensions = selectedDemisterShapeType switch
                    {
                        ShapeInfoType.RectangleShapeInfo => new RectangleInfo(DemisterDimensions.GetTotalLength(), DemisterDimensions.GetTotalHeight()),
                        ShapeInfoType.CircleShapeInfo => new CircleInfo(DemisterDimensions.GetTotalLength() / 2d),
                        _ => throw new NotSupportedException($"{nameof(ShapeInfoType)}:{SelectedDemisterShapeType} is not Supported in a {nameof(ResistancePadModuleInfo)}")
                    };
                }
            }
        }

        [RelayCommand]
        private void EditIPRating(IPRating ipRating)
        {
            EditModelMessage<IPRating> message = new(ipRating, this);
            modalsGenerator.OpenEditModal(message);
        }
        [RelayCommand]
        private void EditResistancePadDimensions(ShapeInfo dimensionsToEdit)
        {
            switch (dimensionsToEdit)
            {
                case RectangleInfo rect:
                    EditModelMessage<RectangleInfo> rectMessage = new(rect, this);
                    modalsGenerator.OpenEditModal(rectMessage);
                    break;
                case CircleInfo circle:
                    EditModelMessage<CircleInfo> circleMessage = new(circle, this);
                    modalsGenerator.OpenEditModal(circleMessage);
                    break;
                default:
                    throw new NotSupportedException($"{nameof(ResistancePadModuleInfo)} does not support shapes other than {ShapeInfoType.RectangleShapeInfo} or {ShapeInfoType.CircleShapeInfo}");
            }
        }


        public ResistancePadModuleInfo CopyPropertiesToModel(ResistancePadModuleInfo model)
        {
            model.Watt =                        this.Watt;
            model.NeedsTouchButton =            this.NeedsTouchButton;
            model.MinDistanceFromSupport =      this.MinDistanceFromSupport;
            model.MinDistanceFromSandblast =    this.MinDistanceFromSandblast;
            model.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            model.IP =                          this.IP.GetDeepClone();
            model.DemisterDimensions =          this.DemisterDimensions.GetDeepClone();
            return model;
        }

        public ResistancePadModuleInfo GetModel()
        {
            return model.GetDeepClone();
        }

        public void SetModel(ResistancePadModuleInfo model)
        {
            SetBaseProperties(model);
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
    }
}
