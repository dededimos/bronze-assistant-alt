using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Drawing;
using ShapesLibrary;
using ShapesLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShapesLibrary.Services.MathCalculations;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class ShapeInfoBaseVm : BaseViewModel , IModelGetterViewModel<ShapeInfo>
    {
        protected ShapeInfo model = ShapeInfo.Undefined();

        public string DimensionsString { get => GetModel().DimensionString; }

        public virtual double Height
        {
            get => model.GetTotalHeight();
            set
            {
                if (SetProperty(model.GetTotalHeight(), value, model, (m, v) => model.SetTotalHeight(v)))
                {
                    OnHeightChanged(value);
                }
            }
        }
        public virtual void OnHeightChanged(double newHeight) { OnPropertyChanged(nameof(DimensionsString)); }

        public virtual double Length
        {
            get => model.GetTotalLength();
            set
            {
                if (SetProperty(model.GetTotalLength(), value, model, (m, v) => model.SetTotalLength(v)))
                {
                    OnLengthChanged(value);
                }
            }
        }
        public virtual void OnLengthChanged(double newLength) { OnPropertyChanged(nameof(DimensionsString)); }

        public virtual double LocationX { get => model.LocationX; set => SetProperty(model.LocationX, value, model, (m, v) => model.LocationX = v); }
        public virtual double LocationY { get => model.LocationY; set => SetProperty(model.LocationY, value, model, (m, v) => model.LocationY = v); }

        [RelayCommand]
        private void ExecuteRotateClockwise()
        {
            RotateClockwise();
        }
        protected virtual void RotateClockwise()
        {
            model.RotateClockwise();
            OnPropertyChanged("");
        }

        [RelayCommand]
        private void ExecuteRotateAntiClockwise()
        {
            RotateAntiClockwise();
        }
        protected virtual void RotateAntiClockwise()
        {
            model.RotateAntiClockwise();
            OnPropertyChanged("");
        }

        public virtual ShapeInfo GetModel() => model.GetDeepClone();

    }
}
