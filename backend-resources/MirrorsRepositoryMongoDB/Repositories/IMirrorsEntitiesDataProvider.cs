using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary;

namespace MirrorsRepositoryMongoDB.Repositories
{
    /// <summary>
    /// The Provider of ALL the ENTITIES of the Mirrors Library , No CRUD operations Available
    /// </summary>
    public interface IMirrorsEntitiesDataProvider
    {
        public event EventHandler<Type>? ProviderDataChanged;
        public IMongoEntitiesCache<MirrorConstraintsEntity> Constraints { get;  }
        public IMongoEntitiesCache<CustomMirrorElementEntity> CustomElements { get; }
        public IMongoEntitiesCache<MirrorSupportEntity> Supports { get;  }
        public IMongoEntitiesCache<MirrorSandblastEntity> Sandblasts { get;  }
        public IMongoEntitiesCache<MirrorLightElementEntity> Lights { get;}
        public IMongoEntitiesCache<MirrorModuleEntity> Modules { get;  }
        public IMongoEntitiesCache<MirrorSeriesElementEntity> Series { get; }
        public IMongoEntitiesCache<MirrorElementPositionEntity> Positions { get;  }
        public IMongoEntitiesCache<MirrorElementPositionOptionsEntity> PositionOptions { get;  }
        public IMongoEntitiesCache<MirrorFinishElementEntity> FinishElements { get; }
        public IMongoEntitiesCache<CustomMirrorTraitEntity> CustomTraits { get; }
        public IMongoEntitiesCache<MirrorApplicationOptionsEntity> MirrorApplicationOptions { get; }
    }

    /// <summary>
    /// The Default Implementation of the <see cref="IMirrorsEntitiesDataProvider"/> serving all The Entities without CRUD operations
    /// </summary>
    public class MirrorsEntitiesDataProvider : IMirrorsEntitiesDataProvider
    {
        public event EventHandler<Type>? ProviderDataChanged;
        private void OnProviderDataChanged(Type type) => ProviderDataChanged?.Invoke(this, type);

        public IMongoEntitiesCache<MirrorConstraintsEntity> Constraints { get; set; }
        public IMongoEntitiesCache<CustomMirrorElementEntity> CustomElements { get; }
        public IMongoEntitiesCache<MirrorSupportEntity> Supports { get; set; }
        public IMongoEntitiesCache<MirrorSandblastEntity> Sandblasts { get; set; }
        public IMongoEntitiesCache<MirrorLightElementEntity> Lights { get; set; }
        public IMongoEntitiesCache<MirrorModuleEntity> Modules { get; set; }
        public IMongoEntitiesCache<MirrorSeriesElementEntity> Series { get; }
        public IMongoEntitiesCache<MirrorElementPositionEntity> Positions { get; set; }
        public IMongoEntitiesCache<MirrorElementPositionOptionsEntity> PositionOptions { get; set; }
        public IMongoEntitiesCache<MirrorFinishElementEntity> FinishElements { get; set; }
        public IMongoEntitiesCache<CustomMirrorTraitEntity> CustomTraits { get; set; }
        public IMongoEntitiesCache<MirrorApplicationOptionsEntity> MirrorApplicationOptions { get; set; }

        public MirrorsEntitiesDataProvider(
            IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity> constraints,
            IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity> customElements,
            IMongoDatabaseEntityRepoCache<MirrorSupportEntity> supports,
            IMongoDatabaseEntityRepoCache<MirrorSandblastEntity> sandblasts,
            IMongoDatabaseEntityRepoCache<MirrorLightElementEntity> lights,
            IMongoDatabaseEntityRepoCache<MirrorModuleEntity> modules,
            IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity> series,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity> positions,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity> positionOptions,
            IMongoDatabaseEntityRepoCache<MirrorFinishElementEntity> finishElements,
            IMongoDatabaseEntityRepoCache<CustomMirrorTraitEntity> customTraits,
            IMongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity> applicationOptions)
        {
            Constraints = constraints;
            Constraints.CacheChanged += (sender,args)=> OnProviderDataChanged(typeof(MirrorConstraintsEntity));
            CustomElements = customElements;
            CustomElements.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(CustomMirrorElementEntity));
            Supports = supports;
            Supports.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorSupportEntity));
            Sandblasts = sandblasts;
            Sandblasts.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorSandblastEntity));
            Lights = lights;
            Lights.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorLightElementEntity));
            Modules = modules;
            Modules.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorModuleEntity));
            Series = series;
            Series.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorSeriesElementEntity));
            Positions = positions;
            Positions.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorElementPositionEntity));
            PositionOptions = positionOptions;
            PositionOptions.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorElementPositionOptionsEntity));
            FinishElements = finishElements;
            FinishElements.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorFinishElementEntity));
            CustomTraits = customTraits;
            CustomTraits.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(CustomMirrorTraitEntity));
            MirrorApplicationOptions = applicationOptions;
            MirrorApplicationOptions.CacheChanged += (sender, args) => OnProviderDataChanged(typeof(MirrorApplicationOptionsEntity));
        }
    }
}
