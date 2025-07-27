using CommonInterfacesBronze;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary
{
    public interface IMongoConnection
    {
        public IMongoDatabase GetDatabase();
        public MongoClient GetClient();
        public IConfiguration GetConfiguration();


    }

    public class MongoConnectionDefault : IMongoConnection
    {
        private readonly string databaseConfigName = "MongoDatabaseName";
        private readonly string connStringConfigKey = "MongoDBConnString";

        private IMongoDatabase database;
        private MongoClient client;
        private readonly IConfiguration configuration;

        private bool hasInitilized;
        public void Initilize()
        {
            if (hasInitilized) return;
            else 
            {
                try
                {
                    //Get the Connection String
                    var conString = configuration.GetConnectionString(connStringConfigKey);

                    var clientSettings = MongoClientSettings.FromConnectionString(conString);
                    //Change the LinqProvider to the previous LINQ2 instead of 3
                    //DEPRACETED OPTION -> Provider Is now LINQ 3 By Default and LINQ2 is Deprecated altogether
                    //clientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

                    //Create new Client by passing the above Settings
                    client = new MongoClient(clientSettings);

                    // Retrieve the Database
                    var databaseName = configuration[databaseConfigName] ?? throw new Exception("Name of Database not found in Configuration File");
                    database = client.GetDatabase(databaseName);

                    //Apply General Mappings
                    BsonClassMap.RegisterClassMap<DbEntity>(cm =>
                    {
                        cm.AutoMap();
                        cm.MapProperty(x => x.Created).SetSerializer(new DateTimeSerializer(true));
                        cm.AddKnownType(typeof(DescriptiveEntity));
                    });

                    BsonClassMap.RegisterClassMap<LocalizedString>(cm =>
                    {
                        cm.AutoMap();
                        cm.SetIgnoreExtraElements(true);
                        cm.MapCreator(locString => new(locString.DefaultValue, locString.LocalizedValues));
                        cm.MapProperty(x => x.DefaultValue).SetDefaultValue("");
                        cm.MapProperty(x => x.LocalizedValues).SetDefaultValue(new Dictionary<string, string>());
                    });

                    BsonClassMap.RegisterClassMap<DescriptiveEntity>(cm =>
                    {
                        cm.AutoMap();
                        cm.SetIgnoreExtraElements(true);
                        cm.MapProperty(x => x.Name).SetDefaultValue(LocalizedString.Undefined());
                        cm.MapProperty(x => x.Description).SetDefaultValue(LocalizedString.Undefined());
                        cm.MapProperty(x => x.ExtendedDescription).SetDefaultValue(LocalizedString.Undefined());
                    });
                    hasInitilized = true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Initilization Failed", ex);
                }
            }
        }

        public MongoConnectionDefault(IConfiguration config)
        {
            this.configuration = config;
            //Get the Connection String
            var conString = configuration.GetConnectionString(connStringConfigKey);

            var clientSettings = MongoClientSettings.FromConnectionString(conString);
            //Change the LinqProvider to the previous LINQ2 instead of 3
            //DEPRACETED OPTION -> Provider Is now LINQ 3 By Default and LINQ2 is Deprecated altogether
            //clientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

            //Create new Client by passing the above Settings
            client = new MongoClient(clientSettings);

            // Retrieve the Database
            var databaseName = config[databaseConfigName] ?? throw new Exception("Name of Database not found in Configuration File");
            database = client.GetDatabase(databaseName);

            //Apply General Mappings
            BsonClassMap.RegisterClassMap<DbEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(x => x.Created).SetSerializer(new DateTimeSerializer(true));
                cm.AddKnownType(typeof(DescriptiveEntity));
            });
            
            BsonClassMap.RegisterClassMap<LocalizedString>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapCreator(locString => new(locString.DefaultValue, locString.LocalizedValues));
                cm.MapProperty(x => x.DefaultValue).SetDefaultValue("");
                cm.MapProperty(x => x.LocalizedValues).SetDefaultValue(new Dictionary<string, string>());
            });

            BsonClassMap.RegisterClassMap<DescriptiveEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapProperty(x => x.Name).SetDefaultValue(LocalizedString.Undefined());
                cm.MapProperty(x => x.Description).SetDefaultValue(LocalizedString.Undefined());
                cm.MapProperty(x => x.ExtendedDescription).SetDefaultValue(LocalizedString.Undefined());
            });

            
        }

        public MongoClient GetClient()
        {
            return client;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }
    }
}
