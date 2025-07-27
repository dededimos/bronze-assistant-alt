using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class RectangleInfoVm : ShapeInfoBaseVm, IEditorViewModel<RectangleInfo>
    {
        protected RectangleInfo Rectangle { get => (RectangleInfo)model; }

        public RectangleInfoVm()
        {
            model = RectangleInfo.ZeroRectangle();
        }

        [ObservableProperty]
        private double topLeftRadius;
        [ObservableProperty]
        private double topRightRadius;
        [ObservableProperty]
        private double bottomLeftRadius;
        [ObservableProperty]
        private double bottomRightRadius;
        [ObservableProperty]
        private bool hasSeperateRadius;
        [ObservableProperty]
        private double totalRadius;

        partial void OnHasSeperateRadiusChanged(bool value)
        {
            if (value is true)
            {
                Rectangle.TopLeftRadius = TopLeftRadius;
                Rectangle.TopRightRadius = TopRightRadius;
                Rectangle.BottomLeftRadius = BottomLeftRadius;
                Rectangle.BottomRightRadius = BottomRightRadius;
            }
            else
            {
                Rectangle.SetCornerRadius(TotalRadius);
            }
        }
        partial void OnTotalRadiusChanged(double value)
        {
            if (!HasSeperateRadius)
            {
                Rectangle.SetCornerRadius(value);
            }
        }
        partial void OnTopLeftRadiusChanged(double value)
        {
            if (HasSeperateRadius && Rectangle.TopLeftRadius != value)
            {
                Rectangle.TopLeftRadius = value;
            }
        }
        partial void OnTopRightRadiusChanged(double value)
        {
            if (HasSeperateRadius && Rectangle.TopRightRadius != value)
            {
                Rectangle.TopRightRadius = value;
            }
        }
        partial void OnBottomLeftRadiusChanged(double value)
        {
            if (HasSeperateRadius && Rectangle.BottomLeftRadius != value)
            {
                Rectangle.BottomLeftRadius = value;
            }
        }
        partial void OnBottomRightRadiusChanged(double value)
        {
            if (HasSeperateRadius && Rectangle.BottomRightRadius != value)
            {
                Rectangle.BottomRightRadius = value;
            }
        }

        /// <summary>
        /// Sets the Radiuses of the Rectangle through a <see cref="RoundedCornersModuleInfo"/> object"/>
        /// </summary>
        /// <param name="roundedCornersInfo"></param>
        public void SetRadius(RoundedCornersModuleInfo roundedCornersInfo) 
        {
            SuppressPropertyNotifications();
            if (roundedCornersInfo.HasTotalRadius)
            {
                HasSeperateRadius = false;
                TotalRadius = roundedCornersInfo.TotalRadius;
            }
            else if(roundedCornersInfo.HasMixedRadius)
            {
                HasSeperateRadius = true;
                TopLeftRadius = roundedCornersInfo.TopLeft;
                TopRightRadius = roundedCornersInfo.TopRight;
                BottomLeftRadius = roundedCornersInfo.BottomLeft;
                BottomRightRadius = roundedCornersInfo.BottomRight;
            }
            ResumePropertyNotifications();
            OnPropertyChanged(string.Empty);
        }

        protected override void RotateClockwise()
        {
            model.RotateClockwise();
            if (HasSeperateRadius)
            {
                TopLeftRadius = Rectangle.TopLeftRadius;
                TopRightRadius = Rectangle.TopRightRadius;
                BottomLeftRadius = Rectangle.BottomLeftRadius;
                BottomRightRadius = Rectangle.BottomRightRadius;
            }

            OnPropertyChanged("");
        }
        protected override void RotateAntiClockwise()
        {
            model.RotateAntiClockwise();
            if (HasSeperateRadius)
            {
                TopLeftRadius = Rectangle.TopLeftRadius;
                TopRightRadius = Rectangle.TopRightRadius;
                BottomLeftRadius = Rectangle.BottomLeftRadius;
                BottomRightRadius = Rectangle.BottomRightRadius;
            }

            OnPropertyChanged("");
        }


        public override RectangleInfo GetModel()
        {
            return Rectangle.GetDeepClone();
        }
        public void SetModel(RectangleInfo rectangle)
        {
            SuppressPropertyNotifications();
            model = rectangle.GetDeepClone();
            if (rectangle.HasZeroRadius || rectangle.HasTotalNonZeroRadius)
            {
                HasSeperateRadius = false;
                TotalRadius = rectangle.TopLeftRadius;
            }
            else
            {
                HasSeperateRadius = true;
                TopLeftRadius = rectangle.TopLeftRadius;
                TopRightRadius = rectangle.TopRightRadius;
                BottomLeftRadius = rectangle.BottomLeftRadius;
                BottomRightRadius = rectangle.BottomRightRadius;
            }

            ResumePropertyNotifications();
            OnPropertyChanged(string.Empty);
        }
        public RectangleInfo CopyPropertiesToModel(RectangleInfo model)
        {
            model.LocationX = this.LocationX;
            model.LocationY = this.LocationY;
            model.Height = this.Height;
            model.Length = this.Length;
            model.TopLeftRadius = Rectangle.TopLeftRadius;
            model.TopRightRadius = Rectangle.TopRightRadius;
            model.BottomLeftRadius = Rectangle.BottomLeftRadius;
            model.BottomRightRadius = Rectangle.BottomRightRadius;
            return model;
        }
    }
}
