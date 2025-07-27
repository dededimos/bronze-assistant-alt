using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using DrawingLibrary.Models;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibraryPDFTemplates;
using MirrorsLib;
using MirrorsLib.DrawingElements;
using MirrorsLib.Enums;
using MirrorsLib.Validators;
using MirrorsModelsLibrary.Models;
using QuestPDF.Fluent;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DrawingLibraryPDFTemplates.DrawPdfDocument;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class MirrorSynthesisDrawingViewModel : BaseViewModel
    {
        private readonly MirrorSynthesisDrawBuilder builder;
        private readonly MirrorSynthesisValidator mirrorValidator = new();
        private readonly IWrappedViewsModalsGenerator wrappedViewModelModalsGenerator;
        private MirrorSynthesis? savedMirror;

        public MirrorSynthesisDrawingViewModel(Func<TechnicalDrawingViewModel> compositeDrawingFactory,
                                           Func<DrawContainerOptionsVm> containerOptionsVmFactory,
                                           MirrorSynthesisDrawBuilder drawBuilder,
                                           DrawingPresentationOptionsGlobalVm globalPresOptionsVm,
                                           Func<DrawPdfDocumentOptionsViewModel> drawPdfOptionsVmFactory,
                                           IWrappedViewsModalsGenerator wrappedViewModelModalsGenerator)
        {
            RearDrawingMirror = compositeDrawingFactory.Invoke();
            SideDrawingMirror = compositeDrawingFactory.Invoke();
            FrontDrawingMirror = compositeDrawingFactory.Invoke();
            RearDrawingGlass = compositeDrawingFactory.Invoke();
            SideDrawingGlass = compositeDrawingFactory.Invoke();
            FrontDrawingGlass = compositeDrawingFactory.Invoke();
            
            ContainerOptions = containerOptionsVmFactory.Invoke();
            ContainerOptions.PropertyChanged += ContainerOptionsVm_PropertyChanged;
            this.builder = drawBuilder;

            GlobalPresentationOptions = globalPresOptionsVm;
            GlobalPresentationOptions.PropertyChanged += GlobalPresentationOptions_PropertyChanged;

            this.wrappedViewModelModalsGenerator = wrappedViewModelModalsGenerator;

            this.ValidationErrors.CollectionChanged += ValidationErrors_CollectionChanged;
            DrawPdfOptionsVm = drawPdfOptionsVmFactory.Invoke(); //No need to set any model , defaults are set in the beginning
        }

        private void ValidationErrors_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Regenerates the Drawing whenever the Presentation Options are Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalPresentationOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            builder.SetDefaultDrawOptions();
            RegenerateCurrentDrawing();
        }

        /// <summary>
        /// Updates Current Draw whenever the Container Options are changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerOptionsVm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RegenerateCurrentDrawing();
        }

        /// <summary>
        /// The Draw of the Whole mirror in the Back
        /// </summary>
        public TechnicalDrawingViewModel RearDrawingMirror { get; }
        /// <summary>
        /// The Draw of the while mirror in the Side
        /// </summary>
        public TechnicalDrawingViewModel SideDrawingMirror { get; }
        /// <summary>
        /// The Draw of the Whole Mirror in the Front
        /// </summary>
        public TechnicalDrawingViewModel FrontDrawingMirror { get; }
        /// <summary>
        /// The Draw of ONLY the Glass on the Back
        /// </summary>
        public TechnicalDrawingViewModel RearDrawingGlass { get; }
        /// <summary>
        /// The Draw of ONLY the Glass on the Side
        /// </summary>
        public TechnicalDrawingViewModel SideDrawingGlass { get; }
        /// <summary>
        /// The Draw of ONLY the Glass on the Front
        /// </summary>
        public TechnicalDrawingViewModel FrontDrawingGlass { get; }

        public TechnicalDrawingViewModel? SelectedDrawing 
        { 
            //Depending on which Draw Type (Mirror or Glass is Selected)
            //Depending on which View is Selected 
            get
            {
                var view = IsFrontDrawSelected ? MirrorDrawingView.FrontView :
                IsRearDrawSelected ? MirrorDrawingView.RearView :
                IsSideDrawSelected ? MirrorDrawingView.SideView :
                MirrorDrawingView.None;
                return view switch
                {
                    MirrorDrawingView.FrontView => IsGlassOnlyDrawSelected ? FrontDrawingGlass : (IsMirrorDrawSelected  ? FrontDrawingMirror : null),
                    MirrorDrawingView.RearView => IsGlassOnlyDrawSelected ? RearDrawingGlass : (IsMirrorDrawSelected ? RearDrawingMirror : null),
                    MirrorDrawingView.SideView => IsGlassOnlyDrawSelected ? SideDrawingGlass : (IsMirrorDrawSelected ? SideDrawingMirror : null),
                    _ => null,
                };
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDrawing))]
        private bool isMirrorDrawSelected = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDrawing))]
        private bool isGlassOnlyDrawSelected;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDrawing))]
        private bool isFrontDrawSelected;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDrawing))]
        private bool isRearDrawSelected = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDrawing))]
        private bool isSideDrawSelected;


        [RelayCommand]
        private void ScaleDraw()
        {
            RearDrawingMirror.ScaleDrawCommand.Execute(null);
            SideDrawingMirror.ScaleDrawCommand.Execute(null);
            FrontDrawingMirror.ScaleDrawCommand.Execute(null);
        }
        [RelayCommand]
        private void DeScaleDraw()
        {
            RearDrawingMirror.DescaleDrawCommand.Execute(null);
            SideDrawingMirror.DescaleDrawCommand.Execute(null);
            FrontDrawingMirror.DescaleDrawCommand.Execute(null);
        }

        public DrawContainerOptionsVm ContainerOptions { get; }
        public DrawingPresentationOptionsGlobalVm GlobalPresentationOptions { get; }
        public DrawPdfDocumentOptionsViewModel DrawPdfOptionsVm { get; }

        /// <summary>
        /// The Errors produced from the Validation of the Mirror Glass
        /// </summary>
        public ObservableCollection<string> ValidationErrors { get; } = [];
        public bool HasErrors { get => ValidationErrors.Any(); }

        /// <summary>
        /// Generates a New Drawing with the given Glass
        /// </summary>
        /// <param name="mirror"></param>
        public void GenerateNewDrawing(MirrorSynthesis mirror)
        {
            savedMirror = mirror;
            var valResult = mirrorValidator.Validate(mirror);
            if (valResult.IsValid)
            {
                ValidationErrors.Clear();
                var containerOptions = ContainerOptions.GetModel();
                builder.SetMirror(mirror)
                    .SetBuilderOptions(new() { ContainerOptions = containerOptions })
                    .SetDefaultDrawOptions();
                
                //Front Draw is Called First , otherwise the Flippings in the Rear Draw will Fail

                var frontDraw = builder.BuildMirrorDraw(MirrorDrawingView.FrontView);
                var rearDraw = builder.BuildMirrorDraw(MirrorDrawingView.RearView);
                var sideDraw = builder.BuildMirrorDraw(MirrorDrawingView.SideView);

                var frontDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.FrontView);
                var rearDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.RearView);
                var sideDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.SideView);

                RearDrawingMirror.GenerateNewDrawing(rearDraw);
                SideDrawingMirror.GenerateNewDrawing(sideDraw);
                FrontDrawingMirror.GenerateNewDrawing(frontDraw);
                RearDrawingGlass.GenerateNewDrawing(rearDrawGlass);
                SideDrawingGlass.GenerateNewDrawing(sideDrawGlass);
                FrontDrawingGlass.GenerateNewDrawing(frontDrawGlass);
            }
            else
            {
                ValidationErrors.Clear();
                foreach (var error in valResult.Errors)
                {
                    savedMirror = null;
                    ValidationErrors.Add($"{ValidationErrors.Count + 1}. {error.ErrorCode}");
                }
            }
            PassChangedGlassOptionsToDrawPdfOptions();
        }

        /// <summary>
        /// Passes Changes of the Glass to the Draw Pdf Options fields changes automatically
        /// </summary>
        public void PassChangedGlassOptionsToDrawPdfOptions()
        {
            if (savedMirror is null)
            {
                DrawPdfOptionsVm.ChangeAutoCodeTitleTechRef("-", "-", "-");
            }
            else
            {
                var title = $"{("lngMirror".TryTranslateKeyWithoutError())} {savedMirror.DimensionsInformation.ShapeType.ToString().TryTranslateKeyWithoutError()}";
                var techRef = savedMirror.DimensionsInformation.ShapeType.ToString().TryTranslateKeyWithoutError();
                DrawPdfOptionsVm.ChangeAutoCodeTitleTechRef(savedMirror.GlassCode, title, techRef);
            }
        }

        /// <summary>
        /// Rebuilds the Composite Draw without resetting the Glass
        /// </summary>
        private void RegenerateCurrentDrawing()
        {
            if (HasErrors) return;
            var containerOptions = ContainerOptions.GetModel();
            builder.SetBuilderOptions(new() { ContainerOptions = containerOptions })
                   .SetDefaultDrawOptions();

            //Front Draw is Called First , otherwise the Flippings in the Rear Draw will Fail

            var frontDraw = builder.BuildMirrorDraw(MirrorDrawingView.FrontView);
            var rearDraw = builder.BuildMirrorDraw(MirrorDrawingView.RearView);
            var sideDraw = builder.BuildMirrorDraw(MirrorDrawingView.SideView);

            var frontDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.FrontView);
            var rearDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.RearView);
            var sideDrawGlass = builder.BuildOnlyGlassDraw(MirrorDrawingView.SideView);

            RearDrawingMirror.GenerateNewDrawing(rearDraw);
            SideDrawingMirror.GenerateNewDrawing(sideDraw);
            FrontDrawingMirror.GenerateNewDrawing(frontDraw);

            RearDrawingGlass.GenerateNewDrawing(rearDrawGlass);
            SideDrawingGlass.GenerateNewDrawing(sideDrawGlass);
            FrontDrawingGlass.GenerateNewDrawing(frontDrawGlass);
        }

        [RelayCommand]
        private void OpenDrawContainerOptions()
        {
            wrappedViewModelModalsGenerator.OpenModal(ContainerOptions, "lngEditDrawContainerOptionsTitle".TryTranslateKey());
        }

        [RelayCommand]
        private void OpenGlobalPresentationOptions()
        {
            wrappedViewModelModalsGenerator.OpenModal(GlobalPresentationOptions, "lngEditDrawingPresentationOptionsGlobal".TryTranslateKey());
        }
        [RelayCommand]
        private void OpenDrawPdfOptions()
        {
            wrappedViewModelModalsGenerator.OpenModal(DrawPdfOptionsVm, "lngEditPdfDrawOptions".TryTranslateKeyWithoutError());
        }

        [RelayCommand]
        private async Task ConvertDrawToPdf()
        {
            if (savedMirror is null)
            {
                MessageService.Error("Draw is Invalid", "Invalid Draw");
                return;
            }

            if (!HasErrors) //uses the already saved validated glass
            {
                var containerOptions = new DrawContainerOptions()
                {
                    //Interchange A4 page to be landscape
                    ContainerHeight = QuestPDF.Helpers.PageSizes.A4.Width,
                    ContainerLength = QuestPDF.Helpers.PageSizes.A4.Height,
                    ContainerMarginTop = 40,
                    ContainerMarginBottom = 40,
                    ContainerMarginRight = 40,
                    ContainerMarginLeft = 40,
                    MaxDimensionDepictedToScale = 1000
                };
                //TechnicalDrawing draw = builder.SetMirror(savedMirror)
                //                               .SetBuilderOptions(new() { CenterDrawToContainer = false, ScaleDrawToContainer = false, ScaleTotalDrawToContainer = true, ContainerOptions = containerOptions })
                //                               .SetDefaultLightThemeDrawOptions()
                //                               .EnableDisableSketchDraws(IsSketchDraw)
                //                               .BuildMirrorDraw(MirrorDrawingView.RearView);

                if (SelectedDrawing is null)
                {
                    MessageService.Warning($"There is no Draw to Print", "No Draw Present");
                    return;
                }
                var draw = SelectedDrawing.GetCurrentModel()?.GetDeepClone() as TechnicalDrawing ?? throw new Exception("Invalid Draw");
                draw.ContainerOptions = containerOptions;
                draw.ScaleTotalDrawToContainer();

                DrawPdfDocumentOptions docOptions = DrawPdfOptionsVm.GetModel();

                DrawPdfDocument doc = new(draw,docOptions);
                //This way the UI Thread does not freeze
                try
                {
                    await Task.Run(() => doc.GeneratePdfAndShow());
                }
                catch (Exception ex)
                {
                    MessageService.LogAndDisplayException(ex);
                }
                
            }
            else
            {
                MessageService.Error("Draw is Invalid", "Invalid Draw");
                return;
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
                ContainerOptions.PropertyChanged -= ContainerOptionsVm_PropertyChanged;
                this.ValidationErrors.CollectionChanged -= ValidationErrors_CollectionChanged;
                savedMirror = null;
                ContainerOptions.Dispose();
                RearDrawingMirror.Dispose();
                SideDrawingMirror.Dispose();
                FrontDrawingGlass.Dispose();
                RearDrawingGlass.Dispose();
                FrontDrawingGlass.Dispose();
                SideDrawingGlass.Dispose();
                GlobalPresentationOptions.PropertyChanged -= GlobalPresentationOptions_PropertyChanged;
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
