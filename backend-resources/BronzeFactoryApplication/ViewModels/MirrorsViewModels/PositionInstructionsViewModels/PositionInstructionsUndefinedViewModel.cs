using CommonInterfacesBronze;
using MirrorsLib.Services.PositionService;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels
{
    public partial class PositionInstructionsUndefinedViewModel : PositionInstructionsBaseViewModel, IEditorViewModel<UndefinedPositionInstructions>
    {
        public UndefinedPositionInstructions CopyPropertiesToModel(UndefinedPositionInstructions model)
        {
            CopyBasePropertiesToModel(model);
            return model;
        }

        public UndefinedPositionInstructions GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(UndefinedPositionInstructions model)
        {
            SetBaseProperties(model);
        }
    }
}

