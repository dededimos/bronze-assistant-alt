using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements.ModulesDrawingOptions;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;
using MirrorsLib.DrawingElements.SupportsDrawingOptions;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;

namespace MirrorsLib.DrawingElements
{
    public class MirrorDrawOptions : IDeepClonable<MirrorDrawOptions>
    {
        public DrawingPresentationOptions SandblastPresentationOptions { get; set; } = new();
        public LineSandblastInfoDimensionOptions LineSandblastDimensionsOptions { get; set; } = new();
        public TwoLineSandblastInfoDimensionOptions TwoLineSandblastDimensionsOptions { get; set; } = new();
        public HoledRectangleSandblastInfoDimensionOptions HoledRectangleSandblastDimensionsOptions { get; set; } = new();
        public CircularSandblastInfoDimensionOptions CircularSandblastDimensionsOptions { get; set; } = new();

        public DrawingPresentationOptions SupportsPresentationOptions { get; set; } = new();
        public MirrorMultiSupportsDimensionOptions MultiSupportsDimensionOptions { get; set; } = new();
        public MirrorVisibleFrameSupportDimensionOptions VisibleFrameSupportRectangleDimensionOptions { get; set; } = new();
        public MirrorVisibleFrameSupportDimensionOptions VisibleFrameSupportCircleDimensionOptions { get; set; } = new();
        public MirrorBackFrameSupportDimensionOptions BackFrameSupportDimensionOptions { get; set; } = new();

        public MagnifierSandblastDimensionOptions MagnifierDimensionsOptions { get; set; } = new();
        public DrawingPresentationOptions GlassPresentationOptions { get; set; } = new();

        public RectangleDimensionsPresentationOptions RectangleMirrorsDimensionOptions { get; set; } = new();
        public CircleDimensionsPresentationOptions CircleMirrorsDimensionOptions { get; set; } = new();
        public CapsuleDimensionsPresentationOptions CapsuleMirrorsPresentationOptions { get; set; } = new();
        public EllipseDimensionsPresentationOptions EllipseMirrorsDimensionsOptions { get; set; } = new();
        public EggDimensionsPresentationOptions EggMirrorsDimensionsOptions { get; set; } = new();
        public CircleSegmentDimensionsPresentationOptions CircleSegmentMirrorsDimensionsOptions { get; set; } = new();
        public CircleQuadrantDimensionsPresentationOptions CircleQuadrantMirrorsDimensionsOptions { get; set; } = new();
        public RegularPolygonDimensionsPresentationOptions RegularPolygonMirrorsDimensionsOptions { get; set; } = new();

        public MirrorDrawOptions GetDeepClone()
        {
            var clone = new MirrorDrawOptions()
            {
                SandblastPresentationOptions = this.SandblastPresentationOptions.GetDeepClone(),
                LineSandblastDimensionsOptions = this.LineSandblastDimensionsOptions.GetDeepClone(),
                TwoLineSandblastDimensionsOptions = this.TwoLineSandblastDimensionsOptions.GetDeepClone(),
                HoledRectangleSandblastDimensionsOptions = this.HoledRectangleSandblastDimensionsOptions.GetDeepClone(),
                CircularSandblastDimensionsOptions = this.CircularSandblastDimensionsOptions.GetDeepClone(),

                SupportsPresentationOptions = this.SupportsPresentationOptions.GetDeepClone(),
                MultiSupportsDimensionOptions = this.MultiSupportsDimensionOptions.GetDeepClone(),
                VisibleFrameSupportRectangleDimensionOptions = this.VisibleFrameSupportRectangleDimensionOptions.GetDeepClone(),
                VisibleFrameSupportCircleDimensionOptions = this.VisibleFrameSupportCircleDimensionOptions.GetDeepClone(),
                BackFrameSupportDimensionOptions = this.BackFrameSupportDimensionOptions.GetDeepClone(),

                MagnifierDimensionsOptions = this.MagnifierDimensionsOptions.GetDeepClone(),
                GlassPresentationOptions = this.GlassPresentationOptions.GetDeepClone(),

                RectangleMirrorsDimensionOptions = this.RectangleMirrorsDimensionOptions.GetDeepClone(),
                CircleMirrorsDimensionOptions = this.CircleMirrorsDimensionOptions.GetDeepClone(),
                CapsuleMirrorsPresentationOptions = this.CapsuleMirrorsPresentationOptions.GetDeepClone(),
                EllipseMirrorsDimensionsOptions = this.EllipseMirrorsDimensionsOptions.GetDeepClone(),
                EggMirrorsDimensionsOptions = this.EggMirrorsDimensionsOptions.GetDeepClone(),
                CircleSegmentMirrorsDimensionsOptions = this.CircleSegmentMirrorsDimensionsOptions.GetDeepClone(),
                CircleQuadrantMirrorsDimensionsOptions = this.CircleQuadrantMirrorsDimensionsOptions.GetDeepClone(),
                RegularPolygonMirrorsDimensionsOptions = this.RegularPolygonMirrorsDimensionsOptions.GetDeepClone(),
            };
            return clone;
        }

