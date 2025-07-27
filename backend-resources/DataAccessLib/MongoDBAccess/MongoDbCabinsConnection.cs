using DataAccessLib.CustomSerilizers;
using DataAccessLib.NoSQLModels;
using DnsClient.Internal;
using GlassesOrdersModels.Models;
using GlassesOrdersModels.Models.SubModels;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDbCommonLibrary;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DataAccessLib.MongoDBAccess;

/// <summary>
/// Establishes a MongoDB Connection
/// </summary>
public class MongoDbCabinsConnection : IMongoDbCabinsConnection
{
    private readonly IMongoDatabase database;

    //The Configuration Keys to retrieve said data
    private readonly string partsCollectionConfigName = "MongoPartsCollectionName";
    private readonly string constraintsCollectionConfigName = "MongoConstraintsCollectionName";
    private readonly string partsListsCollectionConfigName = "MongoPartsListsCollectionName";
    private readonly string settingsCollectionConfigName = "MongoCabinSettingsCollectionName";
    private readonly string glassOrdersCollectionConfigName = "GlassOrdersCollectionName";
    private readonly string glassesStockCollectionConfigName = "MongoGlassesStockCollectionName";

    public MongoClient Client { get; private set; } //This is public in order to use it in Transactions

    public IMongoCollection<CabinPartEntity> PartsCollection { get; set; }
    public IMongoCollection<CabinConstraintsEntity> ConstraintsCollection { get; set; }
    public IMongoCollection<DefaultPartsListEntity> PartsListsCollection { get; set; }
    public IMongoCollection<CabinSettingsEntity> SettingsCollection { get; set; }
    public IMongoCollection<GlassesOrderEntity> GlassOrdersCollection { get; set; }
    public IMongoCollection<StockedGlassRowEntity> GlassesStockCollection { get; set; }

    /// <summary>
    /// Instantiates a New MongoDB Connection with the provided configuration File
    /// </summary>
    /// <param name="config"></param>
    /// <exception cref="Exception"></exception>
    public MongoDbCabinsConnection(IMongoConnection connection)
    {
        //Run Class Mapping
        ApplyMongoDbMappings();
        var config = connection.GetConfiguration();
        
        //Create new Client by passing the Settings
        Client = connection.GetClient();

        //Get the Collection Name from the Configuration File
        string partsCollectionName = config[partsCollectionConfigName] ?? throw new Exception($"{nameof(partsCollectionConfigName)} not found in Configuration File");
        string constraintsCollectionName = config[constraintsCollectionConfigName] ?? throw new Exception($"{nameof(constraintsCollectionConfigName)} not found in Configuration File");
        string partsListCollectionName = config[partsListsCollectionConfigName] ?? throw new Exception($"{nameof(partsListsCollectionConfigName)} not found in Configuration File");
        string settingsCollectionName = config[settingsCollectionConfigName] ?? throw new Exception($"{nameof(settingsCollectionConfigName)} not found in Configuration File");
        string glassOrdersCollectionName = config[glassOrdersCollectionConfigName] ?? throw new Exception($"{nameof(glassOrdersCollectionConfigName)} not found in Configuration File");
        string glassesStockCollectionName = config[glassesStockCollectionConfigName] ?? throw new Exception($"{nameof(glassesStockCollectionConfigName)} not found in Configuration File");

        //Retrieve the Database
        database = connection.GetDatabase();

        //Retrieve the Collection
        PartsCollection = database.GetCollection<CabinPartEntity>(partsCollectionName);
        ConstraintsCollection = database.GetCollection<CabinConstraintsEntity>(constraintsCollectionName);
        PartsListsCollection = database.GetCollection<DefaultPartsListEntity>(partsListCollectionName);
        SettingsCollection = database.GetCollection<CabinSettingsEntity>(settingsCollectionName);
        GlassOrdersCollection = database.GetCollection<GlassesOrderEntity>(glassOrdersCollectionName);
        GlassesStockCollection = database.GetCollection<StockedGlassRowEntity>(glassesStockCollectionName);
        
        //Create any Unique Constraints for the Various Collections of the Database
        //DOES NOT NEED TO RUN APART FROM THE FIRST TIME THE DATABASE HAS RUN ... SO NO NEED TO RUN ANYMORE ... CREATES PROBLEMS AND BIG DELAYS IN STARTUP.
        //CreateUniqueIndexesIfTheyDoNotExist();
    }

