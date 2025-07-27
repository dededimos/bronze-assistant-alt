using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Validators;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;

namespace MirrorsRepositoryMongoDB.Repositories
{
    public class MirrorApplicationOptionsEntitiesRepository : MongoDatabaseEntityRepo<MirrorApplicationOptionsEntity>
    {
        public MirrorApplicationOptionsEntitiesRepository(
            MirrorApplicationOptionsEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorApplicationOptionsEntitiesRepository> logger) :
            base(entityValidator, mirrorsConnection.OptionsCollection, options, logger)
        {
        }

        /// <summary>
        /// Returns the options of the specified type and Application , 
        /// <para>If application type is unspecified returns the Options with the Specified Type for All Application</para>
        /// <para>If the Option Type is unspecified returns all the options for the specified Application</para>
        /// <para>If both OptionType and Applcation Type are unspecified returns All the Options</para>
        /// </summary>
        /// <param name="type">The type of the Option</param>
        /// <param name="applicationType">The Application type</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<MirrorApplicationOptionsEntity>> GetOptionsByType(string? type, BronzeApplicationType applicationType = BronzeApplicationType.UnspecifiedApplication)
        {
            FilterDefinition<MirrorApplicationOptionsEntity> filter;

            if (string.IsNullOrEmpty(type)) filter = FilterDefinition<MirrorApplicationOptionsEntity>.Empty;
            else filter = Builders<MirrorApplicationOptionsEntity>.Filter.Eq(e => e.OptionsType, type);

            //If the application type is specified return only for the specified one
            if (applicationType != BronzeApplicationType.UnspecifiedApplication) filter &= Builders<MirrorApplicationOptionsEntity>.Filter.Eq(e => e.ConcerningApplication, applicationType);
            return await GetEntitiesAsync(filter);
        }
    }
}