        public static MirrorDrawOptions GetDefaultOptions(bool isSketch)
        {
            return new()
            {
                SandblastPresentationOptions = DefaultSandblastPresOptions(isSketch),
                LineSandblastDimensionsOptions = LineSandblastInfoDimensionOptions.Defaults(isSketch),
                TwoLineSandblastDimensionsOptions = TwoLineSandblastInfoDimensionOptions.Defaults(isSketch),
                HoledRectangleSandblastDimensionsOptions = HoledRectangleSandblastInfoDimensionOptions.Defaults(isSketch),
                CircularSandblastDimensionsOptions = CircularSandblastInfoDimensionOptions.Defaults(isSketch),

                SupportsPresentationOptions = DefaultSupportsPresOptions(isSketch),
                MultiSupportsDimensionOptions = MirrorMultiSupportsDimensionOptions.Defaults(isSketch),
                BackFrameSupportDimensionOptions = MirrorBackFrameSupportDimensionOptions.Defaults(isSketch),
                VisibleFrameSupportRectangleDimensionOptions = MirrorVisibleFrameSupportDimensionOptions.DefaultsRectangleRingFrame(isSketch),
                VisibleFrameSupportCircleDimensionOptions = MirrorVisibleFrameSupportDimensionOptions.DefaultsCircleRingFrame(isSketch),

                MagnifierDimensionsOptions = MagnifierSandblastDimensionOptions.Defaults(isSketch),

                GlassPresentationOptions = DefaultMirrorGlassPresOptions(isSketch),

                RectangleMirrorsDimensionOptions = GetDefaultDimensionOptions<RectangleDimensionsPresentationOptions>(isSketch, true),
                CircleMirrorsDimensionOptions = GetDefaultDimensionOptions<CircleDimensionsPresentationOptions>(isSketch, true),
                CapsuleMirrorsPresentationOptions = GetDefaultDimensionOptions<CapsuleDimensionsPresentationOptions>(isSketch, true),
                EllipseMirrorsDimensionsOptions = GetDefaultDimensionOptions<EllipseDimensionsPresentationOptions>(isSketch, true),
                EggMirrorsDimensionsOptions = GetDefaultDimensionOptions<EggDimensionsPresentationOptions>(isSketch, true),
                CircleSegmentMirrorsDimensionsOptions = GetDefaultDimensionOptions<CircleSegmentDimensionsPresentationOptions>(isSketch, true),
                CircleQuadrantMirrorsDimensionsOptions = GetDefaultDimensionOptions<CircleQuadrantDimensionsPresentationOptions>(isSketch, true),
                RegularPolygonMirrorsDimensionsOptions = GetDefaultDimensionOptions<RegularPolygonDimensionsPresentationOptions>(isSketch, true),
            };
        }
        private static DrawingPresentationOptions DefaultSupportsPresOptions(bool isSketch)
        {
            return new()
            {
                TextHeight = DrawingPresentationOptionsGlobal.TextHeight,
                //In sketch takes the color of stroke to fill the patterns
                Fill = isSketch ? DrawingPresentationOptionsGlobal.StrokeSketch : DrawBrushes.Black,
                FillPattern = isSketch ? FillPatternType.HatchLine45DegPattern : FillPatternType.NoPattern,
                Opacity = 1,
                Stroke = isSketch ? DrawingPresentationOptionsGlobal.StrokeSketch : DrawBrushes.Gray,
                StrokeThickness = isSketch ? DrawingPresentationOptionsGlobal.StrokeThicknessSketch : 1,
                StrokeDashArray = isSketch ? DrawingPresentationOptionsGlobal.StrokeDashArraySketch : [],
                UseShadow = false,
            };
        }

