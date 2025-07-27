using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService.Enums;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using ShapesLibrary.Services;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels
{
    public partial class PositionInstructionsRadialEditorViewModel : PositionInstructionsBaseViewModel, IEditorViewModel<PositionInstructionsRadial>
    {
        private double angleFromCenterDeg;
        public double AngleFromCenterDeg 
        {
            get => angleFromCenterDeg;
            set
            {
                if (angleFromCenterDeg != value)
                {
                    angleFromCenterDeg = value;
                    OnPropertyChanged(nameof(AngleFromCenterDeg));

                    angleFromCenterRad = MathCalculations.VariousMath.DegreesToRadians(angleFromCenterDeg);
                    OnPropertyChanged(nameof(AngleFromCenterRad));
                }
            } 
        }

        private double angleFromCenterRad;
        public double AngleFromCenterRad 
        {
            get => angleFromCenterRad;
            set
            {
                if (angleFromCenterRad != value)
                {
                    angleFromCenterRad = value;
                    OnPropertyChanged(nameof(AngleFromCenterRad));
                    
                    angleFromCenterDeg = MathCalculations.VariousMath.RadiansToDegrees(angleFromCenterRad);
                    OnPropertyChanged(nameof(AngleFromCenterDeg));
                }
            }
        }

        [ObservableProperty]
        private RadialDistancing rDistancing;
        [ObservableProperty]
        private double radialDistance;

        public PositionInstructionsRadial CopyPropertiesToModel(PositionInstructionsRadial model)
        {
            CopyBasePropertiesToModel(model);
            model.AngleFromCenterDeg = this.AngleFromCenterDeg;
            model.RDistancing = this.RDistancing;
            model.RadialDistance = this.RadialDistance;
            return model;
        }
        public PositionInstructionsRadial GetModel()
        {
            return CopyPropertiesToModel(new());
        }
        public void SetModel(PositionInstructionsRadial model)
        {
            SetBaseProperties(model);
            this.AngleFromCenterDeg = model.AngleFromCenterDeg;
            this.RDistancing = model.RDistancing;
            this.RadialDistance = model.RadialDistance;
        }
    }
}

