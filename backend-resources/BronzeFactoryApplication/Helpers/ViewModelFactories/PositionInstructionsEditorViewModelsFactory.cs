using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class PositionInstructionsEditorViewModelsFactory : IAbstractFactory<IModelGetterViewModel<PositionInstructionsBase>, PositionInstructionsBase>
    {
        private readonly IServiceProvider serviceProvider;

        public PositionInstructionsEditorViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<PositionInstructionsBase> Create(PositionInstructionsBase instructions)
        {
            switch (instructions)
            {
                case PositionInstructionsBoundingBox box:
                    var boxVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<PositionInstructionsBoundingBox>>>().Invoke();
                    boxVm.SetModel(box);
                    return boxVm;
                case PositionInstructionsRadial radial:
                    var radialVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<PositionInstructionsRadial>>>().Invoke();
                    radialVm.SetModel(radial);
                    return radialVm;
                case UndefinedPositionInstructions undefined:
                    var undefinedVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedPositionInstructions>>>().Invoke();
                    undefinedVm.SetModel(undefined);
                    return undefinedVm;
                default:
                    throw new NotSupportedException($"{typeof(PositionInstructionsBase)} of Type : {instructions.GetType()} is not supported by {this.GetType().Name}");
            }
        }

        public IModelGetterViewModel<PositionInstructionsBase> CreateNew(PositionInstructionsType instructionsType)
        {
            switch (instructionsType)
            {
                case PositionInstructionsType.UndefinedInstructions:
                    var undefined = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedPositionInstructions>>>().Invoke();
                    undefined.SetModel(new());
                    return undefined;
                case PositionInstructionsType.RadialInstructions:
                    var radial = serviceProvider.GetRequiredService<Func<IEditorViewModel<PositionInstructionsRadial>>>().Invoke();
                    radial.SetModel(new());
                    return radial;
                case PositionInstructionsType.BoundingBoxInstructions:
                    var boundingBox = serviceProvider.GetRequiredService<Func<IEditorViewModel<PositionInstructionsBoundingBox>>>().Invoke();
                    boundingBox.SetModel(new());
                    return boundingBox;
                default:
                    throw new NotSupportedException($"{typeof(PositionInstructionsBase)} of Type : {instructionsType} is not supported by {this.GetType().Name}");
            }
        }
    }

}
