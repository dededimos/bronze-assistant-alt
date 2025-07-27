using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService.Enums;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels
{
    public partial class PositionInstructionsBoundingBoxEditorViewModel : PositionInstructionsBaseViewModel, IEditorViewModel<PositionInstructionsBoundingBox>
    {
        [ObservableProperty]
        private HorizontalDistancing hDistancing;
        [ObservableProperty]
        private double horizontalDistance;
        [ObservableProperty]
        private VerticalDistancing vDistancing;
        [ObservableProperty]
        private double verticalDistance;

        public PositionInstructionsBoundingBox CopyPropertiesToModel(PositionInstructionsBoundingBox model)
        {
            CopyBasePropertiesToModel(model);
            model.HDistancing = this.HDistancing;
            model.HorizontalDistance = this.HorizontalDistance;
            model.VDistancing = this.VDistancing;
            model.VerticalDistance = this.VerticalDistance;
            return model;
        }

        public PositionInstructionsBoundingBox GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(PositionInstructionsBoundingBox model)
        {
            SetBaseProperties(model);
            this.HDistancing = model.HDistancing;
            this.HorizontalDistance = model.HorizontalDistance;
            this.VDistancing = model.VDistancing;
            this.VerticalDistance = model.VerticalDistance;
        }
    }
}

