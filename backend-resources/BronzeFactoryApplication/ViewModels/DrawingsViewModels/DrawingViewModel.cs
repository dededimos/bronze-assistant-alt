using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.Helpers.Converters;
using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DrawingViewModel : BaseViewModel
    {
        public DrawingViewModel(Func<DrawingPresentationOptionsVm> presVmFactory,
            IWrappedViewsModalsGenerator wrappedModalsGenerator)
        {
            PresentationOptions = presVmFactory.Invoke();
            PresentationOptions.PropertyChanged += PresentationOptions_PropertyChanged;
            this.wrappedModalsGenerator = wrappedModalsGenerator;
            IsPolygonSimulatable = drawing is IPolygonSimulatableDrawing;
        }
        private readonly IWrappedViewsModalsGenerator wrappedModalsGenerator;
        protected IDrawing drawing = new RectangleDrawing(RectangleInfo.ZeroRectangle());
        protected IDrawing drawingCloneCenteredToContainer = new RectangleDrawing(RectangleInfo.ZeroRectangle());

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
        public string UniqueId { get => drawing.UniqueId; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPathDataGeometry))]
        private Geometry pathDataGeometry = Geometry.Empty;

        [ObservableProperty]
        private Geometry pathDataGeometryClone = Geometry.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPathDataGeometry))]
        private Geometry textPathDataGeometry = Geometry.Empty;

        [ObservableProperty]
        private Geometry textPathDataGeometryClone = Geometry.Empty;

        public Geometry TotalPathDataGeometry 
        { 
            get 
            {
                var group = new GeometryGroup();
                group.Children.Add(PathDataGeometry);
                group.Children.Add(TextPathDataGeometry);
                return group;
            } 
        }
        

        public string Name 
        { 
            get => drawing.Name;
            set => SetProperty(drawing.Name, value, drawing, (m, v) => m.Name = v);
}
        public DrawingPresentationOptionsVm PresentationOptions { get; }
        public int LayerNo { get => drawing.LayerNo; }
        public string? DrawingText
        {
            get => drawing.DrawingText;
            set { if (SetProperty(drawing.DrawingText, value, drawing, (m, v) => m.SetText(v))) UpdateTextData(); }
        }

        [ObservableProperty]
        private string errorMessage = string.Empty;
        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private int polygonSimulationSides = 10;
        partial void OnPolygonSimulationSidesChanged(int value)
        {
            if (value < minNumberOfSimulationSides)
            {
                PolygonSimulationSides = minNumberOfSimulationSides;
            }
            else
            {
                UpdatePathData();
            }
        }
        
        private int minNumberOfSimulationSides = 0;
        private int GetMinNumberOfSimulationSides()
        {
            return drawing is IPolygonSimulatableDrawing simulatable ? simulatable.MinSimulationSides : 0;
        }

        [ObservableProperty]
        private bool simulateAsPolygon = false;
        partial void OnSimulateAsPolygonChanged(bool value)
        {
            if (value is true)
            {
                CombineNormalWithPolygonSimulation = false;
            }
            UpdatePathData();
        }

        [ObservableProperty]
        private bool combineNormalWithPolygonSimulation = false;
        partial void OnCombineNormalWithPolygonSimulationChanged(bool value)
        {
            if (value is true)
            {
                SimulateAsPolygon = false;
            }
            UpdatePathData();
        }


        private bool isPolygonSimulatable;
        public bool IsPolygonSimulatable { get => isPolygonSimulatable; private set => SetProperty(ref isPolygonSimulatable, value, nameof(IsPolygonSimulatable)); }

        [RelayCommand]
        private void OpenEditDrawing()
        {
            wrappedModalsGenerator.OpenModal(this, $"{"lngEdit".TryTranslateKey()} {Name}");
        }

        /// <summary>
        /// Updates the Drawing of this View Model
        /// </summary>
        /// <param name="drawing">The Drawing Update or new</param>
        public void GenerateNewDrawing(IDrawing drawing)
        {
            try
            {
                SuppressPropertyNotifications();
                this.drawing = drawing;
                UpdateDrawingClone();

                //Reset only when the type of drawing changes
                ResetSimulationProperties();
                PresentationOptions.SetModel(this.drawing.PresentationOptions);
                PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(drawing.GetPathDataString() ?? string.Empty);
                PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(drawingCloneCenteredToContainer.GetPathDataString() ?? string.Empty);
                TextPathDataGeometry = DrawingToGeometryTextConverter.GetDrawingTextGeometry(drawing.TextAnchorLine,
                                                                                             drawing.DrawingText,
                                                                                             drawing.PresentationOptions.TextAnchorLineOption,
                                                                                             drawing.PresentationOptions.TextHeight,
                                                                                             true);
                TextPathDataGeometryClone = DrawingToGeometryTextConverter.GetDrawingTextGeometry(drawingCloneCenteredToContainer.TextAnchorLine,
                                                                                                  drawingCloneCenteredToContainer.DrawingText,
                                                                                                  drawingCloneCenteredToContainer.PresentationOptions.TextAnchorLineOption,
                                                                                                  drawingCloneCenteredToContainer.PresentationOptions.TextHeight,
                                                                                                  true);
                HasError = false;
                ErrorMessage = string.Empty;
                OnPropertyChanged(nameof(UniqueId));
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                Log.Information("Draw : {drawName} : {line}{message}{line}{stackTrace}", this.Name,Environment.NewLine,ex.Message,Environment.NewLine,ex.StackTrace);
            }
            finally
            {
                ResumePropertyNotifications();
                OnPropertyChanged("");
            }
        }
        private void UpdateDrawingClone()
        {
            this.drawingCloneCenteredToContainer = drawing.GetCloneCenteredToContainer(new(CloneContainerLength, CloneContainerHeight, 0, CloneContainerLength/2d, CloneContainerHeight/2d));
        }
        public void UpdateCurrentDrawingPaths()
        {
            UpdatePathData();
            UpdateTextData();
        }
        private void UpdatePathData()
        {
            try
            {
                if (IsPolygonSimulatable && SimulateAsPolygon)
                {
                    var simulatable = drawing as IPolygonSimulatableDrawing ?? throw new Exception("Unexpected non Polygon Simulatable Drawing");
                    PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(simulatable.GetPolygonSimulationPathData(PolygonSimulationSides) ?? string.Empty);
                    UpdateDrawingClone();
                    var simulatableClone = drawingCloneCenteredToContainer as IPolygonSimulatableDrawing ?? throw new Exception("Unexpected non Polygon Simulatable Drawing");
                    PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(simulatableClone.GetPolygonSimulationPathData(PolygonSimulationSides) ?? string.Empty);
                }
                else if (IsPolygonSimulatable && CombineNormalWithPolygonSimulation)
                {
                    var simulatable = drawing as IPolygonSimulatableDrawing ?? throw new Exception("Unexpected non Polygon Simulatable Drawing");
                    PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(simulatable.GetNormalAndSimulatedPathData(PolygonSimulationSides) ?? string.Empty);
                    UpdateDrawingClone();
                    var simulatableClone = drawingCloneCenteredToContainer as IPolygonSimulatableDrawing ?? throw new Exception("Unexpected non Polygon Simulatable Drawing");
                    PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(simulatableClone.GetNormalAndSimulatedPathData(PolygonSimulationSides) ?? string.Empty);
                }
                else
                {
                    PathDataGeometry = StringToGeometryConverter.ConvertToGeometry(drawing.GetPathDataString() ?? string.Empty);
                    UpdateDrawingClone();
                    PathDataGeometryClone = StringToGeometryConverter.ConvertToGeometry(drawingCloneCenteredToContainer.GetPathDataString() ?? string.Empty);
                }

                HasError = false;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                MessageService.LogAndDisplayException(ex);
                PathDataGeometry = Geometry.Empty;
                PathDataGeometryClone = Geometry.Empty;
            }
        }
        private void UpdateTextData()
        {
            try
            {
                TextPathDataGeometry = DrawingToGeometryTextConverter.GetDrawingTextGeometry(drawing.TextAnchorLine,
                                                                                             drawing.DrawingText,
                                                                                             drawing.PresentationOptions.TextAnchorLineOption,
                                                                                             drawing.PresentationOptions.TextHeight,
                                                                                             true);
                UpdateDrawingClone();
                TextPathDataGeometryClone = DrawingToGeometryTextConverter.GetDrawingTextGeometry(drawingCloneCenteredToContainer.TextAnchorLine,
                                                                                                  drawingCloneCenteredToContainer.DrawingText,
                                                                                                  drawingCloneCenteredToContainer.PresentationOptions.TextAnchorLineOption,
                                                                                                  drawingCloneCenteredToContainer.PresentationOptions.TextHeight,
                                                                                                  true);
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
        private void ResetSimulationProperties()
        {
            IsPolygonSimulatable = this.drawing is IPolygonSimulatableDrawing;
            minNumberOfSimulationSides = GetMinNumberOfSimulationSides();
            SimulateAsPolygon = false;
            PolygonSimulationSides = minNumberOfSimulationSides;
            CombineNormalWithPolygonSimulation = false;
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
                PresentationOptions.Dispose();
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
