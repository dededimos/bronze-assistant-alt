using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Charachteristics;
using MirrorsLib.Repositories;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Services.PositionService;
using MirrorsRepositoryMongoDB.Entities;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Repositories
{
    /// <summary>
    /// The <see cref="IMirrorsDataProvider"/> implementation that uses MongoDB as the Data Source
    /// </summary>
    public class MirrorsDataProviderMongoImplementation : IMirrorsDataProvider
    {
        public MirrorsDataProviderMongoImplementation(IMirrorsEntitiesDataProvider entitiesCache)
        {
            this.entitiesCache = entitiesCache;

            //Alert the Provider when the Cache of the Entities Changes because this changes also the Data of the Provider
            this.entitiesCache.ProviderDataChanged += (sender,type) => OnProviderDataChanged(MatchEntityTypeWithModelType(type));
            
            Options = new MirrorApplicationOptionsProviderMongoImplementation(entitiesCache.MirrorApplicationOptions);
        }
        private readonly IMirrorsEntitiesDataProvider entitiesCache;

        /// <summary>
        /// An Event that alerts when certain Data of the Provider has Changed
        /// </summary>
        public event EventHandler<Type>? ProviderDataChanged;
        /// <summary>
        /// Fires when the Data of a certain Type of Data has Changed
        /// </summary>
        /// <param name="type"></param>
        private void OnProviderDataChanged(Type type) => ProviderDataChanged?.Invoke(this, type);
        /// <summary>
        /// Matches the type of the Entities with the Type of the Model Objects
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Type MatchEntityTypeWithModelType(Type entityType)
        {
            return entityType switch
            {
                Type t when t == typeof(MirrorConstraintsEntity) => typeof(MirrorConstraints),
                Type t when t == typeof(CustomMirrorElementEntity) => typeof(CustomMirrorElement),
                Type t when t == typeof(MirrorSupportEntity) => typeof(MirrorSupport),
                Type t when t == typeof(MirrorSandblastEntity) => typeof(MirrorSandblast),
                Type t when t == typeof(MirrorLightElementEntity) => typeof(MirrorLightElement),
                Type t when t == typeof(MirrorModuleEntity) => typeof(MirrorModule),
                Type t when t == typeof(MirrorSeriesElementEntity) => typeof(MirrorSeries),
                Type t when t == typeof(MirrorElementPositionEntity) => typeof(MirrorElementPosition),
                Type t when t == typeof(MirrorElementPositionOptionsEntity) => typeof(MirrorElementPositionOptions),
                Type t when t == typeof(MirrorFinishElementEntity) => typeof(MirrorFinishElement),
                Type t when t == typeof(CustomMirrorTraitEntity) => typeof(CustomMirrorTrait),
                Type t when t == typeof(MirrorApplicationOptionsEntity) => typeof(MirrorApplicationOptionsBase),
                _ => throw new NotSupportedException($"The provided entity Type is not Matching any Mirror Object Type"),
            };
        }

        public IMirrorApplicationOptionsProvider Options { get; set; }

        public IEnumerable<MirrorConstraints> GetAllConstraints()
        {
            return entitiesCache.Constraints.Cache.Select(e => e.Constraints).OrderBy(c=>c.ConcerningMirrorShape).ThenBy(c=>c.CanHaveLight);
        }
        public MirrorConstraints? GetSpecificConstraint(BronzeMirrorShape shape)
        {
            return entitiesCache.Constraints.Cache.FirstOrDefault(e => e.Constraints.ConcerningMirrorShape == shape)?.ToConstraints();
        }
        public IEnumerable<IMirrorElement> GetSelectableExclusiveSetElements()
        {
            var elements = new List<IMirrorElement>();
            elements.AddRange(GetAllModules());
            elements.AddRange(GetAllCustomElements());
            elements.AddRange(GetAllLights());
            elements.AddRange(GetAllSupports());
            elements.AddRange(GetAllSandblasts());
            return elements;
        }

        public IEnumerable<MirrorFinishElement> GetAllFinishElements()
        {
            return entitiesCache.FinishElements.Cache.Select(e => e.ToFinishElement()).OrderBy(e => e.LocalizedDescriptionInfo.Name.DefaultValue);
        }
        public IEnumerable<MirrorFinishElement> GetFinishElements(params string[] finishTraitIds)
        {
            return entitiesCache.FinishElements.Cache.Where(e => finishTraitIds.Any(id => id == e.Id)).Select(e => e.ToFinishElement()).OrderBy(e=>e.LocalizedDescriptionInfo.Name.DefaultValue);
        }
        
        public IEnumerable<CustomMirrorTrait> GetAllCustomTraits()
        {
            return entitiesCache.CustomTraits.Cache.Select(e => e.ToCustomMirrorTrait()).OrderBy(e => e.LocalizedDescriptionInfo.Name.DefaultValue);
        }
        public IEnumerable<CustomMirrorTrait> GetCustomTraits(params string[] customTraitsIds)
        {
            return entitiesCache.CustomTraits.Cache.Where(e => customTraitsIds.Any(id => id == e.Id)).Select(e => e.ToCustomMirrorTrait()).OrderBy(e => e.LocalizedDescriptionInfo.Name.DefaultValue);
        }
        
        public IEnumerable<CustomMirrorElement> GetAllCustomElements()
        {
            return entitiesCache.CustomElements.Cache.Select(e => e.ToCustomMirrorElement()).OrderBy(e=>e.CustomElementType.DefaultValue);
        }
        public IEnumerable<CustomMirrorElement> GetCustomElements(params string[] customElementIds)
        {
            return entitiesCache.CustomElements.Cache.Where(e => customElementIds.Any(id => id == e.Id)).Select(e => e.ToCustomMirrorElement());
        }
        /// <summary>
        /// Returns all the Types of Custom Elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetCustomElementsTypes()
        {
            return entitiesCache.CustomElements.Cache.Select(e=> e.CustomElementType).Distinct().OrderBy(e => e.DefaultValue);
        }
        /// <summary>
        /// Returns the Custom Element of the provided type
        /// </summary>
        /// <param name="typeDefaultValue">The Default Value of the Localized String of the Type</param>
        /// <returns></returns>
        public IEnumerable<CustomMirrorElement> GetCustomElementsOfType(string typeDefaultValue)
        {
            return entitiesCache.CustomElements.Cache.Where(e => e.CustomElementType.DefaultValue == typeDefaultValue).Select(e => e.ToCustomMirrorElement());
        }

        public IEnumerable<MirrorSupport> GetAllSupports()
        {
            var finishes = GetAllFinishElements();
            return entitiesCache.Supports.Cache.Select(e => e.ToSupport(finishes)).OrderBy(s => s.SupportInfo.GetType().Name);
        }
        public IEnumerable<MirrorSupport> GetSupports(params string[] supportIds)
        {
            var finishes = GetAllFinishElements();
            return entitiesCache.Supports.Cache.Where(e => supportIds.Any(id => id == e.Id)).Select(e => e.ToSupport(finishes));
        }

        public IEnumerable<MirrorSandblast> GetAllSandblasts()
        {
            return entitiesCache.Sandblasts.Cache.Select(e => e.ToSandblast()).OrderBy(s => s.SandblastInfo.GetType().Name);
        }
        public IEnumerable<MirrorSandblast> GetSandblasts(params string[] sandblastIds)
        {
            return entitiesCache.Sandblasts.Cache.Where(e => sandblastIds.Any(id => id == e.Id)).Select(e => e.ToSandblast());
        }

        public IEnumerable<MirrorLightElement> GetAllLights()
        {
            return entitiesCache.Lights.Cache.Select(e => e.ToLight()).OrderBy(l => l.LightInfo.Kelvin.Sum()).ThenBy(l => l.LightInfo.WattPerMeter);
        }
        public IEnumerable<MirrorLightElement> GetLights(params string[] lightIds)
        {
            return entitiesCache.Lights.Cache.Where(e=> lightIds.Any(id=> id == e.Id)).Select(e=> e.ToLight());
        }

        public MirrorModule? GetModule(string moduleId)
        {
            return entitiesCache.Modules.Cache.FirstOrDefault(e => e.Id == moduleId)?.ToMirrorModule();
        }
        public IEnumerable<MirrorModule> GetAllModules()
        {
            return entitiesCache.Modules.Cache.Select(e => e.ToMirrorModule()).OrderBy(m => m.ModuleInfo.GetType().Name);
        }
        public IEnumerable<MirrorModule> GetModules(params string[] moduleIds)
        {
            return entitiesCache.Modules.Cache.Where(e => moduleIds.Any(id => id == e.Id)).Select(e => e.ToMirrorModule());
        }
        public IEnumerable<MirrorModule> GetModulesOfType(params MirrorModuleType[] types)
        {
            var modules = entitiesCache.Modules.Cache.Where(e => types.Any(t=> t == e.ModuleInfo.ModuleType));
            return modules.Select(e => e.ToMirrorModule());
        }
        public IEnumerable<MirrorModule> GetModulesOfType<TModule>() where TModule : MirrorModuleInfo
        {
            return entitiesCache.Modules.Cache.Where(e => e.ModuleInfo is TModule).Select(e => e.ToMirrorModule());
        }
        public IEnumerable<MirrorModule> GetPositionableModules()
        {
            return GetAllModules().Where(m => m is IMirrorPositionable);
        }

        public IEnumerable<MirrorSeries> GetAllSeries()
        {
            return entitiesCache.Series.Cache.Select(e => e.ToSeries(this));
        }
        public IEnumerable<MirrorSeries> GetSeries(params string[] seriesIds)
        {
            return entitiesCache.Series.Cache.Where(e => seriesIds.Any(id => id == e.Id)).Select(e => e.ToSeries(this));
        }

        public MirrorElementPosition GetPosition(string positionId)
        {
            return entitiesCache.Positions.Cache.FirstOrDefault(p => p.Id == positionId)?.ToPosition()
                ?? MirrorElementPosition.DefaultPositionElement();
        }
        public IEnumerable<MirrorElementPosition> GetAllPositions()
        {
            return entitiesCache.Positions.Cache.Select(e => e.ToPosition());
        }
        public IEnumerable<MirrorElementPosition> GetPositions(params string[] seriesIds)
        {
            return entitiesCache.Positions.Cache.Where(e => seriesIds.Any(id => id == e.Id)).Select(e => e.ToPosition());
        }

        public IEnumerable<MirrorElementPositionOptions> GetAllPositionsOptions()
        {
            var positionsEntities = entitiesCache.Positions.Cache;
            return entitiesCache.PositionOptions.Cache.Select(e => e.ToPositionOptions(positionsEntities));
        }
        public MirrorElementPositionOptions GetPositionOptionsOfElement(string elementId)
        {
            var positionsEntities = entitiesCache.Positions.Cache;
            return entitiesCache.PositionOptions.Cache.FirstOrDefault(e => e.ConcerningElementId == elementId)?.ToPositionOptions(positionsEntities)
                ?? MirrorElementPositionOptions.Empty();
        }
        public IEnumerable<MirrorElementPositionOptions> GetPositionOptionsOfElements(params string[] elementsIds)
        {
            var positionsEntities = entitiesCache.Positions.Cache;
            return entitiesCache.PositionOptions.Cache.Where(e => elementsIds.Any(id => id == e.ConcerningElementId)).Select(e => e.ToPositionOptions(positionsEntities));
        }

        public async Task BuildConstraintsProviderAsync()
        {
            await entitiesCache.Constraints.BuildCacheAsync();
        }
        public async Task BuildFinishTraitsProviderAsync()
        {
            await entitiesCache.FinishElements.BuildCacheAsync();
        }
        public async Task BuildCustomTraitsProviderAsync()
        {
            await entitiesCache.CustomTraits.BuildCacheAsync();
        }

        public async Task BuildCustomElementsProviderAsync()
        {
            await entitiesCache.CustomElements.BuildCacheAsync();
        }

        public async Task BuildSupportsProviderAsync()
        {
            await entitiesCache.Supports.BuildCacheAsync();
        }

        public async Task BuildSandblastsProviderAsync()
        {
            await entitiesCache.Sandblasts.BuildCacheAsync();
        }

        public async Task BuildLightsProviderAsync()
        {
            await entitiesCache.Lights.BuildCacheAsync();
        }

        public async Task BuildModulesProviderAsync()
        {
            await entitiesCache.Modules.BuildCacheAsync();
        }

        public async Task BuildSeriesProviderAsync()
        {
            await entitiesCache.Series.BuildCacheAsync();
        }

        public async Task BuildPositionsProviderAsync()
        {
            await entitiesCache.Positions.BuildCacheAsync();
        }

        public async Task BuildPositionOptionsProviderAsync()
        {
            await entitiesCache.PositionOptions.BuildCacheAsync();
        }
        public async Task BuildOptionsProviderAsync()
        {
            await entitiesCache.MirrorApplicationOptions.BuildCacheAsync();
        }

    }

    public class MirrorApplicationOptionsProviderMongoImplementation : IMirrorApplicationOptionsProvider
    {
        public MirrorApplicationOptionsProviderMongoImplementation(IMongoEntitiesCache<MirrorApplicationOptionsEntity> entitiesCache)
        {
            this.entitiesCache = entitiesCache;
        }
        private readonly IMongoEntitiesCache<MirrorApplicationOptionsEntity> entitiesCache;

        public MirrorCodesBuilderOptions GetMirrorCodeBuildingOptions()
        {
            return entitiesCache.Cache.Select(o=> o.Options).OfType<MirrorCodesBuilderOptions>().FirstOrDefault(o=> !o.GlassOnlyOptions && !o.ComplexCodeOptions) 
                ?? MirrorCodesBuilderOptions.DefaultMirrorCodeOptions();
        }
        public MirrorCodesBuilderOptions GetMirrorComplexCodeBuildingOptions()
        {
            return entitiesCache.Cache.Select(o => o.Options).OfType<MirrorCodesBuilderOptions>().FirstOrDefault(o => !o.GlassOnlyOptions && o.ComplexCodeOptions)
                ?? MirrorCodesBuilderOptions.DefaultMirrorCodeOptions();
        }
        public MirrorCodesBuilderOptions GetMirrorGlassCodeBuildingOptions()
        {
            return entitiesCache.Cache.Select(o => o.Options).OfType<MirrorCodesBuilderOptions>().FirstOrDefault(o => o.GlassOnlyOptions) 
                ?? MirrorCodesBuilderOptions.DefaultGlassCodeOptions();
        }
    }

}
