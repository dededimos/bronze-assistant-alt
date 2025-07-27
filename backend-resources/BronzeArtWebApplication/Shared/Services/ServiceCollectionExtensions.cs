using Microsoft.Extensions.DependencyInjection;
using System;

namespace BronzeArtWebApplication.Shared.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiCallService(this IServiceCollection services, Action<APICallServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<APICallService>();
            return services;
        }
    }
}
