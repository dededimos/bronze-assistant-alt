using CommonHelpers.Exceptions;
using DrawingLibrary;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements.ModulesDrawingOptions;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Supports;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Net.NetworkInformation;

namespace MirrorsLib.DrawingElements
{
    public class MirrorSynthesisDrawBuilder
    {
        public MirrorSynthesisDrawBuilder()
        {
            OptionsFrontDraw = NormalOptions.GetDeepClone();
            OptionsRearDraw = SketchOptions.GetDeepClone();
            OptionsSideDraw = SketchOptions.GetDeepClone();
        }

        private MirrorSynthesis mirror = new();

        private MirrorDrawOptions OptionsFrontDraw { get; set; }
        private MirrorDrawOptions OptionsRearDraw { get; set; }
        private MirrorDrawOptions OptionsSideDraw { get; set; }

        /// <summary>
        /// Normal Options for Drawing
        /// </summary>
        private MirrorDrawOptions NormalOptions { get=> MirrorDrawOptions.GetDefaultOptions(false); } 
        /// <summary>
        /// Sketch Options for Drawing (returns only stroke for all draws like a 2d mechanical drawing)
        /// </summary>
        private MirrorDrawOptions SketchOptions { get=> MirrorDrawOptions.GetDefaultOptions(true); }

        private TechnicalDrawBuilder builder = new();
        private CompositeDrawBuilderOptions builderBaseOptions = new();
        private DrawContainerOptions frontContainerOptions = DrawContainerOptions.GetDefaults();
        private DrawContainerOptions rearContainerOptions = DrawContainerOptions.GetDefaults();
        private DrawContainerOptions sideContainerOptions = DrawContainerOptions.GetDefaults();

        private ShapeInfo? mirrorGlassRear;
        private ShapeInfo? mirrorGlassFront;

        /// <summary>
        /// Sets the Drawing Options to the Default Values according to the Current selected Theme
        /// </summary>
        /// <returns></returns>
        public MirrorSynthesisDrawBuilder SetDefaultDrawOptions()
        {
            OptionsFrontDraw = NormalOptions.GetDeepClone();
            OptionsRearDraw = SketchOptions.GetDeepClone();
            OptionsSideDraw = SketchOptions.GetDeepClone();
            return this;
        }
        public MirrorSynthesisDrawBuilder SetMirror(MirrorSynthesis glass)
        {
            this.mirror = glass;
            return this;
        }
        public MirrorSynthesisDrawBuilder SetBuilderOptions(CompositeDrawBuilderOptions builderOptions)
        {
            frontContainerOptions = builderOptions.ContainerOptions;
            rearContainerOptions = builderOptions.ContainerOptions;
            sideContainerOptions = builderOptions.ContainerOptions;
            builderBaseOptions = builderOptions;
            this.builder = new(builderOptions); //Must change the Container options in the Builder whenever a draw selection changes
            return this;
        }

        public TechnicalDrawing BuildMirrorDraw(MirrorDrawingView drawView)
        {
            SetDrawViewContainerDimensions(drawView);
            return drawView switch
            {
                MirrorDrawingView.FrontView => BuildMirrorFrontDraw(),
                MirrorDrawingView.RearView => BuildMirrorRearDraw(),
                MirrorDrawingView.SideView => BuildMirrorSideDraw(),
                _ => throw new EnumValueNotSupportedException(drawView),
            };
        }
        public TechnicalDrawing BuildOnlyGlassDraw(MirrorDrawingView drawView)
        {
            SetDrawViewContainerDimensions(drawView);
            return drawView switch
            {
                MirrorDrawingView.FrontView => BuildGlassFrontDraw(),
                MirrorDrawingView.RearView => BuildGlassRearDraw(),
                MirrorDrawingView.SideView => BuildGlassSideDraw(),
                _ => throw new EnumValueNotSupportedException(drawView),
            };
        }
        /// <summary>
        /// Sets the DrawBuilders Container Dimensions According to the Selected Draw View
        /// </summary>
        /// <param name="drawView"></param>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        private void SetDrawViewContainerDimensions(MirrorDrawingView drawView)
        {
            switch (drawView)
            {
                case MirrorDrawingView.FrontView:
                    builder.SetContainerDimensions(frontContainerOptions);
                    break;
                case MirrorDrawingView.RearView:
                    builder.SetContainerDimensions(rearContainerOptions);
                    break;
                case MirrorDrawingView.SideView:
                    builder.SetContainerDimensions(sideContainerOptions);
                    break;
                default:
                    throw new EnumValueNotSupportedException(drawView);
            }
        }

        private TechnicalDrawing BuildGlassRearDraw()
        {
            builder.SetCompositeDrawName("MirrorGlassDrawingRear")
                   .SetContainerDimensions(rearContainerOptions);
            BuildMainGlassRear();
            BuildSandblast(MirrorDrawingView.RearView);
            BuildMagnifierSandblasts(MirrorDrawingView.RearView);
            return builder.GetDraw();
        }
        private TechnicalDrawing BuildGlassFrontDraw()
        {
            builder.SetCompositeDrawName("MirrorGlassDrawingFront")
                   .SetContainerDimensions(frontContainerOptions);
            BuildMainGlassFront();
            BuildSandblast(MirrorDrawingView.FrontView);
            BuildMagnifierSandblasts(MirrorDrawingView.FrontView);
            return builder.GetDraw();
        }
        private TechnicalDrawing BuildGlassSideDraw()
        {
            builder.SetCompositeDrawName("MirrorGlassDrawingSide")
                   .SetContainerDimensions(sideContainerOptions);
            BuildMainGlassSideView();
            return builder.GetDraw();
        }

