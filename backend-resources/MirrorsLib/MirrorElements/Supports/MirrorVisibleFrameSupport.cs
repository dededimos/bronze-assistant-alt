using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public class MirrorVisibleFrameSupport : MirrorSupportInfo, IDeepClonable<MirrorVisibleFrameSupport>
    {
        /// <summary>
        /// The Thickness of the Frame on the Front (if zero the frame is not visible in Front)
        /// </summary>
        public double FrontThickness { get; set; }
        /// <summary>
        /// The Depth of the Frame (how thick is on the side)
        /// </summary>
        public double Depth { get; set; }
        /// <summary>
        /// The Rear Thickness of the Frame Support , if there is only one part ,only this thickness has Value
        /// </summary>
        public double RearThickness1 { get; set; }
        /// <summary>
        /// The Rear Thickness of the Second Part of the Frame Support , if there is only one part ,this thickness has no Value
        /// <para>Represents any Flaps for drawing of the frame (overlaps the glass)</para>
        /// </summary>
        public double RearThickness2 { get; set; }
        /// <summary>
        /// How much the glass enters inside the Rear Thickness of the Frame
        /// </summary>
        public double GlassInProfile { get; set; }

        public MirrorVisibleFrameSupport()
        {
            SupportType = MirrorSupportType.MirrorVisibleFrameSupport;
        }

        /// <summary>
        /// Returns how much the glass Should be shrinked in its perimeter in order for the Frame to have the Same Dimension as the Mirror
        /// </summary>
        /// <returns></returns>
        public double GetGlassShrink()
        {
            return GetGlassShrink(RearThickness1, GlassInProfile);
        }
        public static double GetGlassShrink(double rearThickness1, double glassInProfile)
        {
            return rearThickness1 - glassInProfile;
        }

        public override MirrorVisibleFrameSupport GetDeepClone()
        {
            var clone = (MirrorVisibleFrameSupport)MemberwiseClone();
            return clone;
        }

        public List<ShapeInfo> GetSupportRearShapes(ShapeInfo parent)
        {
            List<ShapeInfo> shapes = new();
            if (RearThickness1 != 0)
            {
                if (parent is IRingableShape ringableShape)
                {
                    var outerRing = ringableShape.GetRingShape(RearThickness1);
                    shapes.Add(outerRing as ShapeInfo ?? throw new Exception($"Unexpected Error {nameof(IRingShape)} was not of the Expected {nameof(ShapeInfo)} type"));
                    if (RearThickness2 != 0)
                    {
                        //Clone the parent and shrink its perimeter by the 1st Thickness
                        var clonedParent = parent.GetReducedPerimeterClone(RearThickness1, true);
                        //Then produce the inner ring with the new ReducedPerimeter Shape
                        var innerRing = ((IRingableShape)clonedParent).GetRingShape(RearThickness2);
                        shapes.Add(innerRing as ShapeInfo ?? throw new Exception($"Unexpected Error {nameof(IRingShape)} was not of the Expected {nameof(ShapeInfo)} type"));
                    }
                }
                else
                {
                    throw new NotSupportedException($"{parent.GetType()} does not support Transformations to {nameof(IRingableShape)}");
                }
            }
            return shapes;
        }

        public List<ShapeInfo> GetSupportFrontShapes(ShapeInfo parent)
        {
            List<ShapeInfo> shapes = new();
            if (FrontThickness != 0)
            {
                if (parent is IRingableShape ringableParent)
                {
                    var ringShape = ringableParent.GetRingShape(FrontThickness);
#pragma warning disable CS8604 // Possible null reference argument.
                    shapes.Add(ringShape as ShapeInfo);
#pragma warning restore CS8604 // Possible null reference argument.
                }
                else throw new NotSupportedException($"{parent.GetType()} does not support Transformations to {nameof(IRingableShape)}");
            }
            return shapes;
        }
    }
    public class MirrorVisibleFrameSupportEqualityComparer : IEqualityComparer<MirrorVisibleFrameSupport>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorVisibleFrameSupportEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorSupportInfoBaseEqualityComparer baseComparer;

        public bool Equals(MirrorVisibleFrameSupport? x, MirrorVisibleFrameSupport? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.FrontThickness == y!.FrontThickness &&
                x.Depth == y.Depth &&
                x.RearThickness1 == y.RearThickness1 &&
                x.RearThickness2 == y.RearThickness2 &&
                x.GlassInProfile == y.GlassInProfile;
        }

        public int GetHashCode([DisallowNull] MirrorVisibleFrameSupport obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.FrontThickness, obj.Depth, obj.RearThickness1, obj.RearThickness2, obj.GlassInProfile);
        }
    }
}

