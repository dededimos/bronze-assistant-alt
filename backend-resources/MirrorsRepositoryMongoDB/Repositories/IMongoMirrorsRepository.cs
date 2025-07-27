using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonInterfaces;

namespace MirrorsRepositoryMongoDB.Repositories
{
    /// <summary>
    /// A repository for CRUD Operations and Retrieval of all the ENTITIES of the Mirrors Database
    /// </summary>
    public interface IMongoMirrorsRepository
    {
        public IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity> Constraints { get; }
        public IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity> CustomElements { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSupportEntity> Supports { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSandblastEntity> Sandblasts { get; }
        public IMongoDatabaseEntityRepoCache<MirrorLightElementEntity> Lights { get; }
        public IMongoDatabaseEntityRepoCache<MirrorModuleEntity> Modules { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity> Series { get; }
        public IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity> Positions { get; }
        public IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity> PositionsOptions { get; }

        /// <summary>
        /// Builds the Cache of All Repositories and Optionally reports a Progress on which has finished as it goes
        /// </summary>
        /// <param name="progress">The Progress Of the Building with a message string whenever it finishes</param>
        /// <returns></returns>
        public Task BuildCachesAsync(IProgress<string>? progress = null);
    }

    /// <summary>
    ///The Default Implementation of <see cref="IMongoMirrorsRepository"/>  for CRUD Operations and Retrieval of all the ENTITIES of the Mirrors Database
    /// </summary>
    public class MirrorsMongoRepository : IMongoMirrorsRepository
    {
        public IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity> Constraints { get; }
        public IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity> CustomElements { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSupportEntity> Supports { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSandblastEntity> Sandblasts { get; }
        public IMongoDatabaseEntityRepoCache<MirrorLightElementEntity> Lights { get; }
        public IMongoDatabaseEntityRepoCache<MirrorModuleEntity> Modules { get; }
        public IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity> Series { get; }
        public IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity> Positions { get; }
        public IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity> PositionsOptions { get; }
        public IMongoDatabaseEntityRepoCache<MirrorFinishElementEntity> FinishElements { get; }
        public IMongoDatabaseEntityRepoCache<CustomMirrorTraitEntity> CustomTraits { get; }
        public IMongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity> MirrorOptions { get; }

        public MirrorsMongoRepository(
            IMongoDatabaseEntityRepoCache<MirrorConstraintsEntity> constraints,
            IMongoDatabaseEntityRepoCache<CustomMirrorElementEntity> customElements,
            IMongoDatabaseEntityRepoCache<MirrorSupportEntity> supports,
            IMongoDatabaseEntityRepoCache<MirrorSandblastEntity> sandblasts,
            IMongoDatabaseEntityRepoCache<MirrorLightElementEntity> lights,
            IMongoDatabaseEntityRepoCache<MirrorModuleEntity> modules,
            IMongoDatabaseEntityRepoCache<MirrorSeriesElementEntity> series,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionEntity> positions,
            IMongoDatabaseEntityRepoCache<MirrorElementPositionOptionsEntity> positionsOptions,
            IMongoDatabaseEntityRepoCache<MirrorFinishElementEntity> finishElements,
            IMongoDatabaseEntityRepoCache<CustomMirrorTraitEntity> customTraits,
            IMongoDatabaseEntityRepoCache<MirrorApplicationOptionsEntity> mirrorOptions)
        {
            Constraints = constraints;
            CustomElements = customElements;
            Supports = supports;
            Sandblasts = sandblasts;
            Lights = lights;
            Modules = modules;
            Series = series;
            Positions = positions;
            PositionsOptions = positionsOptions;
            FinishElements = finishElements;
            CustomTraits = customTraits;
            MirrorOptions = mirrorOptions;
        }

        public async Task BuildCachesAsync(IProgress<string>? progress = null)
        {
            try
            {
                await BuildCacheAsync(Constraints, nameof(Constraints), progress);
                await Task.Delay(10);
                await BuildCacheAsync(CustomElements, nameof(CustomElements), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Supports, nameof(Supports), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Sandblasts, nameof(Sandblasts), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Lights, nameof(Lights), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Modules, nameof(Modules), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Series, nameof(Series), progress);
                await Task.Delay(10);
                await BuildCacheAsync(Positions, nameof(Positions), progress);
                await Task.Delay(10);
                await BuildCacheAsync(PositionsOptions, nameof(PositionsOptions), progress);
                await Task.Delay(10);
                await BuildCacheAsync(FinishElements, nameof(FinishElements), progress);
                await Task.Delay(10);
                await BuildCacheAsync(CustomTraits, nameof(CustomTraits), progress);
                await Task.Delay(10);
                await BuildCacheAsync(MirrorOptions, nameof(MirrorOptions), progress);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cache Building Failed with message :{Environment.NewLine}{ex.Message}", ex);
            }
        }

        private static async Task BuildCacheAsync<T>(IMongoDatabaseEntityRepoCache<T> repository, string repositoryName, IProgress<string>? progress)
            where T: IDatabaseEntity
        {
            progress?.Report($"Building cache for {repositoryName}...");
            // Call the BuildCacheAsync method of the repository
            await repository.BuildCacheAsync();
            progress?.Report($"Cache for {repositoryName} built successfully.");
        }
    }
}
