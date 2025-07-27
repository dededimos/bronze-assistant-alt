using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Supports;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorSupportInstructionsEditorViewModel : BaseViewModel, IEditorViewModel<MirrorSupportInstructions>
    {
        [ObservableProperty]
        private SupportLengthOption? lengthOption;
        [ObservableProperty]
        private double lengthOptionValue;
        [ObservableProperty]
        private SupportVerticalDistanceOption? verticalDistanceOption;
        [ObservableProperty]
        private double verticalDistanceOptionValue;
        [ObservableProperty]
        private DistanceBetweenSupportsOption? distanceBetweenOption;
        [ObservableProperty]
        private double distanceBetweenOptionValue;
        [ObservableProperty]
        private int supportsNumber;
        [ObservableProperty]
        private double thickness;
        [ObservableProperty]
        private double depth;

        public MirrorSupportInstructions CopyPropertiesToModel(MirrorSupportInstructions model)
        {
            
            model.LengthOption = this.LengthOption ?? SupportLengthOption.Undefined;
            model.LengthOptionValue = this.LengthOptionValue;
            model.VerticalDistanceOption = this.VerticalDistanceOption ?? SupportVerticalDistanceOption.Undefined;
            model.VerticalDistanceOptionValue = this.VerticalDistanceOptionValue;
            model.DistanceBetweenOption = this.DistanceBetweenOption ?? DistanceBetweenSupportsOption.Undefined;
            model.DistanceBetweenOptionValue = this.DistanceBetweenOptionValue;
            model.SupportsNumber = this.SupportsNumber;
            model.Thickness = this.Thickness;
            model.Depth = this.Depth;
            return model;
        }

        public MirrorSupportInstructions GetModel()
        {
            return this.CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorSupportInstructions model)
        {
            SuppressPropertyNotifications();
            this.LengthOption = model.LengthOption is SupportLengthOption.Undefined ? null : model.LengthOption;
            this.LengthOptionValue = model.LengthOptionValue;
            this.VerticalDistanceOption = model.VerticalDistanceOption is SupportVerticalDistanceOption.Undefined ? null : model.VerticalDistanceOption;
            this.VerticalDistanceOptionValue = model.VerticalDistanceOptionValue;
            this.DistanceBetweenOption = model.DistanceBetweenOption is DistanceBetweenSupportsOption.Undefined ? null : model.DistanceBetweenOption;
            this.DistanceBetweenOptionValue = model.DistanceBetweenOptionValue;
            this.SupportsNumber = model.SupportsNumber;
            this.Thickness = model.Thickness;
            this.Depth = model.Depth;
            ResumePropertyNotifications();
            OnPropertyChanged("");
        }
    }
}
