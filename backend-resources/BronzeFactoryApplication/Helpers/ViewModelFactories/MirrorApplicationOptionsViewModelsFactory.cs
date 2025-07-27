using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Services.CodeBuldingService;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class MirrorApplicationOptionsViewModelsFactory : IAbstractFactory<IModelGetterViewModel<MirrorApplicationOptionsBase>, MirrorApplicationOptionsBase>
    {
        private readonly IServiceProvider serviceProvider;
        public MirrorApplicationOptionsViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<MirrorApplicationOptionsBase> Create(MirrorApplicationOptionsBase options)
        {
            //When the options is instantiated as simple MirrorAppplicationsBase then return an empty getter
            if (options.GetType() == typeof(MirrorApplicationOptionsBase))
            {
                var emptyVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorApplicationOptionsBase>>>().Invoke();
                emptyVm.SetModel(options);
                return emptyVm;
            }
            
            switch (options)
            {
                case MirrorCodesBuilderOptions codesBuilderOptions:
                    var codesBuilderOptionsVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorCodesBuilderOptions>>>().Invoke();
                    codesBuilderOptionsVm.SetModel(codesBuilderOptions);
                    return codesBuilderOptionsVm;
                default:
                    throw new NotSupportedException($"{nameof(MirrorApplicationOptionsBase)} of Type : {options.GetType().Name} is not supported by {this.GetType().Name}");
            }
        }
    }

}
