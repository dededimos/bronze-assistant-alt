using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public class RegularPolygonInfoVm : ShapeInfoBaseVm
    {
        protected RegularPolygonInfo Polygon { get => (RegularPolygonInfo)model; }
        public RegularPolygonInfoVm()
        {
            model = RegularPolygonInfo.ZeroRegularPolygon();
        }

        public int NumberOfSides 
        { 
            get => Polygon.Vertices.Count;
            set
            {
                if (Polygon.Vertices.Count != value)
                {
                    var newValue = Math.Max(3, value);
                    //Nullify Rotation whjen changing sides Number
                    model = new RegularPolygonInfo(Polygon.GetCircumscribedCircle(), newValue);
                    OnPropertyChanged(string.Empty);
                }
            }
        }
        public double CircumscribedRadius
        {
            get => Polygon.CircumscribedRadius;
            set
            {
                if (Polygon.CircumscribedRadius != value)
                {
                    model = new RegularPolygonInfo(new CircleInfo(value,Polygon.LocationX, Polygon.LocationY), Polygon.Vertices.Count,Polygon.RotationRadians);
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged("");
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged("");
        }

        public override RegularPolygonInfo GetModel()
        {
            return Polygon.GetDeepClone();
        }

        public void SetModel(RegularPolygonInfo model)
        {
            this.model = model.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

    }
}