    private static void ApplyMongoDbMappings()
    {
        //needed to deserilize it as dict key in default lists
        BsonSerializer.RegisterSerializer(new EnumSerializer<PartSpot>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinModelEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinDrawNumber>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinSynthesisModel>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinDirection>(BsonType.String));

        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinThicknessEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<GlassThicknessEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<GlassDrawEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<GlassFinishEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinFinishEnum>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<GlassTypeEnum>(BsonType.String));

        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinPartType>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinProfileType>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinHandleType>(BsonType.String));
        BsonSerializer.RegisterSerializer(new EnumSerializer<CabinStripType>(BsonType.String));

        //Custom serilizer for the Cabin Identifier
        BsonSerializer.RegisterSerializer(new CabinIdentifierSerilizer());


        //Register Order Status as Int (To not store all Flags each Time)
        BsonSerializer.RegisterSerializer(new EnumSerializer<OrderStatus>(BsonType.Int32));

        MapPartsClasses();
        MapConstraintsClasses();
        MapPartsListsClasses();
        MapCabinsClasses();
        MapGlassesAndExtras();
        MapGlassOrder();
    }

    /// <summary>
    /// Registers the CabinPart Class with its inheritance tree,
    /// Registers all the Parts SubClasses
    /// </summary>
    private static void MapPartsClasses()
    {

        BsonClassMap.RegisterClassMap<CabinIdentifierContainer>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        //Use Automap and then Set Default Value for the Dictionary so that its not null if not values are present inside it
        BsonClassMap.RegisterClassMap<CabinPartEntity>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(partEntity => new(partEntity.Part,partEntity.AdditionalPartsPerStructure, partEntity.LanguageDescriptions));
            cm.MapField(x => x.AdditionalPartsPerStructure).SetDefaultValue(new Dictionary<CabinIdentifier, List<CabinPart>>());

            ////Ignore the Id if it has the Default Value -- this way when inserting a new id is given ?
            //cm.GetMemberMap(m => m.Id).SetIgnoreIfDefault(true);
        });

        //Register Parts Class Hierarchy Because ICabinPart is an interface cannot be declared in entity
        BsonClassMap.RegisterClassMap<CabinPart>(cm =>
        {
            cm.AutoMap();

            //Ignores any Extra Elements in Deserilization and wont throw exceptions  (a field existing in db but not in application consuming it)
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            //Need to set the Default Value because initially Parts did not have this Property and otherwise it ignores the Field Initilizer and sets it to null
            cm.MapField(x => x.AdditionalParts).SetDefaultValue(new List<CabinPart>());
            cm.MapField(x => x.Quantity).SetDefaultValue(1d);

            cm.AddKnownType(typeof(CabinAngle));
            cm.AddKnownType(typeof(CabinHandle));
            cm.AddKnownType(typeof(CabinHinge));
            cm.AddKnownType(typeof(GlassToGlassHinge));
            cm.AddKnownType(typeof(Hinge9B));
            cm.AddKnownType(typeof(HingeDB));
            cm.AddKnownType(typeof(Profile));
            cm.AddKnownType(typeof(ProfileHinge));
            cm.AddKnownType(typeof(GlassStrip));
            cm.AddKnownType(typeof(CabinSupport));
            cm.AddKnownType(typeof(FloorStopperW));
            cm.AddKnownType(typeof(SupportBar));
            cm.MapProperty(c => c.Part); //IF ENUM AS STRING THEN => .SetSerializer(new EnumSerializer<CabinPartType>(MongoDB.Bson.BsonType.String));
            //cm.MapProperty(c => c.Part).SetElementName("PartEnumValue").SetSerializer(new EnumSerializer<CabinPartType>(MongoDB.Bson.BsonType.String));
        });

        BsonClassMap.RegisterClassMap<CabinAngle>();
        BsonClassMap.RegisterClassMap<CabinHandle>();
        BsonClassMap.RegisterClassMap<CabinHinge>();
        BsonClassMap.RegisterClassMap<GlassToGlassHinge>();
        BsonClassMap.RegisterClassMap<Hinge9B>();
        BsonClassMap.RegisterClassMap<HingeDB>();

        //Register the Constructor becasuse the Part Property is Readonly and gets set only in the cosntructor no later.
        BsonClassMap.RegisterClassMap<Profile>(cm => { cm.AutoMap(); cm.MapCreator(prof => new Profile(prof.Part, prof.ProfileType)); });
        BsonClassMap.RegisterClassMap<ProfileHinge>(cm => { cm.AutoMap(); cm.MapCreator(prof => new ProfileHinge(prof.Part, prof.ProfileType)); });

        BsonClassMap.RegisterClassMap<GlassStrip>();
        BsonClassMap.RegisterClassMap<CabinSupport>();
        BsonClassMap.RegisterClassMap<FloorStopperW>();
        BsonClassMap.RegisterClassMap<SupportBar>();
    }


    /// <summary>
    /// Registers the CabinConstraints Class with its inheritance tree,
    /// Registers all the Constraints Subclasses
    /// </summary>
    private static void MapConstraintsClasses()
    {
        BsonClassMap.RegisterClassMap<CabinConstraintsEntity>();
        BsonClassMap.RegisterClassMap<CabinConstraints>(cm =>
        {
            cm.AutoMap();

            //Ignores any Extra Elements in Deserilization and wont throw exceptions  (a field existing in db but not in application consuming it)
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            cm.MapProperty(c => c.AllowedGlasses).SetDefaultValue(new List<AllowedGlass>());

            cm.AddKnownType(typeof(Constraints9S));
            cm.AddKnownType(typeof(Constraints9A));
            cm.AddKnownType(typeof(Constraints94));
            cm.AddKnownType(typeof(Constraints9C));
            cm.AddKnownType(typeof(Constraints9B));
            cm.AddKnownType(typeof(Constraints9F));
            cm.AddKnownType(typeof(ConstraintsDB));
            cm.AddKnownType(typeof(ConstraintsE));
            cm.AddKnownType(typeof(ConstraintsW));
            cm.AddKnownType(typeof(ConstraintsWFlipper));
            cm.AddKnownType(typeof(ConstraintsHB));
            cm.AddKnownType(typeof(ConstraintsV4));
            cm.AddKnownType(typeof(ConstraintsVS));
            cm.AddKnownType(typeof(ConstraintsVA));
            cm.AddKnownType(typeof(ConstraintsVF));
            cm.AddKnownType(typeof(ConstraintsNB));
            cm.AddKnownType(typeof(ConstraintsNP));
            cm.AddKnownType(typeof(ConstraintsWS));
            cm.AddKnownType(typeof(UndefinedConstraints));

        });
        //Constraints
        BsonClassMap.RegisterClassMap<Constraints9S>(cm =>
        {
            cm.AutoMap();
            //Use the Older Name of the Overlap for this Object for backwards compatibility with all versions of the Application
            cm.MapMember(c => c.Overlap).SetElementName("OverlapEPIK");
        });
        BsonClassMap.RegisterClassMap<Constraints9A>(cm => 
        {
            cm.AutoMap();
            //Use the Older Name of the Overlap for this Object for backwards compatibility with all versions of the Application
            cm.MapMember(c => c.Overlap).SetElementName("OverlapEPIK");
        });
        BsonClassMap.RegisterClassMap<Constraints94>(cm =>
        {
            cm.AutoMap();
            //Use the Older Name of the Overlap for this Object for backwards compatibility with all versions of the Application
            cm.MapMember(c => c.Overlap).SetElementName("OverlapEPIK");
        });
        BsonClassMap.RegisterClassMap<Constraints9C>(cm =>
        {
            cm.AutoMap();
            //Set Default Values for these three properties which are new in the newer application version (17/05/2023)
            cm.MapProperty(c => c.MinDoorDistanceFromWallOpened).SetDefaultValue(0);
            cm.MapProperty(c => c.Overlap).SetDefaultValue(0);
            cm.MapProperty(c => c.CoverDistance).SetDefaultValue(0);
        });
        BsonClassMap.RegisterClassMap<Constraints9B>();
        BsonClassMap.RegisterClassMap<Constraints9F>();
        BsonClassMap.RegisterClassMap<ConstraintsDB>();
        BsonClassMap.RegisterClassMap<ConstraintsE>();
        BsonClassMap.RegisterClassMap<ConstraintsW>();
        BsonClassMap.RegisterClassMap<ConstraintsWFlipper>();
        BsonClassMap.RegisterClassMap<ConstraintsHB>();
        BsonClassMap.RegisterClassMap<ConstraintsV4>();
        BsonClassMap.RegisterClassMap<ConstraintsVS>();
        BsonClassMap.RegisterClassMap<ConstraintsVA>();
        BsonClassMap.RegisterClassMap<ConstraintsVF>();
        BsonClassMap.RegisterClassMap<ConstraintsNB>();
        BsonClassMap.RegisterClassMap<ConstraintsNP>();
        BsonClassMap.RegisterClassMap<ConstraintsWS>(cm =>
        {
            cm.AutoMap();
            //Set Default Values for this property which is new in the newer application version (17/05/2023)
            cm.MapProperty(c => c.MinDoorDistanceFromWallOpened).SetDefaultValue(0);
        });
        BsonClassMap.RegisterClassMap<UndefinedConstraints>();
    }
    private static void MapPartsListsClasses()
    {
        BsonClassMap.RegisterClassMap<DefaultPartsListEntity>();
        BsonClassMap.RegisterClassMap<DefaultPartsList>(cm =>
        {
            cm.AutoMap();

            //Ignores any Extra Elements in Deserilization and wont throw exceptions
            //(a field existing in db but not in application consuming/using the database)
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
        });
        BsonClassMap.RegisterClassMap<PartSpotDefaults>(cm =>
        {
            cm.AutoMap();
            cm.MapField(x => x.DefaultQuantity).SetDefaultValue(1d);
        });
        BsonClassMap.RegisterClassMap<PartSet>();
    }
    private static void MapCabinsClasses()
    {
        BsonClassMap.RegisterClassMap<CabinPartsListEntity>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

    }
    private static void MapGlassesAndExtras()
    {
        BsonClassMap.RegisterClassMap<Glass>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
        });

        BsonClassMap.RegisterClassMap<CabinExtra>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            cm.MapCreator(e => new(e.ExtraType));
            cm.AddKnownType(typeof(StepCut));
        });

        BsonClassMap.RegisterClassMap<StockedGlassRowEntity>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

    }
    private static void MapGlassOrder()
    {
        BsonClassMap.RegisterClassMap<GlassesOrderEntity>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
        });
        BsonClassMap.RegisterClassMap<CabinRowEntity>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            //A serilizer to Serilize/Deserilize Guids that are stored in the Legacy Byte Order that is deprectated in the newer versions of MongoDB (3+)
            cm.MapMember(e => e.SynthesisKey).SetSerializer(new LegacyGuidSerializer());
            cm.MapMember(e => e.CabinKey).SetSerializer(new LegacyGuidSerializer());
        });
        BsonClassMap.RegisterClassMap<GlassOrderRowEntity>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            //A serilizer to Serilize/Deserilize Guids that are stored in the Legacy Byte Order that is deprectated in the newer versions of MongoDB (3+)
            cm.MapMember(e => e.CabinRowKey).SetSerializer(new LegacyGuidSerializer());
            
        });

        BsonClassMap.RegisterClassMap<CabinOrderRow>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);

            //A serilizer to Serilize/Deserilize Guids that are stored in the Legacy Byte Order that is deprectated in the newer versions of MongoDB (3+)
            cm.MapMember(e=> e.SynthesisKey).SetSerializer(new LegacyGuidSerializer());
            cm.MapMember(e => e.CabinKey).SetSerializer(new LegacyGuidSerializer());
        });
        BsonClassMap.RegisterClassMap<GlassOrderRow>((cm) =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
            //A serilizer to Serilize/Deserilize Guids that are stored in the Legacy Byte Order that is deprectated in the newer versions of MongoDB (3+)
            cm.MapMember(e=> e.CabinRowKey).SetSerializer(new LegacyGuidSerializer());
            //Property that was added After the Release , so there are saved Rows without this field and need to be defaulted to something.
            cm.MapField(x => x.IsFromStock).SetDefaultValue(false);

        });

        BsonClassMap.RegisterClassMap<GlassesOrderSmall>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.SetIgnoreExtraElementsIsInherited(true);
        });
    }

    [Obsolete("Indexes have already been Created Once")]
    /// <summary>
    /// Creates Indexes with Unique Constraints for All the Collections
    /// This Method will make a Request which requires Authentication , its best to Run Lazy before the first request of Data from the Db
    /// This method should run only once For the Lifetime of the Database . After that the indexes are created either way inside the db and will do nothing
    /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
    private void CreateUniqueIndexesIfTheyDoNotExist()
