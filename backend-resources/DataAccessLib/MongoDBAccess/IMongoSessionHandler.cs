using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.MongoDBAccess
{
    public interface IMongoSessionHandler
    {
        /// <summary>
        /// Starts a client Session
        /// </summary>
        /// <returns></returns>
        public Task<IClientSessionHandle> StartSessionAsync();
    }
}
