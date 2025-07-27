using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.ShapeInfoModels
{
    public class CompositeShapeInfo<T> : CompositeShapeInfo, IDeepClonable<CompositeShapeInfo<T>>
        where T : ShapeInfo
    {
        public CompositeShapeInfo()
        {
            //NEEDED FOR DESERILIZATION IN MONGODB
        }
        public CompositeShapeInfo(IEnumerable<T> shapes) : base(new List<ShapeInfo>(shapes)) { }
        public override IReadOnlyList<T> Shapes => _shapes.Select(s=> (T)s).ToList();

        public override CompositeShapeInfo<T> GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            throw new NotSupportedException($"{nameof(CompositeShapeInfo<T>)} does not support {nameof(GetReducedPerimeterClone)}");
        }
        public override CompositeShapeInfo<T> GetDeepClone()
        {
            var shapes = Shapes.Select(s => (T)s.GetDeepClone());
            return new(shapes);
        }
    }

    public class CompositeShapeInfo : ShapeInfo , IDeepClonable<CompositeShapeInfo>
    {
        public CompositeShapeInfo():base(0,0)
        {
            //NEEDED FOR DESERILIZATION IN MONGODB
        }
        public CompositeShapeInfo(IEnumerable<ShapeInfo> shapes) : base(0,0)
        {
            ShapeType = Enums.ShapeInfoType.CompositeShapeInfo;
            _shapes.AddRange(shapes);
        }
        //MUST NOT MAKE READONLY -- IS BEING USED BY MONGO DB SERILIZER
        protected List<ShapeInfo> _shapes = [];
        public virtual IReadOnlyList<ShapeInfo> Shapes { get => _shapes; }

        public override double LocationX { get => GetBoundingBox().LocationX; set => SetLocationX(value); }
        private void SetLocationX(double x)
        {
            if (_shapes.Count == 0) return;
            var diff = x - LocationX;

            if (diff != 0)
            {
                foreach (var s in _shapes)
                {
                    s.TranslateX(diff);
                }
            }
        }
        public override double LocationY { get => GetBoundingBox().LocationY; set => SetLocationY(value); }
        private void SetLocationY(double y)
        {
            if (_shapes.Count == 0) return;
            var diff = y - LocationY;

            if (diff != 0)
            {
                foreach (var s in _shapes)
                {
                    s.TranslateY(diff);
                }
            }
        }
        public override PointXY GetCentroid()
        {
            return GetLocation();
        }

        public override bool Contains(PointXY point)
        {
            return _shapes.Any(s=> s.Contains(point));
        }
        public override bool Contains(ShapeInfo shape)
        {
            return _shapes.Any(s=> s.Contains(shape));
        }
        public override RectangleInfo GetBoundingBox()
        {
            if (_shapes.Count == 0) return RectangleInfo.ZeroRectangle();

            double minX = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double minY = double.PositiveInfinity;
            double maxY = double.NegativeInfinity;
            foreach (var shape in _shapes)
            {
                var b = shape.GetBoundingBox();
                minX = Math.Min(minX, b.LeftX);
                maxX = Math.Max(maxX, b.RightX);
                minY = Math.Min (minY, b.TopY);
                maxY = Math.Max (maxY, b.BottomY);
            }

            return new RectangleInfo(minX, minY, maxX, maxY);
        }
        public override CompositeShapeInfo GetDeepClone()
        {
            return new(_shapes.Select(s=> s.GetDeepClone()).ToList());
        }
        public override double GetPerimeter()
        {
            return _shapes.Sum(s => s.GetPerimeter());
        }
        public override double GetArea()
        {
            return _shapes.Sum(s => s.GetArea());
        }
        public override CompositeShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            throw new NotSupportedException($"{nameof(CompositeShapeInfo)} does not support {nameof(GetReducedPerimeterClone)}");
        }
        public override double GetTotalHeight()
        {
            return GetBoundingBox().Height;
        }
        public override double GetTotalLength()
        {
            return GetBoundingBox().Length;
        }
        public override bool IntersectsWithPoint(PointXY point)
        {
            return _shapes.Any(s => s.IntersectsWithPoint(point));
        }
        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            return _shapes.Any(s => s.IntersectsWithShape(shape));
        }
        public override void RotateAntiClockwise()
        {
            var location = this.GetLocation();
            foreach (var s in _shapes)
            {
                //Rotate its location
                var newLocation = MathCalculations.Points.RotatePointAroundOrigin(location, s.GetLocation(), -Math.PI / 2);
                
                //Rotate the Shape to its origin
                s.RotateAntiClockwise();

                //Correct origin to the rotated one
                s.LocationX = newLocation.X;
                s.LocationY = newLocation.Y;
            }
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateClockwise()
        {
            var location = this.GetLocation();
            foreach (var s in _shapes)
            {
                //Rotate its location
                var newLocation = MathCalculations.Points.RotatePointAroundOrigin(location, s.GetLocation(), Math.PI / 2);
                //Rotate the Shape to its origin
                s.RotateClockwise();

                //Correct origin to the rotated one
                s.LocationX = newLocation.X;
                s.LocationY = newLocation.Y;
            }
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void FlipHorizontally(double flipOriginX)
        {
            foreach (var shape in Shapes)
            {
                shape.FlipHorizontally(flipOriginX);
            }
        }
        public override void FlipVertically(double flipOriginY)
        {
            foreach (var shape in Shapes)
            {
                shape.FlipVertically(flipOriginY);
            }
        }
        public override void Scale(double scaleFactor)
        {
            var location = this.GetLocation();

            foreach (var s in _shapes)
            {
                s.ScaleFromOrigin(scaleFactor, location);
            }
        }
        public override void SetTotalHeight(double height)
        {
            throw new NotSupportedException($"{nameof(CompositeShapeInfo)} does not support {nameof(SetTotalHeight)}");
        }
        public override void SetTotalLength(double length)
        {
            throw new NotSupportedException($"{nameof(CompositeShapeInfo)} does not support {nameof(SetTotalLength)}");
        }
    }
    public class CompositeShapeInfoEqualityComparer : IEqualityComparer<CompositeShapeInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CompositeShapeInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }
        private readonly ShapeInfoEqualityComparer baseComparer;

        public bool Equals(CompositeShapeInfo? x, CompositeShapeInfo? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.Shapes.SequenceEqual(y.Shapes, baseComparer);
        }

        public int GetHashCode([DisallowNull] CompositeShapeInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            int hash = 17;
            foreach (var shape in obj.Shapes)
            {
                unchecked
                {
                    hash = HashCode.Combine(hash, baseComparer.GetHashCode(shape)); 
                }
            }
            return hash;
        }
    }
}