#pragma warning restore IDE0051 // Remove unused private members
    {
        //Index for Parts Collection
        //Create the key(s) Definitions (using the expression will throw a SerilizeException if the Property is an Interface)

        //Options for Unique Keys in the Database
        var uniqueOptions = new CreateIndexOptions() { Unique = true };

        //Unique Index for the Code of Each Part (Not parts with the same Code)
        var indexKeyParts = Builders<CabinPartEntity>.IndexKeys.Ascending("Part.Code");
        PartsCollection.Indexes.CreateOne(new CreateIndexModel<CabinPartEntity>(indexKeyParts, uniqueOptions));

        //Unique Index for the Default Collections Assigned to a Set of Cabins (Draw,Model,SynthesisModel)
        var indexKeyConstraints = Builders<CabinConstraintsEntity>.IndexKeys.Ascending(e=> e.DrawNumber).Ascending(e=> e.SynthesisModel).Ascending(e=>e.Model);
        ConstraintsCollection.Indexes.CreateOne(new CreateIndexModel<CabinConstraintsEntity>(indexKeyConstraints, uniqueOptions));

        var indexKeyPartsLists = Builders<DefaultPartsListEntity>.IndexKeys.Ascending(e=> e.DrawNumber).Ascending(e=>e.SynthesisModel).Ascending(e=> e.Model);
        PartsListsCollection.Indexes.CreateOne(new CreateIndexModel<DefaultPartsListEntity>(indexKeyPartsLists, uniqueOptions));

        var indexKeySettings = Builders<CabinSettingsEntity>.IndexKeys.Ascending(e=> e.DrawNumber).Ascending(e=> e.SynthesisModel).Ascending(e=> e.Model);
        SettingsCollection.Indexes.CreateOne(new CreateIndexModel<CabinSettingsEntity>(indexKeySettings, uniqueOptions));

        //Unique Key and index for the OrderId (apart from its object Id)
        var indexKeyGlassOrders = Builders<GlassesOrderEntity>.IndexKeys.Descending(e=> e.OrderId);
        GlassOrdersCollection.Indexes.CreateOne(new CreateIndexModel<GlassesOrderEntity>(indexKeyGlassOrders, uniqueOptions));

        //Index Keys for All the Rows

        var indexKeyPaoRefCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.ReferencePA0)}");
        var indexKeyOrderIdCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderId)}");
        var indexKeyQuantityCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.Quantity)}");
        var indexKeyCabinKeyCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.CabinKey)}");
        var indexKeyCodeCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Code)}");
        var indexKeyHeightCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Height)}");
        var indexKeyLengthCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.NominalLength)}");
        var indexKeyGlassFinishCabin = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.GlassFinish)}");
        var indexKeyHasStepCabin = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.HasStep)}");
        var indexKeyThicknessesCabin = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Thicknesses)}");
        var indexKeyModelCabin = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Model)}");
        var indexKeyDrawCabin = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.IsPartOfDraw)}");
        var indexKeyMetalFinishCabin = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.CabinRows)}.{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.MetalFinish)}");

        var indexKeyPaoRefGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.ReferencePA0)}");
        var indexKeyOrderIdGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderId)}");
        var indexKeyObjectIdGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.Id)}");
        var indexKeyQuantityGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.Quantity)}");
        var indexKeyCabinKeyGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.CabinRowKey)}");
        var indexKeyHeightGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.Height)}");
        var indexKeyLengthGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.Length)}");
        var indexKeyStepHeightGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.StepHeight)}");
        var indexKeyStepLengthGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.StepLength)}");
        var indexKeyFinishGlass = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.Finish)}");
        var indexKeyThicknessGlass = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.Thickness)}");
        var indexKeyDrawGlass = Builders<GlassesOrderEntity>.IndexKeys.Ascending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.Draw)}");
        var indexKeyCornerRadiusRGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.CornerRadiusTopRight)}");
        var indexKeyCornerRadiusLGlass = Builders<GlassesOrderEntity>.IndexKeys.Descending($"{nameof(GlassesOrderEntity.GlassRows)}.{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.CornerRadiusTopLeft)}");

        GlassOrdersCollection.Indexes.CreateMany(
        [
            new(indexKeyPaoRefCabin),
            new(indexKeyOrderIdCabin),
            new(indexKeyQuantityCabin),
            new(indexKeyCabinKeyCabin,uniqueOptions), //CabinKey is Unique per Cabin
            new(indexKeyCodeCabin),
            new(indexKeyHeightCabin),
            new(indexKeyLengthCabin),
            new(indexKeyGlassFinishCabin),
            new(indexKeyHasStepCabin),
            new(indexKeyThicknessesCabin),
            new(indexKeyModelCabin),
            new(indexKeyDrawCabin),
            new(indexKeyMetalFinishCabin),

            new(indexKeyPaoRefGlass),
            new(indexKeyOrderIdGlass),
            new(indexKeyObjectIdGlass,uniqueOptions), // ObjectId is Unique
            new(indexKeyQuantityGlass),
            new(indexKeyCabinKeyGlass), //Not Unique Multiple Glasses Can Have the Same Key (And in Different Orders)
            new(indexKeyHeightGlass),
            new(indexKeyLengthGlass),
            new(indexKeyStepHeightGlass),
            new(indexKeyStepLengthGlass),
            new(indexKeyFinishGlass),
            new(indexKeyThicknessGlass),
            new(indexKeyDrawGlass),
            new(indexKeyCornerRadiusRGlass),
            new(indexKeyCornerRadiusLGlass),
        ]);
    }

}

