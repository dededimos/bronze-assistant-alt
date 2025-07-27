using DocumentFormat.OpenXml.Wordprocessing;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models;
using DrawingLibrary.Models.ConcreteGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class TechnicalDrawingViewModel(Func<DrawingViewModel> drawingViewModelFactory, Func<DimensionLineDrawingViewModel> dimensionViewModelFactory) : BaseViewModel
    {
        private readonly Func<DrawingViewModel> drawingViewModelFactory = drawingViewModelFactory;
        private readonly Func<DimensionLineDrawingViewModel> dimensionViewModelFactory = dimensionViewModelFactory;
        private ITechnicalDrawing? currentDrawing;
        public string Name { get => currentDrawing?.Name ?? "-"; }
        public int LayerNo { get => currentDrawing?.LayerNo ?? 0; }
        public double ContainerLength => currentDrawing?.ContainerOptions.ContainerLength ?? 500;
        public double ContainerHeight => currentDrawing?.ContainerOptions.ContainerHeight ?? 500;
        public Thickness ContainerMargin => new(currentDrawing?.ContainerOptions.ContainerMarginLeft ?? 80,
                                                currentDrawing?.ContainerOptions.ContainerMarginTop ?? 80 ,
                                                currentDrawing?.ContainerOptions.ContainerMarginRight ?? 80,
                                                currentDrawing?.ContainerOptions.ContainerMarginBottom ?? 80);
        public ObservableCollection<string> ErrorMessages { get; } = [];
        public ObservableCollection<DrawingViewModel> Drawings { get; set; } = [];
        public ObservableCollection<DimensionLineDrawingViewModel> Dimensions { get; set; } = [];
        public ObservableCollection<DrawingViewModel> HelpLines { get; set; } = [];

        /// <summary>
        /// Updates the Composite Drawing , by either setting a completly new one or updating any inner drawings only
        /// </summary>
        /// <param name="drawing"></param>
        public void GenerateNewDrawing(ITechnicalDrawing drawing)
        {
            //Clear and dispose
            List<DrawingViewModel> drawingsStore = new(Drawings);
            List<DimensionLineDrawingViewModel> dimensionsStore = new(Dimensions);
            List<DrawingViewModel> helplinesStore = new(HelpLines);
            Drawings.Clear();
            Dimensions.Clear();
            HelpLines.Clear();
            ErrorMessages.Clear();
            foreach (var drawingVm in drawingsStore) drawingVm.Dispose();
            foreach (var dimensionVm in dimensionsStore) dimensionVm.Dispose();
            foreach (var helplineVm in helplinesStore) helplineVm.Dispose();

            //Set the New
            currentDrawing = drawing;
            
            foreach (var d in currentDrawing.Drawings)
            {
                var vm = drawingViewModelFactory.Invoke();
                vm.GenerateNewDrawing(d);
                Drawings.Add(vm);

                if (vm.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {vm.Name} : {vm.ErrorMessage}");
                    //vm.Dispose();
                }
            }

            foreach (var d in currentDrawing.Dimensions)
            {
                var vm = dimensionViewModelFactory.Invoke();
                vm.GenerateNewDimension(d);
                Dimensions.Add(vm);

                if (vm.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {vm.Name} : {vm.ErrorMessage}");
                    //vm.Dispose();
                }
            }
            foreach (var d in currentDrawing.HelpLines)
            {
                var vm = drawingViewModelFactory.Invoke();
                vm.GenerateNewDrawing(d);
                HelpLines.Add(vm);

                if (vm.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {vm.Name} : {vm.ErrorMessage}");
                    //vm.Dispose();
                }
            }

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(LayerNo));
            OnPropertyChanged(nameof(ContainerLength));
            OnPropertyChanged(nameof(ContainerHeight));
            OnPropertyChanged(nameof(ContainerMargin));
        }

        private void UpdateCurrentDrawingPaths()
        {
            ErrorMessages.Clear();
            foreach (var vm in Drawings)
            {
                vm.UpdateCurrentDrawingPaths();
            }
            foreach (var vm in Dimensions)
            {
                vm.UpdateCurrentDrawingPaths();
            }
            foreach (var vm in HelpLines)
            {
                vm.UpdateCurrentDrawingPaths();
            }

            foreach (var draw in Drawings)
            {
                if (draw.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {draw.Name} : {draw.ErrorMessage}");
                }
            }
            foreach (var dim in Dimensions)
            {
                if (dim.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {dim.Name} : {dim.ErrorMessage}");
                }
            }
            foreach (var helpline in HelpLines)
            {
                if (helpline.HasError)
                {
                    ErrorMessages.Add($"{ErrorMessages.Count + 1}. {helpline.Name} : {helpline.ErrorMessage}");
                }
            }
        }

        [RelayCommand]
        private void ScaleDraw()
        {
            if (currentDrawing is not null)
            {
                currentDrawing.Scale(1.1,currentDrawing.BoundingBox.GetLocation());
                UpdateCurrentDrawingPaths();
            }
        }
        [RelayCommand]
        private void DescaleDraw()
        {
            if (currentDrawing is not null)
            {
                currentDrawing.Scale(1 / 1.1d,currentDrawing.BoundingBox.GetLocation());
                UpdateCurrentDrawingPaths();
            }
        }

        public ITechnicalDrawing? GetCurrentModel() => currentDrawing;

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
                foreach (var d in Drawings) d.Dispose();
                foreach (var d in Dimensions) d.Dispose();
                foreach (var d in HelpLines) d.Dispose();
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
