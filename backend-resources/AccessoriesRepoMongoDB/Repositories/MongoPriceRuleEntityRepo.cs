using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Repositories
{
    /// <summary>
    /// The Database Repository for the Custom PriceRules
    /// </summary>
    public class MongoPriceRuleEntityRepo : MongoEntitiesRepository<CustomPriceRuleEntity>
    {
        private readonly IMongoCollection<UserAccessoriesOptionsEntity> usersOptionsCollection;

        public MongoPriceRuleEntityRepo(CustomPriceRuleEntityValidator validator,
            IMongoDbAccessoriesConnection connection,
            ILogger<MongoEntitiesRepository<CustomPriceRuleEntity>> logger) : base(validator, connection.PriceRulesCollection, logger)
        {
            this.usersOptionsCollection = connection.UserOptionsCollection;
        }

        public override async Task DeleteEntityAsync(ObjectId id)
        {
            //Find if any User Options contains this Id inside its Price Rules
            var idToString = id.ToString();
            FilterDefinition<UserAccessoriesOptionsEntity> usersFilter = Builders<UserAccessoriesOptionsEntity>.Filter.AnyEq(o => o.CustomPriceRules, idToString);

            //if it does throw with the UsersNames Containing this Rule and Prevent Deletion
            var resultOptions = await usersOptionsCollection.FindAsync(usersFilter);
            var foundOptions = await resultOptions.ToListAsync();

            if (foundOptions.Count != 0)
            {
                var usersStrings = foundOptions.Count <= 10 ? foundOptions.Select(o => $"{foundOptions.IndexOf(o) + 1} {o.Name}-({o.Id})") : foundOptions.Take(10).Select(o => $"{foundOptions.IndexOf(o) + 1} {o.Name}-({o.Id})");
                var usersJoinedStrings = string.Join(Environment.NewLine, usersStrings);
                logger.LogInformation("There where Users that have assigned this PriceRule with Id:{id}{newLine}{traits}", idToString, Environment.NewLine, usersJoinedStrings);
                throw new Exception($"Cannot Delete PriceRule while its Assigned to Users{Environment.NewLine}{usersJoinedStrings}{Environment.NewLine}...");
            }
            else
            {
                //If no Users where found that use this Price Rule Delete it
                await base.DeleteEntityAsync(id);
            }
        }
    }
}