/// <summary>
/// A serilizer to Serilize/Deserilize Guids that are stored in the Legacy Byte Order that is deprectated in the newer versions of MongoDB (3+)
/// <para>This serilizer is only used by CabinRowKey - Synthesis Key - Glass OrderRow Key , which use a guid property</para>
/// </summary>
public class LegacyGuidSerializer : SerializerBase<Guid>
{
    //private static readonly GuidRepresentation LegacyRepresentation = GuidRepresentation.Unspecified;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
    {
        var guidBytes = value.ToByteArray();
        // Ensure byte order matches UUID Legacy (subtype 3)
        var legacyBytes = new byte[16];
        legacyBytes[0] = guidBytes[3];
        legacyBytes[1] = guidBytes[2];
        legacyBytes[2] = guidBytes[1];
        legacyBytes[3] = guidBytes[0];
        legacyBytes[4] = guidBytes[5];
        legacyBytes[5] = guidBytes[4];
        legacyBytes[6] = guidBytes[7];
        legacyBytes[7] = guidBytes[6];
        legacyBytes[8] = guidBytes[8];
        legacyBytes[9] = guidBytes[9];
        legacyBytes[10] = guidBytes[10];
        legacyBytes[11] = guidBytes[11];
        legacyBytes[12] = guidBytes[12];
        legacyBytes[13] = guidBytes[13];
        legacyBytes[14] = guidBytes[14];
        legacyBytes[15] = guidBytes[15];

        context.Writer.WriteBinaryData(new BsonBinaryData(legacyBytes, BsonBinarySubType.UuidLegacy));
    }

