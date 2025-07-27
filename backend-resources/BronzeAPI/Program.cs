using AccessoriesRepoMongoDB;
using AccessoriesRepoMongoDB.Repositories;
using DataAccessLib;
using DataAccessLib.MongoDBAccess;
using DataAccessLib.MongoDBAccess.RepoImplementations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDbCommonLibrary;
using BathAccessoriesModelsLibrary.Services;
using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using BronzeAPI.Helpers;
using UsersRepoMongoDb;
using Scalar.AspNetCore;
using MongoDbCommonLibrary.CommonValidators;

namespace BronzeAPI
{
    /*
     INFORMATION : 
    This API is connected with the API Managment of Azure 
    to Form Requests from the Api Managment to this Api :
    add ? before the query parameters start , add & for any subsequent parameter , add the subscription key in the end
    The Subscription key is declared in the Azure API Managment for all or specific APIs of the APIMANAGMENT SERVICE
    The 'bronzeapi' name is also declared in the APIMANAGMENT in the Settings of the BronzeApi , and this acts as an identifier to distinguish each api used in the API Managment from the others
    after that the query is as it would have been done to the API itself
    example: https://bronzeapimanagment.azure-api.net/bronzeapi/api/Accessories/GetTraitsClasses?isoFourLetterIdentifier=el-GR&subscription-key=SUBSCRIPTION_KEY_VALUE
     */
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string allowBronzeWebAppPolicy = "AllowBronzeWebApplication";
            // To Allow in Development the Blazor Application to Communicate with the API
            // Cors is needed as follows
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(allowBronzeWebAppPolicy, builder =>
                {
                    // Construct all the Allowed ports that the Blazor App is running to during Development
                    var allowedOriginsHttp = new[] { 28041, 44341, 5000, 5001 }.Select(port => $"http://localhost:{port}").ToArray();
                    var allowedOriginsHttps = new[] { 28041, 44341, 5000, 5001 }.Select(port => $"https://localhost:{port}").ToArray();
                    // Configure the API to allow from these origins all headers and all methods
                    builder.WithOrigins(allowedOriginsHttp.Concat(allowedOriginsHttps).ToArray()).AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddMicrosoftIdentityWebApi(jwtOptions =>
                            {
                                builder.Configuration.Bind("AzureAdB2C", jwtOptions);
                                jwtOptions.TokenValidationParameters.RoleClaimType = "extension_BronzeRole";//This is the Claim Property Used to Identify The Role of the User
                            },
                            micIdentOptions =>
                            {
                                builder.Configuration.Bind("AzureAdB2C", micIdentOptions);
                            });

            //MongoDbDependencies Universal
            builder.Services.AddSingleton<IMongoConnection, MongoConnectionDefault>();

            //MongoDB Dependencies (Cabins)
            builder.Services.AddSingleton<IMongoDbCabinsConnection, MongoDbCabinsConnection>();
            builder.Services.AddSingleton<IMongoSessionHandler, MongoSessionHandler>();
            builder.Services.AddSingleton<ICabinPartsRepository, MongoCabinPartsRepository>();
            builder.Services.AddSingleton<ICabinConstraintsRepository, MongoCabinConstraintsRepository>();
            builder.Services.AddSingleton<ICabinPartsListsRepository, MongoCabinPartsListsRepository>();
            builder.Services.AddSingleton<ICabinSettingsRepository, MongoCabinSettingsRepository>();
            builder.Services.AddSingleton<IGlassesStockRepository, MongoGlassesStockRepository>();
            builder.Services.AddSingleton<IGlassOrderRepository, MongoGlassOrderRepository>();

            //MongoDB Dependencies (Accessories)
            builder.Services.AddSingleton<IMongoDbAccessoriesConnection, MongoDbAccessoriesConnection>();
            builder.Services.AddSingleton<IAccessoryEntitiesRepository, MongoAccessoryEntitiesRepository>();
            builder.Services.AddSingleton<ITraitClassEntitiesRepository, MongoTraitClassEntitiesRepository>();
            builder.Services.AddSingleton<ITraitEntitiesRepository,MongoTraitEntitiesRepository>();
            builder.Services.AddSingleton<ITraitGroupEntitiesRepository, MongoTraitGroupEntitiesRepository>();

            // Add the User Options Repository
            builder.Services.AddSingleton<UserAccessoriesOptionsEntityValidator>();
            builder.Services.AddSingleton<UserAccessoriesOptionsRepository>();

            builder.Services.AddSingleton<CustomPriceRuleEntityValidator>();
            builder.Services.AddSingleton<MongoPriceRuleEntityRepo>();
            builder.Services.AddSingleton<UserInfoEntityValidator>();
            builder.Services.AddSingleton<UsersRepositoryMongo>(sp =>
            {
                var validator = sp.GetRequiredService<UserInfoEntityValidator>();
                var accConnection = sp.GetRequiredService<IMongoDbAccessoriesConnection>();
                var logger = sp.GetRequiredService<ILogger<MongoEntitiesRepository<UserInfoEntity>>>();
                return new UsersRepositoryMongo(validator, accConnection, logger);
            });

            builder.Services.AddSingleton<ItemStockMongoRepository>(provider =>
            {
                ItemStockEntityValidator validator = new(true);
                var logger = provider.GetRequiredService<ILogger<ItemStockMongoRepository>>();
                var mongoConnection = provider.GetRequiredService<IMongoDbAccessoriesConnection>();
                return new ItemStockMongoRepository(validator, mongoConnection, Microsoft.Extensions.Options.Options.Create(new MongoDatabaseEntityRepoOptions()), logger);
            });

            builder.Services.AddMongoAccessoriesDtoRepository(o =>
            {
                o.IsCachingEnabled = true;
                o.CacheExpirationRelativeToLastCache = TimeSpan.FromMinutes(15);
            });
            // The Helper to Transform Urls to the Desired Container Address
            builder.Services.AddAccessoriesUrlHelper((options) =>
            {
                options.ThumbnailSuffix = "-Thumb";
                options.SmallPhotoSuffix = "-Small";
                options.MediumPhotoSuffix = "-Medium";
                options.LargePhotoSuffix = "-Large";
                options.ContainerAddress = "https://storagebronze.blob.core.windows.net/accessories-images/";
            },ServiceLifetime.Singleton);


            builder.Services.AddControllers(options =>
            {
                //Adds Global Exception Handling with the below Custom Class
                options.Filters.Add(new GlobalExceptionHandlingAttribute());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Not Valid anymore for .NET9
            //builder.Services.AddSwaggerGen(options => 
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BronzeAPI", Version = "v1" });
            //});
            //Instead : 
            builder.Services.AddOpenApi();


            var app = builder.Build();
            
            //Disable the cahcing mechanism of Entities Repository the API Uses its Own
            var accEntitiesRepo = app.Services.GetRequiredService<IAccessoryEntitiesRepository>();
            accEntitiesRepo.SetCaching(false);
            var optionsRepo = app.Services.GetRequiredService<UserAccessoriesOptionsRepository>();
            optionsRepo.SetCaching(false);
            var priceRulesRepo = app.Services.GetRequiredService<MongoPriceRuleEntityRepo>();
            priceRulesRepo.SetCaching(false);
            var usersRepo = app.Services.GetRequiredService<UsersRepositoryMongo>();
            usersRepo.SetCaching(false);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); //Use this instead of use Swagger (makes a json file that maps the API)
                app.MapScalarApiReference(); //the new swagger
                //Not Valid from.NET9
                //app.UseSwagger(); 
                //app.UseSwaggerUI(options =>
                //{
                //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BronzeAPI v1");
                //});

                // Apply the CORS policy only in Development for the Blazor App
                app.UseCors(allowBronzeWebAppPolicy);
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}