        private TechnicalDrawing BuildMirrorRearDraw()
        {
            builder.SetCompositeDrawName("MirrorDrawingRear")
                   .SetContainerDimensions(rearContainerOptions);

            BuildMainGlassRear();
            BuildSandblast(MirrorDrawingView.RearView);
            BuildMagnifierSandblasts(MirrorDrawingView.RearView);
            BuildSupportsRearShapes();
            return builder.GetDraw();
        }
        private TechnicalDrawing BuildMirrorFrontDraw()
        {
            builder.SetCompositeDrawName("MirrorDrawingFront")
                   .SetContainerDimensions(frontContainerOptions);
            BuildMainGlassFront();
            BuildSandblast(MirrorDrawingView.FrontView);
            BuildMagnifierSandblasts(MirrorDrawingView.FrontView);
            BuildVisibleFrameFrontShape();
            return builder.GetDraw();
        }
        private TechnicalDrawing BuildMirrorSideDraw()
        {
            builder.SetCompositeDrawName("MirrorGlassDrawingSide")
                   .SetContainerDimensions(sideContainerOptions);
            BuildMainGlassSideView();
            BuildSupportsSideShapes();
            return builder.GetDraw();
        }

        private void BuildSupportsRearShapes()
        {
            if (mirror.Support is null) return;

            //Build Supports per Support Shape switch
            switch (mirror.Support.SupportInfo.SupportType)
            {
                case MirrorSupportType.MirrorMultiSupport:
                    var multiSupportShape = mirror.Support.SupportRearShape as CompositeShapeInfo<RectangleInfo> ?? throw new Exception($"Unexpected Error , {MirrorSupportType.MirrorMultiSupport} Shape is not of the Allowed type : {typeof(CompositeShapeInfo<RectangleInfo>).Name}");
                    BuildMultiSupportsRearShape(multiSupportShape, $"{mirror.Support.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                case MirrorSupportType.MirrorVisibleFrameSupport:
                    var visibleFrameSupportShape = mirror.Support.SupportRearShape as CompositeShapeInfo ?? throw new Exception($"Unexpected Error , {MirrorSupportType.MirrorVisibleFrameSupport} Shape is not of the Allowed type : {typeof(CompositeShapeInfo).Name}");
                    BuildVisibleFrameRearShape(visibleFrameSupportShape, $"{mirror.Support.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                case MirrorSupportType.MirrorBackFrameSupport:
                    var backFrameSupportShape = mirror.Support.SupportRearShape as CompositeShapeInfo ?? throw new Exception($"Unexpected Error , {MirrorSupportType.MirrorBackFrameSupport} Shape is not of the Allowed type : {typeof(CompositeShapeInfo).Name}");
                    BuildBackFrameRearShape(backFrameSupportShape, $"{mirror.Support.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                default:
                    throw new EnumValueNotSupportedException(mirror.Support.SupportInfo.SupportType);
            }
        }
        private void BuildSupportsSideShapes()
        {
            if (mirror.Support is null) return;

            switch (mirror.Support.SupportSideShape)
            {
                case RectangleInfo rect:
                    builder.AddRectangle(rect, OptionsSideDraw.SupportsPresentationOptions.GetDeepClone(), ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<RectangleDimensionsPresentationOptions>(), $"{mirror.Support.SupportInfo.SupportType}SideDraw");
                    break;
                case CompositeShapeInfo<RectangleInfo> combo:
                    foreach (var shape in combo.Shapes)
                    {
                        builder.AddRectangle(shape, OptionsSideDraw.SupportsPresentationOptions.GetDeepClone(), ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<RectangleDimensionsPresentationOptions>(), $"{mirror.Support.SupportInfo.SupportType}SideDraw");
                    }
                    break;
                case null: throw new Exception($"{nameof(mirror.Support.SupportSideShape)} was null and could not be Generated...");
                default:
                    throw new NotSupportedException($"{mirror.Support.SupportSideShape?.GetType().Name} is not Supported for Side View Draw Generation");
            }
        }

        private void BuildVisibleFrameRearShape(CompositeShapeInfo supportShapes, string drawName)
        {
            if (supportShapes.Shapes.Count > 2) throw new Exception($"Unexpected number:{supportShapes.Shapes.Count} of Shapes in VisibleFrameRearShape , expected Number : 1 or 2");
            if (mirrorGlassFront is null) throw new Exception($"{nameof(mirrorGlassFront)} has not been Built");
            
            //Flip the shapes for the rear View
            foreach (var s in supportShapes.Shapes) s.FlipHorizontally(mirrorGlassFront.GetBoundingBox().LocationX);
            
            if (supportShapes.Shapes.Count >= 1)
            {
                switch (supportShapes.Shapes[0])
                {
                    case RectangleRingInfo rectRing:
                        var dimOptionsRectRing = OptionsRearDraw.VisibleFrameSupportRectangleDimensionOptions.BodyDimensionOptions as RectangleRingDimensionsPresentationOptions 
                            ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(RectangleRingInfo).Name} , assigned wrong type : {OptionsRearDraw.VisibleFrameSupportRectangleDimensionOptions.BodyDimensionOptions.GetType().Name}");
                        builder.AddRectangleRing(rectRing, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(), dimOptionsRectRing.GetDeepClone(), drawName);
                        break;
                    case CircleRingInfo circleRing:
                        var dimOptionsCircleRing = OptionsRearDraw.VisibleFrameSupportCircleDimensionOptions.BodyDimensionOptions as CircleRingDimensionsPresentationOptions 
                            ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(CircleRingInfo).Name} , assigned wrong type : {OptionsRearDraw.VisibleFrameSupportCircleDimensionOptions.BodyDimensionOptions.GetType().Name}");
                        builder.AddCircleRing(circleRing, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(), dimOptionsCircleRing.GetDeepClone(), drawName);
                        break;
                    default:
                        throw new NotSupportedException($"{supportShapes.Shapes[0].GetType().Name} is not a supported type for a visible Frame Generation");
                }
            }
            if (supportShapes.Shapes.Count == 2)
            {
                switch (supportShapes.Shapes[1])
                {
                    case RectangleRingInfo rectRing:
                        var dimOptionsRectRing = OptionsRearDraw.VisibleFrameSupportRectangleDimensionOptions.ExtraBodyDimensionOptions as RectangleRingDimensionsPresentationOptions 
                            ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(RectangleRingInfo).Name} , assigned wrong type : {OptionsRearDraw.VisibleFrameSupportRectangleDimensionOptions.BodyDimensionOptions.GetType().Name}");
                        builder.AddRectangleRing(rectRing, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(), dimOptionsRectRing.GetDeepClone(), drawName);
                        break;
                    case CircleRingInfo circleRing:
                        var dimOptionsCircleRing = OptionsRearDraw.VisibleFrameSupportCircleDimensionOptions.ExtraBodyDimensionOptions as CircleRingDimensionsPresentationOptions 
                            ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(CircleRingInfo).Name} , assigned wrong type : {OptionsRearDraw.VisibleFrameSupportCircleDimensionOptions.BodyDimensionOptions.GetType().Name}");
                        builder.AddCircleRing(circleRing, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(), dimOptionsCircleRing.GetDeepClone(), drawName);
                        break;
                    default:
                        throw new NotSupportedException($"{supportShapes.Shapes[1].GetType().Name} is not a supported type for a visible Frame Generation");
                }
            }
        }
        private void BuildVisibleFrameFrontShape() 
        {
            if (!mirror.HasVisibleFrame()) return;
            if (mirror.Support is null) throw new Exception($"Unexpected error , Mirror has No Support when it should have Visible Frame");
            if (mirror.Support.SupportInfo is not MirrorVisibleFrameSupport) throw new Exception($"Unexpected Error Placed Support Seems not to be the Expected type: {nameof(MirrorVisibleFrameSupport)}");
            if (mirror.Support.SupportFrontShape is null) throw new Exception($"Mirror Support Front Shape has not been Built");
            var frontShape = mirror.Support.SupportFrontShape.GetDeepClone();
            
            switch (frontShape)
            {
                case RectangleRingInfo rectangleRing:
                    var frontDrawPresentationOptions = OptionsFrontDraw.SupportsPresentationOptions.GetDeepClone();
                    frontDrawPresentationOptions.Fill = mirror.Support.Finish.FinishColorBrush;
                    builder.AddRectangleRing(rectangleRing,
                                             frontDrawPresentationOptions,
                                             OptionsFrontDraw.VisibleFrameSupportRectangleDimensionOptions.BodyDimensionOptions.GetDeepClone() as RectangleRingDimensionsPresentationOptions 
                                             ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(RectangleRingInfo).Name} , assigned wrong type : {OptionsFrontDraw.VisibleFrameSupportRectangleDimensionOptions.BodyDimensionOptions.GetType().Name}"),
                                             $"{mirror.Support.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                case CircleRingInfo circleRing:
                    builder.AddCircleRing(circleRing,
                                          OptionsFrontDraw.SupportsPresentationOptions.GetDeepClone(),
                                          OptionsFrontDraw.VisibleFrameSupportCircleDimensionOptions.BodyDimensionOptions.GetDeepClone() as CircleRingDimensionsPresentationOptions
                                          ?? throw new Exception($"Unexpected type of Presentation options for type {typeof(CircleRingInfo).Name} , assigned wrong type : {OptionsFrontDraw.VisibleFrameSupportCircleDimensionOptions.BodyDimensionOptions.GetType().Name}"),
                    $"{mirror.Support.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                default:
                    throw new NotSupportedException($"{frontShape.GetType().Name} is not a supported type for FRONT visible frame drawing generation");
            }
        }
        private void BuildBackFrameRearShape(CompositeShapeInfo supportShapes, string drawName)
        {
            foreach (var shape in supportShapes.Shapes)
            {
                switch (shape)
                {
                    case RectangleRingInfo mainBody:
                        builder.AddRectangleRing(mainBody, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(), OptionsRearDraw.BackFrameSupportDimensionOptions.BodyDimensionOptions.GetDeepClone(), drawName);
                        break;
                    case LineInfo diagonalLine:
                        var drawing = new LineDrawing(diagonalLine, OptionsRearDraw.SupportsPresentationOptions.GetDeepClone())
                        {
                            Name = drawName
                        };
                        builder.AddDraw(drawing);
                        break;
                    default:
                        throw new NotSupportedException($"{shape.GetType().Name} is an unsupported shape type for the creation of a MirrorBackFrame Draw");
                }
            }
        }
        private void BuildMultiSupportsRearShape(CompositeShapeInfo<RectangleInfo> supportShapes, string drawName)
        {
            foreach (var shape in supportShapes.Shapes)
            {
                builder.AddRectangle(shape,
                                     OptionsRearDraw.SupportsPresentationOptions.GetDeepClone(),
                                     OptionsRearDraw.MultiSupportsDimensionOptions.SupportBodyDimensionOptions.GetDeepClone(),
                                     drawName);
            }
        }

        /// <summary>
        /// Builds the Magnifying Mirrors Sandblasts according to the Selected drawing View
        /// </summary>
        /// <param name="drawingView"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        private void BuildMagnifierSandblasts(MirrorDrawingView drawingView)
        {
            var sandblastedMagnifiers = mirror.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType);
            if (sandblastedMagnifiers.Count == 0) return;
            
            foreach (var magnifier in sandblastedMagnifiers)
            {
                var magnifierShape = ((MagnifierSandblastedModuleInfo)magnifier.ModuleInfo).SandblastDimensions.GetDeepClone();
                switch (drawingView)
                {
                    case MirrorDrawingView.FrontView:
                        if (mirrorGlassFront is null) throw new Exception($"{mirrorGlassFront} has not been built"); ;
                        BuildMagnifierSandblast(magnifierShape, OptionsFrontDraw.SandblastPresentationOptions, OptionsFrontDraw.MagnifierDimensionsOptions, mirrorGlassFront);
                        break;
                    case MirrorDrawingView.RearView:
                        if (mirrorGlassRear is null) throw new Exception($"Rear Mirror Has not been Built yet...");
                        if (mirrorGlassFront is null) throw new Exception($"{mirrorGlassFront} has not been built"); ;
                        magnifierShape.FlipHorizontally(mirrorGlassFront.GetBoundingBox().LocationX);
                        BuildMagnifierSandblast(magnifierShape, OptionsRearDraw.SandblastPresentationOptions, OptionsRearDraw.MagnifierDimensionsOptions, mirrorGlassRear);
                        break;
                    case MirrorDrawingView.SideView:
                        throw new NotSupportedException($"Magnifiers are not Drawn on Side View");
                    default:
                        throw new EnumValueNotSupportedException(drawingView);
                }
            }
        }
        private void BuildMagnifierSandblast(CircleRingInfo magnifierShape,
                                             DrawingPresentationOptions presentationOptions,
                                             MagnifierSandblastDimensionOptions dimensionOptions,
                                             ShapeInfo parentMirrorShape)
        {
            builder.AddCircleRing(magnifierShape, presentationOptions.GetDeepClone(), dimensionOptions.SandblastBodyDimensionOptions.GetDeepClone());
            var bBox = parentMirrorShape.GetBoundingBox();

            if (OptionsRearDraw.MagnifierDimensionsOptions.ShowDistanceFromCenter)
            {
                //Distance from center VerticalDimension
                var startV = new PointXY(magnifierShape.CenterX, magnifierShape.CenterY);
                var endV = new PointXY(magnifierShape.CenterX, bBox.LocationY);
                var distanceV = MathCalculations.Points.GetDistanceBetweenPoints(startV, endV);
                if (distanceV != 0)
                    builder.AddDimension(startV, endV, DrawingPresentationOptions.DefaultDimensionOptions(), DimensionLineOptions.DefaultTwoArrowLineOptions(), distanceV, "Magnifier Distance From CenterY");

                //Distance from center HorizontalDimension
                var startH = new PointXY(magnifierShape.CenterX, magnifierShape.CenterY);
                var endH = new PointXY(bBox.LocationX, magnifierShape.CenterY);
                var distanceH = MathCalculations.Points.GetDistanceBetweenPoints(startH, endH);
                if (distanceH != 0)
                    builder.AddDimension(startH, endH, DrawingPresentationOptions.DefaultDimensionOptions(), DimensionLineOptions.DefaultTwoArrowLineOptions(), distanceH, "Magnifier Distance From CenterX");
            }
        }

