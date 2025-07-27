using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models;
using ShapesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapesLibrary.ShapeInfoModels;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using static ShapesLibrary.Services.MathCalculations;
using ShapesLibrary.Enums;
using DrawingLibrary.Interfaces;
using ShapesLibrary.Services;
using CommonInterfacesBronze;

namespace DrawingLibrary
{
    public class TechnicalDrawBuilder
    {
        private TechnicalDrawing draw = new("UndefinedCompositeDraw");
        private CompositeDrawBuilderOptions builderOptions;

        public TechnicalDrawBuilder(CompositeDrawBuilderOptions? options = null)
        {
            this.builderOptions = options ?? new();
        }

        public TechnicalDrawBuilder SetCompositeDrawName(string name)
        {
            draw.Name = name;
            return this;
        }
        public TechnicalDrawBuilder SetContainerDimensions(DrawContainerOptions containerOptions)
        {
            builderOptions.ContainerOptions = containerOptions.GetDeepClone();
            return this;
        }
        public TechnicalDrawBuilder ScaleDrawToContainer()
        {
            builderOptions.ScaleDrawToContainer = true;
            return this;
        }
        public TechnicalDrawBuilder CenterDrawToContainer()
        {
            builderOptions.CenterDrawToContainer = true;
            return this;
        }
        public TechnicalDrawBuilder UseOnlyPositivePositioning()
        {
            builderOptions.UseOnlyPositivePositioning = true;
            return this;
        }
        public TechnicalDrawBuilder SetOptions(CompositeDrawBuilderOptions options)
        {
            builderOptions = options.GetDeepClone();
            return this;
        }
        public TechnicalDrawBuilder AddDraw(IDrawing drawing)
        {
            draw.AddDrawing(drawing);
            return this;
        }
        public TechnicalDrawBuilder AddRectangle(RectangleInfo rectangle,
                                                 DrawingPresentationOptions presentationOptions,
                                                 RectangleDimensionsPresentationOptions dimensionPresentationOptions,
                                                 string? drawName = null,
                                                 params IDrawing[] clips)
        {
            var rect = new RectangleDrawing(rectangle, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) rect.Name = nameof(RectangleDrawing);
            else rect.Name = drawName;
            foreach (var clip in clips)
            {
                rect.AddClip(clip);
            }
            draw.AddDrawing(rect);
            return this;
        }

        public static IDrawing CreateClipDrawing(ShapeInfo shape,string? clipName = null)
        {
            return shape switch
            {
                RectangleInfo rect => new RectangleDrawing(rect) { Name = clipName ?? $"{nameof(RectangleDrawing)}Clip" },
                CircleInfo circle => new CircleDrawing(circle) { Name = clipName ?? $"{nameof(CircleDrawing)}Clip" },
                _ => throw new NotSupportedException($"{shape.GetType().Name} is not a supported type for Creating a Clip"),
            };
        }

