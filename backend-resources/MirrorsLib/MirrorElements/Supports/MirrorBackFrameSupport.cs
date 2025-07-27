using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public class MirrorBackFrameSupport : MirrorSupportInfo, IDeepClonable<MirrorBackFrameSupport>
    {
        public double Thickness { get; set; }
        public double Depth { get; set; }
        public double DistanceFromEdge { get; set; }
        public MirrorBackFrameSupport()
        {
            SupportType = MirrorSupportType.MirrorBackFrameSupport;
        }
        public override MirrorBackFrameSupport GetDeepClone()
        {
            return (MirrorBackFrameSupport)MemberwiseClone();
        }

        public RectangleRingInfo GetFrameRectangleRingShape(ShapeInfo parent)
        {
            var box = parent.GetBoundingBox();
            var clone = box.GetReducedPerimeterClone(DistanceFromEdge, true);
            return clone.GetEquivalentRingShape(Thickness);
        }
        public List<LineInfo> GetDiagonalConnectionLinesShapes(RectangleRingInfo frame)
        {
            var innerFrameRing = frame.GetInnerRingWholeShape();
            //Create a diagonal Line for each edge of the frame representing the Connections between the cut profiles
            var topLeft = new LineInfo(frame.LeftX, frame.TopY, innerFrameRing.LeftX, innerFrameRing.TopY);
            var topRight = new LineInfo(innerFrameRing.RightX, innerFrameRing.TopY, frame.RightX, frame.TopY);
            var bottomLeft = new LineInfo(frame.LeftX, frame.BottomY, innerFrameRing.LeftX, innerFrameRing.BottomY);
            var bottomRight = new LineInfo(innerFrameRing.RightX, innerFrameRing.BottomY, frame.RightX, frame.BottomY);
            return [topLeft, topRight, bottomLeft, bottomRight];
        }
    }
    public class MirrorBackFrameSupportEqualityComparer : IEqualityComparer<MirrorBackFrameSupport>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorBackFrameSupportEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorSupportInfoBaseEqualityComparer baseComparer;

        public bool Equals(MirrorBackFrameSupport? x, MirrorBackFrameSupport? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness &&
                x.Depth == y.Depth &&
                x.DistanceFromEdge == y.DistanceFromEdge;
        }

        public int GetHashCode([DisallowNull] MirrorBackFrameSupport obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness, obj.Depth, obj.DistanceFromEdge);
        }
    }
}

