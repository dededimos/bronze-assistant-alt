using AccessoriesRepoMongoDB;
using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using AccessoriesRepoMongoDB.Validators;
using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ApplicationServices.NavigationService;
using BronzeFactoryApplication.ApplicationServices.NumberingServices;
using BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using BronzeFactoryApplication.ViewModels.DrawingsViewModels;
using BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels.MirrorModuleWithElementInfoVms;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using BronzeFactoryApplication.ViewModels.SettingsViewModels;
using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
using BronzeFactoryApplication.Views;
using CommonInterfacesBronze;
using DrawingLibrary;
using GalaxyStockHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MirrorsLib;
using MirrorsLib.DrawingElements;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using MirrorsLib.Services;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using MirrorsRepositoryMongoDB;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Repositories;
using MirrorsRepositoryMongoDB.Validators;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonValidators;
using ShapesLibrary.Interfaces;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UsersRepoMongoDb;

namespace BronzeFactoryApplication.Helpers.StartupHelpers
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds a simple Func Factory for <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        public static void AddTransientAndFactory<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddTransient<TService>();
            services.AddSingleton<Func<TService>>(s => s.GetRequiredService<TService>);
        }
        /// <summary>
        /// Adds a simple Func Factory for <typeparamref name="TInterface"/>
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        public static void AddTransientAndFactory<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddTransient<TInterface, TImplementation>();
            services.AddSingleton<Func<TInterface>>(s => s.GetRequiredService<TInterface>);
        }

        /// <summary>
        /// Adds a singleton Service as Lazy
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        public static void AddLazySingleton<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddSingleton<TService>();
            services.AddSingleton(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
        }
        
        public static void AddWindowModal<TViewModel, TOpenModalService>(this IServiceCollection services)
            where TViewModel : ModalViewModel
            where TOpenModalService : class
        {
            services.AddTransientAndFactory<TViewModel>();
            services.AddSingleton<TOpenModalService>();
        }
        /// <summary>
        /// Adds the <typeparamref name="TModelViewModel"/> Editor of the Selected <typeparamref name="TModel"/>
        /// as well as the <see cref="IEditModelModalViewModel{TModel}"/> modal and the Respective Factories for all Of those
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TModelViewModel"></typeparam>
        /// <param name="services"></param>
        public static void AddEditModelModalViewModel<TModel, TModelViewModel>(this IServiceCollection services)
            where TModelViewModel : BaseViewModel, IEditorViewModel<TModel>
            where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>
        {
            services.AddTransient<IEditorViewModel<TModel>, TModelViewModel>();
            services.AddSingleton<Func<IEditorViewModel<TModel>>>(s => s.GetRequiredService<IEditorViewModel<TModel>>);
            services.AddTransient<IEditModelModalViewModel<TModel>, EditModelModalViewModel<TModel>>();
            services.AddSingleton<Func<IEditModelModalViewModel<TModel>>>(s => s.GetRequiredService<IEditModelModalViewModel<TModel>>);
        }

        /// <summary>
        /// Adds services for all the in WindowModals
        /// </summary>
        /// <param name="services"></param>
        public static void AddInWindowModals(this IServiceCollection services)
        {
            //The Container that Holds All InWindow Modals
            services.AddSingleton<ModalsContainerViewModel>();
            //Modal Editing Constraints of a Cabin
            services.AddWindowModal<EditCabinConstraintsModalViewModel, OpenModalAdvancedCabinPropsService>();
            //Modal Editing The Main Properties of a Cabin
            services.AddWindowModal<EditCabinModalViewModel, OpenEditCabinModalService>();
            //Modal Adding a Structure to the Glasses Orders
            services.AddWindowModal<AddSynthesisToOrderModalViewModel, OpenAddSynthesisToOrderModalService>();
            //Modal for Editting GlassRows - Vm is Registered in Cabin Services
            services.AddSingleton<OpenEditGlassRowModalService>();
            //Modal Retrieving and Showing Glasses Orders from the Database
            services.AddWindowModal<GlassesOrdersDisplayModalViewModel, OpenRetrieveOrdersModalService>();
            //Modal To Edit Details of a Glasses Order (Number , Notes e.t.c.)
            services.AddWindowModal<EditGlassesOrderDetailsModalViewModel, OpenEditGlassesOrderDetailsModalService>();
            //Modal to Add/Select Settings for the Glasses Order Xls Generation
            services.AddWindowModal<XlsSettingsModalViewModel, OpenXlsSettingsSelectionModalService>();
            //The ViewModel of the Xls Order Generation Settings 
            services.AddTransientAndFactory<XlsSettingsGlassesViewModel>();
            //The Modal of Editing Xls Settings 
            services.AddWindowModal<XlsSettingsEditModalViewModel, OpenEditXlsSettingsModalService>();
            //The Modal For Accepting Glass Orders Retrieval
            services.AddWindowModal<EditGlassOrderRowQuantityModalViewModel, OpenEditGlassRowQuantityModalService>();
            //The Modal for the Galaxy Orders Modal (THIS IS NOT AN IN WINDOW MODAL)
            services.AddTransient<GalaxyOrdersDisplayViewModel>();
            services.AddTransient(s => new ImportOrderModalViewModel(s.GetRequiredService<GalaxyOrdersDisplayViewModel>()));
            services.AddSingleton<Func<ImportOrderModalViewModel>>((s) => s.GetRequiredService<ImportOrderModalViewModel>);
            services.AddSingleton<OpenImportOrderModalService>();
            //The Modal for the Stock Orders (THIS IS NOT AN IN WINDOW MODAL)
            services.AddWindowModal<GlassesStockModalViewModel, OpenGlassesStockModalService>();
            //The Modal to Edit a Cabin Part in a Current Structure
            services.AddWindowModal<LiveEditPartModalViewModel, OpenLiveEditPartModalService>();
            //The Modal to Edit The List of SubParts in a Cabin Part
            services.AddWindowModal<EditSubPartsModalViewModel, OpenEditSubPartsModal>();
            //The Modal For Viewing and Printing a Cabin Bill Of Materials
            services.AddWindowModal<PrintCabinBomModalViewModel, OpenPrintCabinBomModalService>();
            //The Modal for Editing the PartSets of a DefaultParts List of a Cabin Structure
            services.AddWindowModal<EditPartSetsModalViewModel, OpenEditPartSetsModalService>();
            //The Modal for Matching/Swap a Stocked Glass with a Currently Made Glass
            services.AddWindowModal<GlassMatchingModalViewModel, OpenGlassMatchingModalService>();
            //The Modal for the Swap Option Selections
            services.AddWindowModal<SelectSwapOptionsModalViewModel, OpenSelectSwapOptionsModalService>();
            //The Modal for Printing and Preview of a Glass Draw
            services.AddWindowModal<PrintPriviewGlassDrawModalViewModel, OpenPrintPreviewGlassModalService>();
            // The Modal for Editing Localized Strings
            services.AddWindowModal<LocalizedStringEditModalViewModel, OpenEditLocalizedStringModalService>();
            // The Modal for Viewing Images
            services.AddWindowModal<ImageViewerModalViewModel, OpenImageViewerModalService>();
            // The Modal for Editing Trait Classes
            services.AddWindowModal<EditTraitClassesModalViewModel, OpenEditTraitClassModalService>();
            // The Modal for Editing TraitGroup Entities
            services.AddWindowModal<EditTraitGroupModalViewModel, OpenEditTraitGroupModalService>();
            // The Modal to Convert Accessories into Trait Groups
            services.AddWindowModal<EntityToJsonXmlModalViewModel, OpenEntityToJsonXmlModal>();
            //The Modal to Edit User Info
            services.AddWindowModal<EditUsersInfoModalViewModel, OpenEditUserInfoModalService>();
            // The Modal to Edit User Accessories Options
            services.AddWindowModal<EditUserAccOptionsModalViewModel, OpenEditUserAccOptionsModalService>();
            //The Modal to Edit Custom Price Rules
            services.AddWindowModal<EditCustomPriceRulesModalViewModel, OpenEditCustomPriceRuleModalService>();
            //The Modal to Add Rows to the Mirrors Order
            services.AddWindowModal<AddRowToMirrorsOrderModalViewModel, OpenAddRowToMirrorOrderModalService>();

            //Add the Modal ViewModel that wraps viewmodels along with its factory Method
            services.AddTransientAndFactory<IWrappedViewModelModalViewModel, WrappedViewModelModalViewModel>();
            //The Service that Opens Wrapped ViewModel Modals
            services.AddSingleton<IWrappedViewsModalsGenerator, WrappedViewsModalsGenerator>();

            //The Service with which every modal Closes (Holds Closing Events)
            services.AddSingleton<CloseModalService>();
        }
        public static void AddEditModelInWindowModals(this IServiceCollection services)
        {
            //Add the EditModel Modal ViewModels
            services.AddEditModelModalViewModel<LocalizedString, LocalizedStringViewModel>();
            services.AddEditModelModalViewModel<IPRating, IPRatingEditorViewModel>();

            services.AddEditModelModalViewModel<RectangleInfo, RectangleInfoVm>();
            services.AddEditModelModalViewModel<RectangleRingInfo, RectangleRingInfoVm>();

            services.AddEditModelModalViewModel<CircleInfo, CircleInfoVm>();
            services.AddEditModelModalViewModel<CircleRingInfo, CircleRingInfoVm>();

            services.AddEditModelModalViewModel<CapsuleInfo, CapsuleInfoVm>();
            services.AddEditModelModalViewModel<EllipseInfo, EllipseInfoVm>();
            services.AddEditModelModalViewModel<CircleSegmentInfo, CircleSegmentInfoVm>();
            services.AddEditModelModalViewModel<CircleQuadrantInfo, CircleQuadrantInfoVm>();
            services.AddEditModelModalViewModel<EggShapeInfo, EggShapeInfoVm>();

            services.AddSingleton<IEditModelModalsViewModelFactory, EditModelModalsViewModelFactory>();
            services.AddSingleton<IEditModelModalsGenerator, EditModelModalsGenerator>();
            services.AddSingleton<ShapeInfoEditorViewModelsFactory>();
        }
        public static void AddMirrorsDataSpecificServices(this IServiceCollection services)
        {
            //Mirrors Connection
            services.AddSingleton<IMongoDbMirrorsConnection, MongoDbMirrorsConnection>();

            //The ViewModel to Edit Database Entities
            services.AddTransientAndFactory<MongoDatabaseEntityEditorViewModel>();

            //Mirror Constraints Repository
            services.AddSingleton<MirrorConstraintsEntityValidator>(new MirrorConstraintsEntityValidator(includeIdValidation: false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorConstraintsEntity>, MirrorConstraintsRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity>, MongoDatabaseEntityRepoCache<MirrorConstraintsEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorConstraintsEntity>, MirrorConstraintsEntityEditorViewModel>();
            services.AddSingleton<ConstraintsRepoManagerViewModel>();

            //Validator and Base Elements
            services.AddSingleton<MirrorElementEntityValidator>(new MirrorElementEntityValidator(false));
            services.AddSingleton<MirrorElementTraitEntityValidator>(new MirrorElementTraitEntityValidator(false));

            //Mirror Finish Traits
            services.AddSingleton<MirrorFinishElementEntityValidator>(new MirrorFinishElementEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorFinishElementEntity>, MirrorFinishElementEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorFinishElementEntity>, MongoDatabaseEntityRepoCache<MirrorFinishElementEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorFinishElementEntity>, MirrorFinishElementEntityEditorViewModel>();
            services.AddSingleton<MirrorFinishesRepoManagerViewModel>();

            //Mirror Custom Traits
            services.AddSingleton<CustomMirrorTraitEntityValidator>(new CustomMirrorTraitEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<CustomMirrorTraitEntity>, MirrorCustomTraitsRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<CustomMirrorTraitEntity>, MongoDatabaseEntityRepoCache<CustomMirrorTraitEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<CustomMirrorTraitEntity>, MirrorCustomTraitEntityEditorViewModel>();
            services.AddSingleton<MirrorCustomTraitsRepoManagerViewModel>();

            //The CustomElements Repositories
            services.AddSingleton<CustomMirrorElementEntityValidator>(new CustomMirrorElementEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<CustomMirrorElementEntity>, CustomMirrorElementsRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity>, MongoDatabaseEntityRepoCache<CustomMirrorElementEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<CustomMirrorElementEntity>, CustomMirrorElementEntityEditorViewModel>();
            services.AddSingleton<CustomMirrorElementsRepoManagerViewModel>();


            //Mirror Supports Repository
            services.AddSingleton<MirrorSupportEntityValidator>(new MirrorSupportEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorSupportEntity>, MirrorSupportEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorSupportEntity>, MongoDatabaseEntityRepoCache<MirrorSupportEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorSupportEntity>, MirrorSupportEntityEditorViewModel>();
            services.AddSingleton<MirrorSupportsRepoManagerViewModel>();

            //Mirror Sandblasts Repository
            services.AddSingleton<MirrorSandblastEntityValidator>(new MirrorSandblastEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorSandblastEntity>, MirrorSandblastEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorSandblastEntity>, MongoDatabaseEntityRepoCache<MirrorSandblastEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorSandblastEntity>, MirrorSandblastEntityEditorViewModel>();
            services.AddSingleton<MirrorSandblastsRepoManagerViewModel>();

            //Mirror Lights Repository
            services.AddSingleton<MirrorLightElementEntityValidator>(new MirrorLightElementEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorLightElementEntity>, MirrorLightElementEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorLightElementEntity>, MongoDatabaseEntityRepoCache<MirrorLightElementEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorLightElementEntity>, MirrorLightElementEntityEditorViewModel>();
            services.AddSingleton<MirrorLightElementsRepoManagerViewModel>();

            //Mirror Modules Repository
            services.AddSingleton<MirrorModuleEntityValidator>(new MirrorModuleEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorModuleEntity>, MirrorModuleEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorModuleEntity>, MongoDatabaseEntityRepoCache<MirrorModuleEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorModuleEntity>, MirrorModuleEntityEditorViewModel>();
            services.AddSingleton<MirrorModulesRepoManagerViewModel>();

            //Mirror Series Repository
            services.AddSingleton<MirrorSeriesElementEntityValidator>(new MirrorSeriesElementEntityValidator(false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorSeriesElementEntity>, MirrorSeriesElementEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity>, MongoDatabaseEntityRepoCache<MirrorSeriesElementEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorSeriesElementEntity>, MirrorSeriesElementEntityEditorViewModel>();
            services.AddSingleton<MirrorSeriesRepoManagerViewModel>();

            //Mirror Positions Repository
            services.AddSingleton<MirrorElementPositionEntityValidator>(new MirrorElementPositionEntityValidator(includeIdValidation: false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorElementPositionEntity>, MirrorElementPositionEntitiesRepostiory>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity>, MongoDatabaseEntityRepoCache<MirrorElementPositionEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorElementPositionEntity>, MirrorElementPositionEntityEditorViewModel>();
            services.AddSingleton<MirrorPositionsRepoManagerViewModel>();

            //Mirror Positions Options Repository
            services.AddSingleton<MirrorElementPositionOptionsEntityValidator>(new MirrorElementPositionOptionsEntityValidator(includeIdValidation: false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorElementPositionOptionsEntity>, MirrorPositionOptionsRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity>, MongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorElementPositionOptionsEntity>, MirrorElementPositionOptionsEntityEditorViewModel>();
            services.AddSingleton<PositionOptionsRepoManagerViewModel>();

            //Mirror Application Settings Repository
            services.AddSingleton<MirrorApplicationOptionsEntityValidator>(new MirrorApplicationOptionsEntityValidator(includeIdValidation: false));
            services.AddSingleton<IMongoDatabaseEntityRepo<MirrorApplicationOptionsEntity>, MirrorApplicationOptionsEntitiesRepository>();
            services.AddSingleton<IMongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity>, MongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity>>();
            services.AddTransientAndFactory<IMirrorEntityEditorViewModel<MirrorApplicationOptionsEntity>, MirrorApplicationOptionsEntityEditorViewModel>();
            services.AddSingleton<MirrorAplicationOptionsRepoManagerViewModel>();

            //Mirrors Orders Repository
            services.AddSingleton<MirrorsOrderEntityValidator>(new MirrorsOrderEntityValidator(false));
            services.AddSingleton<MirrorsOrdersRepository>();

            services.AddSingleton<IMirrorsEntitiesDataProvider, MirrorsEntitiesDataProvider>();
            services.AddSingleton<IMirrorsDataProvider, MirrorsDataProviderMongoImplementation>();
            services.AddSingleton<IMongoMirrorsRepository, MirrorsMongoRepository>();

            //The ViewModel Managing Mirrors Entities and the Wrapped Modal around it
            services.AddSingleton<MirrorsEntitiesManagmentViewModel>();
        }
        public static void AddMirrorsViewModelFactories(this IServiceCollection services)
        {
            services.AddTransientAndFactory<MirrorSynthesisBuilder>();

            services.AddSingleton<ModuleEditorViewModelsFactory>();
            services.AddSingleton<SandblastEditorViewModelsFactory>();
            services.AddSingleton<SupportsEditorViewModelsFactory>();
            services.AddSingleton<PositionInstructionsEditorViewModelsFactory>();
            services.AddSingleton<MirrorApplicationOptionsViewModelsFactory>();
            services.AddTransientAndFactory<MirrorSynthesisEditorViewModel>();
            services.AddTransientAndFactory<MirrorSynthesisEditorWithDrawViewModel>();
            services.AddShapeInfoViewModels();
            services.AddMirrorElementViewModels();

            //The Refactored One
            services.AddSingleton<ShapeInfoVmsFactory>();
        }
        public static void AddShapeInfoViewModels(this IServiceCollection services)
        {
            services.AddTransientAndFactory<RectangleInfoVm>();
            services.AddTransientAndFactory<CircleInfoVm>();
            services.AddTransientAndFactory<CapsuleInfoVm>();
            services.AddTransientAndFactory<EllipseInfoVm>();
            services.AddTransientAndFactory<CircleQuadrantInfoVm>();
            services.AddTransientAndFactory<CircleSegmentInfoVm>();
            services.AddTransientAndFactory<EggShapeInfoVm>();
            services.AddTransientAndFactory<RegularPolygonInfoVm>();
        }
        public static void AddMirrorElementViewModels(this IServiceCollection services)
        {
            //ElementInfo (Add also as not an interface...)
            services.AddTransientAndFactory<IEditorViewModel<MirrorElementBase>, MirrorElementInfoEditorViewModel>();
            services.AddTransientAndFactory<MirrorElementInfoEditorViewModel>();
            
            //Modules Editors Interface and direct implementation as well as MirrorModule<generic> editor
            services.AddTransientAndFactory<MirrorModuleEditorVmBase>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorModule>, MirrorModuleEditorVmBase>();
            
            services.AddTransientAndFactory<IEditorViewModel<BluetoothModuleInfo>, BluetoothModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorProcessModuleInfo>, ProcessModuleEditorViewModel>();

            services.AddTransientAndFactory<IEditorViewModel<MagnifierModuleInfo>, MagnifierModuleEditorViewModel>();
            services.AddTransientAndFactory<MagnifierModuleEditorViewModel>();
            
            services.AddTransientAndFactory<IEditorViewModel<MagnifierSandblastedModuleInfo>, MagnifierSandblastedModuleEditorViewModel>();
            services.AddTransientAndFactory<MagnifierSandblastedModuleEditorViewModel>();

            services.AddTransientAndFactory<IEditorViewModel<ResistancePadModuleInfo>, ResistancePadModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<ScreenModuleInfo>, ScreenModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<TouchButtonModuleInfo>, TouchButtonModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<TransformerModuleInfo>, TransformerModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<RoundedCornersModuleInfo>, RoundedCornersModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorLampModuleInfo>, MirrorLampModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorBackLidModuleInfo>, MirrorBackLidModuleEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<UndefinedMirrorModuleInfo>, MirrorModuleInfoUndefinedViewModel>();

            //Sandblasts
            services.AddTransientAndFactory<IEditorViewModel<TwoLineSandblast>, TwoLineSandblastEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<HoledRectangleSandblast>, HoledRectangleSandblastEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<LineSandblast>, LineSandblastEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<CircularSandblast>, CircularSandblastEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<UndefinedSandblastInfo>, MirrorSandblastInfoUndefinedViewModel>();
            
            //Generalized Sandblast (no interface implementation)
            services.AddTransientAndFactory<MirrorSandblastEditorViewModel>();

            //Supports
            services.AddTransientAndFactory<IEditorViewModel<MirrorSupportInstructions>, MirrorSupportInstructionsEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorMultiSupports>, MirrorMultiSupportsEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorVisibleFrameSupport>, MirrorVisibleFrameSupportEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorBackFrameSupport>, MirrorBackFrameSupportEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<UndefinedMirrorSupportInfo>, MirrorSupportInfoUndefinedViewModel>();

            //Generalized Support (no interface implementation
            services.AddTransientAndFactory<MirrorSupportEditorViewModel>();

            //Light Info Element
            services.AddTransientAndFactory<IEditorViewModel<MirrorLightInfo>, MirrorLightInfoEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorAdditionalLightInfo>, MirrorAdditionalLightInfoEditorViewModel>();

            //Positions
            services.AddTransientAndFactory<IEditorViewModel<PositionInstructionsBoundingBox>, PositionInstructionsBoundingBoxEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<PositionInstructionsRadial>, PositionInstructionsRadialEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<UndefinedPositionInstructions>, PositionInstructionsUndefinedViewModel>();
            services.AddTransientAndFactory<MirrorElementPositionEditorViewModel>();
            

            //Series
            services.AddTransientAndFactory<IEditorViewModel<MirrorSeriesInfo>, MirrorSeriesInfoEditorViewModel>();

            //Constraints
            services.AddTransientAndFactory<IEditorViewModel<MirrorConstraints>, MirrorConstraintsEditorViewModel>();
            //Without Interface => Direct Implementation , the above should be deprecated if manager vm excludes constraints.
            services.AddTransientAndFactory<MirrorConstraintsEditorViewModel>();

            //Options
            services.AddTransientAndFactory<IEditorViewModel<MirrorCodesBuilderOptions>, MirrorCodesBuilderOptionsEditorViewModel>();
            services.AddTransientAndFactory<IEditorViewModel<MirrorApplicationOptionsBase>, MirrorApplicationOptionsUndefinedViewModel>();
            services.AddTransientAndFactory<ElementCodeAffixOptionsEditorViewModel>();//The concrete type is requested in the application, Not used as an editor ViewModel although it is
        }
        public static void AddNewDrawingServices(this IServiceCollection services)
        {
            services.AddTransientAndFactory<DrawingViewModel>();
            services.AddTransientAndFactory<DimensionLineDrawingViewModel>();
            services.AddTransientAndFactory<TechnicalDrawingViewModel>();

            services.AddTransientAndFactory<DrawBrushVm>();
            services.AddTransientAndFactory<DrawContainerOptionsVm>();
            services.AddTransientAndFactory<DrawingPresentationOptionsVm>();
            services.AddTransientAndFactory<DimensionLineOptionsVm>();
            services.AddSingleton<TechnicalDrawBuilder>();
            services.AddSingleton<MirrorSynthesisDrawBuilder>();
            services.AddTransientAndFactory<MirrorGlassDrawOptionsVm>();
            services.AddTransientAndFactory<MirrorSynthesisDrawingViewModel>();
            //Global Options only one Vm
            services.AddSingleton<DrawingPresentationOptionsGlobalVm>();

            services.AddTransientAndFactory<CapsuleDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<CircleDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<CircleRingDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<RectangleDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<RectangleRingDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<RegularPolygonDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<EllipseDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<CircleQuadrantDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<CircleSegmentDimensionsPresentationOptionsVm>();
            services.AddTransientAndFactory<EggDimensionsPresentationOptionsVm>();

            services.AddTransientAndFactory<LineSandblastInfoDimensionOptionsVm>();
            services.AddTransientAndFactory<CircularSandblastInfoDimensionOptionsVm>();
            services.AddTransientAndFactory<HoledRectangleSandblastInfoDimensionOptionsVm>();
            services.AddTransientAndFactory<TwoLineSandblastInfoDimensionOptionsVm>();

            services.AddTransientAndFactory<DrawPdfDocumentOptionsViewModel>();
        }
        public static void AddMirrorOrdersSepecificServices(this IServiceCollection services)
        {
            services.AddSingleton<MirrorsOrderViewModel>();
            services.AddSingleton<SelectMirrorsOrderViewModel>();
            services.AddTransientAndFactory<MirrorOrderRowViewModel>();
            services.AddTransientAndFactory<MirrorOrderRowUndoViewModel>();
        }

        /// <summary>
        /// Adds all the ViewModels of all the Cabins and their subclasses
        /// </summary>
        /// <param name="services"></param>
        public static void AddCabinsViewModels(this IServiceCollection services)
        {
            #region 1. Bronze6000 Registrations
            services.AddTransient<Parts9SViewModel>();
            services.AddTransient<Constraints9SViewModel>();
            services.AddTransient<Cabin9SViewModel>();
            services.AddSingleton<Func<Cabin9SViewModel>>((s) => s.GetRequiredService<Cabin9SViewModel>);

            services.AddTransient<Parts94ViewModel>();
            services.AddTransient<Constraints94ViewModel>();
            services.AddTransient<Cabin94ViewModel>();
            services.AddSingleton<Func<Cabin94ViewModel>>((s) => s.GetRequiredService<Cabin94ViewModel>);

            services.AddTransient<Parts9AViewModel>();
            services.AddTransient<Constraints9AViewModel>();
            services.AddTransient<Cabin9AViewModel>();
            services.AddSingleton<Func<Cabin9AViewModel>>((s) => s.GetRequiredService<Cabin9AViewModel>);

            services.AddTransient<Parts9BViewModel>();
            services.AddTransient<Constraints9BViewModel>();
            services.AddTransient<Cabin9BViewModel>();
            services.AddSingleton<Func<Cabin9BViewModel>>((s) => s.GetRequiredService<Cabin9BViewModel>);

            services.AddTransient<Parts9CViewModel>();
            services.AddTransient<Constraints9CViewModel>();
            services.AddTransient<Cabin9CViewModel>();
            services.AddSingleton<Func<Cabin9CViewModel>>((s) => s.GetRequiredService<Cabin9CViewModel>);

            services.AddTransient<Parts9FViewModel>();
            services.AddTransient<Constraints9FViewModel>();
            services.AddTransient<Cabin9FViewModel>();
            services.AddSingleton<Func<Cabin9FViewModel>>((s) => s.GetRequiredService<Cabin9FViewModel>);

            #endregion

            #region 2. DB Registration

            services.AddTransient<PartsDBViewModel>();
            services.AddTransient<ConstraintsDBViewModel>();
            services.AddTransient<CabinDBViewModel>();
            services.AddSingleton<Func<CabinDBViewModel>>((s) => s.GetRequiredService<CabinDBViewModel>);

            #endregion

            #region 3. Free Registration

            services.AddTransient<PartsEViewModel>();
            services.AddTransient<ConstraintsEViewModel>();
            services.AddTransient<CabinEViewModel>();
            services.AddSingleton<Func<CabinEViewModel>>((s) => s.GetRequiredService<CabinEViewModel>);

            services.AddTransient<PartsWViewModel>();
            services.AddTransient<ConstraintsWViewModel>();
            services.AddTransient<CabinWViewModel>();
            services.AddSingleton<Func<CabinWViewModel>>((s) => s.GetRequiredService<CabinWViewModel>);

            services.AddTransient<PartsWFlipperViewModel>();
            services.AddTransient<ConstraintsWFlipperViewModel>();
            services.AddTransient<CabinWFlipperViewModel>();
            services.AddSingleton<Func<CabinWFlipperViewModel>>((s) => s.GetRequiredService<CabinWFlipperViewModel>);
            #endregion

            #region 4. HB Registration

            services.AddTransient<PartsHBViewModel>();
            services.AddTransient<ConstraintsHBViewModel>();
            services.AddTransient<CabinHBViewModel>();
            services.AddSingleton<Func<CabinHBViewModel>>((s) => s.GetRequiredService<CabinHBViewModel>);

            #endregion

            #region 5. Inox304 Registration

            services.AddTransient<PartsV4ViewModel>();
            services.AddTransient<ConstraintsV4ViewModel>();
            services.AddTransient<CabinV4ViewModel>();
            services.AddSingleton<Func<CabinV4ViewModel>>((s) => s.GetRequiredService<CabinV4ViewModel>);

            services.AddTransient<PartsVAViewModel>();
            services.AddTransient<ConstraintsVAViewModel>();
            services.AddTransient<CabinVAViewModel>();
            services.AddSingleton<Func<CabinVAViewModel>>((s) => s.GetRequiredService<CabinVAViewModel>);

            services.AddTransient<PartsVFViewModel>();
            services.AddTransient<ConstraintsVFViewModel>();
            services.AddTransient<CabinVFViewModel>();
            services.AddSingleton<Func<CabinVFViewModel>>((s) => s.GetRequiredService<CabinVFViewModel>);

            services.AddTransient<PartsVSViewModel>();
            services.AddTransient<ConstraintsVSViewModel>();
            services.AddTransient<CabinVSViewModel>();
            services.AddSingleton<Func<CabinVSViewModel>>((s) => s.GetRequiredService<CabinVSViewModel>);
            #endregion

            #region 6. NB Registration

            services.AddTransient<PartsNBViewModel>();
            services.AddTransient<ConstraintsNBViewModel>();
            services.AddTransient<CabinNBViewModel>();
            services.AddSingleton<Func<CabinNBViewModel>>((s) => s.GetRequiredService<CabinNBViewModel>);

            #endregion

            #region 7. NP Registration

            services.AddTransient<PartsNPViewModel>();
            services.AddTransient<ConstraintsNPViewModel>();
            services.AddTransient<CabinNPViewModel>();
            services.AddSingleton<Func<CabinNPViewModel>>((s) => s.GetRequiredService<CabinNPViewModel>);

            #endregion

            #region 8. WS Registration

            services.AddTransient<PartsWSViewModel>();
            services.AddTransient<ConstraintsWSViewModel>();
            services.AddTransient<CabinWSViewModel>();
            services.AddSingleton<Func<CabinWSViewModel>>((s) => s.GetRequiredService<CabinWSViewModel>);

            #endregion

            //All the Various CabinViewModels Factories
            services.AddSingleton((s) =>
            {
                CabinViewModelFactory vmsFactory = new();
                vmsFactory.RegisterViewModelFactory<Cabin9S>(s.GetRequiredService<Func<Cabin9SViewModel>>());
                vmsFactory.RegisterViewModelFactory<Cabin94>(s.GetRequiredService<Func<Cabin94ViewModel>>());
                vmsFactory.RegisterViewModelFactory<Cabin9A>(s.GetRequiredService<Func<Cabin9AViewModel>>());
                vmsFactory.RegisterViewModelFactory<Cabin9B>(s.GetRequiredService<Func<Cabin9BViewModel>>());
                vmsFactory.RegisterViewModelFactory<Cabin9C>(s.GetRequiredService<Func<Cabin9CViewModel>>());
                vmsFactory.RegisterViewModelFactory<Cabin9F>(s.GetRequiredService<Func<Cabin9FViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinDB>(s.GetRequiredService<Func<CabinDBViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinE>(s.GetRequiredService<Func<CabinEViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinW>(s.GetRequiredService<Func<CabinWViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinWFlipper>(s.GetRequiredService<Func<CabinWFlipperViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinHB>(s.GetRequiredService<Func<CabinHBViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinV4>(s.GetRequiredService<Func<CabinV4ViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinVA>(s.GetRequiredService<Func<CabinVAViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinVF>(s.GetRequiredService<Func<CabinVFViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinVS>(s.GetRequiredService<Func<CabinVSViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinNB>(s.GetRequiredService<Func<CabinNBViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinNP>(s.GetRequiredService<Func<CabinNPViewModel>>());
                vmsFactory.RegisterViewModelFactory<CabinWS>(s.GetRequiredService<Func<CabinWSViewModel>>());
                return vmsFactory;
            });
        }

        /// <summary>
        /// Adds all the Repositories from Mongo DB and related Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddMongoDbRepos(this IServiceCollection services)
        {
            //Add the MongoDB Connection Service
            services.AddSingleton<IMongoConnection, MongoConnectionDefault>();

            //The Service that holds a TransactionSession and passes it over through the Methods that can use it to finish a trasaction
            services.AddSingleton<IMongoSessionHandler, MongoSessionHandler>();

            #region 1.Cabin Related Services
            //Add the MongoDB Connection for the Cabins
            services.AddSingleton<IMongoDbCabinsConnection, MongoDbCabinsConnection>();

            //The Database repository of the Cabin Parts
            services.AddSingleton<ICabinPartsRepository, MongoCabinPartsRepository>();

            //The Database Repository of the Constaints of Cabins
            services.AddSingleton<ICabinConstraintsRepository, MongoCabinConstraintsRepository>();

            //The Database Repository of the CabinParts Lists
            services.AddSingleton<ICabinPartsListsRepository, MongoCabinPartsListsRepository>();

            //The Database Repository of the Cabin Settings
            services.AddSingleton<ICabinSettingsRepository, MongoCabinSettingsRepository>();

            //The Database Repository of the Stocked Glasses
            services.AddSingleton<IGlassesStockRepository, MongoGlassesStockRepository>();

            //The Repository of all Cabin Parts/Settings /Constraints/DefaultLists
            services.AddSingleton<ICabinMemoryRepository, MongoCabinMemoryRepository>();

            //Contains the Repository of the Glasses Orders
            services.AddSingleton<IGlassOrderRepository, MongoGlassOrderRepository>();
            #endregion

            #region 2. Accessories Related Services
            //The Container with all the Needed Collections refering to Accessories
            services.AddSingleton<IMongoDbAccessoriesConnection, MongoDbAccessoriesConnection>();

            //The Repostiory for the Trait Classes of the Accessories
            services.AddSingleton<ITraitClassEntitiesRepository, MongoTraitClassEntitiesRepository>();
            //The Repository for the Traits of the Accessories
            services.AddSingleton<ITraitEntitiesRepository, MongoTraitEntitiesRepository>();
            //The Repository for the TraitGroups of AccessoriesTraits
            services.AddSingleton<ITraitGroupEntitiesRepository, MongoTraitGroupEntitiesRepository>();
            //The Repository for the Accessories
            services.AddSingleton<IAccessoryEntitiesRepository, MongoAccessoryEntitiesRepository>();
            // Add the AccessoriesUserOptions Repository
            services.AddSingleton<UserAccessoriesOptionsEntityValidator>();
            services.AddSingleton<UserAccessoriesOptionsRepository>();
            // Add the CustomPriceRules Repository
            services.AddSingleton<CustomPriceRuleEntityValidator>();
            services.AddSingleton<MongoPriceRuleEntityRepo>();
            // Add the Users Repository
            services.AddSingleton<UserInfoEntityValidator>();
            services.AddSingleton<UsersRepositoryMongo>(sp =>
            {
                var validator = sp.GetRequiredService<UserInfoEntityValidator>();
                var accConnection = sp.GetRequiredService<IMongoDbAccessoriesConnection>();
                var logger = sp.GetRequiredService<ILogger<MongoEntitiesRepository<UserInfoEntity>>>();
                return new UsersRepositoryMongo(validator, accConnection, logger);
            });
            #endregion
        }

        /// <summary>
        /// Adds the ViewModels for all the Main Pages
        /// </summary>
        /// <param name="services"></param>
        public static void AddMainPagesViewModels(this IServiceCollection services)
        {
            // The ViewModel of the Main Menu along with its Factory
            services.AddTransient<MenuViewModel>();
            services.AddSingleton<Func<MenuViewModel>>((s) => s.GetRequiredService<MenuViewModel>);

            //The ViewModel behind the MainView Where Structures and Glass Orders are Created
            //services.AddSingleton<ShowerCabinsModuleViewModel>();
            services.AddLazySingleton<ShowerCabinsModuleViewModel>();

            //The ViewModel behind the View responsible for Searching Orders
            //services.AddSingleton<OrdersModuleViewModel>();
            services.AddLazySingleton<OrdersModuleViewModel>();

            //The ViewModel behind the Various App Helpers View
            //services.AddSingleton<VariousAppHelpersViewModel>();
            services.AddLazySingleton<VariousAppHelpersViewModel>();

            //The ViewModel behind the ManagmentView
            //services.AddSingleton<ManagmentViewModel>();
            services.AddLazySingleton<ManagmentViewModel>();

            //The ViewModel behind the Accessories View
            //services.AddSingleton<AccessoriesModuleViewModel>();
            services.AddLazySingleton<AccessoriesModuleViewModel>();

            //The ViewModel behind the Mirrors Module View
            //services.AddSingleton<MirrorsModuleViewModel>();
            services.AddLazySingleton<MirrorsModuleViewModel>();

            //The Viewmodel behind the Wharehouse stock Module View
            services.AddSingleton<GalaxyStockService>(provider=>
            {
                var logger = provider.GetRequiredService<ILogger<GalaxyStockService>>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var key = configuration.GetSection("ThirdPartyCalls").GetValue<string>("GalaxyKey") ?? string.Empty;
                var stockCall = configuration.GetSection("ThirdPartyCalls").GetValue<string>("GalaxyStockCall") ?? string.Empty;
                var loginCall = configuration.GetSection("ThirdPartyCalls").GetValue<string>("GalaxyLogInCall") ?? string.Empty;
                return new GalaxyStockService(logger, key, stockCall, loginCall, new());
            });
            services.AddLazySingleton<WharehouseModuleViewModel>();

            services.AddSingleton<ItemStockMongoRepository>(provider =>
            {
                ItemStockEntityValidator validator = new(true);
                var logger = provider.GetRequiredService<ILogger<ItemStockMongoRepository>>();
                var mongoConnection = provider.GetRequiredService<IMongoDbAccessoriesConnection>();
                return new ItemStockMongoRepository(validator,mongoConnection,Microsoft.Extensions.Options.Options.Create(new MongoDatabaseEntityRepoOptions()), logger);
            });

        }
        /// <summary>
        /// Adds the Navigation Services to move around Views
        /// </summary>
        /// <param name="services"></param>
        public static void AddNavigationServices(this IServiceCollection services)
        {
            // The Navigation Store Service , storing Current ViewModel show (which is Tied with a Datatemplate to the View)
            services.AddSingleton<NavigationStore>();
            // The Navigation service for the Main Menu
            services.AddSingleton<MainMenuNavigationService>();
            // The Navigation service for the Cabin Creation View
            services.AddSingleton<CabinsModuleNavigationService>();
            // The Navigation Service for the Search Module
            services.AddSingleton<SearchOrdersModuleNavigationService>();
            // The Navigation Service for the Managment View
            services.AddSingleton<ManagmentViewNavigationService>();
            // The Navigation Service for the VariousAppHelpers
            services.AddSingleton<VariousAppHelpersModuleNavigationService>();
            // The Navigation Service for the Bathroom Accessories
            services.AddSingleton<BathroomAccessoriesNavigationService>();
            // The Navigation Service for the Mirrors
            services.AddSingleton<MirrorsModuleNavigationService>();
            // The Navigation Service for the Wharehouse Stock
            services.AddSingleton<WharehouseStockNavigationService>();
        }

        /// <summary>
        /// Adds all the Needed Services for the Settings to work through SQLite
        /// </summary>
        /// <param name="services"></param>
        /// <param name="localDatabaseFolderPath">the path to the Local Database</param>
        public static void AddSQLiteSettingsServices(this IServiceCollection services, string localDatabaseFolderPath)
        {
            //The Context of the SQLite Database that Provides Various Settings throughout the Application
            services.AddSingleton(new SettingsDbContextFactory($"Data Source={localDatabaseFolderPath}\\appSettings.db"));

            //A Service that Configurates the Initial Application Settings (Language / Theme)
            services.AddSingleton<ISettingsConfigurator, SettingsConfigurator>();

            //A Service that provides Application Settings through an SQLite Database
            services.AddSingleton<IGeneralSettingsProvider, GeneralSettingsProvider>();

            //A Service that provides Xls Generation Settings through an SQLite Database
            services.AddSingleton<IXlsSettingsProvider, XlsSettingsProvider>();

            //A Service that provides Search Order Settings through an SQLite Database
            services.AddSingleton<ISearchOrdersViewSettingsProvider, SearchOrdersViewSettingsProvider>();

            //A Service that provides Settings for the GlassStockService
            services.AddSingleton<IGlassesStockSettingsProvider, GlassesStockSettingsProvider>();
            services.AddTransient<GlassesStockSettingsViewModel>();
            services.AddSingleton<Func<GlassesStockSettingsViewModel>>(s => s.GetRequiredService<GlassesStockSettingsViewModel>);

            //The ViewModel managing the Settings for the xls Generation
            services.AddTransient<XlsSettingsGlassesViewModel>();
            services.AddSingleton<Func<XlsSettingsGlassesViewModel>>((s) => s.GetRequiredService<XlsSettingsGlassesViewModel>);
            //The ViewModel responsible for the Settings of Searching Orders
            services.AddSingleton<SearchOrdersViewSettingsViewModel>();
        }
        /// <summary>
        /// Adds All Services Related to Cabins
        /// </summary>
        /// <param name="services"></param>
        public static void AddAllCabinRelatedServices(this IServiceCollection services)
        {
            //A Validator Object to determine wheather a Code is coming from a Cabin
            services.AddSingleton<ValidatorCabinCode>();
            //The Factory that builds Cabins
            services.AddSingleton<CabinFactory>();
            //The Director Object for Calculating Glasses
            services.AddSingleton<GlassesBuilderDirector>();
            //The Glass Swapper Object for Swapping Glasses in a Cabin
            services.AddSingleton<CabinGlassSwapper>();
            //The Validator Object for Cabins 
            services.AddSingleton<CabinValidator>();
            //The Calculation Service for Cabins (Consumes Director Object)
            services.AddSingleton<CabinCalculationsService>();
            //The Translator for the Codes of a Synthesis into Cabin Structures
            services.AddSingleton<SynthesisCodeTranslator>();

            //The ViewModel for the Creation of a Synthesis Structure
            services.AddSingleton<SynthesisViewModel>();
            //The ViewModel Behind the Controls for Showing the Draw of a Synthesis
            services.AddTransient<SynthesisDrawViewModel>();
            //The Factory for the DrawViewModel of a Synthesis
            services.AddSingleton<Func<SynthesisDrawViewModel>>((s) => s.GetRequiredService<SynthesisDrawViewModel>);

            //Service that Applies Cabin Parts in Sets (When one changes , the rest do Also)
            services.AddSingleton<PartSetsApplicatorService>();

            //The Service Providing Glass Matching Functionality
            services.AddTransient<GlassMatchingService>();
            services.AddSingleton<Func<GlassMatchingService>>((s) => s.GetRequiredService<GlassMatchingService>);
            //The ViewModel of Glass Matching
            services.AddTransient<CabinsGlassMatchesViewModel>();
            services.AddSingleton<Func<CabinsGlassMatchesViewModel>>((s) => s.GetRequiredService<CabinsGlassMatchesViewModel>);

            //The Service providing functionality for the Stocked Glasses
            services.AddSingleton<GlassesStockService>();

            //The Numbering Service for Glasses
            services.AddTransient<GlassNumberingService>();
            services.AddSingleton<Func<GlassNumberingService>>((s) => s.GetRequiredService<GlassNumberingService>);

            //The ViewModel for the Current GlassesOrder Creation along with its Factory
            services.AddTransient<GlassesOrderViewModel>();
            services.AddSingleton<Func<GlassesOrderViewModel>>((s) => s.GetRequiredService<GlassesOrderViewModel>);

            //The ViewModel of a Glass Along with its Factory
            services.AddTransient<GlassViewModel>();
            services.AddSingleton<Func<GlassViewModel>>((s) => s.GetRequiredService<GlassViewModel>);

            //The EDITViewModel of a Glass Order Row along with its Factory
            services.AddTransient<GlassRowEditViewModel>();
            services.AddSingleton<Func<GlassRowEditViewModel>>((s) => s.GetRequiredService<GlassRowEditViewModel>);

            //The ViewModel of a Glass Order Row along with its Factory
            services.AddTransient<GlassOrderRowViewModel>();
            services.AddSingleton<Func<GlassOrderRowViewModel>>((s) => s.GetRequiredService<GlassOrderRowViewModel>);
            services.AddSingleton<GlassOrderRowViewModelFactory>();

            //The ViewModel Responsible behind the Module which Selects and Builds Structures from ItemCodes
            services.AddTransient<ChooseCabinModelViewModel>();

            //The ViewModel responsible for the Cabin Bill Of Materials
            services.AddTransient<CabinBomViewModel>();
            services.AddSingleton<Func<CabinBomViewModel>>((s) => s.GetRequiredService<CabinBomViewModel>);

            //The Calculations Table ViewModel Handles the Calculations Timing and passes the Data to the Calculation Table View
            services.AddTransient<CabinCalculationsTableViewModel>();
            services.AddSingleton<Func<CabinCalculationsTableViewModel>>(s => s.GetRequiredService<CabinCalculationsTableViewModel>);
        }
        public static void AddAllAccessoriesServices(this IServiceCollection services)
        {
            services.AddSingleton<AccessoriesEntitiesModuleViewModel>();

            services.AddTransient<DbEntityViewModel>();
            services.AddTransient<DescriptiveEntityViewModel>();
            services.AddTransient<BathAccessoryEntityViewModel>();
            services.AddTransient<TraitEntityViewModel>();
            services.AddTransient<TraitClassEntityViewModel>();
            services.AddTransient<TraitGroupEntityViewModel>();
            services.AddTransient<UserAccessoriesOptionsViewModel>();
            services.AddTransient<CustomPriceRuleEntityViewModel>();
            services.AddTransient<UserInfoEntityViewModel>();
            services.AddTransient<LocalizedStringViewModel>();
            services.AddTransient<SelectFileViewModel>();

            services.AddSingleton<Func<DbEntityViewModel>>(s => s.GetRequiredService<DbEntityViewModel>);
            services.AddSingleton<Func<DescriptiveEntityViewModel>>(s => s.GetRequiredService<DescriptiveEntityViewModel>);
            services.AddSingleton<Func<BathAccessoryEntityViewModel>>(s => s.GetRequiredService<BathAccessoryEntityViewModel>);
            services.AddSingleton<Func<TraitEntityViewModel>>(s => s.GetRequiredService<TraitEntityViewModel>);
            services.AddSingleton<Func<TraitClassEntityViewModel>>(s => s.GetRequiredService<TraitClassEntityViewModel>);
            services.AddSingleton<Func<TraitGroupEntityViewModel>>(s => s.GetRequiredService<TraitGroupEntityViewModel>);
            services.AddSingleton<Func<UserAccessoriesOptionsViewModel>>(s => s.GetRequiredService<UserAccessoriesOptionsViewModel>);
            services.AddSingleton<Func<CustomPriceRuleEntityViewModel>>(s => s.GetRequiredService<CustomPriceRuleEntityViewModel>);
            services.AddSingleton<Func<UserInfoEntityViewModel>>(s => s.GetRequiredService<UserInfoEntityViewModel>);
            services.AddSingleton<Func<LocalizedStringViewModel>>(s => s.GetRequiredService<LocalizedStringViewModel>);
            services.AddSingleton<Func<SelectFileViewModel>>(s => s.GetRequiredService<SelectFileViewModel>);
        }
    }
}
