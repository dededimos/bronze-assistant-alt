using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class LineSandblast : MirrorSandblastInfo, IDeepClonable<LineSandblast>
    {
        public double? DistanceFromTop { get; set; }
        public double? DistanceFromBottom { get; set; }

        public double? DistanceFromLeft { get; set; }
        public double? DistanceFromRight { get; set; }

        public double? FixedLength { get; set; }
        public bool IsVertical { get; set; }
        public double CornerRadius { get; set; }
        
        public LineSandblast()
        {
            SandblastType = MirrorSandblastType.LineSandblast;
        }
        public override LineSandblast GetDeepClone()
        {
            var clone = (LineSandblast)MemberwiseClone();
            return clone;
        }

        /// <summary>
        /// Get the Shape information of the Sandblast when placed to a specific mirror
        /// </summary>
        /// <param name="parent">The Mirror that the sandblast is placed into</param>
        /// <returns>The Sandblast</returns>
        public override RectangleInfo GetShapeInfo(ShapeInfo parent)
        {
            //Get the Length of the Sandblast from the Instructions in its properties
            double length = GetLength(parent);
            //Construct the Rectangle without location
            RectangleInfo rect = new(length, Thickness,CornerRadius,0,0);
            //Rotate it if its vertical
            if (IsVertical) rect.RotateClockwise();
            //Set its Location according to the mirrors ShapeDimensions
            SetLocation(rect, parent);

            return rect;
        }

        /// <summary>
        /// Gets the length of the sandblast line according to the mirror ShapeDimensions and the distance information of the sandblast
        /// </summary>
        /// <param name="mirror"></param>
        /// <returns></returns>
        private double GetLength(ShapeInfo parent)
        {
            //If there is a fixed length, Ignore everything else and return it
            if (FixedLength is not null) return FixedLength.Value;
            double length;
            if (IsVertical)
            {
                //If its vertical set it to the mirrors Height and Deduct whatever information is available and relevant
                length = parent.GetTotalHeight();
                if (DistanceFromTop is not null) length -= DistanceFromTop.Value;
                if (DistanceFromBottom is not null) length -= DistanceFromBottom.Value;
                return length;
            }
            else
            {
                //if not vertical set it to the mirrors Length and Deduct whatever information is available and relevant
                length = parent.GetTotalLength();
                if (DistanceFromLeft is not null) length -= DistanceFromLeft.Value;
                if (DistanceFromRight is not null) length -= DistanceFromRight.Value;
                return length;
            }
        }
        /// <summary>
        /// Sets the location of the sandblast line(rectangle) in a Mirror shape
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="parent"></param>
        private void SetLocation(RectangleInfo rect, ShapeInfo parent)
        {
            var parentBoundingBox = parent.GetBoundingBox();
            if (DistanceFromTop is not null) rect.SetTopY(parentBoundingBox.TopY + DistanceFromTop.Value);
            else if (DistanceFromBottom is not null) rect.SetBottomY(parentBoundingBox.BottomY - DistanceFromBottom.Value);

            if (DistanceFromLeft is not null) rect.SetLeftX(parentBoundingBox.LeftX + DistanceFromLeft.Value);
            else if (DistanceFromRight is not null) rect.SetRightX(parentBoundingBox.RightX - DistanceFromRight.Value);
        }
    }
    public class LineSandblastEqualityComparer : IEqualityComparer<LineSandblast>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public LineSandblastEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorSandblastInfoBaseEqualityComparer baseComparer;

        public bool Equals(LineSandblast? x, LineSandblast? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.DistanceFromTop == y!.DistanceFromTop &&
                x.DistanceFromBottom == y.DistanceFromBottom &&
                x.DistanceFromLeft == y.DistanceFromLeft &&
                x.DistanceFromRight == y.DistanceFromRight &&
                x.FixedLength == y.FixedLength &&
                x.CornerRadius == y.CornerRadius &&
                x.IsVertical == y.IsVertical;
        }

        public int GetHashCode([DisallowNull] LineSandblast obj)
        {
            int hash = HashCode.Combine(baseComparer.GetHashCode(obj), obj.DistanceFromTop, obj.DistanceFromBottom, obj.DistanceFromLeft, obj.DistanceFromRight);
            return HashCode.Combine(hash, obj.FixedLength, obj.CornerRadius, obj.IsVertical);
        }
    }
}

