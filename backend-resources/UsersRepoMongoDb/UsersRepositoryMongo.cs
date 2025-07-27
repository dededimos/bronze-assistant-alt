using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDbCommonLibrary;

namespace UsersRepoMongoDb
{
    public class UsersRepositoryMongo : MongoEntitiesRepository<UserInfoEntity>
    {
        public UsersRepositoryMongo(UserInfoEntityValidator validator, IWithUsersCollection connection, ILogger<MongoEntitiesRepository<UserInfoEntity>> logger) : base(validator, connection.UsersCollection, logger)
        {

        }
    }

    public interface IWithUsersCollection
    {
        public IMongoCollection<UserInfoEntity> UsersCollection { get; }
    }
}
