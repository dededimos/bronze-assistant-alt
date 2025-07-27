using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DrawingPresentationOptionsGlobalVm : BaseViewModel
    {

        public DrawingPresentationOptionsGlobalVm(Func<DrawBrushVm> brushVmFactory)
        {
            StrokeSketchDark = brushVmFactory.Invoke();
            StrokeSketchDark.SetModel(DrawingPresentationOptionsGlobal.StrokeSketchDark);
            StrokeSketchDark.PropertyChanged += StrokeSketchDark_PropertyChanged;

            StrokeSketchLight = brushVmFactory.Invoke();
            StrokeSketchLight.SetModel(DrawingPresentationOptionsGlobal.StrokeSketchLight);
            StrokeSketchLight.PropertyChanged += StrokeSketchLight_PropertyChanged;

            DarkFillDimensions = brushVmFactory.Invoke();
            DarkFillDimensions.SetModel(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DarkFill);
            DarkFillDimensions.PropertyChanged += DarkFillDimensions_PropertyChanged;

            LightFillDimensions = brushVmFactory.Invoke();
            LightFillDimensions.SetModel(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.LightFill);
            LightFillDimensions.PropertyChanged += LightFillDimensions_PropertyChanged;

            DarkStrokeDimensions = brushVmFactory.Invoke();
            DarkStrokeDimensions.SetModel(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DarkStroke);
            DarkStrokeDimensions.PropertyChanged += DarkStrokeDimensions_PropertyChanged;

            LightStrokeDimensions = brushVmFactory.Invoke();
            LightStrokeDimensions.SetModel(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.LightStroke);
            LightStrokeDimensions.PropertyChanged += LightStrokeDimensions_PropertyChanged;
        }

        #region Global
        public double TextHeight
        {
            get => DrawingPresentationOptionsGlobal.TextHeight;
            set
            {
                if (DrawingPresentationOptionsGlobal.TextHeight != value)
                {
                    DrawingPresentationOptionsGlobal.TextHeight = value;
                    OnPropertyChanged(nameof(TextHeight));
                    if (IsDimensionsTextHeightEqualToGlobal) OnPropertyChanged(nameof(DimensionsTextHeight));
                }
            }
        }

        public DimensionDrawingOption NormalDrawDimensionDrawingOption 
        { 
            get => DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            set
            {
                if (DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption != value)
                {
                    DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption = value;
                    OnPropertyChanged(nameof(NormalDrawDimensionDrawingOption));
                }
            }
        }
        public DimensionDrawingOption SketchDrawDimensionDrawingOption
        {
            get => DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption;
            set
            {
                if (DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption != value)
                {
                    DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption = value;
                    OnPropertyChanged(nameof(SketchDrawDimensionDrawingOption));
                }
            }
        }

        public DrawBrushVm StrokeSketchDark { get; }
        private void StrokeSketchDark_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.StrokeSketchDark = StrokeSketchDark.GetModel();
            OnPropertyChanged(nameof(StrokeSketchDark));
        }
        public DrawBrushVm StrokeSketchLight { get; }
        private void StrokeSketchLight_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.StrokeSketchLight = StrokeSketchLight.GetModel();
            OnPropertyChanged(nameof(StrokeSketchLight));
        }

        //Just for reference does not change by this property
        public DrawBrush CurrentStrokeSketch { get => DrawingPresentationOptionsGlobal.StrokeSketch; }

        public double StrokeThicknessSketch
        {
            get => DrawingPresentationOptionsGlobal.StrokeThicknessSketch;
            set
            {
                if (DrawingPresentationOptionsGlobal.StrokeThicknessSketch != value)
                {
                    DrawingPresentationOptionsGlobal.StrokeThicknessSketch = value;
                    OnPropertyChanged(nameof(StrokeThicknessSketch));
                }
            }
        }

        public List<double> StrokeDashArraySketch
        {
            get => DrawingPresentationOptionsGlobal.StrokeDashArraySketch;
            set
            {
                if (DrawingPresentationOptionsGlobal.StrokeDashArraySketch != value)
                {
                    DrawingPresentationOptionsGlobal.StrokeDashArraySketch = value;
                    OnPropertyChanged(nameof(StrokeDashArraySketch));
                }
            }
        }
        public string StrokeDashArraySketchString
        {
            get => new DoubleCollection(StrokeDashArraySketch).ToString();
            set
            {
                SetStrokeDashArraySketch(value);
            }
        }
        private void SetStrokeDashArraySketch(string stringValue)
        {
            if (stringValue != StrokeDashArraySketchString)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    DrawingPresentationOptionsGlobal.StrokeDashArraySketch = [];
                }
                else
                {
                    try
                    {
                        var collection = DoubleCollection.Parse(stringValue);
                        DrawingPresentationOptionsGlobal.StrokeDashArraySketch = new(collection);
                    }
                    catch (Exception ex)
                    {
                        DrawingPresentationOptionsGlobal.StrokeDashArraySketch = [];
                        MessageService.DisplayException(ex);
                    }
                }
                OnPropertyChanged(nameof(StrokeDashArraySketch));
                OnPropertyChanged(nameof(StrokeDashArraySketchString));
            }
        }
        #endregion

        #region Dimension Line Global
        public double ArrowThickness
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowThickness;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowThickness != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowThickness = value;
                    OnPropertyChanged(nameof(ArrowThickness));
                }
            }
        }

        public double ArrowLength
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowLength;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowLength != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowLength = value;
                    OnPropertyChanged(nameof(ArrowLength));
                }
            }
        }

        public double TextMarginFromDimensionLine
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine = value;
                    OnPropertyChanged(nameof(TextMarginFromDimensionLine));
                }
            }
        }

        public double OneEndLineLength
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.OneEndLineLength;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.OneEndLineLength != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.OneEndLineLength = value;
                    OnPropertyChanged(nameof(OneEndLineLength));
                }
            }
        }

        public int DimensionValueRoundingDecimals
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DimensionValueRoundingDecimals;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DimensionValueRoundingDecimals != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DimensionValueRoundingDecimals = value;
                    OnPropertyChanged(nameof(DimensionValueRoundingDecimals));
                }
            }
        }

        public double TwoLinesDimensionArrowLengthThresholdMultiplier
        {
            get => DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TwoLinesDimensionArrowLengthThresholdMultiplier;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TwoLinesDimensionArrowLengthThresholdMultiplier != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TwoLinesDimensionArrowLengthThresholdMultiplier = value;
                    OnPropertyChanged(nameof(TwoLinesDimensionArrowLengthThresholdMultiplier));
                }
            }
        }
        #endregion

        #region Dimension Pres Options Global
        public double DimensionsTextHeight
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight = value;
                    OnPropertyChanged(nameof(DimensionsTextHeight));
                }
            }
        }
        public bool IsDimensionsTextHeightEqualToGlobal
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.IsTextHeightEqualToGlobal;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.IsTextHeightEqualToGlobal != value)
                {
                    //If set to true , then set the TextHeight to NaN this way it equals global , otherwise set it to the value of the Global and user can change it 
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight = value is true ? double.NaN : DrawingPresentationOptionsGlobal.TextHeight;
                    OnPropertyChanged(nameof(DimensionsTextHeight));
                    OnPropertyChanged(nameof(IsDimensionsTextHeightEqualToGlobal));
                }
            }






        }

        public DrawBrushVm DarkFillDimensions { get; }
        private void DarkFillDimensions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DarkFill = DarkFillDimensions.GetModel();
            OnPropertyChanged(nameof(DarkFillDimensions));
        }

        public DrawBrushVm LightFillDimensions { get; }
        private void LightFillDimensions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.LightFill = LightFillDimensions.GetModel();
            OnPropertyChanged(nameof(LightFillDimensions));
        }

        //Just for Reference , does not change by this property
        public DrawBrush FillDimensions { get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Fill; }

        public DrawBrushVm DarkStrokeDimensions { get; }
        private void DarkStrokeDimensions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DarkStroke = DarkStrokeDimensions.GetModel();
            OnPropertyChanged(nameof(DarkStrokeDimensions));
        }
        public DrawBrushVm LightStrokeDimensions { get; }
        private void LightStrokeDimensions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.LightStroke = LightStrokeDimensions.GetModel();
            OnPropertyChanged(nameof(LightStrokeDimensions));
        }

        //Just for Reference , does not change by this property
        public DrawBrush StrokeDimensions { get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Stroke; }

        public double StrokeThicknessDimensions
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeThickness;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeThickness != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeThickness = value;
                    OnPropertyChanged(nameof(StrokeThicknessDimensions));
                }
            }
        }

        public List<double> StrokeDashArrayDimensions
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray = value;
                    OnPropertyChanged(nameof(StrokeDashArrayDimensions));
                }
            }
        }
        public string StrokeDashArrayDimensionsString
        {
            get => new DoubleCollection(StrokeDashArrayDimensions).ToString();
            set
            {
                SetStrokeDashArrayDimensions(value);
            }
        }
        private void SetStrokeDashArrayDimensions(string stringValue)
        {
            if (stringValue != StrokeDashArrayDimensionsString)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray = [];
                }
                else
                {
                    try
                    {
                        var collection = DoubleCollection.Parse(stringValue);
                        DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray = new(collection);
                    }
                    catch (Exception ex)
                    {
                        DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray = [];
                        MessageService.DisplayException(ex);
                    }
                }
                OnPropertyChanged(nameof(StrokeDashArrayDimensions));
                OnPropertyChanged(nameof(StrokeDashArrayDimensionsString));
            }
        }

        public List<double> StrokeDashArrayHelpLines
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines = value;
                    OnPropertyChanged(nameof(StrokeDashArrayHelpLines));
                }
            }
        }
        public string StrokeDashArrayHelpLinesString
        {
            get => new DoubleCollection(StrokeDashArrayHelpLines).ToString();
            set
            {
                SetStrokeDashArrayHelpLines(value);
            }
        }
        private void SetStrokeDashArrayHelpLines(string stringValue)
        {
            if (stringValue != StrokeDashArrayHelpLinesString)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines = [];
                }
                else
                {
                    try
                    {
                        var collection = DoubleCollection.Parse(stringValue);
                        DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines = new(collection);
                    }
                    catch (Exception ex)
                    {
                        DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines = [];
                        MessageService.DisplayException(ex);
                    }
                }
                OnPropertyChanged(nameof(StrokeDashArrayHelpLines));
                OnPropertyChanged(nameof(StrokeDashArrayHelpLinesString));
            }
        }

        public double OpacityDimensions
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Opacity;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Opacity != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Opacity = value;
                    OnPropertyChanged(nameof(OpacityDimensions));
                }
            }
        }

        public bool UseShadowDimensions
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.UseShadow;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.UseShadow != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.UseShadow = value;
                    OnPropertyChanged(nameof(UseShadowDimensions));
                }
            }
        }

        public double DimensionMarginFromShape
        {
            get => DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape;
            set
            {
                if (DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape != value)
                {
                    DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.DimensionMarginFromShape = value;
                    OnPropertyChanged(nameof(DimensionMarginFromShape));
                }
            }
        }
        #endregion

    }

}