        public static DrawingPresentationOptions DefaultMirrorGlassPresOptions(bool isSketch)
        {
            return new()
            {
                TextHeight = DrawingPresentationOptionsGlobal.TextHeight,
                Fill = isSketch ? DrawBrushes.Empty : DrawBrushes.GlassGradientBlue,
                Opacity = 1,
                Stroke = isSketch ? DrawingPresentationOptionsGlobal.StrokeSketch : DrawBrushes.Empty,
                StrokeThickness = isSketch ? DrawingPresentationOptionsGlobal.StrokeThicknessSketch : 1,
                StrokeDashArray = isSketch ? DrawingPresentationOptionsGlobal.StrokeDashArraySketch : [],
                UseShadow = isSketch ? false : true,
            };
        }
        public static DrawingPresentationOptions DefaultSandblastPresOptions(bool isSketch)
        {
            return new()
            {
                TextHeight = DrawingPresentationOptionsGlobal.TextHeight,
                //In sketch takes the color of stroke to fill the patterns
                Fill = isSketch ? DrawingPresentationOptionsGlobal.StrokeSketch : DrawBrushes.SandblastGradientGray,
                FillPattern = isSketch ? FillPatternType.DotPattern : FillPatternType.NoPattern,
                Opacity = 1,
                Stroke = isSketch ? DrawingPresentationOptionsGlobal.StrokeSketch : DrawBrushes.Gray,
                StrokeThickness = isSketch ? DrawingPresentationOptionsGlobal.StrokeThicknessSketch : 1,
                StrokeDashArray = isSketch ? DrawingPresentationOptionsGlobal.StrokeDashArraySketch : [],
                UseShadow = false,
            };
        }
        public static T GetDefaultDimensionOptions<T>(bool isSketch, bool isMainDraw = false) where T : ShapeDimensionsPresentationOptions
        {
#pragma warning disable CS8603 // Possible null reference return. (NOT POSSIBLE)
            return typeof(T) switch
            {
                Type t when t == typeof(RectangleDimensionsPresentationOptions) => GetDefaultRectangleDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(RectangleRingDimensionsPresentationOptions) => GetDefaultRectangleRingDimensionOptions(isSketch) as T,
                Type t when t == typeof(CircleDimensionsPresentationOptions) => GetDefaultCircleDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(CircleRingDimensionsPresentationOptions) => GetDefaultCircleRingDimensionOptions(isSketch) as T,
                Type t when t == typeof(CapsuleDimensionsPresentationOptions) => GetDefaultCapsuleDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(CircleQuadrantDimensionsPresentationOptions) => GetDefaultQuadrantDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(EllipseDimensionsPresentationOptions) => GetDefaultEllipseDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(EggDimensionsPresentationOptions) => GetDefaultEggDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(CircleSegmentDimensionsPresentationOptions) => GetDefaultSegmentDimensionOptions(isSketch, isMainDraw) as T,
                Type t when t == typeof(RegularPolygonDimensionsPresentationOptions) => GetDefaultRegularPolygonDimensionOptions(isSketch, isMainDraw) as T,
                _ => throw new NotSupportedException($"Type {typeof(T).Name} is not supported for Default Dimension Options")
            };
#pragma warning restore CS8603 // Possible null reference return.
        }
        private static RectangleDimensionsPresentationOptions GetDefaultRectangleDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<RectangleDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOptions = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOptions)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowRadiuses = false;
                    break;
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                case DimensionDrawingOption.AllowAllDimensionDraws:
                    optionsDefaults.ShowRadiuses = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOptions);
            }
            optionsDefaults.RadiusOptionWhenTotalRadius = RectangleRadiusDimensionShowOption.ShowTopLeftRadius;
            optionsDefaults.RadiusOptionWhenAllZero = RectangleRadiusDimensionShowOption.ShowNone;
            optionsDefaults.SetRadiusMarginFromShape(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape);

            optionsDefaults.TopLeftRadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions()
                                                                           .WithStartRotationAngle(-3 * Math.PI / 4);
            optionsDefaults.TopLeftRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.TopRightRadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions()
                                                                            .WithStartRotationAngle(-Math.PI / 4);
            optionsDefaults.TopRightRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.BottomLeftRadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions()
                                                                              .WithStartRotationAngle(3 * Math.PI / 4);
            optionsDefaults.BottomLeftRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.BottomRightRadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions()
                                                                               .WithStartRotationAngle(Math.PI / 4);
            optionsDefaults.BottomRightRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            return optionsDefaults;
        }
        private static CircleDimensionsPresentationOptions GetDefaultCircleDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<CircleDimensionsPresentationOptions>(isSketch, isMainDraw);
            DimensionDrawingOption dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowRadius = false;
                    optionsDefaults.ShowDiameter = false;
                    break;
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                    optionsDefaults.ShowRadius = false;
                    optionsDefaults.ShowDiameter = isMainDraw;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowRadius = false;
                    optionsDefaults.ShowDiameter = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.ShowDiameterRadiusInsideShape = false;
            optionsDefaults.RadiusDiameterPositionAngleRadians = -Math.PI / 4;
            optionsDefaults.DiameterRadiusMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.DiameterRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline); ;
            optionsDefaults.DiameterLineOptions = DimensionLineOptions.DefaultDiameterLineOptions();
            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions();
            return optionsDefaults;
        }
        private static CapsuleDimensionsPresentationOptions GetDefaultCapsuleDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<CapsuleDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowDiameter = false;
                    optionsDefaults.ShowRadius = false;
                    optionsDefaults.ShowRectangleDimension = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowDiameter = false;
                    optionsDefaults.ShowRadius = true;
                    optionsDefaults.ShowRectangleDimension = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.DiameterRadiusDimensionPosition = CapsuleRadiusDimensionPosition.TopLeft;
            optionsDefaults.RectangleDimensionPosition = CapsuleRectangleDimensionPosition.LeftTop;
            optionsDefaults.DiameterRadiusMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.RectangleDimensionMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;

            optionsDefaults.DiameterLineOptions = DimensionLineOptions.DefaultDiameterLineOptions().WithStartRotationAngle(-3 * Math.PI / 4);
            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions().WithStartRotationAngle(-3 * Math.PI / 4);
            optionsDefaults.DiameterRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.RectangleDimensionLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions();
            optionsDefaults.RectangleDimensionPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline);
            return optionsDefaults;
        }
        private static CircleQuadrantDimensionsPresentationOptions GetDefaultQuadrantDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<CircleQuadrantDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowRadius = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowRadius = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.RadiusMarginFromShape = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine;
            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions();
            optionsDefaults.RadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterYAnchorline);
            return optionsDefaults;
        }
        private static EllipseDimensionsPresentationOptions GetDefaultEllipseDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<EllipseDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowRadiusMajor = false;
                    optionsDefaults.ShowRadiusMinor = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowRadiusMajor = true;
                    optionsDefaults.ShowRadiusMinor = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.RadiusDimensionPosition = EllipseRadiusDimensionPosition.LeftTop;
            optionsDefaults.RadiusMajorLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions()
                                                                         .WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.RADIUSPREFIX);
            optionsDefaults.RadiusMajorPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline);
            optionsDefaults.RadiusMinorLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions()
                                                                         .WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.RADIUSPREFIX);
            optionsDefaults.RadiusMinorPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline);
            return optionsDefaults;
        }
        private static EggDimensionsPresentationOptions GetDefaultEggDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<EggDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowCircleRadius = false;
                    optionsDefaults.ShowEllipseRadius = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowCircleRadius = true;
                    optionsDefaults.ShowEllipseRadius = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.CircleRadiusDimensionsPosition = EggCircleRadiusDimensionPosition.LeftTop;

            optionsDefaults.EllipseRadiusLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions().WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ELLIPSERADIUSPREFIX);
            optionsDefaults.EllipseRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline);

            optionsDefaults.CircleRadiusLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions().WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.CIRCLERADIUSPREFIX);
            optionsDefaults.CircleRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline);
            return optionsDefaults;
        }
        private static CircleSegmentDimensionsPresentationOptions GetDefaultSegmentDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<CircleSegmentDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowCircleRadius = false;
                    optionsDefaults.ShowDiameter = false;
                    optionsDefaults.ShowChordLength = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowCircleRadius = true;
                    optionsDefaults.ShowDiameter = true;
                    optionsDefaults.ShowChordLength = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.ChordLengthMarginFromShape = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine;
            optionsDefaults.RadiusDiameterMarginFromShape = optionsDefaults.ChordLengthMarginFromShape + 10;
            optionsDefaults.HeightMarginFromShape = optionsDefaults.RadiusDiameterMarginFromShape + 20;
            optionsDefaults.LengthMarginFromShape = optionsDefaults.RadiusDiameterMarginFromShape + 20;

            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions().WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.CIRCLERADIUSPREFIX);
            optionsDefaults.RadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterXAnchorline);

            optionsDefaults.DiameterLineOptions = DimensionLineOptions.DefaultRadiusLineOptions().WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.CIRCLEDIAMETERPREFIX);
            optionsDefaults.DiameterPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterXAnchorline);

            optionsDefaults.ChordLengthLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions().WithDimensionTextPrefix(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.CHORDLENGTHPREFIX);
            optionsDefaults.ChordLengthPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterYAnchorline);

            optionsDefaults.CircleExtensionPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithFill(DrawBrushes.Empty);
            return optionsDefaults;
        }
        private static RectangleRingDimensionsPresentationOptions GetDefaultRectangleRingDimensionOptions(bool isSketch)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<RectangleRingDimensionsPresentationOptions>(isSketch);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowThickness = false;
                    optionsDefaults.ShowInnerHeight = false;
                    optionsDefaults.ShowInnerLength = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowThickness = true;
                    optionsDefaults.ShowInnerHeight = false;
                    optionsDefaults.ShowInnerLength = false;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }

            optionsDefaults.ShowLength = false;
            optionsDefaults.ShowHeight = false;

            optionsDefaults.LengthPosition = RectangleLengthDimensionPosition.Top;
            optionsDefaults.HeightPosition = RectangleHeightDimensionPosition.Left;

            optionsDefaults.ThicknessMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.ThicknessPosition = RectangleRingThicknessDimensionPosition.LeftMiddle;

            optionsDefaults.InnerLengthMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.InnerLengthPosition = RectangleLengthDimensionPosition.Top;

            optionsDefaults.InnerHeightMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.InnerHeightPosition = RectangleHeightDimensionPosition.Left;

            optionsDefaults.ThicknessLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions().WithNonCenteredTextOnTwoLineDimension();
            optionsDefaults.ThicknessPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.InnerLengthLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions();
            optionsDefaults.InnerLengthPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterYAnchorline);

            optionsDefaults.InnerHeightLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions();
            optionsDefaults.InnerHeightPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterXAnchorline);

            return optionsDefaults;
        }
        private static CircleRingDimensionsPresentationOptions GetDefaultCircleRingDimensionOptions(bool isSketch)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<CircleRingDimensionsPresentationOptions>(isSketch);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowThickness = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowThickness = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }
            optionsDefaults.ShowDiameterRadiusInsideShape = false;
            optionsDefaults.RadiusDiameterPositionAngleRadians = -Math.PI / 4;
            optionsDefaults.DiameterLineOptions = DimensionLineOptions.DefaultDiameterLineOptions();
            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions();
            optionsDefaults.DiameterRadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.ThicknessLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions().WithNonCenteredTextOnTwoLineDimension();
            optionsDefaults.ThicknessPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch);
            return optionsDefaults;
        }
        private static RegularPolygonDimensionsPresentationOptions GetDefaultRegularPolygonDimensionOptions(bool isSketch, bool isMainDraw)
        {
            var optionsDefaults = GetDefaultShapeDimensionPresentationOptions<RegularPolygonDimensionsPresentationOptions>(isSketch, isMainDraw);
            var dimensionsDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionsDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    optionsDefaults.ShowRadius = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    optionsDefaults.ShowRadius = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionsDrawingOption);
            }
            optionsDefaults.RadiusMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            optionsDefaults.RadiusLineOptions = DimensionLineOptions.DefaultRadiusLineOptions();
            optionsDefaults.RadiusPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            optionsDefaults.HelpCirclePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithFill(DrawBrushes.Empty);

            return optionsDefaults;
        }
        private static T GetDefaultShapeDimensionPresentationOptions<T>(bool isSketch, bool isMainDraw = false)
            where T : ShapeDimensionsPresentationOptions, new()
        {
            T options = new()
            {
                ShowHeight = true,
                HeightMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape,
                HeightPosition = RectangleHeightDimensionPosition.Right,
                HeightLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                HeightPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterXAnchorline),
                ShowCenterHelpLines = isMainDraw && isSketch,
                HelpLinesPresentationOptions = DrawingPresentationOptions.DefaultHelpLineOptions(isSketch),

                ShowLength = true,
                LengthMarginFromShape = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape,
                LengthPosition = RectangleLengthDimensionPosition.Bottom,
                LengthLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                LengthPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferGreaterYAnchorline),
            };
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    options.ShowDimensions = false;
                    break;
                //If main draw do not hide
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                    options.ShowDimensions = isMainDraw;
                    break;
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                case DimensionDrawingOption.AllowAllDimensionDraws:
                    options.ShowDimensions = true;
                    options.ShowHeight = true;
                    options.ShowLength = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }

            switch (options)
            {
                case RectangleRingDimensionsPresentationOptions:
                case CapsuleDimensionsPresentationOptions:
                case RectangleDimensionsPresentationOptions:
                case CircleQuadrantDimensionsPresentationOptions:
                case EllipseDimensionsPresentationOptions:
                case EggDimensionsPresentationOptions:
                case CircleSegmentDimensionsPresentationOptions:
                case RegularPolygonDimensionsPresentationOptions:
                    return options;
                case CircleRingDimensionsPresentationOptions:
                case CircleDimensionsPresentationOptions:
                    options.ShowHeight = false;
                    options.ShowLength = false;
                    return options;
                default:
                    throw new NotSupportedException($"The Options Type : {options.GetType().Name} is not Supported");
            }
        }


    }

}
