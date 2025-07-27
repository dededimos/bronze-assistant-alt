using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorBackFrameSupportEditorViewModel : MirrorSupportInfoBaseViewModel, IEditorViewModel<MirrorBackFrameSupport>
    {
        [ObservableProperty]
        private double depth;
        [ObservableProperty]
        private double thickness;
        [ObservableProperty]
        private double distanceFromEdge;

        public MirrorBackFrameSupport CopyPropertiesToModel(MirrorBackFrameSupport model)
        {
            CopyBasePropertiesToModel(model);
            model.Depth = this.Depth;
            model.Thickness = this.Thickness;
            model.DistanceFromEdge = this.DistanceFromEdge;
            return model;
        }

        public MirrorBackFrameSupport GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorBackFrameSupport model)
        {
            SetBaseProperties(model);
            this.Thickness = model.Thickness;
            this.Depth = model.Depth;
            this.DistanceFromEdge = model.DistanceFromEdge;
        }
    }
}
