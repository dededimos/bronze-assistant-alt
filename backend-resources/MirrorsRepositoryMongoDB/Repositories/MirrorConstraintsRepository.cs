using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Validators;
using MongoDbCommonLibrary;

namespace MirrorsRepositoryMongoDB.Repositories
{
    public class MirrorConstraintsRepository : MongoDatabaseEntityRepo<MirrorConstraintsEntity>
    {
        public MirrorConstraintsRepository(
            MirrorConstraintsEntityValidator entityValidator,
            IMongoDbMirrorsConnection mirrorsConnection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MirrorConstraintsRepository> logger)
            : base(entityValidator, mirrorsConnection.ConstraintsCollection, options, logger)
        {
        }
    }
}
