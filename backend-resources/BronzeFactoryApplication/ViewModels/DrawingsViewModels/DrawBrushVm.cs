using BronzeFactoryApplication.Helpers.Other;
using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DrawBrushVm : BaseViewModel, IEditorViewModel<DrawBrush>
    {
        public DrawBrush BrushModel { get => GetModel(); }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BrushModel))]
        private string color = "#FF0000";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BrushModel))]
        private double gradientAngleDegrees;

        [ObservableProperty]
        private string colorStopToAdd = "#FF0000";
        [ObservableProperty]
        private double offsetStopToAdd = 0;

        public ObservableCollection<DrawGradientStop> GradientStops { get; } = [];
        public bool IsSolidColor { get => GradientStops.Count == 0; }


        [RelayCommand]
        private void AddStop()
        {
            //Error on Empty
            if (string.IsNullOrWhiteSpace(ColorStopToAdd))
            {
                MessageService.Warning("Please Choose a Valid Color , Cannot be Empty When Adding","Empty Chosen Color");
                return;
            }
            //Error on Invalid Format
            try
            {
                WPFHelpers.ConvertHexadecimalStringToColor(ColorStopToAdd, true);
            }
            catch (FormatException ex)
            {
                MessageService.DisplayException(ex);
                return;
            }

            GradientStops.Add(new DrawGradientStop(ColorStopToAdd,OffsetStopToAdd));
            OnPropertyChanged(nameof(GradientStops));
            OnPropertyChanged(nameof(IsSolidColor));
            OnPropertyChanged(nameof(BrushModel));
        }
        [RelayCommand]
        private void RemoveStop(DrawGradientStop stopToRemove)
        {
            bool removed = GradientStops.Remove(stopToRemove);
            if (!removed)
            {
                MessageService.Warning("The Stop You Are Trying to Remove Does Not Exist", "Stop Not Found");
                return;
            }
            
            OnPropertyChanged(nameof(GradientStops));
            OnPropertyChanged(nameof(IsSolidColor));
            OnPropertyChanged(nameof(BrushModel));
        }
        /// <summary>
        /// DOES NOT COPY ANYTHING - THIS IS A STUPID METHOD CANNOT WORK ON IMMUTABLE OBJECTS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DrawBrush CopyPropertiesToModel(DrawBrush model)
        {
            return GetModel();
        }

        public DrawBrush GetModel()
        {
            if (IsSolidColor)
            {
                return new(this.Color);
            }
            else
            {
                return new(GradientAngleDegrees, GradientStops.Select(s => s.GetDeepClone()).ToArray());
            }
        }

        public void SetModel(DrawBrush model)
        {
            SuppressPropertyNotifications();
            this.Color = model.Color;
            this.GradientAngleDegrees = model.GradientAngleDegrees;
            this.GradientStops.Clear();
            foreach (var stop in model.GradientStops)
            {
                GradientStops.Add(stop.GetDeepClone());
            }
            ResumePropertyNotifications();
            OnPropertyChanged(string.Empty);
        }
    }

}
