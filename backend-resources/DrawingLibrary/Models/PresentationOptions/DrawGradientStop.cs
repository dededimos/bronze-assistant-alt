using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class DrawGradientStop(string color, double offset) : IDeepClonable<DrawGradientStop> , IEqualityComparerCreator<DrawGradientStop>
    {
        public string Color { get; } = color;
        public double Offset { get; } = offset;

        public static IEqualityComparer<DrawGradientStop> GetComparer()
        {
            return new DrawGradientStopEqualityComparer();
        }

        public DrawGradientStop GetDeepClone()
        {
            return (DrawGradientStop)this.MemberwiseClone();
        }
    }
    public class DrawGradientStopEqualityComparer : IEqualityComparer<DrawGradientStop>
    {
        public bool Equals(DrawGradientStop? x, DrawGradientStop? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return x.Offset == y.Offset &&
                x.Color == y.Color;
        }

        public int GetHashCode([DisallowNull] DrawGradientStop obj)
        {
            return HashCode.Combine(obj.Color, obj.Offset);
        }
    }
}
