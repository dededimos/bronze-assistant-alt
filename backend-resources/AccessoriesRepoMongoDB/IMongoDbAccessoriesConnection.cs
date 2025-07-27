using AccessoriesRepoMongoDB.Entities;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRepoMongoDb;
using static ItemStockMongoRepository;

namespace AccessoriesRepoMongoDB
{
    public interface IMongoDbAccessoriesConnection : IWithUsersCollection , IWithItemStockCollection
    {
        /// <summary>
        /// The Connection Client
        /// </summary>
        MongoClient Client { get; }
        /// <summary>
        /// The Collection of the Trait Classes for the Accessories
        /// </summary>
        IMongoCollection<TraitClassEntity> TraitClassesCollection { get; }
        /// <summary>
        /// The Collection of Traits for the Accessories
        /// </summary>
        IMongoCollection<TraitEntity> TraitsCollection { get; }
        /// <summary>
        /// The Collection of the Accessories
        /// </summary>
        IMongoCollection<BathAccessoryEntity> AccessoriesCollection { get; }
        /// <summary>
        /// The Collection of Trait Groups
        /// </summary>
        IMongoCollection<TraitGroupEntity> TraitGroupsCollection { get; }
        /// <summary>
        /// The Collection of User Options
        /// </summary>
        IMongoCollection<UserAccessoriesOptionsEntity> UserOptionsCollection { get; }
        /// <summary>
        /// The Collection of Custom Price Rules
        /// </summary>
        IMongoCollection<CustomPriceRuleEntity> PriceRulesCollection { get; }
    }
    public class MongoDbAccessoriesConnection : IMongoDbAccessoriesConnection
    {
        private readonly string traitClassesCollectionConfigName = "MongoTraitClassesCollectionName";
        private readonly string traitsCollectionConfigName = "MongoTraitsCollectionName";
        private readonly string traitGroupsCollectionConfigName = "MongoTraitGroupsCollectionName";
        private readonly string accessoriesCollectionConfigName = "MongoAccessoriesCollectionName";
        private readonly string userAccessoriesOptionsConfigName = "MongoUserAccessoriesOptionsCollectionName";
        private readonly string priceRulesCollectionConfigName = "MongoPriceRulesCollectionName";
        private readonly string usersCollectionConfigName = "UsersCollectionName";
        private readonly string stockCollectionConfigName = "MongoStockCollectionName";

        public MongoClient Client { get; }
        public IMongoCollection<TraitClassEntity> TraitClassesCollection { get; }
        public IMongoCollection<TraitEntity> TraitsCollection { get; }
        public IMongoCollection<BathAccessoryEntity> AccessoriesCollection { get; }
        public IMongoCollection<TraitGroupEntity> TraitGroupsCollection { get; }
        /// <summary>
        /// The Collection of User Options
        /// </summary>
        public IMongoCollection<UserAccessoriesOptionsEntity> UserOptionsCollection { get; }
        /// <summary>
        /// The Collection of Custom Price Rules
        /// </summary>
        public IMongoCollection<CustomPriceRuleEntity> PriceRulesCollection { get; }
        /// <summary>
        /// The Collection of Users
        /// </summary>
        public IMongoCollection<UserInfoEntity> UsersCollection { get; }
        /// <summary>
        /// The Collection of Item Stock
        /// </summary>
        public IMongoCollection<ItemStockEntity> StockCollection { get; }

        public MongoDbAccessoriesConnection(IMongoConnection connection)
        {
            ApplyMongoDbMappings();

            var config = connection.GetConfiguration();
            Client = connection.GetClient();

            string traitClassesCollectionName = config[traitClassesCollectionConfigName] ?? throw new Exception($"{nameof(traitClassesCollectionConfigName)} not found in Configuration File");
            string traitsCollectionName = config[traitsCollectionConfigName] ?? throw new Exception($"{nameof(traitsCollectionConfigName)} not found in Configuration File");
            string accessoriesCollectionName = config[accessoriesCollectionConfigName] ?? throw new Exception($"{nameof(accessoriesCollectionConfigName)} not found in Configuration File");
            string traitGroupsCollectionName = config[traitGroupsCollectionConfigName] ?? throw new Exception($"{nameof(traitGroupsCollectionConfigName)} not found in Configuration File");
            string userOptionsCollectionName = config[userAccessoriesOptionsConfigName] ?? throw new Exception($"{nameof(userAccessoriesOptionsConfigName)} not found in Configuration File");
            string priceRulesCollectionName = config[priceRulesCollectionConfigName] ?? throw new Exception($"{nameof(priceRulesCollectionConfigName)} not found in Configuration File");
            string usersCollectionName = config[usersCollectionConfigName] ?? throw new Exception($"{nameof(usersCollectionConfigName)} not found in Configuration File");
            string stockCollectionName = config[stockCollectionConfigName] ?? throw new Exception($"{nameof(stockCollectionConfigName)} not found in Configuration File");

            var database = connection.GetDatabase();
            TraitClassesCollection = database.GetCollection<TraitClassEntity>(traitClassesCollectionName);
            TraitsCollection = database.GetCollection<TraitEntity>(traitsCollectionName);
            AccessoriesCollection = database.GetCollection<BathAccessoryEntity>(accessoriesCollectionName);
            TraitGroupsCollection = database.GetCollection<TraitGroupEntity>(traitGroupsCollectionName);
            UserOptionsCollection = database.GetCollection<UserAccessoriesOptionsEntity>(userOptionsCollectionName);
            PriceRulesCollection = database.GetCollection<CustomPriceRuleEntity>(priceRulesCollectionName);
            UsersCollection = database.GetCollection<UserInfoEntity>(usersCollectionName);
            StockCollection = database.GetCollection<ItemStockEntity>(stockCollectionName);
            //CreateUniqueIndexesIfTheyDoNotExist();
        }

