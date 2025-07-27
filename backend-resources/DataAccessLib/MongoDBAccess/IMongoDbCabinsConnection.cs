using DataAccessLib.NoSQLModels;
using MongoDB.Driver;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;

namespace DataAccessLib.MongoDBAccess;
public interface IMongoDbCabinsConnection
{
    /// <summary>
    /// The Client
    /// </summary>
    MongoClient Client { get; }
    /// <summary>
    /// The Collection Holding all the Cabin Parts . Queryable by Code also
    /// </summary>
    IMongoCollection<CabinPartEntity> PartsCollection { get; set; }
    /// <summary>
    /// The Collection Holding all the Constraints . Queryable by Model-Draw-SynthesisModel
    /// </summary>
    IMongoCollection<CabinConstraintsEntity> ConstraintsCollection { get; set; }
    /// <summary>
    /// The Collection Holding the Various Parts Lists of Each Model . Queryable by Model-Draw-SynthesisModel
    /// </summary>
    IMongoCollection<DefaultPartsListEntity> PartsListsCollection { get; set; }
    /// <summary>
    /// The Collection Holding the Starting Set Values in each Model . Queryable by Model-Draw-SynthesisModel
    /// </summary>
    IMongoCollection<CabinSettingsEntity> SettingsCollection { get; set; }
    /// <summary>
    /// The Collection Holding all the Glass Orders
    /// </summary>
    IMongoCollection<GlassesOrderEntity> GlassOrdersCollection { get; set; }
    /// <summary>
    /// The Collection Holding all the Stocked Glassed
    /// </summary>
    IMongoCollection<StockedGlassRowEntity> GlassesStockCollection { get; set; }
}