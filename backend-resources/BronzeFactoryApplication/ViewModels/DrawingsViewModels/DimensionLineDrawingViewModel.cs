using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.Helpers.Converters;
using DocumentFormat.OpenXml.Wordprocessing;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.ConcreteGraphics;
using ShapesLibrary.Interfaces;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DimensionLineDrawingViewModel : BaseViewModel
    {
        public DimensionLineDrawingViewModel(Func<DrawingPresentationOptionsVm> presVmFactory,
                                             Func<DimensionLineOptionsVm> lineOptionsVmFactory,
                                             IWrappedViewsModalsGenerator wrappedModalsGenerator) 
        {
            LineOptions = lineOptionsVmFactory.Invoke();
            LineOptions.PropertyChanged += LineOptions_PropertyChanged;
            PresentationOptions = presVmFactory.Invoke();
            PresentationOptions.PropertyChanged += PresentationOptions_PropertyChanged;
            this.wrappedModalsGenerator = wrappedModalsGenerator;
        }
        private readonly IWrappedViewsModalsGenerator wrappedModalsGenerator;
        private DimensionLineDrawing dimension = DimensionLineDrawing.Zero();
        protected DimensionLineDrawing dimensionCloneCenteredToContainer = DimensionLineDrawing.Zero();

        private void LineOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateCurrentDrawingPaths();
        }
        private void PresentationOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DrawingPresentationOptionsVm.DrawingTextHeight) || string.IsNullOrEmpty(e.PropertyName))
            {
                UpdateTextData();
            }
        }

        /// <summary>
        /// The Height Dimension of the Container that holds the centered clone of the drawing
        /// </summary>
        public double CloneContainerHeight { get; private set; } = 500;
        /// <summary>
        /// The Length Dimension of the Container that holds the centered clone of the drawing
        /// </summary>
        public double CloneContainerLength { get; private set; } = 500;
        public string Name 
        { 
            get => dimension.Name;
            set => SetProperty(dimension.Name, value, dimension, (m, v) => m.Name = v);
        }
        public int LayerNo { get => dimension.LayerNo; }
        public DimensionLineOptionsVm LineOptions { get; }
        public DrawingPresentationOptionsVm PresentationOptions { get; }
        public string UniqueId { get => dimension.UniqueId; }
        public string? DrawingText
        {
            get => dimension.DrawingText;
            set { if (SetProperty(dimension.DrawingText, value, dimension, (m, v) => m.SetText(v))) UpdateTextData(); }
        }

        [ObservableProperty]
        private Geometry pathDataGeometry = Geometry.Empty;
        [ObservableProperty]
        private Geometry pathDataGeometryClone = Geometry.Empty;
        [ObservableProperty]
        private Geometry textPathDataGeometry = Geometry.Empty;
        [ObservableProperty]
        private Geometry dimensionArrowheadsGeometry = Geometry.Empty;
        [ObservableProperty]
        private Geometry dimensionArrowheadsGeometryClone = Geometry.Empty;
        [ObservableProperty]
        private Geometry textPathDataGeometryClone = Geometry.Empty;

        public Geometry TotalPathDataGeometry
        {
            get
            {
                var group = new GeometryGroup();
                group.Children.Add(PathDataGeometry);
                group.Children.Add(TextPathDataGeometry);
                if (DimensionArrowheadsGeometry != Geometry.Empty)
                {
                    group.Children.Add(DimensionArrowheadsGeometry);
                }
                return group;
            }
        }
        [ObservableProperty]
        private string errorMessage = string.Empty;
        [ObservableProperty]
        private bool hasError;

        [RelayCommand]
        private void OpenEditDrawing()
        {
            wrappedModalsGenerator.OpenModal(this, $"{"lngEdit".TryTranslateKey()} {Name}");
        }

        /// <summary>
        /// Updates the Dimension Drawing of this View Model
        /// </summary>
        /// <param name="dimension">The new Dimension Drawing</param>
        public void GenerateNewDimension(DimensionLineDrawing dimension)
        {
            try
            {
                SuppressPropertyNotifications();
                this.dimension = dimension;
                UpdateDimensionClone();

                PresentationOptions.SetModel(this.dimension.PresentationOptions);
                LineOptions.SetModel(this.dimension.LineOptions);
                PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(this.dimension.GetPathDataString() ?? string.Empty);
                PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(this.dimensionCloneCenteredToContainer.GetPathDataString() ?? string.Empty);
                DimensionArrowheadsGeometry = StringToGeometryConverter.ConvertToGeometry(this.dimension.GetArrowsPathDataString() ?? string.Empty);
                DimensionArrowheadsGeometryClone = StringToGeometryConverter.ConvertToGeometry(this.dimensionCloneCenteredToContainer.GetArrowsPathDataString() ?? string.Empty);
                bool centerTextOption = true;
                if (this.dimension.ShouldRenderTwoLineDimension(this.dimension.Shape.GetTotalLength()))
                {
                    centerTextOption = this.dimension.LineOptions.CenterTextOnTwoLineDimension;
                }
                TextPathDataGeometry = DrawingToGeometryTextConverter.GetDrawingTextGeometry(this.dimension.TextAnchorLine,
                                                                                             this.dimension.DrawingText,
                                                                                             this.dimension.PresentationOptions.TextAnchorLineOption,
                                                                                             this.dimension.PresentationOptions.TextHeight,
                                                                                             centerTextOption);
                bool centerTextOptionClone = true;
                if (this.dimensionCloneCenteredToContainer.ShouldRenderTwoLineDimension(this.dimensionCloneCenteredToContainer.Shape.GetTotalLength()))
                {
                    centerTextOptionClone = this.dimensionCloneCenteredToContainer.LineOptions.CenterTextOnTwoLineDimension;
                }
                TextPathDataGeometryClone = DrawingToGeometryTextConverter.GetDrawingTextGeometry(this.dimensionCloneCenteredToContainer.TextAnchorLine,
                                                                                             this.dimensionCloneCenteredToContainer.DrawingText,
                                                                                             this.dimensionCloneCenteredToContainer.PresentationOptions.TextAnchorLineOption,
                                                                                             this.dimensionCloneCenteredToContainer.PresentationOptions.TextHeight,
                                                                                             centerTextOptionClone);

                HasError = false;
                ErrorMessage = string.Empty;
                OnPropertyChanged(nameof(UniqueId));
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                Log.Information("Draw : {drawName} : {line}{message}{line}{stackTrace}",
                                this.Name,
                                Environment.NewLine,
                                ex.Message,
                                Environment.NewLine,
                                ex.StackTrace);
            }
            finally
            {
                ResumePropertyNotifications();
                OnPropertyChanged("");
            }
        }
        private void UpdateDimensionClone()
        {
            this.dimensionCloneCenteredToContainer = this.dimension.GetCloneCenteredToContainer(new(CloneContainerLength, CloneContainerHeight, 0, CloneContainerLength/2d, CloneContainerHeight/2d));
        }
        public void UpdateCurrentDrawingPaths()
        {
            UpdatePathData();
            UpdateTextData();
        }
        private void UpdateTextData()
        {
            try
            {
                bool centerTextOption = true;
                if (dimension.ShouldRenderTwoLineDimension(dimension.Shape.GetTotalLength())) centerTextOption = dimension.LineOptions.CenterTextOnTwoLineDimension;
                TextPathDataGeometry = DrawingToGeometryTextConverter.GetDrawingTextGeometry(dimension.TextAnchorLine, dimension.DrawingText, dimension.PresentationOptions.TextAnchorLineOption, dimension.PresentationOptions.TextHeight, centerTextOption);

                UpdateDimensionClone();
                bool centerTextOptionClone = true;
                if (dimensionCloneCenteredToContainer.ShouldRenderTwoLineDimension(dimensionCloneCenteredToContainer.Shape.GetTotalLength())) centerTextOptionClone = dimensionCloneCenteredToContainer.LineOptions.CenterTextOnTwoLineDimension;
                TextPathDataGeometryClone = DrawingToGeometryTextConverter.GetDrawingTextGeometry(dimensionCloneCenteredToContainer.TextAnchorLine,
                                                                                                  dimensionCloneCenteredToContainer.DrawingText,
                                                                                                  dimensionCloneCenteredToContainer.PresentationOptions.TextAnchorLineOption,
                                                                                                  dimensionCloneCenteredToContainer.PresentationOptions.TextHeight,
                                                                                                  centerTextOptionClone);

                HasError = false;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                TextPathDataGeometry = Geometry.Empty;
                TextPathDataGeometryClone = Geometry.Empty;
            }
        }
        private void UpdatePathData()
        {
            try
            {
                PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(dimension.GetPathDataString() ?? string.Empty);
                DimensionArrowheadsGeometry = StringToGeometryConverter.ConvertToGeometry(this.dimension.GetArrowsPathDataString() ?? string.Empty);
                UpdateDimensionClone();
                PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(this.dimensionCloneCenteredToContainer.GetPathDataString() ?? string.Empty);
                DimensionArrowheadsGeometryClone = StringToGeometryConverter.ConvertToGeometry(this.dimensionCloneCenteredToContainer.GetArrowsPathDataString() ?? string.Empty);

                HasError = false;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                MessageService.LogAndDisplayException(ex);
                PathDataGeometry = Geometry.Empty;
                DimensionArrowheadsGeometry = Geometry.Empty;
                PathDataGeometryClone = Geometry.Empty;
                DimensionArrowheadsGeometryClone = Geometry.Empty;
            }
        }
        
        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                PresentationOptions.PropertyChanged -= PresentationOptions_PropertyChanged;
                LineOptions.PropertyChanged -= LineOptions_PropertyChanged;
                PresentationOptions.Dispose();
                LineOptions.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
