using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class DrawBrush : IDeepClonable<DrawBrush>
    {
        public DrawBrush(){ Color = string.Empty; }
        public DrawBrush(string color) => Color = color;
        public DrawBrush(double angleDegrees,params DrawGradientStop[] stops) : this()
        {
            GradientAngleDegrees = angleDegrees;
            var stopsList = new List<DrawGradientStop>();
            foreach (var stop in stops)
            {
                stopsList.Add(stop);
            }
            GradientStops = stopsList;
        }
        public string Color { get; }
        public IReadOnlyList<DrawGradientStop> GradientStops { get; } = [];
        public double GradientAngleDegrees { get; }
        public bool IsSolidColor { get => GradientStops.Count == 0; }

        /// <summary>
        /// Converts the Gradient Angle to the Start End Points of the Gradient (normalized from 0 to 1)
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <returns></returns>
        public static (PointXY startPoint,PointXY EndPoint) GetGradientAngleNormalizedPoints(double angleDegrees)
        {
            double normalizedAngle = MathCalculations.VariousMath.NormalizeAngleDegrees(angleDegrees);
            var angleRadians = normalizedAngle * Math.PI / 180;
            var startPoint = new PointXY(0, 0);
            var endPoint = new PointXY(Math.Cos(angleRadians), Math.Sin(angleRadians));

        //Normalize the End Point to Range [0,1]
        //Context:
        //Original endX and endY Values:
        //The values of endX and endY are calculated using Math.Cos(angleRadians) and Math.Sin(angleRadians), respectively.
        //These values will be in the range of[-1, 1] because Cos and Sin functions return values within this range.
        //Normalization Shifting Values:
        //By adding 1 to endX and endY, you shift the range from[-1, 1] to[0, 2].
        //For example:
        //If endX = -1, then endX + 1 = 0.
        //If endX = 1, then endX + 1 = 2.
            var endX = endPoint.X + 1 / 2;
            var endY = endPoint.Y + 1 / 2;

            return (startPoint, new(endX,endY));
        }
        public static DrawBrush Empty => DrawBrushes.Empty;
        public bool IsEmptyOrTransparent => IsSolidColor && (string.IsNullOrEmpty(Color) || (Color.Length == 9 && Color.StartsWith("#00")) || (Color.Length == 8 && Color.StartsWith("00")));
        public DrawBrush GetDeepClone()
        {
            if (IsSolidColor)
            {
                return new(Color);
            }
            else
            {
                return new(GradientAngleDegrees, [.. GradientStops]);
            }
        }
    }
    public class DrawBrushEqualityComparer : IEqualityComparer<DrawBrush>
    {
        private readonly DrawGradientStopEqualityComparer gradientComparer = new();

        public bool Equals(DrawBrush? x, DrawBrush? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return x.Color == y.Color &&
                x.GradientAngleDegrees == y.GradientAngleDegrees &&
                x.GradientStops.SequenceEqual(y.GradientStops, gradientComparer);
        }

        public int GetHashCode([DisallowNull] DrawBrush obj)
        {
            int hash = HashCode.Combine(obj.Color, obj.GradientAngleDegrees);
            foreach (var item in obj.GradientStops)
            {
                unchecked
                {
                    hash = HashCode.Combine(hash, gradientComparer.GetHashCode(item)); 
                }
            }
            return hash;
        }
    }
}
