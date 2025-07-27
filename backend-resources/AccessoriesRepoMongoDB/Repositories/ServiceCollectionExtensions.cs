using BathAccessoriesModelsLibrary;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Repositories
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoAccessoriesDtoRepository(this IServiceCollection services,Action<MongoAccessoriesRepositoryOptions> options)
        {
            // Register the Options to the DI Container , this way the options is requested in the above Action and manipulated during declaration
            services.Configure(options);
            
            // Register the Service Implementation
            services.AddSingleton<IMongoAccessoriesDTORepository, MongoAccessoriesDTORepository>();
            return services;
        }
    }
}
