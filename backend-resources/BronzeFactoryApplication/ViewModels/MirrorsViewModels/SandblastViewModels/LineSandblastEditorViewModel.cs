using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Sandblasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    public partial class LineSandblastEditorViewModel : MirrorSandblastInfoBaseViewModel, IEditorViewModel<LineSandblast>
    {
        [ObservableProperty]
        private double? distanceFromTop;
        [ObservableProperty]
        private double? distanceFromBottom;
        [ObservableProperty]
        private double? distanceFromLeft;
        [ObservableProperty]
        private double? distanceFromRight;
        [ObservableProperty]
        private double? fixedLength;
        [ObservableProperty]
        private bool isVertical;
        [ObservableProperty]
        private double cornerRadius;

        public LineSandblastEditorViewModel()
        {
            SandblastType = MirrorsLib.Enums.MirrorSandblastType.LineSandblast;
        }

        public LineSandblast CopyPropertiesToModel(LineSandblast model)
        {
            base.CopyBasePropertiesToModel(model);
            model.DistanceFromTop= this.DistanceFromTop;
            model.DistanceFromBottom = this.DistanceFromBottom;
            model.DistanceFromLeft = this.DistanceFromLeft;
            model.DistanceFromRight = this.DistanceFromRight;
            model.FixedLength = this.FixedLength;
            model.IsVertical = this.IsVertical;
            model.CornerRadius = this.CornerRadius;
            return model;
        }

        public LineSandblast GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(LineSandblast model)
        {
            SupressedPropertyNotificationsBlock(() =>
            {
                SetBaseProperties(model);
                this.DistanceFromTop = model.DistanceFromTop;
                this.DistanceFromBottom = model.DistanceFromBottom;
                this.DistanceFromLeft = model.DistanceFromLeft;
                this.DistanceFromRight = model.DistanceFromRight;
                this.FixedLength = model.FixedLength;
                this.IsVertical = model.IsVertical;
                this.CornerRadius = model.CornerRadius;
            },true);
        }
    }
    
}
