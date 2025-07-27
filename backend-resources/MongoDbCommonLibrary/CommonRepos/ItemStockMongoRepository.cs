using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Repository for ItemStockEntity handling MongoDB operations.
/// </summary>
public class ItemStockMongoRepository : MongoDatabaseEntityRepo<ItemStockEntity>
{
    /// <summary>
    /// Constructor for the ItemStockMongoRepository
    /// </summary>
    /// <param name="entityValidator"></param>
    /// <param name="stockCollectionProvider"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public ItemStockMongoRepository(ItemStockEntityValidator entityValidator,
                                    IWithItemStockCollection stockCollectionProvider,
                                    IOptions<MongoDatabaseEntityRepoOptions> options,
                                    ILogger<ItemStockMongoRepository> logger) 
        : base(entityValidator, stockCollectionProvider.StockCollection, options, logger)
    {



    }

    /// <summary>
    /// An object which can provide the ItemStockCollection
    /// </summary>
    public interface IWithItemStockCollection
    {
        IMongoCollection<ItemStockEntity> StockCollection { get; }
    }
}
