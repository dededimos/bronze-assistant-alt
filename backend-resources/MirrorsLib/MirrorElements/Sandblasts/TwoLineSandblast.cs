using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class TwoLineSandblast : MirrorSandblastInfo, IDeepClonable<TwoLineSandblast>
    {
        public double VerticalEdgeDistance { get; set; }
        public double HorizontalEdgeDistance { get; set; }
        public bool IsVertical { get; set; }
        public double CornerRadius { get; set; }
        public TwoLineSandblast()
        {
            SandblastType = MirrorSandblastType.TwoLineSandblast;
        }
        public override TwoLineSandblast GetDeepClone()
        {
            var clone = (TwoLineSandblast)MemberwiseClone();
            return clone;
        }

        public override CompositeShapeInfo<RectangleInfo> GetShapeInfo(ShapeInfo parent)
        {
            //Calculate the First Rectangle and Clone it
            //Assume Length and Height According to the Distance Information and Orientation
            double length;
            double height;
            var parentBox = parent.GetBoundingBox();

            if (IsVertical)
            {
                height = parentBox.Height - VerticalEdgeDistance * 2;
                length = Thickness;
            }
            else
            {
                height = Thickness;
                length = parentBox.Length - HorizontalEdgeDistance * 2;
            }
            RectangleInfo rect1 = new(length, height);
            RectangleInfo rect2 = rect1.GetDeepClone();

            //Place the Rectangles according to the provided information
            //One of the X,Y coordinates is centered (as length was calculated by the distance) and the Other is placed on the designated Distance from the edge

            if (IsVertical)
            {
                rect1.LocationY = parentBox.LocationY; //centered
                rect2.LocationY = parentBox.LocationY; //centered
                rect1.SetLeftX(parentBox.LeftX + HorizontalEdgeDistance);
                rect2.SetRightX(parentBox.RightX - HorizontalEdgeDistance);
            }
            else
            {
                rect1.LocationX = parentBox.LocationX; //centered
                rect2.LocationX = parentBox.LocationX; //centered
                rect1.SetTopY(parentBox.TopY + VerticalEdgeDistance);
                rect2.SetBottomY(parentBox.BottomY - VerticalEdgeDistance);
            }
            return new([rect1, rect2]);
        }
    }
    public class TwoLineSandblastEqualityComparer : IEqualityComparer<TwoLineSandblast>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public TwoLineSandblastEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }
        private readonly MirrorSandblastInfoBaseEqualityComparer baseComparer;

        public bool Equals(TwoLineSandblast? x, TwoLineSandblast? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.VerticalEdgeDistance == y!.VerticalEdgeDistance &&
                x.HorizontalEdgeDistance == y.HorizontalEdgeDistance &&
                x.CornerRadius == y.CornerRadius &&
                x.IsVertical == y.IsVertical;
        }

        public int GetHashCode([DisallowNull] TwoLineSandblast obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj),obj.VerticalEdgeDistance, obj.HorizontalEdgeDistance, obj.CornerRadius, obj.IsVertical);
        }
    }
}

