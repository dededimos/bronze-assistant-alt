using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Validators;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Repositories
{
    public class CustomMirrorElementsRepository : MongoDatabaseEntityRepo<CustomMirrorElementEntity>
    {
        public CustomMirrorElementsRepository(
            CustomMirrorElementEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection, 
            IOptions<MongoDatabaseEntityRepoOptions> options, 
            ILogger<CustomMirrorElementsRepository> logger) 
            : base(entityValidator, mirrorsConnection.CustomElementsConnection, options, logger)
        {
            
        }
    }
    public class MirrorSupportEntitiesRepository : MongoDatabaseEntityRepo<MirrorSupportEntity>
    {
        public MirrorSupportEntitiesRepository(
            MirrorSupportEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorSupportEntitiesRepository> logger) :
            base(entityValidator, mirrorsConnection.SupportsCollection, options, logger)
        {
        }
    }
    public class MirrorSandblastEntitiesRepository : MongoDatabaseEntityRepo<MirrorSandblastEntity>
    {
        public MirrorSandblastEntitiesRepository(
            MirrorSandblastEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorSandblastEntitiesRepository> logger)
            : base(entityValidator, mirrorsConnection.SandblastsCollection, options, logger)
        {
        }
    }
    public class MirrorLightElementEntitiesRepository : MongoDatabaseEntityRepo<MirrorLightElementEntity>
    {
        public MirrorLightElementEntitiesRepository(
            MirrorLightElementEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorLightElementEntitiesRepository> logger)
            : base(entityValidator, mirrorsConnection.LightsCollection, options, logger)
        {
        }
    }
    public class MirrorModuleEntitiesRepository : MongoDatabaseEntityRepo<MirrorModuleEntity>
    {
        public MirrorModuleEntitiesRepository(
            MirrorModuleEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorModuleEntitiesRepository> logger) 
            : base(entityValidator, mirrorsConnection.ModulesCollection, options, logger)
        {
            
        }
    }
    public class MirrorSeriesElementEntitiesRepository : MongoDatabaseEntityRepo<MirrorSeriesElementEntity>
    {
        public MirrorSeriesElementEntitiesRepository(
            MirrorSeriesElementEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorSeriesElementEntitiesRepository> logger)
            : base(entityValidator, mirrorsConnection.SeriesCollection, options, logger)
        {
            
        }
        protected override async Task ValidateOrThrowAsync(MirrorSeriesElementEntity entity)
        {
            await base.ValidateOrThrowAsync(entity);
            //Additional Logic : Series must have Unique Combinations of Shape/Support/Sandblast
            //So to validate before saving we must see that there is no other series with this kind of combinations as the current one .

            //Shapes
            var shapeFilter = Builders<MirrorSeriesElementEntity>.Filter.Eq(e=> e.Constraints.ConcerningMirrorShape, entity.Constraints.ConcerningMirrorShape);

            var sandblastFilters = new List<FilterDefinition<MirrorSeriesElementEntity>>();
            //Sandblasts
            if (entity.Constraints.AllowedSandblasts.Count != 0)
            {
                sandblastFilters.Add(Builders<MirrorSeriesElementEntity>.Filter.AnyIn(e => e.Constraints.AllowedSandblasts, entity.Constraints.AllowedSandblasts));
            }
            if (entity.Constraints.AcceptsMirrorsWithoutSandblast)
            {
                sandblastFilters.Add(Builders<MirrorSeriesElementEntity>.Filter.Eq(e => e.Constraints.AcceptsMirrorsWithoutSandblast, true));
            }
            //if more than one do it with OR to include both cases of sandblasts , else just set the filter
            var sandblastFilter = sandblastFilters.Count > 1
                ? Builders<MirrorSeriesElementEntity>.Filter.Or(sandblastFilters)
                : sandblastFilters.Count == 1
                ? sandblastFilters.First()
                : Builders<MirrorSeriesElementEntity>.Filter.Empty;

            //Supports
            var supportFilters = new List<FilterDefinition<MirrorSeriesElementEntity>>();
            if (entity.Constraints.AllowedSupports.Count != 0)
            {
                supportFilters.Add(Builders<MirrorSeriesElementEntity>.Filter.AnyIn(e => e.Constraints.AllowedSupports, entity.Constraints.AllowedSupports));
            }
            if (entity.Constraints.AcceptsMirrorsWithoutSupport)
            {
                supportFilters.Add(Builders<MirrorSeriesElementEntity>.Filter.Eq(e => e.Constraints.AcceptsMirrorsWithoutSupport, true));
            }
            //if more than one do it with OR to include both cases of sandblasts , else just set the filter
            var supportFilter = supportFilters.Count > 1
                ? Builders<MirrorSeriesElementEntity>.Filter.Or(supportFilters)
                : supportFilters.Count == 1
                ? supportFilters.First()
                : Builders<MirrorSeriesElementEntity>.Filter.Empty;

            //All three must be present as well as the Customized Series boolean
            var combinedFilters = Builders<MirrorSeriesElementEntity>.Filter
                .And([
                    shapeFilter, 
                    supportFilter, 
                    sandblastFilter,
                    //Check only sereis that are of the same customization Group nonCustom Dims or CustomDims
                    Builders<MirrorSeriesElementEntity>.Filter.Eq(e=> e.IsCustomizedMirrorSeries , entity.IsCustomizedMirrorSeries)
                    ]);

            //Exclude the current entity from the results (cannot compare it to itself)
            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                //not equal Filter
                combinedFilters = Builders<MirrorSeriesElementEntity>.Filter.And([combinedFilters, Builders<MirrorSeriesElementEntity>.Filter.Ne(e => e.Id, entity.Id)]);
            }

            List<MirrorSeriesElementEntity> matchingSeries = await collection.Find(combinedFilters).ToListAsync();
            if (matchingSeries != null && matchingSeries.Count > 0)
            {
                throw new ValidationException($"There Are already Series with Matching Shape-Sandblast-Support Combinations{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, matchingSeries.Select(s=> $"Code:{s.Code}--Name:{s.LocalizedDescriptionInfo.Name.DefaultValue}"))}");
            }
        }


        protected override void OnAfterIdAssignmentInsertOperation(MirrorSeriesElementEntity entity)
        {
            //Assign the Id of the Series to each one of the Mirrors selected Inside the Series
            //When inserting the Series there is No Id Assigned not Until just before insertion by the Repository
            foreach (var mirror in entity.StandardMirrors)
            {
                mirror.SeriesReferenceId = entity.Id;
            }
        }
        public override Task UpdateEntityAsync(MirrorSeriesElementEntity entity)
        {
            //On any update re-assign the Id of the Series to the Mirrors (any new ones should get the Id, the old ones should have the same id as the Series)
            foreach (var mirror in entity.StandardMirrors)
            {
                mirror.SeriesReferenceId = entity.Id;
            }
            return base.UpdateEntityAsync(entity);
        }
    }
    public class MirrorElementPositionEntitiesRepostiory : MongoDatabaseEntityRepo<MirrorElementPositionEntity>
    {
        public MirrorElementPositionEntitiesRepostiory(
            MirrorElementPositionEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorElementPositionEntitiesRepostiory> logger)
            : base(entityValidator, mirrorsConnection.PositionsCollection, options, logger)
        {
            //TODO If any Positions are used in any Position Options Entity Prevent Deletion
        }
    }
    public class MirrorPositionOptionsRepository : MongoDatabaseEntityRepo<MirrorElementPositionOptionsEntity>
    {
        public MirrorPositionOptionsRepository(
            MirrorElementPositionOptionsEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorPositionOptionsRepository> logger) 
            : base(entityValidator, mirrorsConnection.PositionsOptionsCollection, options, logger)
        {
            
        }
    }
    public class MirrorFinishElementEntitiesRepository : MongoDatabaseEntityRepo<MirrorFinishElementEntity>
    {
        public MirrorFinishElementEntitiesRepository(
            MirrorFinishElementEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorFinishElementEntitiesRepository> logger)
            : base(entityValidator, mirrorsConnection.FinishTraitsCollection, options, logger)
        {

        }
    }
    public class MirrorCustomTraitsRepository : MongoDatabaseEntityRepo<CustomMirrorTraitEntity>
    {
        public MirrorCustomTraitsRepository(
            CustomMirrorTraitEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorCustomTraitsRepository> logger)
            : base(entityValidator, mirrorsConnection.CustomTraitsCollection, options, logger)
        {

        }
    }
}