    public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.GetCurrentBsonType();
        if (bsonType != BsonType.Binary)
        {
            throw new FormatException($"Expected BsonType.Binary, but got {bsonType} for GUID deserialization.");
        }

        var binaryData = context.Reader.ReadBinaryData();
        if (binaryData.SubType != BsonBinarySubType.UuidLegacy)
        {
            throw new FormatException($"Expected UuidLegacy subtype (3), but got {binaryData.SubType}.");
        }

        var bytes = binaryData.Bytes;
        // Convert from UUID Legacy byte order to .NET Guid order
        var guidBytes = new byte[16];
        guidBytes[0] = bytes[3];
        guidBytes[1] = bytes[2];
        guidBytes[2] = bytes[1];
        guidBytes[3] = bytes[0];
        guidBytes[4] = bytes[5];
        guidBytes[5] = bytes[4];
        guidBytes[6] = bytes[7];
        guidBytes[7] = bytes[6];
        guidBytes[8] = bytes[8];
        guidBytes[9] = bytes[9];
        guidBytes[10] = bytes[10];
        guidBytes[11] = bytes[11];
        guidBytes[12] = bytes[12];
        guidBytes[13] = bytes[13];
        guidBytes[14] = bytes[14];
        guidBytes[15] = bytes[15];

        return new Guid(guidBytes);
    }
}


