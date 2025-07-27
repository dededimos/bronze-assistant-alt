using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Charachteristics;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Services.PositionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Repositories
{
    /// <summary>
    /// The Provider of All the Data Objects Needed for the Mirrors Library
    /// </summary>
    public interface IMirrorsDataProvider
    {
        /// <summary>
        /// An event Alerting that certain Data of the Provider has Changed
        /// </summary>
        public event EventHandler<Type>? ProviderDataChanged;


        IEnumerable<MirrorConstraints> GetAllConstraints();
        MirrorConstraints? GetSpecificConstraint(BronzeMirrorShape shape);
        IEnumerable<IMirrorElement> GetSelectableExclusiveSetElements();
        Task BuildConstraintsProviderAsync();

        IEnumerable<MirrorFinishElement> GetAllFinishElements();
        IEnumerable<MirrorFinishElement> GetFinishElements(params string[] finishesIds);
        Task BuildFinishTraitsProviderAsync();

        IEnumerable<CustomMirrorTrait> GetAllCustomTraits();
        IEnumerable<CustomMirrorTrait> GetCustomTraits(params string[] customTraitsIds);
        Task BuildCustomTraitsProviderAsync();

        IEnumerable<CustomMirrorElement> GetAllCustomElements();
        IEnumerable<CustomMirrorElement> GetCustomElements(params string[] customElementIds);
        /// <summary>
        /// Returns all the Types of Custom Elements
        /// </summary>
        /// <returns></returns>
        IEnumerable<LocalizedString> GetCustomElementsTypes();
        /// <summary>
        /// Returns all the Custom Elements of a certain Type
        /// </summary>
        /// <param name="typeDefaultValue">The Default Value of the Localized String representing the Type of the Custom Element</param>
        /// <returns></returns>
        IEnumerable<CustomMirrorElement> GetCustomElementsOfType(string typeDefaultValue);
        Task BuildCustomElementsProviderAsync();

        IEnumerable<MirrorSupport> GetAllSupports();
        IEnumerable<MirrorSupport> GetSupports(params string[] supportIds);
        Task BuildSupportsProviderAsync();

        IEnumerable<MirrorSandblast> GetAllSandblasts();
        IEnumerable<MirrorSandblast> GetSandblasts(params string[] sandblastIds);
        Task BuildSandblastsProviderAsync();

        IEnumerable<MirrorLightElement> GetAllLights();
        IEnumerable<MirrorLightElement> GetLights(params string[] lightIds);
        Task BuildLightsProviderAsync();

        /// <summary>
        /// Gets the Modules of the provided types <see cref="MirrorModuleType"/>
        /// as Clones of the Original Modules
        /// </summary>
        /// <param name="types">The types of the modules to return</param>
        /// <returns></returns>
        IEnumerable<MirrorModule> GetModulesOfType(params MirrorModuleType[] types);
        IEnumerable<MirrorModule> GetModulesOfType<TModule>() where TModule : MirrorModuleInfo;
        IEnumerable<MirrorModule> GetAllModules();
        IEnumerable<MirrorModule> GetModules(params string[] moduleIds);
        /// <summary>
        /// Get a Module by its id , or null if no Module Exists with the provided id
        /// </summary>
        /// <param name="moduleId">The Id of the needed module</param>
        /// <returns></returns>
        MirrorModule? GetModule(string moduleId);
        IEnumerable<MirrorModule> GetPositionableModules();
        Task BuildModulesProviderAsync();

        IEnumerable<MirrorSeries> GetAllSeries();
        IEnumerable<MirrorSeries> GetSeries(params string[] seriesIds);
        Task BuildSeriesProviderAsync();

        IEnumerable<MirrorElementPosition> GetAllPositions();
        IEnumerable<MirrorElementPosition> GetPositions(params string[] positionsIds);
        MirrorElementPosition GetPosition(string positionId);
        Task BuildPositionsProviderAsync();

        IEnumerable<MirrorElementPositionOptions> GetAllPositionsOptions();
        /// <summary>
        /// Returns the Position Options of a certain Element , 
        /// <para>If there are no Registered Options , the data provider interface should return Default Options</para>
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        MirrorElementPositionOptions GetPositionOptionsOfElement(string elementId);
        IEnumerable<MirrorElementPositionOptions> GetPositionOptionsOfElements(params string[] elementsIds);
        Task BuildPositionOptionsProviderAsync();

        public IMirrorApplicationOptionsProvider Options { get; }
        Task BuildOptionsProviderAsync();
    }

    /// <summary>
    /// An object that can provide all various Options concerning Mirrors
    /// </summary>
    public interface IMirrorApplicationOptionsProvider
    {
        /// <summary>
        /// Returns the Code Builder Options for providing codes into a Mirror
        /// </summary>
        /// <returns></returns>
        MirrorCodesBuilderOptions GetMirrorCodeBuildingOptions();
        /// <summary>
        /// Returns the Code Builder Options for providing Complex codes into a Mirror
        /// </summary>
        /// <returns></returns>
        MirrorCodesBuilderOptions GetMirrorComplexCodeBuildingOptions();
        /// <summary>
        /// Returns the Code Builder Options for providing codes into the GLASS of a Mirror
        /// </summary>
        /// <returns></returns>
        MirrorCodesBuilderOptions GetMirrorGlassCodeBuildingOptions();
    }


}