        /// <summary>
        /// Builds the Mirror's Sandblasts according to the Selected drawing view 
        /// </summary>
        /// <param name="drawingView"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        private void BuildSandblast(MirrorDrawingView drawingView)
        {
            if (mirror.Sandblast is null) return;
            if (mirrorGlassFront is null) throw new Exception($"{mirrorGlassFront} has not been built");

            var sandblastShape = mirror.Sandblast.SandblastShape?.GetDeepClone() ?? throw new Exception($"Cannot build Sandblast Draw when Sandblast Shape Has not been built");
            DrawingPresentationOptions presentationOptions;
            ShapeInfo parentMirrorShape;

            switch (drawingView)
            {
                case MirrorDrawingView.FrontView:
                    presentationOptions = OptionsFrontDraw.SandblastPresentationOptions;
                    parentMirrorShape = mirrorGlassFront;
                    break;
                case MirrorDrawingView.RearView:
                    if (mirrorGlassRear is null) throw new Exception($"{mirrorGlassRear} has not been built");
                    sandblastShape.FlipHorizontally(mirrorGlassFront.LocationX);
                    presentationOptions = OptionsRearDraw.SandblastPresentationOptions;
                    parentMirrorShape = mirrorGlassRear;
                    break;
                case MirrorDrawingView.SideView:
                    throw new NotSupportedException($"Sandblast Building is not supported for {nameof(MirrorDrawingView)} : {drawingView}");
                default:
                    throw new EnumValueNotSupportedException(drawingView);
            }

            switch (sandblastShape)
            {
                case RectangleRingInfo ring:
                    BuildHoledRectangleSandblast(ring,
                                                 presentationOptions,
                                                 drawingView == MirrorDrawingView.RearView ? OptionsRearDraw.HoledRectangleSandblastDimensionsOptions : OptionsFrontDraw.HoledRectangleSandblastDimensionsOptions,
                                                 parentMirrorShape,
                                                 $"{mirror.Sandblast.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                case RectangleInfo rect:
                    BuildLineSandblast(rect,
                                       presentationOptions,
                                       drawingView == MirrorDrawingView.RearView ? OptionsRearDraw.LineSandblastDimensionsOptions : OptionsFrontDraw.LineSandblastDimensionsOptions,
                                       parentMirrorShape,
                                       $"{mirror.Sandblast.LocalizedDescriptionInfo.Name.DefaultValue} Draw");
                    break;
                case CompositeShapeInfo<RectangleInfo> twoLineSandblast:
                    BuildTwoLineSandblast(twoLineSandblast,
                                          presentationOptions,
                                          drawingView == MirrorDrawingView.RearView ? OptionsRearDraw.TwoLineSandblastDimensionsOptions : OptionsFrontDraw.TwoLineSandblastDimensionsOptions,
                                          parentMirrorShape,
                                          $"{mirror.Sandblast.LocalizedDescriptionInfo.Name.DefaultValue}");
                    break;
                case CircleRingInfo circleRing:
                    BuildCircularSandblast(circleRing,
                                           presentationOptions,
                                           drawingView == MirrorDrawingView.RearView ? OptionsRearDraw.CircularSandblastDimensionsOptions : OptionsFrontDraw.CircularSandblastDimensionsOptions,
                                           parentMirrorShape,
                                           $"{mirror.Sandblast.LocalizedDescriptionInfo.Name.DefaultValue}");
                    break;
                default:
                    throw new NotSupportedException($"Type of {parentMirrorShape.GetType().Name} is not Supported for Drawing");
            }
        }
        private void BuildCircularSandblast(CircleRingInfo circleRing,
                                            DrawingPresentationOptions presentationOptions,
                                            CircularSandblastInfoDimensionOptions dimensionOptions,
                                            ShapeInfo parentMirrorShape,
                                            string drawName)
        {
            builder.AddCircleRing(circleRing, presentationOptions.GetDeepClone(), dimensionOptions.SandblastBodyDimensionOptions.GetDeepClone(), drawName);
            if (dimensionOptions.ShowDistanceFromEdge)
            {
                PointXY point1;
                PointXY point2;
                var bBoxMirror = parentMirrorShape.GetBoundingBox();
                switch (dimensionOptions.EdgeDistanceDimensionPosition)
                {
                    case CircularSandblastEdgeDimensionPosition.Left:
                        point1 = new(circleRing.LeftX, circleRing.LocationY);
                        point2 = new(bBoxMirror.LeftX, circleRing.LocationY);
                        break;
                    case CircularSandblastEdgeDimensionPosition.Top:
                        point1 = new(circleRing.LocationX, circleRing.TopY);
                        point2 = new(circleRing.LocationX, bBoxMirror.TopY);
                        break;
                    case CircularSandblastEdgeDimensionPosition.Right:
                        point1 = new(circleRing.RightX, circleRing.LocationY);
                        point2 = new(bBoxMirror.RightX, circleRing.LocationY);
                        break;
                    case CircularSandblastEdgeDimensionPosition.Bottom:
                        point1 = new(circleRing.LocationX, circleRing.BottomY);
                        point2 = new(circleRing.LocationX, bBoxMirror.BottomY);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(dimensionOptions.EdgeDistanceDimensionPosition);
                }
                var dimPresOptions = dimensionOptions.DistanceFromEdgePresentationOptions.GetDeepClone();
                var dimLineOptions = dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone();
                var dimValue = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                if (dimValue != 0) builder.AddDimension(point1, point2, dimPresOptions, dimLineOptions, dimValue, $"EdgeDistance of {drawName}");
            }
        }
        private void BuildTwoLineSandblast(CompositeShapeInfo<RectangleInfo> twoLines,
                                           DrawingPresentationOptions presentationOptions,
                                           TwoLineSandblastInfoDimensionOptions dimensionOptions,
                                           ShapeInfo parentMirrorShape,
                                           string drawName)
        {
            var line1 = twoLines.Shapes[0];
            var line2 = twoLines.Shapes[1];

            //Hide Length of Sandblast if equal to glass
            var sandblastBodyDimOptions = dimensionOptions.SandblastBodyDimensionOptions.GetDeepClone();
            var sandblastBodyDimOptions2 = sandblastBodyDimOptions.GetDeepClone();
            sandblastBodyDimOptions2.ShowDimensions = false;
            if (line2.Length == parentMirrorShape.GetTotalLength())
            {
                sandblastBodyDimOptions.ShowLength = false;
            }
            if (line2.Height == parentMirrorShape.GetTotalHeight())
            {
                sandblastBodyDimOptions.ShowHeight = false;
            }

            builder.AddRectangle(line1, presentationOptions.GetDeepClone(), sandblastBodyDimOptions2, $"Line1 of {drawName}");
            builder.AddRectangle(line2, presentationOptions.GetDeepClone(), sandblastBodyDimOptions, $"Line2 of {drawName}");

            if (dimensionOptions.AtLeastOneEdgeDitanceVisible)
            {
                var mirrorbBox = parentMirrorShape.GetBoundingBox();

                #region Vertical Distance
                if (dimensionOptions.ShowVerticalDistance)
                {
                    //Line 1
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.VerticalDistanceFromEdgePosition)
                    {
                        case LineSandblastVerticalDistancePosition.Left:
                            point1 = new(line1.LeftX, line1.TopY);
                            point2 = new(line1.LeftX, mirrorbBox.TopY);
                            break;
                        case LineSandblastVerticalDistancePosition.Middle:
                            point1 = new(line1.LocationX, line1.TopY);
                            point2 = new(line1.LocationX, mirrorbBox.TopY);
                            break;
                        case LineSandblastVerticalDistancePosition.Right:
                            point1 = new(line1.RightX, line1.TopY);
                            point2 = new(line1.RightX, mirrorbBox.TopY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.VerticalDistanceFromEdgePosition);
                    }
                    var lineOptionsV1 = dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone();
                    var dimValueV1 = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    var presOptions = dimensionOptions.DistanceFromEdgePresentationOptions.GetDeepClone();
                    if (dimValueV1 != 0) builder.AddDimension(point1, point2, presOptions, lineOptionsV1, dimValueV1, $"TopDistanceFromEdge of Line1 of {drawName}");
                }
                #endregion

                #region Horizontal Distance
                if (dimensionOptions.ShowHorizontalDistance)
                {
                    //Line 1
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.HorizontalDistanceFromEdgePosition)
                    {
                        case LineSandblastHorizontalDistancePosition.Top:
                            point1 = new(line1.LeftX, line1.TopY);
                            point2 = new(mirrorbBox.LeftX, line1.TopY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Middle:
                            point1 = new(line1.LeftX, line1.LocationY);
                            point2 = new(mirrorbBox.LeftX, line1.LocationY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Bottom:
                            point1 = new(line1.LeftX, line1.BottomY);
                            point2 = new(mirrorbBox.LeftX, line1.BottomY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.HorizontalDistanceFromEdgePosition);
                    }
                    var lineOptionsH1 = dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone();
                    var dimValueH1 = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    var presOptions = dimensionOptions.DistanceFromEdgePresentationOptions.GetDeepClone();
                    if (dimValueH1 != 0) builder.AddDimension(point1, point2, presOptions, lineOptionsH1, dimValueH1, $"HorizontalDistanceFromEdge of Line1 of {drawName}");
                }
                #endregion
            }

        }
        private void BuildLineSandblast(RectangleInfo line,
                                        DrawingPresentationOptions presentationOptions,
                                        LineSandblastInfoDimensionOptions dimensionOptions,
                                        ShapeInfo parentMirrorShape,
                                        string drawName)
        {
            builder.AddRectangle(line, presentationOptions.GetDeepClone(), dimensionOptions.SandblastBodyDimensionOptions.GetDeepClone(), drawName);

            if (dimensionOptions.AtLeastOneDistanceFromEdgeVisible)
            {
                var presOptions = dimensionOptions.DistanceFromEdgePresentationOptions;
                var mirrorbBox = parentMirrorShape.GetBoundingBox();
                #region TopDistance
                if (dimensionOptions.ShowDistanceFromTop)
                {
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.VerticalDistanceFromEdgePosition)
                    {
                        case LineSandblastVerticalDistancePosition.Left:
                            point1 = new(line.LeftX, line.TopY);
                            point2 = new(line.LeftX, mirrorbBox.TopY);
                            break;
                        case LineSandblastVerticalDistancePosition.Middle:
                            point1 = new(line.LocationX, line.TopY);
                            point2 = new(line.LocationX, mirrorbBox.TopY);
                            break;
                        case LineSandblastVerticalDistancePosition.Right:
                            point1 = new(line.RightX, line.TopY);
                            point2 = new(line.RightX, mirrorbBox.TopY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.VerticalDistanceFromEdgePosition);
                    }
                    //Find Distance , sandblast Object might not state it and its dynamic usually
                    var dimValue = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    builder.AddDimension(point1,
                                         point2,
                                         presOptions.GetDeepClone(),
                                         dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone(),
                                         dimValue,
                                         $"TopDistanceFromEdge of {drawName}");
                }
                #endregion
                #region BottomDistance
                if (dimensionOptions.ShowDistanceFromBottom)
                {
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.VerticalDistanceFromEdgePosition)
                    {
                        case LineSandblastVerticalDistancePosition.Left:
                            point1 = new(line.LeftX, line.BottomY);
                            point2 = new(line.LeftX, mirrorbBox.BottomY);
                            break;
                        case LineSandblastVerticalDistancePosition.Middle:
                            point1 = new(line.LocationX, line.BottomY);
                            point2 = new(line.LocationX, mirrorbBox.BottomY);
                            break;
                        case LineSandblastVerticalDistancePosition.Right:
                            point1 = new(line.RightX, line.BottomY);
                            point2 = new(line.RightX, mirrorbBox.BottomY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.VerticalDistanceFromEdgePosition);
                    }
                    //Find Distance , sandblast Object might not state it and its dynamic usually
                    var dimValue = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    builder.AddDimension(point1,
                                         point2,
                                         presOptions.GetDeepClone(),
                                         dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone(),
                                         dimValue,
                                         $"BottomDistanceFromEdge of {drawName}");
                }
                #endregion
                #region LeftDistance
                if (dimensionOptions.ShowDistanceFromLeft)
                {
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.HorizontalDistanceFromEdgePosition)
                    {
                        case LineSandblastHorizontalDistancePosition.Top:
                            point1 = new(line.LeftX, line.TopY);
                            point2 = new(mirrorbBox.LeftX, line.TopY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Middle:
                            point1 = new(line.LeftX, line.LocationY);
                            point2 = new(mirrorbBox.LeftX, line.LocationY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Bottom:
                            point1 = new(line.LeftX, line.BottomY);
                            point2 = new(mirrorbBox.LeftX, line.BottomY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.VerticalDistanceFromEdgePosition);
                    }
                    //Find Distance , sandblast Object might not state it and its dynamic usually
                    var dimValue = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    builder.AddDimension(point1,
                                         point2,
                                         presOptions.GetDeepClone(),
                                         dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone(),
                                         dimValue,
                                         $"LeftDistanceFromEdge of {drawName}");
                }
                #endregion
                #region RightDistance
                if (dimensionOptions.ShowDistanceFromRight)
                {
                    PointXY point1;
                    PointXY point2;
                    switch (dimensionOptions.HorizontalDistanceFromEdgePosition)
                    {
                        case LineSandblastHorizontalDistancePosition.Top:
                            point1 = new(line.RightX, line.TopY);
                            point2 = new(mirrorbBox.RightX, line.TopY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Middle:
                            point1 = new(line.RightX, line.LocationY);
                            point2 = new(mirrorbBox.RightX, line.LocationY);
                            break;
                        case LineSandblastHorizontalDistancePosition.Bottom:
                            point1 = new(line.RightX, line.BottomY);
                            point2 = new(mirrorbBox.RightX, line.BottomY);
                            break;
                        default:
                            throw new EnumValueNotSupportedException(dimensionOptions.VerticalDistanceFromEdgePosition);
                    }
                    //Find Distance , sandblast Object might not state it and its dynamic usually
                    var dimValue = MathCalculations.Points.GetDistanceBetweenPoints(point1, point2);
                    builder.AddDimension(point1,
                                         point2,
                                         presOptions.GetDeepClone(),
                                         dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone(),
                                         dimValue,
                                         $"RightDistanceFromEdge of {drawName}");
                }
                #endregion
            }
        }
        private void BuildHoledRectangleSandblast(RectangleRingInfo ring,
                                                  DrawingPresentationOptions presentationOptions,
                                                  HoledRectangleSandblastInfoDimensionOptions dimensionOptions,
                                                  ShapeInfo parentMirrorShape,
                                                  string drawName)
        {
            builder.AddRectangleRing(ring, presentationOptions.GetDeepClone(), dimensionOptions.SandblastBodyDimensionOptions.GetDeepClone(), drawName);
            if (dimensionOptions.EdgeDistanceDimensionPosition == RectangleEdgeDistanceDimensionAnchorPoint.None) return;

            PointXY point1;
            PointXY point2;
            var mirrorBbox = mirror.DimensionsInformation.GetBoundingBox();

            switch (dimensionOptions.EdgeDistanceDimensionPosition)
            {
                case RectangleEdgeDistanceDimensionAnchorPoint.TopMiddle:
                    point1 = new PointXY(ring.LocationX, ring.TopY);
                    point2 = new PointXY(ring.LocationX, mirrorBbox.TopY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.BottomMiddle:
                    point1 = new PointXY(ring.LocationX, ring.BottomY);
                    point2 = new PointXY(ring.LocationX, mirrorBbox.BottomY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.LeftMiddle:
                    point1 = new PointXY(ring.LeftX, ring.LocationY);
                    point2 = new PointXY(mirrorBbox.LeftX, ring.LocationY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.RightMiddle:
                    point1 = new PointXY(ring.RightX, ring.LocationY);
                    point2 = new PointXY(mirrorBbox.RightX, ring.LocationY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.TopLeft:
                    point1 = new PointXY(ring.LeftX, ring.TopY);
                    point2 = new PointXY(mirrorBbox.LeftX, ring.TopY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.TopRight:
                    point1 = new PointXY(ring.RightX, ring.TopY);
                    point2 = new PointXY(mirrorBbox.RightX, ring.TopY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.BottomLeft:
                    point1 = new PointXY(ring.LeftX, ring.BottomY);
                    point2 = new PointXY(mirrorBbox.LeftX, ring.BottomY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.BottomRight:
                    point1 = new PointXY(ring.RightX, ring.BottomY);
                    point2 = new PointXY(mirrorBbox.RightX, ring.BottomY);
                    break;
                case RectangleEdgeDistanceDimensionAnchorPoint.None: //Already checked for early escape
                default:
                    throw new EnumValueNotSupportedException(dimensionOptions.EdgeDistanceDimensionPosition);
            }
            builder.AddDimension(point1,
                                 point2,
                                 dimensionOptions.DistanceFromEdgePresentationOptions.GetDeepClone(),
                                 dimensionOptions.DistanceFromEdgeLineOptions.GetDeepClone(),
                                 mirrorBbox.RightX - ring.RightX,
                                 $"EdgeDistance of {drawName}");
        }

        /// <summary>
        /// Builds the Main Glass Rear Drawing
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="NotSupportedException"></exception>
        private void BuildMainGlassRear()
        {
            mirrorGlassRear = mirror.MirrorGlassShape.GetDeepClone();
            mirrorGlassRear.FlipHorizontally(mirrorGlassRear.GetBoundingBox().LocationX);
            var presOptions = OptionsRearDraw.GlassPresentationOptions.GetDeepClone();

            List<IDrawing> clips = [];
            var processes = mirror.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType);
            foreach (var process in processes)
            {
                var processModuleInfo = process.ModuleInfo as MirrorProcessModuleInfo
                    ?? throw new NotSupportedException($"Unexpected {typeof(MirrorModuleInfo).Name} Type for a Process Module (Unsupported Type :{process.ModuleInfo.GetType().Name})");
                var shape = processModuleInfo.ProcessShape.GetDeepClone();
                shape.FlipHorizontally(mirrorGlassRear.GetBoundingBox().LocationX); //flip to match mirror Rear flipping
                var clip = TechnicalDrawBuilder.CreateClipDrawing(shape, process.LocalizedDescriptionInfo.Name.DefaultValue);

                clips.Add(clip);
            }

            switch (mirrorGlassRear)
            {
                case RectangleInfo rectangle:
                    builder.AddRectangle(rectangle,
                                         presOptions,
                                         OptionsRearDraw.RectangleMirrorsDimensionOptions.GetDeepClone(),
                                         "RectangleMirrorGlassDraw",
                                         [..clips]);
                    break;
                case CircleInfo circle:
                    builder.AddCircle(circle,
                                      presOptions,
                                      OptionsRearDraw.CircleMirrorsDimensionOptions.GetDeepClone(),
                                      "CircleMirrorGlassDraw",
                                         [.. clips]);
                    break;
                case CapsuleInfo capsule:
                    builder.AddCapsule(capsule, presOptions, OptionsRearDraw.CapsuleMirrorsPresentationOptions.GetDeepClone(), "CapsuleMirrorGlassDraw");
                    break;
                case EllipseInfo ellipse:
                    builder.AddEllipse(ellipse, presOptions, OptionsRearDraw.EllipseMirrorsDimensionsOptions.GetDeepClone(), "EllipseMirrorGlassDraw");
                    break;
                case EggShapeInfo egg:
                    builder.AddEgg(egg, presOptions, OptionsRearDraw.EggMirrorsDimensionsOptions.GetDeepClone(), "EggMirrorGlassDraw");
                    break;
                case CircleSegmentInfo segment:
                    builder.AddCircleSegment(segment, presOptions, OptionsRearDraw.CircleSegmentMirrorsDimensionsOptions.GetDeepClone(), "CircleSegmentMirrorGlassDraw");
                    break;
                case CircleQuadrantInfo quadrant:
                    builder.AddCircleQuadrant(quadrant, presOptions, OptionsRearDraw.CircleQuadrantMirrorsDimensionsOptions.GetDeepClone(), "CircleQuadrantMirrorGlassDraw");
                    break;
                case RegularPolygonInfo regularPolygon:
                    builder.AddRegularPolygon(regularPolygon, presOptions, OptionsRearDraw.RegularPolygonMirrorsDimensionsOptions.GetDeepClone(), "RegularPolygonMirrorGlassDraw");
                    break;
                default:
                    throw new NotSupportedException($"Shape Type : {mirrorGlassRear.GetType().Name} is not Supported");
            }
        }
        /// <summary>
        /// Builds the Main Glass Front Drawing
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void BuildMainGlassFront()
        {
            mirrorGlassFront = mirror.MirrorGlassShape.GetDeepClone();
            
            var presOptions = OptionsFrontDraw.GlassPresentationOptions.GetDeepClone();

            List<IDrawing> clips = [];
            var processes = mirror.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType);
            foreach (var process in processes)
            {
                var processModuleInfo = process.ModuleInfo as MirrorProcessModuleInfo 
                    ?? throw new NotSupportedException($"Unexpected {typeof(MirrorModuleInfo).Name} Type for a Process Module (Unsupported Type :{process.ModuleInfo.GetType().Name})");
                var shape = processModuleInfo.ProcessShape.GetDeepClone();
                var clip = TechnicalDrawBuilder.CreateClipDrawing(shape,process.LocalizedDescriptionInfo.Name.DefaultValue);
                clips.Add(clip);
            }

            switch (mirrorGlassFront)
            {
                case RectangleInfo rectangle:
                    builder.AddRectangle(rectangle,
                                         presOptions,
                                         OptionsFrontDraw.RectangleMirrorsDimensionOptions.GetDeepClone(),
                                         "RectangleMirrorGlassDraw",
                                         [.. clips]);
                    break;
                case CircleInfo circle:
                    builder.AddCircle(circle,
                                      presOptions,
                                      OptionsFrontDraw.CircleMirrorsDimensionOptions.GetDeepClone(),
                                      "CircleMirrorGlassDraw",
                                      [.. clips]);
                    break;
                case CapsuleInfo capsule:
                    builder.AddCapsule(capsule, presOptions, OptionsFrontDraw.CapsuleMirrorsPresentationOptions.GetDeepClone(), "CapsuleMirrorGlassDraw");
                    break;
                case EllipseInfo ellipse:
                    builder.AddEllipse(ellipse, presOptions, OptionsFrontDraw.EllipseMirrorsDimensionsOptions.GetDeepClone(), "EllipseMirrorGlassDraw");
                    break;
                case EggShapeInfo egg:
                    builder.AddEgg(egg, presOptions, OptionsFrontDraw.EggMirrorsDimensionsOptions.GetDeepClone(), "EggMirrorGlassDraw");
                    break;
                case CircleSegmentInfo segment:
                    builder.AddCircleSegment(segment, presOptions, OptionsFrontDraw.CircleSegmentMirrorsDimensionsOptions.GetDeepClone(), "CircleSegmentMirrorGlassDraw");
                    break;
                case CircleQuadrantInfo quadrant:
                    builder.AddCircleQuadrant(quadrant, presOptions, OptionsFrontDraw.CircleQuadrantMirrorsDimensionsOptions.GetDeepClone(), "CircleQuadrantMirrorGlassDraw");
                    break;
                case RegularPolygonInfo regularPolygon:
                    builder.AddRegularPolygon(regularPolygon, presOptions, OptionsFrontDraw.RegularPolygonMirrorsDimensionsOptions.GetDeepClone(), "RegularPolygonMirrorGlassDraw");
                    break;
                default:
                    throw new NotSupportedException($"Shape Type : {mirrorGlassFront.GetType().Name} is not Supported");
            }
        }
        /// <summary>
        /// Builds the Main Glass Side Drawing
        /// </summary>
        private void BuildMainGlassSideView()
        {
            builder.AddRectangle(mirror.GetMirrorGlassSideView(),
                                 OptionsSideDraw.GlassPresentationOptions.GetDeepClone(),
                                 OptionsSideDraw.RectangleMirrorsDimensionOptions,
                                 "GlassSideDraw");
        }
    }

}
