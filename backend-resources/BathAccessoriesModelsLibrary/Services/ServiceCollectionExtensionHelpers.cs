using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary.Services
{
    public static class ServiceCollectionExtensionHelpers
    {
        public static IServiceCollection AddAccessoriesUrlHelper(this IServiceCollection services, Action<AccessoriesUrlHelperOptions> options, ServiceLifetime lifeTime = ServiceLifetime.Scoped)
        {
            services.Configure(options);
            if (lifeTime == ServiceLifetime.Scoped)
            {
                services.AddScoped<AccessoriesUrlHelper>();
            }
            else
            {
                services.AddSingleton<AccessoriesUrlHelper>();
            }
            return services;
        }
    }
}