        public TechnicalDrawBuilder AddCircle(CircleInfo circle,
                                              DrawingPresentationOptions presentationOptions,
                                              CircleDimensionsPresentationOptions dimensionPresentationOptions,
                                              string? drawName = null,
                                              params IDrawing[] clips)
        {
            var c = new CircleDrawing(circle, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) c.Name = nameof(CircleDrawing);
            else c.Name = drawName;
            foreach (var clip in clips)
            {
                c.AddClip(clip);
            }
            draw.AddDrawing(c);
            return this;
        }
        public TechnicalDrawBuilder AddCapsule(CapsuleInfo capsule,
                                               DrawingPresentationOptions presentationOptions,
                                               CapsuleDimensionsPresentationOptions dimensionPresentationOptions,
                                               string? drawName = null)
        {
            var c = new CapsuleDrawing(capsule, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) c.Name = nameof(CapsuleDrawing);
            else c.Name = drawName;
            draw.AddDrawing(c);
            return this;
        }
        public TechnicalDrawBuilder AddCircleQuadrant(CircleQuadrantInfo quadrant,
                                                      DrawingPresentationOptions presentationOptions,
                                                      CircleQuadrantDimensionsPresentationOptions dimensionPresentationOptions,
                                                      string? drawName = null)
        {
            var c = new CircleQuadrantDrawing(quadrant, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) c.Name = nameof(CircleQuadrantDrawing);
            else c.Name = drawName;
            draw.AddDrawing(c);
            return this;
        }
        public TechnicalDrawBuilder AddEllipse(EllipseInfo ellipse,
                                               DrawingPresentationOptions presentationOptions,
                                               EllipseDimensionsPresentationOptions dimensionPresentationOptions,
                                               string? drawName = null)
        {
            var c = new EllipseDrawing(ellipse, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) c.Name = nameof(EllipseDrawing);
            else c.Name = drawName;
            draw.AddDrawing(c);
            return this;
        }
        public TechnicalDrawBuilder AddEgg(EggShapeInfo egg,
                                           DrawingPresentationOptions presentationOptions,
                                           EggDimensionsPresentationOptions dimensionPresentationOptions,
                                           string? drawName = null)
        {
            var e = new EggDrawing(egg, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) e.Name = nameof(EggDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }
        public TechnicalDrawBuilder AddCircleSegment(CircleSegmentInfo segment,
                                                     DrawingPresentationOptions presentationOptions,
                                                     CircleSegmentDimensionsPresentationOptions dimensionPresentationOptions,
                                                     string? drawName = null)
        {
            var e = new CircleSegmentDrawing(segment, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) e.Name = nameof(CircleSegmentDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }
        public TechnicalDrawBuilder AddRectangleRing(RectangleRingInfo ring,
                                                     DrawingPresentationOptions presentationOptions,
                                                     RectangleRingDimensionsPresentationOptions dimensionPresentationOptions,
                                                     string? drawName = null)
        {
            var e = new RectangleRingDrawing(ring, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) e.Name = nameof(RectangleRingDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }
        public TechnicalDrawBuilder AddCircleRing(CircleRingInfo ring,
                                                  DrawingPresentationOptions presentationOptions,
                                                  CircleRingDimensionsPresentationOptions dimensionPresentationOptions,
                                                  string? drawName = null)
        {
            var e = new CircleRingDrawing(ring, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) e.Name = nameof(CircleRingDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }
        public TechnicalDrawBuilder AddRegularPolygon(RegularPolygonInfo polygon,
                                                      DrawingPresentationOptions presentationOptions,
                                                      RegularPolygonDimensionsPresentationOptions dimensionPresentationOptions,
                                                      string? drawName = null)
        {
            var e = new RegularPolygonDrawing(polygon, presentationOptions, dimensionPresentationOptions);
            if (drawName is null) e.Name = nameof(RegularPolygonDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }
        public TechnicalDrawBuilder AddPolygon(PolygonInfo polygon,
                                               DrawingPresentationOptions presentationOptions,
                                               string? drawName = null)
        {
            var e = new PolygonDrawing(polygon, presentationOptions);
            if (drawName is null) e.Name = nameof(PolygonDrawing);
            else e.Name = drawName;
            draw.AddDrawing(e);
            return this;
        }

        public TechnicalDrawBuilder AddDimension(PointXY point1,
                                                 PointXY point2,
                                                 DrawingPresentationOptions presentationOptions,
                                                 DimensionLineOptions lineOptions,
                                                 double dimensionValue,
                                                 string name)
        {
            var builder = DimensionLineDrawing.GetBuilder();
            var dimension = builder.SetLineEdgePoints(point1, point2)
                                   .SetDimensionLineOptions(lineOptions)
                                   .SetPresentationOptions(presentationOptions)
                                   .SetDimensionValue(dimensionValue)
                                   .SetName(name)
                                   .BuildDimensionLine();
            draw.AddDimension(dimension);
            return this;
        }

        public TechnicalDrawing GetDraw()
        {
            this.draw.ContainerOptions = builderOptions.ContainerOptions.GetDeepClone();
            if (builderOptions.SetTotalDrawLocationToZero) this.draw.SetTotalDrawLocation(new(0,0));
            
            //Apply only one with total taking precedence
            if (builderOptions.ScaleTotalDrawToContainer) this.draw.ScaleTotalDrawToContainer();
            else if (builderOptions.ScaleDrawToContainer) this.draw.ScaleDrawToContainer();
            
            if (builderOptions.CenterDrawToContainer) this.draw.CenterDrawToContainer();
            if (builderOptions.UseOnlyPositivePositioning) this.draw.TransformOnlyToPositivePositioning();

            var draw = this.draw;

            ResetBuilder();
            return draw;
        }
        private void ResetBuilder()
        {
            //Reset the Builder
            this.draw = new("UndefinedCompositeDraw");
        }
    }
    public class CompositeDrawBuilderOptions : IDeepClonable<CompositeDrawBuilderOptions>
    {
        /// <summary>
        /// Weather to center the draw to the Container it resides in
        /// </summary>
        public bool CenterDrawToContainer { get; set; } = true;
        /// <summary>
        /// Marks weather to Scale the Draw to container before serving it
        /// </summary>
        public bool ScaleDrawToContainer { get; set; } = true;
        /// <summary>
        /// Scales the Total Draw to the Container (Calculates the Sizes of the Dimension Lines to the Scaling)
        /// </summary>
        public bool ScaleTotalDrawToContainer { get; set; } = false;
        /// <summary>
        /// Weather to mark the Draw to use only Positive Positioning (No Negative coordinates)
        /// </summary>
        public bool UseOnlyPositivePositioning { get; set; } = false;
        /// <summary>
        /// Puts the Draw at 0,0 calculating also its Dimensions
        /// </summary>
        public bool SetTotalDrawLocationToZero { get; set; } = false;
        /// <summary>
        /// Measures and Options of the Container holding the Draw
        /// </summary>
        public DrawContainerOptions ContainerOptions { get; set; } = new();

        public CompositeDrawBuilderOptions GetDeepClone()
        {
            var clone = (CompositeDrawBuilderOptions)this.MemberwiseClone();
            clone.ContainerOptions = this.ContainerOptions.GetDeepClone();
            return clone;
        }
    }

}
