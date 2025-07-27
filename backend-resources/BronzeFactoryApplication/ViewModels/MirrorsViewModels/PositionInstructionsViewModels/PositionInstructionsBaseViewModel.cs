using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels
{
    public partial class PositionInstructionsBaseViewModel : BaseViewModel
    {
        private PositionInstructionsType instructionsType;
        public PositionInstructionsType InstructionsType 
        {
            get => instructionsType;
            protected set => SetProperty(ref instructionsType, value, nameof(InstructionsType));
        }

        public void CopyBasePropertiesToModel(PositionInstructionsBase instructions)
        {
            //Empty
            //The Instructions type is set in the constructor of the InstructionObjects always and cannot be manipulated by the ViewModel
        }
        public void SetBaseProperties(PositionInstructionsBase instructions)
        {
            this.InstructionsType = instructions.InstructionsType;
        }
    }
}

