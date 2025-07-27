using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Charachteristics;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using MirrorsRepositoryMongoDB.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonCustomSerilizers;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonExceptions;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MirrorsLib.IPRating;

namespace MirrorsRepositoryMongoDB
{
    public interface IMongoDbMirrorsConnection
    {
        MongoClient Client { get; }
        IMongoCollection<MirrorConstraintsEntity> ConstraintsCollection { get; }
        IMongoCollection<CustomMirrorElementEntity> CustomElementsConnection { get; }
        IMongoCollection<MirrorSupportEntity> SupportsCollection { get; }
        IMongoCollection<MirrorSandblastEntity> SandblastsCollection { get; }
        IMongoCollection<MirrorLightElementEntity> LightsCollection { get; }
        IMongoCollection<MirrorModuleEntity> ModulesCollection { get; }
        IMongoCollection<MirrorSeriesElementEntity> SeriesCollection { get; }
        IMongoCollection<MirrorElementPositionEntity> PositionsCollection { get; }
        IMongoCollection<MirrorElementPositionOptionsEntity> PositionsOptionsCollection { get; }
        IMongoCollection<MirrorFinishElementEntity> FinishTraitsCollection { get; }
        IMongoCollection<CustomMirrorTraitEntity> CustomTraitsCollection { get; }
        IMongoCollection<MirrorApplicationOptionsEntity> OptionsCollection { get; }
        IMongoCollection<MirrorsOrderEntity> MirrorsOrdersCollection { get; }
    }

    public class MongoDbMirrorsConnection : IMongoDbMirrorsConnection
    {
        private readonly string constraintsCollectionConfigName = "MongoMirrorConstraintsCollectionName";
        private readonly string customElementsCollectionConfigName = "MongoCustomMirrorElementsCollectionName";
        private readonly string supportsCollectionConfigName = "MongoMirrorSupportsCollectionName";
        private readonly string sandblastsCollectionConfigName = "MongoSandblastsCollectionConfigName";
        private readonly string lightsCollectionConfigName = "MongoMirrorLightsCollectionName";
        private readonly string modulesCollectionConfigName = "MongoModulesCollectionConfigName";
        private readonly string seriesCollectionConfigName = "MongoMirrorSeriesCollectionName";
        private readonly string positionsCollectionConfigName = "MongoMirrorPositionsCollectionName";
        private readonly string positionsOptionsCollectionConfigName = "MongoMirrorPositionsOptionsCollectionName";
        private readonly string mirrorFinishElementsCollectionConfigName = "MongoMirrorFinishElementsCollectionName";
        private readonly string mirrorCustomTraitsCollectionConfigName = "MongoMirrorCustomTraitsCollectionName";
        private readonly string mirrorOptionsCollectionConfigName = "MongoMirrorApplicationOptionsCollectionName";
        private readonly string mirrorsOrdersCollectionConfigName = "MongoMirrorsOrdersCollectionName";

        public MongoClient Client { get; }
        public IMongoCollection<MirrorConstraintsEntity> ConstraintsCollection { get; }
        public IMongoCollection<CustomMirrorElementEntity> CustomElementsConnection { get; }
        public IMongoCollection<MirrorSupportEntity> SupportsCollection { get; }
        public IMongoCollection<MirrorSandblastEntity> SandblastsCollection { get; }
        public IMongoCollection<MirrorLightElementEntity> LightsCollection { get; }
        public IMongoCollection<MirrorModuleEntity> ModulesCollection { get; }
        public IMongoCollection<MirrorSeriesElementEntity> SeriesCollection { get; }
        public IMongoCollection<MirrorElementPositionEntity> PositionsCollection { get; }
        public IMongoCollection<MirrorElementPositionOptionsEntity> PositionsOptionsCollection { get; }
        public IMongoCollection<MirrorFinishElementEntity> FinishTraitsCollection { get; }
        public IMongoCollection<CustomMirrorTraitEntity> CustomTraitsCollection { get; }
        public IMongoCollection<MirrorApplicationOptionsEntity> OptionsCollection { get; }
        public IMongoCollection<MirrorsOrderEntity> MirrorsOrdersCollection { get; }