        private static void ApplyMongoDbMappings()
        {
            BsonSerializer.RegisterSerializer(new EnumSerializer<TypeOfTrait>(BsonType.String));
            BsonClassMap.RegisterClassMap<TraitClassEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(tc => tc.SortNo).SetDefaultValue(99999);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<TraitEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(t => t.SortNo).SetDefaultValue(99999);
                cm.MapProperty(t => t.AssignedGroups).SetDefaultValue(new HashSet<string>());
                cm.AddKnownType(typeof(PrimaryTypeTraitEntity));
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<BathAccessoryEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapField(a => a.Code).SetDefaultValue(string.Empty);
                cm.MapProperty(a => a.SortNo).SetDefaultValue(99999);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<AccessoryFinishInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<TraitGroupEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(tg => tg.SortNo).SetDefaultValue(99999);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<UserAccessoriesOptionsEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<UserAccessoriesDiscountsDTO>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<PriceInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<CustomPriceRuleEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<PriceRuleCondition>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<UserInfoEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
        }

        [Obsolete("This Method is not used anymore , as the Unique Indexes have been created ONCE and Finito!!!")]
#pragma warning disable IDE0051 // Remove unused private members
        private void CreateUniqueIndexesIfTheyDoNotExist()
#pragma warning restore IDE0051 // Remove unused private members
        {

            //Options for Unique Keys in the Database
            var uniqueOptions = new CreateIndexOptions() { Unique = true };

            //There is by default always 1 index for the Object Id
            var accessoriesIndexesNotExist = AccessoriesCollection.Indexes.List().ToList().Count < 2;
            if (accessoriesIndexesNotExist)
            {
                //Unique Index for the Code of Each Accessory
                var indexKeyAccessories = Builders<BathAccessoryEntity>.IndexKeys.Ascending(a => a.Code);
                AccessoriesCollection.Indexes.CreateOne(new CreateIndexModel<BathAccessoryEntity>(indexKeyAccessories, uniqueOptions));
            }

            //There is by default always 1 index for the Object Id
            var TraitClassesIndexesNotExist = TraitClassesCollection.Indexes.List().ToList().Count < 2;
            if (TraitClassesIndexesNotExist)
            {
                //Unique Index for the TypeOfTrait on the Trait Classes
                var indexKeyTraitClasses = Builders<TraitClassEntity>.IndexKeys.Ascending(tc => tc.TraitType);
                TraitClassesCollection.Indexes.CreateOne(new CreateIndexModel<TraitClassEntity>(indexKeyTraitClasses, uniqueOptions));
            }

            //There is by default always 1 index for the Object Id
            var traitsIndexesNotExist = TraitsCollection.Indexes.List().ToList().Count < 3;
            if (traitsIndexesNotExist)
            {
                //Cannot Have Traits with the Same Type and Name
                var indexKeyTraits = Builders<TraitEntity>.IndexKeys.Ascending(t => t.TraitType).Ascending(t => t.Trait.DefaultValue);
                TraitsCollection.Indexes.CreateOne(new CreateIndexModel<TraitEntity>(indexKeyTraits, uniqueOptions));

                //Cannot Have Traits with the Same Type and Name
                var indexKeyTraitCode = Builders<TraitEntity>.IndexKeys.Ascending(t => t.TraitType).Ascending(t => t.Code);
                TraitsCollection.Indexes.CreateOne(new CreateIndexModel<TraitEntity>(indexKeyTraitCode, uniqueOptions));
            }

            //There is by default always 1 index for the Object Id
            var TraitGroupIndexesNotExist = TraitGroupsCollection.Indexes.List().ToList().Count < 2;
            if (TraitGroupIndexesNotExist)
            {
                //Unique Index for the TypeOfTrait on the Trait Classes
                var indexKeyTraitGroups = Builders<TraitGroupEntity>.IndexKeys.Ascending(tc => tc.Code);
                TraitGroupsCollection.Indexes.CreateOne(new CreateIndexModel<TraitGroupEntity>(indexKeyTraitGroups, uniqueOptions));
            }

            var userOptionsIndexesNotExist = UserOptionsCollection.Indexes.List().ToList().Count < 2;
            if (userOptionsIndexesNotExist)
            {
                //Unique Index for the Default Name of the Options
                var indexKeyUserOptionsName = Builders<UserAccessoriesOptionsEntity>.IndexKeys.Ascending(o => o.Name.DefaultValue);
                UserOptionsCollection.Indexes.CreateOne(new CreateIndexModel<UserAccessoriesOptionsEntity>(indexKeyUserOptionsName , uniqueOptions));
            }

            var usersIndexesNotExist = UsersCollection.Indexes.List().ToList().Count < 3;
            if (usersIndexesNotExist)
            {
                //Unique Index for the UserObjectId
                var indexKeyUsersUserName = Builders<UserInfoEntity>.IndexKeys.Ascending(u => u.UserName);
                var indexKeyUsersGraphId = Builders<UserInfoEntity>.IndexKeys.Ascending(u => u.GraphUserObjectId);
                UsersCollection.Indexes.CreateOne(new CreateIndexModel<UserInfoEntity>(indexKeyUsersUserName, uniqueOptions));
                UsersCollection.Indexes.CreateOne(new CreateIndexModel<UserInfoEntity>(indexKeyUsersGraphId, uniqueOptions));
            }
        }
    }
}
