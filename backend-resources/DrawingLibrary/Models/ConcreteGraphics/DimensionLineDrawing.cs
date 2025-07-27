using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using static ShapesLibrary.Services.MathCalculations;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    /// <summary>
    /// Represents a Dimensions Line Drawing
    /// </summary>
    public class DimensionLineDrawing : LineDrawing , IDeepClonable<DimensionLineDrawing>
    {
        private DimensionLineDrawing(LineInfo lineInfo) : base(lineInfo) { }

        /// <summary>
        /// The text written on the Dimension Line
        /// </summary>
        public override string? DrawingText { get => GetDimensionText(); }
        private string GetDimensionText()
        {
            if (DimensionOverrideText is not null)
            {
                return DimensionOverrideText;
            }
            else
            {
                var dimensionVal = DimensionValue ?? double.NaN;
                var dimensionValueText = double.IsNaN(dimensionVal) ? string.Empty : dimensionVal.ToString();
                return $"{LineOptions.DimensionTextPrefix}{dimensionValueText}{LineOptions.DimensionUnitString}";
            }
        }

        /// <summary>
        /// Overrides the dimension value with this text
        /// </summary>
        public string? DimensionOverrideText { get; set; } = null;

        private double? dimensionValue;
        public double? DimensionValue
        {
            get => dimensionValue is not null ? Math.Round((double)dimensionValue, LineOptions.DimensionValueRoundingDecimals, MidpointRounding.AwayFromZero) : null;
            set => dimensionValue = value;
        }

        /// <summary>
        /// The Options of the Dimension Line 
        /// </summary>
        public DimensionLineOptions LineOptions { get; set; } = new();
        public override RectangleInfo GetBoundingBox()
        {
            if (IsSingleStartArrowLineWithExtension())
            {
                //Calculate :
                //the Extension End Point the 
                //the new EndPoint
                //The Start is Given

                //Find the Actual End of the line as now its not the initial Length but the OneEndLineLength
                Vector2D directionToEnd = new Vector2D(Shape.Start, Shape.End).Normalize();
                PointXY newStartPoint = new PointXY(Shape.StartX + directionToEnd.X * LineOptions.ArrowLength, Shape.StartY + directionToEnd.Y * LineOptions.ArrowLength);
                PointXY newEndPoint = new(newStartPoint.X + directionToEnd.X * LineOptions.OneEndLineLength, newStartPoint.Y + directionToEnd.Y * LineOptions.OneEndLineLength);
                double extensionEndX;
                if (newEndPoint.X > newStartPoint.X)
                {
                    // Extend to the right
                    extensionEndX = newEndPoint.X + LineOptions.OneEndLineLength * 2;
                }
                else
                {
                    // Extend to the Left
                    extensionEndX = newEndPoint.X - LineOptions.OneEndLineLength * 2;
                }
                PointXY extensionEndPoint = new PointXY(extensionEndX, newEndPoint.Y);

                return MathCalculations.Containment.GetBoundingBox(Shape.GetLocation(), newEndPoint, extensionEndPoint);
            }
            else return base.GetBoundingBox();
        }
        protected override LineInfo GetTextAnchorLine()
        {
            //The Given Anchor line is actually a paraallel segment to the initial dimension , with the same length 
            //1. When the initial dimension has arrows the anchor line has to trim its ends based on which arrows are present
            //2. If the Initial Dimension will be divided in two seperate lines (because the length would be small to fit the arrows)
            //then the Anchorline should also be one of those seprated lines . This way the text will be shifted to one of those and not in the center
            //3. When there is a single Start Arrow dimension then set the Anchor Line as the Horizontal Extension of the End Line

            var lineFromWhichToGetParallel = Shape;

            //Calculate the Extension Line if its a single start arrow dimension
            if (IsSingleStartArrowLineWithExtension())
            {
                //Find the Actual End of the line as now its not the initial Length but the OneEndLineLength
                Vector2D directionToEnd = new Vector2D(Shape.Start, Shape.End).Normalize();
                PointXY newStartPoint = new PointXY(Shape.StartX + directionToEnd.X * LineOptions.ArrowLength, Shape.StartY + directionToEnd.Y * LineOptions.ArrowLength);
                PointXY newEndPoint = new(newStartPoint.X + directionToEnd.X * LineOptions.OneEndLineLength, newStartPoint.Y + directionToEnd.Y * LineOptions.OneEndLineLength);
                
                double extensionEndX;
                if (newEndPoint.X > newStartPoint.X)
                {
                    // Extend to the right
                    extensionEndX = newEndPoint.X + LineOptions.OneEndLineLength * 2;
                }
                else
                {
                    // Extend to the Left
                    extensionEndX = newEndPoint.X - LineOptions.OneEndLineLength * 2;
                }
                lineFromWhichToGetParallel = new LineInfo(newEndPoint.X, newEndPoint.Y, extensionEndX, newEndPoint.Y);
            }
            //When line is seperated
            else if (!LineOptions.CenterTextOnTwoLineDimension && ShouldRenderTwoLineDimension(Shape.GetTotalLength()))
            {
                //Find the unit vector and offset the initial line by its length Plus the ArrowLength , towards its end .
                //This way the anchorline start after the end Arrow
                Vector2D segmentVector = new Vector2D(new PointXY(Shape.StartX, Shape.StartY), new PointXY(Shape.EndX, Shape.EndY));
                var length = segmentVector.Magnitude();
                var offset = length + LineOptions.ArrowLength;
                var unit = segmentVector.UnitVector;
                var offsetX = unit.X * offset;
                var offsetY = unit.Y * offset;
                lineFromWhichToGetParallel = new LineInfo(Shape.StartX + offsetX, Shape.StartY + offsetY, Shape.EndX + offsetX, Shape.EndY + offsetY);                
            }

            LineInfo[] parallels = MathCalculations.Line.GetParallelsAtDistance(lineFromWhichToGetParallel, LineOptions.TextMarginFromDimension);
            if (parallels.Length == 0) throw new Exception($"The calculations could not produce a parallel for the {nameof(DimensionLineDrawing)} : {Name}");
            //If only one solution return it without any further calculations
            if (parallels.Length == 1) return parallels[0];

            //both parallels StartXs or StartYs are the same in distance with their Ends . So we only need to compare
            //the position of one of those points to find where the lines lie .

            switch (PresentationOptions.TextAnchorLineOption)
            {
                case AnchorLinePreferenceOption.PreferGreaterXAnchorline:
                    return parallels.OrderByDescending(p => p.StartX).First();
                case AnchorLinePreferenceOption.PreferGreaterYAnchorline:
                    return parallels.OrderByDescending(p => p.StartY).First();
                case AnchorLinePreferenceOption.PreferSmallerXAnchorline:
                    return parallels.OrderBy(p => p.StartX).First();
                case AnchorLinePreferenceOption.PreferSmallerYAnchorline:
                    return parallels.OrderBy(p => p.StartY).First();
                case AnchorLinePreferenceOption.PreferSmallerXGreaterYAnchorline:
                    return parallels.OrderBy(p => p.StartX).ThenByDescending(p => p.StartY).First();
                case AnchorLinePreferenceOption.PreferGreaterXSmallerYAnchorline:
                    return parallels.OrderByDescending(p => p.StartX).ThenBy(p => p.StartY).First();
                case AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline:
                    return parallels.OrderBy(p => p.StartX).ThenBy(p => p.StartY).First();
                case AnchorLinePreferenceOption.PreferGreaterXGreaterYAnchorline:
                    return parallels.OrderByDescending(p => p.StartX).ThenByDescending(p => p.StartY).First();
                default:
                    throw new EnumValueNotSupportedException(PresentationOptions.TextAnchorLineOption);
            }
        }
        public override void SetText(string? text)
        {
            DimensionOverrideText = text;
        }
        protected override void BuildPathData()
        {
            var totalLength = Shape.GetTotalLength();
            if (totalLength == 0) return;

            if (ShouldRenderTwoLineDimension(totalLength)) BuildDoubleLinePathDataStringArrowless(totalLength);
            else BuildSingleLinePathDataStringArrowless();
        }
        protected override void BuildReversePathData()=> throw new NotSupportedException($"{nameof(DimensionLineDrawing)} does not Support Reverse Path Data Building");
        /// <summary>
        /// Returns the Path Data of the Arrows
        /// </summary>
        /// <returns></returns>
        public string GetArrowsPathDataString()
        {
            pathDataBuilder.ResetBuilder();
            var arrows = GetArrowPolygons();
            foreach (var arrow in arrows)
            {
                pathDataBuilder.AddPolygon(arrow);
            }
            return pathDataBuilder.GetPathData();
        }

        /// <summary>
        /// Returns the Drawings of the Arrows if any
        /// </summary>
        /// <returns></returns>
        private List<PolygonInfo> GetArrowPolygons()
        {
            if (!LineOptions.IncludeEndArrow && !LineOptions.IncludeStartArrow) return [];
            List<PolygonInfo> arrows = [];

            var totalLength = Shape.GetTotalLength();
            if (totalLength == 0) return [];
            var lineEquation = Shape.GetEquation();

            var start = new PointXY(Shape.StartX, Shape.StartY);
            var end = new PointXY(Shape.EndX, Shape.EndY);
            bool isInvertedAlignement = false; // When the Arrow is inverted it means that the Arrow body is outside the bounds of the main Shape

            if (ShouldRenderTwoLineDimension(totalLength)) isInvertedAlignement = true;
            if (LineOptions.IncludeStartArrow) arrows.Add(GetArrowPolygon(lineEquation, start, isInvertedAlignement));
            if (LineOptions.IncludeEndArrow) arrows.Add(GetArrowPolygon(lineEquation, end, isInvertedAlignement));

            return arrows;
        }

        /// <summary>
        /// Weather the Path Data should be rendered as a Two Line Dimension instead of one
        /// </summary>
        /// <param name="totalLength"></param>
        /// <returns></returns>
        public bool ShouldRenderTwoLineDimension(double totalLength) => (LineOptions.IsTwoLinesDimension || totalLength <= LineOptions.ArrowLength * LineOptions.TwoLinesDimensionArrowLengthThresholdMultiplier) && (LineOptions.IncludeStartArrow && LineOptions.IncludeEndArrow);

        /// <summary>
        /// Returns the Path data string of the Dimension Line without the Arrows , when the Line is single
        /// </summary>
        /// <param name="lineEquation">The Equation of the Line</param>
        /// <param name="lineStartIntersectionPoint">The Start Point of the Line</param>
        /// <param name="lineEndIntersectionPoint">The End Point of the Line</param>
        /// <returns></returns>
        private void BuildSingleLinePathDataStringArrowless()
        {
            PointXY newStart;
            PointXY newEnd;
            var equation = Shape.GetEquation();

            //The Line is shorter when there are arrows (if we draw it with the same length , there is a small tip in the end if the strokeThickness of the line is big)
            if (LineOptions.IncludeStartArrow && LineOptions.ArrowLength != 0)
            {
                //Shift the Start of the Line by the arrow length
                //Find the point with the Arrow Length distance from the Start Point
                var points = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(equation, new(Shape.StartX, Shape.StartY), LineOptions.ArrowLength);
                //Get the point with the minimum distance from the line (the one on the line segment)
                var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point1);
                var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point2);
                newStart = d1 < d2 ? points.point1 : points.point2;
            }
            else newStart = new(Shape.StartX, Shape.StartY);

            if (LineOptions.IncludeEndArrow && LineOptions.ArrowLength != 0)
            {
                //Shift the End of the Line by the arrow length
                //Find the point with the Arrow Length distance from the End Point
                var points = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(equation, new(Shape.EndX, Shape.EndY), LineOptions.ArrowLength);
                //Get the point with the minimum distance from the line (the one on the line Segment)
                var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point1);
                var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point2);
                newEnd = d1 < d2 ? points.point1 : points.point2;
            }
            //If this is a single Arrow Line the apply the OneLineLength by using the same shifting method as with the arrows but now use the OneLineLength Distance
            else if(IsSingleStartArrowLineWithExtension())
            {
                //Shift the "NewStart" of the Line by the OneLineLength
                //Find the point with this distance from the NewStart Point
                var points = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(equation, new(newStart.X, newStart.Y), LineOptions.OneEndLineLength);
                //Get the point with the max distance from the line (the one closest on the EndX,EndY)
                var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point1);
                var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point2);
                newEnd = d2 < d1 ? points.point1 : points.point2;
            }
            //If there is NO Arrow , then Build according to the provided start end
            else newEnd = new(Shape.EndX, Shape.EndY);

            //Draw the line between the Arrows or just a line if there are no arrows
            pathDataBuilder
            .MoveTo(newStart.X, newStart.Y)
            .AddLine(newEnd.X, newEnd.Y);

            //If this is a Dimension Line with a single Arrow in the Start and if it is not Horizontal
            //Then Draw an Extra Line where the Dimension Value will be Put . (This line will be also the Text Anchor)
            //Draw the Line on the Direction where it does not get closer to start but further away. (Only when line NOT Horizontal)
            if (IsSingleStartArrowLineWithExtension())
            {
                if (newEnd.X > newStart.X)
                {
                    // Extend to the right
                    pathDataBuilder.AddLine(newEnd.X + LineOptions.OneEndLineLength * 2, newEnd.Y);
                }
                else
                {
                    // Extend to the left
                    pathDataBuilder.AddLine(newEnd.X - LineOptions.OneEndLineLength * 2, newEnd.Y);
                }
            }

        }

        /// <summary>
        /// Weather this Dimension line has A start Arrow only , and An Extension Horizontal Line on its End
        /// </summary>
        /// <returns></returns>
        private bool IsSingleStartArrowLineWithExtension()
        {
            return LineOptions.IncludeStartArrow && !LineOptions.IncludeEndArrow && (Math.Abs(Shape.StartY - Shape.EndY)) > DoubleSafeEqualityComparer.DefaultEpsilon;
        }
        /// <summary>
        /// Builds the Double Line Path Data without Arrows
        /// </summary>
        /// <param name="totalLength">Total Length of the Single Line</param>
        private void BuildDoubleLinePathDataStringArrowless(double totalLength)
        {
            //Find the points that the lines start each time
            PointXY newStartOfStart;
            PointXY newStartOfEnd;
            var equation = Shape.GetEquation();
            //The Line is shorter when there are arrows (if we draw it with the same length , there is a small tip in the end if the strokeThickness of the line is big)
            if (LineOptions.IncludeStartArrow && LineOptions.ArrowLength != 0)
            {
                //Shift the Start of the Line by the arrow length
                //Find the point with the Arrow Length distance from the Start Point
                var points = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(equation, new(Shape.StartX, Shape.StartY), LineOptions.ArrowLength);
                //Get the point with the minimum distance from the line (the one on the line)
                var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point1);
                var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point2);

                //Here we need the Points OUTSIDE of the Shape
                newStartOfStart = d1 > d2 ? points.point1 : points.point2;
            }
            else newStartOfStart = new(Shape.StartX, Shape.StartY);

            if (LineOptions.IncludeEndArrow && LineOptions.ArrowLength != 0)
            {
                //Shift the Start of the Line by the arrow length
                //Find the point with the Arrow Length distance from the Start Point
                var points = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(equation, new(Shape.EndX, Shape.EndY), LineOptions.ArrowLength);
                //Get the point with the minimum distance from the line (the one on the line)
                var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point1);
                var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, points.point2);

                //Here we need the Points OUTSIDE of the Shape
                newStartOfEnd = d1 > d2 ? points.point1 : points.point2;
            }
            else newStartOfEnd = new(Shape.EndX, Shape.EndY);

            //From the midPoint of the Line get the Equidistant points that are the inbetween distance plus the Distance we need each of the Separate Lines to be 
            var midPoint = Shape.GetLineMidPoint();

            //These are the end points of each seperate line , but now because they are dynamic we do not know which one goes with the start and which one with the End
            var (point1, point2) = Line.GetPointsAtLineEquallyDistantFromPoint(equation, midPoint, totalLength / 2d + LineOptions.OneEndLineLength);
            //To find it we will use clockwise only drawing for all the points 
            var orderedPoints = Points.GetPointsByClockwiseOrder(newStartOfStart, newStartOfEnd, point1, point2);
            //We know the points are four every time so we make two dimensionLines and provide the PathData

            pathDataBuilder.MoveTo(orderedPoints[0].X, orderedPoints[0].Y)
                           .AddLine(orderedPoints[1].X, orderedPoints[1].Y)
                           .MoveTo(orderedPoints[2].X, orderedPoints[2].Y)
                           .AddLine(orderedPoints[3].X, orderedPoints[3].Y);
        }

        /// <summary>
        /// Returns the Polygon shape of a Dimension Arrow
        /// </summary>
        /// <param name="lineEquation">The Line Equation in which the arrow is being drawn</param>
        /// <param name="arrowTip">The Point of the Arrow's tip</param>
        /// <param name="isInvertedAlignment">When the Arrow is inverted it means that the Arrow body is outside the Dimension Line (ex. Two Arrows pointing one another with their gap being the dimension line)</param>
        /// <returns></returns>
        private PolygonInfo GetArrowPolygon(LineEquation lineEquation,PointXY arrowTip,bool isInvertedAlignment = false)
        {
            //Find the intersection of the Arrow's triangle with the line (the arrow's tip is the line's edge , one of the edges of the triangle is prependicular to the line and intersects it at a certain point)
            var lineIntersectionPoint = GetArrowIntersectionPoint(lineEquation, arrowTip.X, arrowTip.Y, isInvertedAlignment) ?? new PointXY(double.NaN, double.NaN);
            if (lineIntersectionPoint.IsNaN) PolygonInfo.ZeroPolygon();

            //Find the prependicular Line Equations for each of the two intersection points
            var perpendicular = MathCalculations.Line.GetPerpendicular(lineEquation, lineIntersectionPoint);
            //Find the Equidistant points on these prependiculars at equal distance from the Intersection points
            //To Draw the Triangles
            var equidistantPoints = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(perpendicular, lineIntersectionPoint, LineOptions.ArrowThickness / 2d);

            List<PointXY> vertices = [arrowTip, equidistantPoints.point1, equidistantPoints.point2];
            PolygonInfo polygon = new(vertices);
            return polygon;
        }

        /// <summary>
        /// Gets the intersection point of the line with the arrows vertical vertice to the line
        /// (Certain distance is the Arrow's Length)
        /// Gives two solutions only one lies within the line Segment
        /// </summary>
        /// <param name="line">The Equation of the Line</param>
        /// <param name="arrowTipX">The X Coordinate of the Start or End of the Line</param>
        /// <param name="arrowTipY">The Y Coordinate of the Start or End of the Line</param>
        /// <param name="invertArrowAlignment">If True the return intersection point will be aligned in the opposite direction (out of bounds of the main Shape)</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private PointXY? GetArrowIntersectionPoint(LineEquation line, double arrowTipX, double arrowTipY, bool invertArrowAlignment = false)
        {
            //Find the Point where the Arrow is intersecting the line vertically at a given distance from the start/end (edge) of the line
            //The mathematical solution gives two solutions (two points on either end of the given point )
            var (possiblePoint1, possiblePoint2) = MathCalculations.Line.GetPointsAtLineEquallyDistantFromPoint(line, new(arrowTipX, arrowTipY), LineOptions.ArrowLength);
            //check which of the two solutions is inside the line segment by finding 

            //THERE IS A BUG HERE => IF THE LINE LENGTH IS LESS THAN THE ARROW LENGTH THEN THE METHOD RETURNED A NULL INTERSECTION POINT (when comparing bounds)
            //The Fix is
            //for invertAlignment : Take the Point with the biggest distance from the Line
            //for normalAllignment: Take the Point with the smallest distance from the line (that would be zero mostly)
            var d1 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, possiblePoint1);
            var d2 = MathCalculations.Line.GetDistanceOfLineSegmentFromPoint(Shape, possiblePoint2);

            if (invertArrowAlignment)
            {
                //take more distant point
                return d1 > d2 ? possiblePoint1 : possiblePoint2;
            }
            else
            {
                //take closer point
                return d1 > d2 ? possiblePoint2 : possiblePoint1;
            }
        }

        public override DimensionLineDrawing GetDeepClone()
        {
            var clone = (DimensionLineDrawing)base.GetDeepClone();
            clone.LineOptions = this.LineOptions.GetDeepClone();
            return clone;
        }
        public override DimensionLineDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (DimensionLineDrawing)base.GetDeepClone(generateUniqueId);
            clone.LineOptions = this.LineOptions.GetDeepClone();
            return clone;
        }
        public override DimensionLineDrawing GetCloneCenteredToContainer(RectangleInfo container)
        {
            var clone = (DimensionLineDrawing)base.GetCloneCenteredToContainer(container);
            return clone;
        }

        public static DimensionLineBuilder GetBuilder() => new();
        public static DimensionLineDrawing Zero() => new(LineInfo.Zero());
        /// <summary>
        /// Builds Dimension Lines
        /// <para>Conventions : Always Draws from lower X to greater X , otherwise from Hight Ys to lower Ys</para>
        /// <para>This ensures that the text is always from Left to Right and readable</para>
        /// </summary>
        public class DimensionLineBuilder
        {
            private DimensionLineDrawing lineDrawing = DimensionLineDrawing.Zero();

            private DimensionLineBuilder ResetBuilder()
            {
                lineDrawing = new(LineInfo.Zero());
                return this;
            }
            /// <summary>
            /// Sets the line edge points automatically , drawing Clockwise
            /// </summary>
            /// <param name="point1">One of the Points</param>
            /// <param name="point2">Another one of the points</param>
            /// <returns>The Builder</returns>
            public DimensionLineBuilder SetLineEdgePoints(PointXY point1, PointXY point2)
            {
                //Put as start the Point with the lowest X , if Xs are tied the one with the greatest Y
                //If Ys are also tied , it does not matter which goes first , they are the same
                PointXY start;
                PointXY end;
                if (point1.X != point2.X)
                {
                    start = point1.X < point2.X ? point1 : point2;
                    end = start.X == point1.X ? point2 : point1;
                }
                else
                {
                    start = point1.Y > point2.X ? point1 : point2;
                    end = start.Y == point1.Y ? point2 : point1;
                }

                return SetStart(start).SetEnd(end);
            }
            public DimensionLineBuilder SetStart(PointXY start)
            {
                lineDrawing.Shape.StartX = start.X;
                lineDrawing.Shape.StartY = start.Y;
                return this;
            }
            public DimensionLineBuilder SetEnd(PointXY end)
            {
                lineDrawing.Shape.EndX = end.X;
                lineDrawing.Shape.EndY = end.Y;
                return this;
            }
            public DimensionLineBuilder SetPresentationOptions(DrawingPresentationOptions? options)
            {
                lineDrawing.PresentationOptions = options ?? new();
                return this;
            }
            public DimensionLineBuilder SetDimensionLineOptions(DimensionLineOptions? options)
            {
                lineDrawing.LineOptions = options ?? new();
                return this;
            }
            public DimensionLineBuilder SetDimensionValue(double value)
            {
                lineDrawing.DimensionValue = value;
                return this;
            }
            public DimensionLineBuilder SetName(string name)
            {
                lineDrawing.Name = name;
                return this;
            }
            public DimensionLineDrawing BuildDimensionLine()
            {
                var line = lineDrawing.GetDeepClone();

                if (line.LineOptions.StartRotationAngle != 0) line.Shape.RotateAroundStart(line.LineOptions.StartRotationAngle);

                ResetBuilder();
                return line;
            }
        }
    }
}
