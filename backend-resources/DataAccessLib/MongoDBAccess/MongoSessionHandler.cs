using MongoDB.Driver;
using MongoDbCommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.MongoDBAccess
{
    /// <summary>
    /// A Service to retrive the Scope of a Session in order to execute Transactions in Mongo Database
    /// This way the MongoClient is not exposed inside the repositories
    /// </summary>
    public class MongoSessionHandler : IMongoSessionHandler
    {
        private readonly MongoClient client;

        public MongoSessionHandler(IMongoConnection connection)
        {
            this.client = connection.GetClient();
        }

        /// <summary>
        /// Starts a client Session
        /// Execute Operations within Session Transaction and Close afterwards
        /// </summary>
        /// <returns></returns>
        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await client.StartSessionAsync();
        }
    }
}
