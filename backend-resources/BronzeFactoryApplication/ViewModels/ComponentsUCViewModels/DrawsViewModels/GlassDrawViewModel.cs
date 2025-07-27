using FluentValidation.Results;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGGlassDrawsLibrary;
using SVGGlassDrawsLibrary.ProcessDraws;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels
{
    public partial class GlassDrawViewModel : BaseViewModel
    {
        private GlassViewModel? glassVm;
        public GlassViewModel? GlassVm { get => glassVm; }

        private readonly ValidatorGlass glassValidator = new();

        [ObservableProperty]
        private GlassPerimeterDraw glassPerimeter = GlassPerimeterDraw.Empty();

        [ObservableProperty]
        private ObservableCollection<DimensionLineDraw> dimensions = new();

        [ObservableProperty]
        private ObservableCollection<DrawShape> draws = new();

        [ObservableProperty]
        private bool isGlassValid;
        [ObservableProperty]
        private string errorsText = string.Empty;

        public void SetGlassDraw(GlassViewModel glassVm)
        {
            this.glassVm = glassVm;
            RefreshDraws(glassVm.GetGlass());
            OnPropertyChanged(nameof(GlassVm));
            this.glassVm.PropertyChanged += GlassVm_PropertyChanged;
        }

        /// <summary>
        /// Refreshes the Draw of the Glass
        /// </summary>
        /// <param name="glass"></param>
        private void RefreshDraws(Glass glass)
        {
            if (!IsValidGlass(glass))
            {
                //If not Valid the set all to Empty and Glass to Not Valid
                GlassPerimeter = GlassPerimeterDraw.Empty();
                Dimensions = new();
                this.Draws = new();
                return;
            }

            GlassDraw draw = new(glass,true);

            //??NOT WORKING PROPERLY FOR GLASS PROCCESSES FOR SOME REASON I CANNOT FIND ... #TODO
            //draw.TranslateShapeToPositiveDimensions(); //ensure Dimensions do not get out of ViewBox
            draw.PerimeterDraw.Stroke = "black";
            GlassPerimeter = draw.PerimeterDraw;
            Dimensions = new(draw.Dimensions);
            List<DrawShape> exceptDims = new()
            {
                GlassPerimeter
            };
            exceptDims.AddRange(draw.Processes);
            this.Draws = new(exceptDims);
        }

        private bool IsValidGlass(Glass glass)
        {
            ValidationResult validationResult = glassValidator.Validate(glass);
            if (validationResult.IsValid)
            {
                IsGlassValid = true;
                ErrorsText = string.Empty;
                return true;
            }
            else
            {
                var errorCodes = validationResult.Errors.Select(e => e.ErrorCode);
                StringBuilder builder = new();
                int index = 0;
                foreach (string error in errorCodes)
                {
                    index++;
                    builder.Append(index)
                           .Append('.')
                           .Append(' ')
                           .Append(error.TryTranslateKey())
                           .Append(Environment.NewLine)
                           .Append(Environment.NewLine);
                }
                IsGlassValid = false;
                ErrorsText = builder.ToString();
                return false;
            }
        }


        private void GlassVm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(glassVm is not null)
            RefreshDraws(glassVm.GetGlass());
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
                if(glassVm is not null)
                this.glassVm.PropertyChanged -= GlassVm_PropertyChanged;
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}
