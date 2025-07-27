using CommonInterfacesBronze;
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
    public partial class DrawingPresentationOptionsVm : BaseViewModel , IEditorViewModel<DrawingPresentationOptions>
    {
        public DrawingPresentationOptionsVm(Func<DrawBrushVm> drawBrushVmFactory)
        {
            Fill = drawBrushVmFactory.Invoke();
            Stroke = drawBrushVmFactory.Invoke();
            model = new();
            Fill.SetModel(model.Fill);
            Stroke.SetModel(model.Stroke);
        }

        private void Stroke_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            model.Stroke = this.Stroke.GetModel();
            OnPropertyChanged(nameof(Stroke));
        }

        private void Fill_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            model.Fill = this.Fill.GetModel();
            OnPropertyChanged(nameof(Fill));
        }

        private DrawingPresentationOptions model;

        //The Fill and Stroke Vms do not Live change the Backing Model . A hook should be present to propagate the changes to the Model
        //as per the above too handlers , this way the model is always on par with the viewmodel.
        public DrawBrushVm Fill { get; }
        public DrawBrushVm Stroke { get; }

        public FillPatternType FillPattern
        {
            get => model.FillPattern;
            set => SetProperty(model.FillPattern, value, model, (m, v) => m.FillPattern = v);
        }

        public double StrokeThickness
        {
            get => model.StrokeThickness;
            set => SetProperty(model.StrokeThickness, value, model, (m, v) => m.StrokeThickness = v);
        }
        public List<double> StrokeDashArray
        {
            get => model.StrokeDashArray;
            set
            {
                if (SetProperty(model.StrokeDashArray, value, model, (m, v) => m.StrokeDashArray = v))
                {
                    OnPropertyChanged(nameof(StrokeDashArrayString));
                }
            }
        }
        
        public string StrokeDashArrayString
        {
            get => new DoubleCollection(StrokeDashArray).ToString();
            set
            {
                SetStrokeDashArray(value);
            }
        }


        public double Opacity
        {
            get => model.Opacity;
            set 
            {
                if (model.Opacity != value)
                {
                    //Set to 1 if bigger than 1
                    model.Opacity = value > 1 ? 1 : value;
                    OnPropertyChanged(nameof(Opacity));
                }
            }
        }
        public double DrawingTextHeight
        {
            get => model.TextHeight;
            set => SetProperty(model.TextHeight, value, model, (m, v) => m.TextHeight = v);
        }
        public bool UseShadow
        {
            get => model.UseShadow;
            set => SetProperty(model.UseShadow, value, model, (m, v) => m.UseShadow = v);
        }

        private void SetStrokeDashArray(string stringValue)
        {
            if (stringValue != StrokeDashArrayString)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    model.StrokeDashArray = [];
                }
                else
                {
                    try
                    {
                        var collection = DoubleCollection.Parse(stringValue);
                        model.StrokeDashArray = new(collection);
                    }
                    catch (Exception ex)
                    {
                        model.StrokeDashArray = [];
                        MessageService.DisplayException(ex);
                    }
                }
                OnPropertyChanged(nameof(StrokeDashArray));
            }
        }

        public void SetModel(DrawingPresentationOptions model)
        {
            //unregister old
            Fill.PropertyChanged -= Fill_PropertyChanged;
            Stroke.PropertyChanged -= Stroke_PropertyChanged;

            this.model = model;
            Fill.SetModel(this.model.Fill);
            Stroke.SetModel(this.model.Stroke);
            //register new
            Fill.PropertyChanged += Fill_PropertyChanged;
            Stroke.PropertyChanged += Stroke_PropertyChanged;
            OnPropertyChanged("");
        }

        public DrawingPresentationOptions CopyPropertiesToModel(DrawingPresentationOptions model)
        {
            model.Fill = this.Fill.GetModel();
            model.FillPattern = this.FillPattern;
            model.Stroke = this.Stroke.GetModel();
            model.StrokeThickness = this.StrokeThickness;
            model.StrokeDashArray = new(this.StrokeDashArray);
            model.Opacity = this.Opacity;
            model.TextHeight = this.DrawingTextHeight;
            model.UseShadow = this.UseShadow;
            return model;
        }

        public DrawingPresentationOptions GetModel()
        {
            return model;
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
                Fill.PropertyChanged -= Fill_PropertyChanged;
                Stroke.PropertyChanged -= Stroke_PropertyChanged;
                Fill.Dispose();
                Stroke.Dispose();
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