        public MongoDbMirrorsConnection(IMongoConnection connection)
        {
            ApplyMongoDbMappings();
            var config = connection.GetConfiguration();
            Client = connection.GetClient();

            string constraintsCollectionName = config[constraintsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(constraintsCollectionConfigName);
            string customElementsCollectionName = config[customElementsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(customElementsCollectionConfigName);
            string supportsCollectionName = config[supportsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(supportsCollectionConfigName);
            string sandblastsCollectionName = config[sandblastsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(sandblastsCollectionConfigName);
            string lightsCollectionName = config[lightsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(lightsCollectionConfigName);
            string modulesCollectionName = config[modulesCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(modulesCollectionConfigName);
            string seriesCollectionName = config[seriesCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(seriesCollectionConfigName);
            string positionsCollectionName = config[positionsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(positionsCollectionConfigName);
            string positionsOptionsCollectionName = config[positionsOptionsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(positionsOptionsCollectionConfigName);
            string finishElementsCollectionName = config[mirrorFinishElementsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(mirrorFinishElementsCollectionConfigName);
            string customTraitsCollectionName = config[mirrorCustomTraitsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(mirrorCustomTraitsCollectionConfigName);
            string mirrorOptionsCollectionName = config[mirrorOptionsCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(mirrorOptionsCollectionConfigName);
            string mirrorsOrdersCollectionName = config[mirrorsOrdersCollectionConfigName] ?? throw new ConfigurationKeyNotFoundException(mirrorsOrdersCollectionConfigName);

            var database = connection.GetDatabase();
            ConstraintsCollection = database.GetCollection<MirrorConstraintsEntity>(constraintsCollectionName);
            CustomElementsConnection = database.GetCollection<CustomMirrorElementEntity>(customElementsCollectionName);
            SupportsCollection = database.GetCollection<MirrorSupportEntity>(supportsCollectionName);
            SandblastsCollection = database.GetCollection<MirrorSandblastEntity>(sandblastsCollectionName);
            LightsCollection = database.GetCollection<MirrorLightElementEntity>(lightsCollectionName);
            ModulesCollection = database.GetCollection<MirrorModuleEntity>(modulesCollectionName);
            SeriesCollection = database.GetCollection<MirrorSeriesElementEntity>(seriesCollectionName);
            PositionsCollection = database.GetCollection<MirrorElementPositionEntity>(positionsCollectionName);
            PositionsOptionsCollection = database.GetCollection<MirrorElementPositionOptionsEntity>(positionsOptionsCollectionName);
            FinishTraitsCollection = database.GetCollection<MirrorFinishElementEntity>(finishElementsCollectionName);
            CustomTraitsCollection = database.GetCollection<CustomMirrorTraitEntity>(customTraitsCollectionName);
            OptionsCollection = database.GetCollection<MirrorApplicationOptionsEntity>(mirrorOptionsCollectionName);
            MirrorsOrdersCollection = database.GetCollection<MirrorsOrderEntity>(mirrorsOrdersCollectionName);

            CreateUniqueIndexesIfTheyDoNotExist();
        }

        private static void ApplyMongoDbMappings()
        {
            //In order to Serialize/Deserialize Dictionary<BronzeMirrorShape,string> in CodeBuilderOptions ElementCodeAffixOptions
            BsonSerializer.RegisterSerializer(new EnumSerializer<BronzeMirrorShape>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<MirrorOrientedShape>(BsonType.String));

            //In order to Serialize/Deserialize Dictionary<MirrorModuleType,ElementCodeAffixOptions> in CodeBuilderOptions
            BsonSerializer.RegisterSerializer(new EnumSerializer<MirrorModuleType>(BsonType.String));
            //In order to Serialize/Deserialize Dictionary<MirrorCodeOptionsElementType,ElementCodeAffixOptions> in CodeBuilderOptions
            BsonSerializer.RegisterSerializer(new EnumSerializer<MirrorCodeOptionsElementType>(BsonType.String));

            BsonSerializer.RegisterSerializer(new EnumSerializer<IPWaterRating>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<IPSolidsRating>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<IPAdditionalLetter>(BsonType.String));
            BsonSerializer.RegisterSerializer(new EnumSerializer<MirrorGlassType>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<MirrorGlassThickness>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<IlluminationOption>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<CircleSegmentOrientation>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<EggOrientation>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<CapsuleOrientation>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<EllipseOrientation>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<CircleQuadrantPart>(BsonType.Int32));

            //In order to deserilize the separators Property in CodeBuilderOptions
            BsonSerializer.RegisterSerializer(typeof(Dictionary<int, string>), new DictionaryIntKeySerializer<string>());


            RegisterMirrorElementClassMap();
            RegisterMirrorShapeInfoClassMap();
            RegisterMirrorOrdersClassMap();
            RegisterMirrorApplicationOptionsClassMap();
        }
        public void CreateUniqueIndexesIfTheyDoNotExist()
        {
            //Options for Unique Keys in the Database
            var uniqueOptions = new CreateIndexOptions() { Unique = true };
            var constraintsIndexesNotExist = ConstraintsCollection.Indexes.List().ToList().Count < 2;
            if (constraintsIndexesNotExist)
            {
                var indexKeyConstraints = Builders<MirrorConstraintsEntity>.IndexKeys.Ascending(c => c.Constraints.ConcerningMirrorShape);
                ConstraintsCollection.Indexes.CreateOne(new CreateIndexModel<MirrorConstraintsEntity>(indexKeyConstraints, uniqueOptions));
            }
            //var seriesIndexesNotExist = SeriesCollection.Indexes.List().ToList().Count < 2;
            //var positionsIndexesNotExist = MirrorPositionsCollection.Indexes.List().ToList().Count < 2;

            //Allow only one Options Object for each Element Id
            var positionsOptionsIndexesNotExist = PositionsOptionsCollection.Indexes.List().ToList().Count < 2;
            if (positionsOptionsIndexesNotExist)
            {
                var indexKeyPositionsOptions = Builders<MirrorElementPositionOptionsEntity>.IndexKeys.Ascending(o => o.ConcerningElementId);
                PositionsOptionsCollection.Indexes.CreateOne(new CreateIndexModel<MirrorElementPositionOptionsEntity>(indexKeyPositionsOptions, uniqueOptions));
            }

            var mirrorsOrdersIndexesNotExist = MirrorsOrdersCollection.Indexes.List().ToList().Count < 2;
            if (mirrorsOrdersIndexesNotExist)
            {
                var indexKeyMirrorsOrders = Builders<MirrorsOrderEntity>.IndexKeys.Ascending(o => o.OrderNo);
                MirrorsOrdersCollection.Indexes.CreateOne(new CreateIndexModel<MirrorsOrderEntity>(indexKeyMirrorsOrders, uniqueOptions));
            }

            //Unique Series Per Shape , Has Light , Sandblast
            //if (seriesIndexesNotExist)
            //{
            //    var indexKeySeries = Builders<MirrorElementEntity<MirrorSeriesInfo>>.IndexKeys.Ascending(s => s.SpecificInfo.ShapeOfSeries).Ascending(s => s.SpecificInfo.HasLight);
            //    SeriesCollection.Indexes.CreateOne(new CreateIndexModel<MirrorElementEntity<MirrorSeriesInfo>>(indexKeySeries, uniqueOptions));
            //}
        }

        private static void RegisterMirrorShapeInfoClassMap()
        {
            BsonClassMap.RegisterClassMap<ShapeInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.MapProperty(s => s.TotalLength).SetDefaultValue(0);
                cm.MapProperty(s => s.TotalHeight).SetDefaultValue(0);
                cm.AddKnownType(typeof(UndefinedShapeInfo));
                cm.AddKnownType(typeof(RectangleInfo));
                cm.AddKnownType(typeof(CircleInfo));
                cm.AddKnownType(typeof(CircleSegmentInfo));
                cm.AddKnownType(typeof(CircleQuadrantInfo));
                cm.AddKnownType(typeof(CapsuleInfo));
                cm.AddKnownType(typeof(EggShapeInfo));
                cm.AddKnownType(typeof(EllipseInfo));
                cm.AddKnownType(typeof(LineInfo));
                cm.AddKnownType(typeof(CompositeShapeInfo));
            });
            BsonClassMap.RegisterClassMap<CompositeShapeInfo>(cm =>
            {
                cm.AutoMap();
                //Map the backing Field . This way during deserilization it automatically sets it and the Shape readonly can be read . 
                //The Field must not be readonly 
                //The Field's Name is Set to Shapes. Any queries to the Readonly List will be done against the _shapes saved in database as Shapes.
                cm.MapField("_shapes").SetElementName(nameof(CompositeShapeInfo.Shapes)).SetDefaultValue(() => new List<ShapeInfo>());
                //All Generics must be introduced seperately . In the Future any missing generics will throw exceptions in the Deserilization ...
                cm.AddKnownType(typeof(CompositeShapeInfo<RectangleInfo>));
                cm.AddKnownType(typeof(CompositeShapeInfo<CircleInfo>));
            });

            BsonClassMap.RegisterClassMap<CompositeShapeInfo<RectangleInfo>>(cm =>
            {
                cm.AutoMap();
                //Othere wise it gives a random Discriminator of type CompositeShapeInfo~SomeNumber and it does not know where to assing it ?!
                cm.SetDiscriminator("CompositeShapeInfo-RectangleInfo");
            });
            BsonClassMap.RegisterClassMap<CompositeShapeInfo<CircleInfo>>(cm =>
            {
                cm.AutoMap();
                cm.SetDiscriminator("CompositeShapeInfo-CircleInfo");
            });

            BsonClassMap.RegisterClassMap<RectangleInfo>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(RectangleRingInfo));
            });
            BsonClassMap.RegisterClassMap<CircleInfo>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(CircleRingInfo));
            });
            BsonClassMap.RegisterClassMap<CircleSegmentInfo>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cm => new CircleSegmentInfo(cm.Orientation, cm.Length, cm.Height, cm.LocationX, cm.LocationY));
                cm.AddKnownType(typeof(CircleSegmentRingInfo));
            });
            BsonClassMap.RegisterClassMap<CircleQuadrantInfo>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(CircleQuadrantRingInfo));
            });
            BsonClassMap.RegisterClassMap<CapsuleInfo>(cm =>
            {
                cm.AutoMap();
                //Map also the Orientation for the Db Object
                cm.MapProperty(c => c.Orientation).SetDefaultValue(CapsuleOrientation.Undefined);
                cm.AddKnownType(typeof(CapsuleRingInfo));
            });
            BsonClassMap.RegisterClassMap<EggShapeInfo>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(EggShapeRingInfo));
            });
            BsonClassMap.RegisterClassMap<EllipseInfo>(cm =>
            {
                cm.AutoMap();
                //Map also the Orientation for the Db Object
                cm.MapProperty(e => e.Orientation).SetDefaultValue(EllipseOrientation.Undefined);
                cm.AddKnownType(typeof(EllipseRingInfo));
            });
            BsonClassMap.RegisterClassMap<UndefinedShapeInfo>();
        }
        private static void RegisterMirrorElementClassMap()
        {
            BsonClassMap.RegisterClassMap<MirrorConstraints>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<MirrorConstraintsEntity>();

            BsonClassMap.RegisterClassMap<MirrorElementEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(MirrorSandblastEntity));
                cm.AddKnownType(typeof(MirrorSupportEntity));
                cm.AddKnownType(typeof(MirrorLightElementEntity));
                cm.AddKnownType(typeof(MirrorSeriesElementEntity));
                cm.AddKnownType(typeof(MirrorElementPositionEntity));
                cm.AddKnownType(typeof(MirrorModuleEntity));
                cm.AddKnownType(typeof(MirrorElementEntity));
                cm.AddKnownType(typeof(MirrorFinishElementEntity));
                cm.AddKnownType(typeof(CustomMirrorElementEntity));
                cm.AddKnownType(typeof(MirrorElementTraitEntity));
            });

            BsonClassMap.RegisterClassMap<MirrorElementTraitEntity>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(CustomMirrorTraitEntity));
            });

            //Ip Class Registration
            BsonClassMap.RegisterClassMap<IPRating>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            //Sandblasts
            BsonClassMap.RegisterClassMap<MirrorSandblastEntity>();
            BsonClassMap.RegisterClassMap<MirrorSandblastInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(LineSandblast));
                cm.AddKnownType(typeof(TwoLineSandblast));
                cm.AddKnownType(typeof(HoledRectangleSandblast));
                cm.AddKnownType(typeof(CircularSandblast));
                cm.AddKnownType(typeof(UndefinedSandblastInfo));
            });

            //Supports
            BsonClassMap.RegisterClassMap<MirrorSupportEntity>();
            BsonClassMap.RegisterClassMap<MirrorSupportInstructions>(cm => 
            { 
                cm.AutoMap(); 
                cm.SetIgnoreExtraElements(true); 
                cm.SetIgnoreExtraElementsIsInherited(true); 
            });
            BsonClassMap.RegisterClassMap<MirrorSupportInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(MirrorMultiSupports));
                cm.AddKnownType(typeof(MirrorVisibleFrameSupport));
                cm.AddKnownType(typeof(MirrorBackFrameSupport));
                cm.AddKnownType(typeof(UndefinedMirrorSupportInfo));
            });

            //Light
            BsonClassMap.RegisterClassMap<MirrorLightElementEntity>();
            BsonClassMap.RegisterClassMap<MirrorLightInfo>(cm => 
            { 
                cm.AutoMap(); 
                cm.SetIgnoreExtraElements(true); 
                cm.SetIgnoreExtraElementsIsInherited(true); 
            });
            BsonClassMap.RegisterClassMap<MirrorAdditionalLightInfo>(cm => 
            { 
                cm.AutoMap(); 
                cm.SetIgnoreExtraElements(true); 
                cm.SetIgnoreExtraElementsIsInherited(true); 
            });

            //Modules
            BsonClassMap.RegisterClassMap<MirrorModuleEntity>();
            BsonClassMap.RegisterClassMap<MirrorModuleInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(BluetoothModuleInfo));
                cm.AddKnownType(typeof(MagnifierModuleInfo));
                cm.AddKnownType(typeof(MirrorBackLidModuleInfo));
                cm.AddKnownType(typeof(MirrorLampModuleInfo));
                cm.AddKnownType(typeof(ResistancePadModuleInfo));
                cm.AddKnownType(typeof(RoundedCornersModuleInfo));
                cm.AddKnownType(typeof(ScreenModuleInfo));
                cm.AddKnownType(typeof(TouchButtonModuleInfo));
                cm.AddKnownType(typeof(TransformerModuleInfo));
                cm.AddKnownType(typeof(MirrorProcessModuleInfo));
            });
            BsonClassMap.RegisterClassMap<BluetoothModuleInfo>();
            BsonClassMap.RegisterClassMap<MagnifierModuleInfo>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(MagnifierSandblastedModuleInfo));
            });
            BsonClassMap.RegisterClassMap<MagnifierSandblastedModuleInfo>();
            BsonClassMap.RegisterClassMap<MirrorBackLidModuleInfo>();
            BsonClassMap.RegisterClassMap<MirrorLampModuleInfo>();
            BsonClassMap.RegisterClassMap<ResistancePadModuleInfo>();
            BsonClassMap.RegisterClassMap<RoundedCornersModuleInfo>();
            BsonClassMap.RegisterClassMap<ScreenModuleInfo>();
            BsonClassMap.RegisterClassMap<TouchButtonModuleInfo>();
            BsonClassMap.RegisterClassMap<TransformerModuleInfo>();

            BsonClassMap.RegisterClassMap<MirrorElementPositionEntity>();
            BsonClassMap.RegisterClassMap<PositionInstructionsBase>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(PositionInstructionsBoundingBox));
                cm.AddKnownType(typeof(PositionInstructionsRadial));
                cm.AddKnownType(typeof(UndefinedPositionInstructions));
            });

            BsonClassMap.RegisterClassMap<PositionInstructionsBoundingBox>();
            BsonClassMap.RegisterClassMap<PositionInstructionsRadial>();
            BsonClassMap.RegisterClassMap<UndefinedPositionInstructions>();

            BsonClassMap.RegisterClassMap<MirrorElementPositionOptionsEntity>();

            BsonClassMap.RegisterClassMap<MirrorElementShortDTO>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(MirrorPlacedSandblastDTO));
                cm.AddKnownType(typeof(MirrorPlacedSupportDTO));
                cm.AddKnownType(typeof(MirrorPlacedModuleDTO));
                cm.AddKnownType(typeof(MirrorPlacedLightDTO));
                cm.AddKnownType(typeof(MirrorPlacedElementPositionDTO));
                cm.AddKnownType(typeof(PlacedCustomMirrorElementDTO));
            });
        }

        private static void RegisterMirrorOrdersClassMap()
        {
            BsonClassMap.RegisterClassMap<MirrorsOrderEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.SetDiscriminatorIsRequired(true);
            });
            BsonClassMap.RegisterClassMap<MirrorOrderRowEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.SetDiscriminatorIsRequired(true);
            });
        }

        private static void RegisterMirrorApplicationOptionsClassMap()
        {
            BsonClassMap.RegisterClassMap<MirrorApplicationOptionsEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<MirrorApplicationOptionsBase>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
                cm.AddKnownType(typeof(MirrorCodesBuilderOptions));
            });


            BsonClassMap.RegisterClassMap<MirrorCodesBuilderOptions>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
            BsonClassMap.RegisterClassMap<ElementCodeAffixOptions>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });
        }
    }
}
