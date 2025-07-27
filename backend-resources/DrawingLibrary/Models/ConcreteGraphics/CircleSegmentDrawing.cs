using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class CircleSegmentDrawing : ShapeDrawing<CircleSegmentInfo,CircleSegmentDimensionsPresentationOptions>,IPolygonSimulatableDrawing, IDeepClonable<CircleSegmentDrawing>
    {
        public override CircleSegmentDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<CircleSegmentDimensionsPresentationOptions>();
        public bool SkipChordDrawing { get; set; }
        public int MinSimulationSides => CircleSegmentInfo.MINSIMULATIONSIDES;

        public CircleSegmentDrawing(CircleSegmentInfo segmentInfo) : base(segmentInfo) { }
        public CircleSegmentDrawing(CircleSegmentInfo segmentInfo, DrawingPresentationOptions? presentationOptions = null) : base(segmentInfo, presentationOptions) { }
        public CircleSegmentDrawing(CircleSegmentInfo segmentInfo, DrawingPresentationOptions? presentationOptions = null, CircleSegmentDimensionsPresentationOptions? dimensionsOptions = null, bool skipChordDrawing = false) : this(segmentInfo, presentationOptions)
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
            SkipChordDrawing = skipChordDrawing;
        }

        protected override void BuildPathData()
        {
            pathDataBuilder.AddCircleSegment(Shape, SkipChordDrawing);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddCircleSegmentHole(Shape);
        }
        public string GetPolygonSimulationPathData(int sides)
        {
            var polygon = Shape.GetPolygonSimulation(sides);
            pathDataBuilder.ResetBuilder().AddPolygon(polygon);
            return pathDataBuilder.GetPathData();
        }
        public string GetNormalAndSimulatedPathData(int sides)
        {
            pathDataBuilder.ResetBuilder();
            BuildPathData();
            AddClipPathData();
            return pathDataBuilder.AddPolygon(Shape.GetPolygonSimulation(sides)).GetPathData();
        }

        public override CircleSegmentDrawing GetDeepClone()
        {
           var clone = (CircleSegmentDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CircleSegmentDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (CircleSegmentDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];

            var builder = DimensionLineDrawing.GetBuilder();

            #region Radius Diam Dimensions
            if (DimensionsPresentationOptions.ShowCircleRadius || DimensionsPresentationOptions.ShowDiameter)
            {
                //place the Dimensions on the Opposite Segment on the tip of the Arc
                var presOptions = DimensionsPresentationOptions.ShowCircleRadius ? DimensionsPresentationOptions.RadiusPresentationOptions : DimensionsPresentationOptions.DiameterPresentationOptions;
                var lineOptions = DimensionsPresentationOptions.ShowCircleRadius ? DimensionsPresentationOptions.RadiusLineOptions : DimensionsPresentationOptions.DiameterLineOptions;
                var margin = DimensionsPresentationOptions.RadiusDiameterMarginFromShape;
                PointXY start;
                PointXY end;
                var tip = Shape.GetArcTipPoint();

                switch (Shape.Orientation)
                {
                    case CircleSegmentOrientation.PointingTop:
                    case CircleSegmentOrientation.PointingLeft:
                        start = new(tip.X + margin, tip.Y);
                        end = new(start.X + lineOptions.OneEndLineLength, start.Y);
                        break;
                    case CircleSegmentOrientation.PointingBottom:
                    case CircleSegmentOrientation.PointingRight:
                        start = new(tip.X - margin, tip.Y);
                        end = new(start.X - lineOptions.OneEndLineLength, start.Y);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }

                var radiusDiamDim = builder.SetPresentationOptions(presOptions)
                    .SetDimensionLineOptions(lineOptions)
                    .SetStart(start)
                    .SetEnd(end)
                    .SetDimensionValue(DimensionsPresentationOptions.ShowCircleRadius ? originalShape.GetRadius() : originalShape.GetRadius()* 2d)
                    .SetName($"{(DimensionsPresentationOptions.ShowCircleRadius ? "Radius of" : "Diameter Of")} ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(radiusDiamDim);
            }
            #endregion

            #region Chord Length Dimension
            //Do not Show Chord if Length/Height is Shown and Chord is one of those
            if (Shape.IsChordHorizontal() && Shape.IsSmallSegment() && DimensionsPresentationOptions.ShowLength) return dims;
            if (Shape.IsChordHorizontal() is false && Shape.IsSmallSegment() && DimensionsPresentationOptions.ShowHeight) return dims;

            if (DimensionsPresentationOptions.ShowChordLength)
            {
                PointXY start;
                PointXY end;
                var margin = DimensionsPresentationOptions.ChordLengthMarginFromShape;
                var halfChord = Shape.ChordLength / 2d;

                switch (Shape.Orientation)
                {
                    case CircleSegmentOrientation.PointingTop:
                        start = new(Shape.LocationX - halfChord, Shape.LocationY + margin);
                        end = new(start.X + Shape.ChordLength, start.Y);
                        break;
                    case CircleSegmentOrientation.PointingBottom:
                        start = new(Shape.LocationX - halfChord, Shape.LocationY - margin);
                        end = new(start.X + Shape.ChordLength, start.Y);
                        break;
                    case CircleSegmentOrientation.PointingLeft:
                        start = new(Shape.LocationX + margin, Shape.LocationY - halfChord);
                        end = new(start.X, start.Y + Shape.ChordLength);
                        break;
                    case CircleSegmentOrientation.PointingRight:
                        start = new(Shape.LocationX - margin, Shape.LocationY - halfChord);
                        end = new(start.X, start.Y + Shape.ChordLength);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }
                var chordDim = builder.SetPresentationOptions(DimensionsPresentationOptions.ChordLengthPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.ChordLengthLineOptions)
                    .SetStart(start)
                    .SetEnd(end)
                    .SetDimensionValue(originalShape.ChordLength)
                    .SetName($"Chord Length of ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(chordDim);
            }

            #endregion

            return dims;
        }
        public override IEnumerable<IDrawing> GetHelpLinesDrawings()
        {
            List<IDrawing> helplines = [];
            if (DimensionsPresentationOptions.ShowHelpCircleExtension)
            {
                var oppositeSegment = new CircleSegmentDrawing(Shape.GetOppositeSegment(), DimensionsPresentationOptions.CircleExtensionPresentationOptions, skipChordDrawing: true);
                helplines.Add(oppositeSegment);
            }
            return helplines;
        }
    }
}
