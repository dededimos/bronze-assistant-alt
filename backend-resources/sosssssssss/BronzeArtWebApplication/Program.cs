using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;
using AKSoftware.Localization.MultiLanguages;
using System.Reflection;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared.ViewModels;
using MudBlazor;
using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using BronzeRulesPricelistLibrary.Builders;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using BlazorDownloadFile;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Validators;
using SVGCabinDraws;
using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using static BathAccessoriesModelsLibrary.Services.ServiceCollectionExtensionHelpers;
using BathAccessoriesModelsLibrary.Services;
using BronzeArtWebApplication.Shared.Services.SaveToStorageServices;
using BronzeArtWebApplication.Shared.ReportModels;
using BronzeArtWebApplication.Shared.Services.OtherServices;
using BronzeArtWebApplication.Shared.Services.CacheService;
using BathAccessoriesModelsLibrary;
using Microsoft.JSInterop;

namespace BronzeArtWebApplication
{
    #region NOTES SEO IMPROVEMENT
    /*
     GENERAL NOTES FOR SEO! IMPROVEMENT
    1)Blazor can prerender content this way bots crawling the syrte that have not js enabled can render the content without it
    ----------------------------------------------
    Luckily, with Blazor you don’t have to choose between rendering HTML on the server or client. Blazor can do both! You can use Blazor WebAssembly or Blazor Server. In both flavors of Blazor, you can use pre-rendering.

    To optimize for SEO and SMO, your <body> content is the most important. 
    But to further improve and take control of how your content appears, you need to add metadata to your <head>. 
    The challenge in doing this with Blazor is that your application is running inside of the body of the document. 
    To update the head of the document, you would have to create a homebrew solution combining server-side Razor 
    and client-side JavaScript interop.
    But in the upcoming release of .NET 6, we are blessed with two new goodies to get our head 
    in order—the PageTitle and HeadContent component.
    --------------------------------------------------------------------------------------------------------------------
    a)The new PageTitle component is a Blazor component 
    that you can put anywhere inside of your Blazor application. 
    Even though the Blazor application is being rendered in the body of the document, 
    Blazor will take care of updating the title tag in the head when you use the new PageTitle component.

    You don’t necessarily need to put the PageTitle inside of your page components. 
    You can also put the PageTitle component inside of other Blazor components, 
    but that may make it harder to keep track of which component is changing the title.

    Which brings us to the following question: What happens when there are multiple PageTitle components rendered on the same page?

    @page "/counter"
    
    <PageTitle>Page Title 1</PageTitle>
    <PageTitle>Page Title 2</PageTitle>
    
    HTML
    
    Blazor will not create two title tags inside of the head of your document.
    Instead, the last PageTitle component to be rendered will be reflected as the title of your document. 
    In this case that would be “Page Title 2”.
    --------------------------------------------------------------------------------------------------------------------
    b) The new HeadContent component is similar to the PageTitle component. 
    But instead of setting the title of the document, the content of the component will be moved to the head of your document.

    You could also use the HeadContent component to set the title of the document, 
    but HeadContent will simply append the title tag without looking for any existing title to update.
    You could accidentally create multiple page titles which is invalid HTML. 

    So what happens when you render multiple HeadContent components?
    @page "/counter"

    <HeadContent>
      <meta name="description" content="Use this page to count things!" />
      <meta name="author" content="Niels Swimberghe">
      <link rel="icon" href="favicon.ico" type="image/x-icon">
    </HeadContent>
    <HeadContent>
      <meta name="description" content="This is the description from the second HeadContent" />
    </HeadContent>

    Blazor will not merge the content of the two HeadContent components, 
    but instead will use the content that is rendered last. 
    This means in the example above, the content of the second HeadContent component will be put 
    into the head of the document, and the content in the first HeadContent component is lost.
    
    Due to this behavior, it is best to keep the PageTitle and PageContent components 
    in your Blazor page components instead of descendant components.
    --------------------------------------------------------------------------------------------------------------------
    c)There is a third component that hasn’t been mentioned yet. 
    The HeadOutlet component is the component responsible for outputting the title 
    from the PageTitle component and the content from the HeadContent component.

    If you’re using the updated Blazor Wasm template from .NET 6, 
    you’re already using this component because it is pre-configured for you in the Program.cs file. (I DID NOT , HAD TO ADD THIS MANUALLY)

    This is the default for the Blazor Wasm template, which renders the Blazor app and HeadOutlet on the client and also depends on JavaScript. 
    As discussed earlier, many search engine bots and social media bots will not execute your JavaScript. 
    As a result, the bots will not see the result of your client-side App or HeadOutlet component.

    It will take some extra work to modify your Blazor Wasm application, but you can follow this guide from the ASP.NET Docs to prerender your app: 
    Prerender and integrate ASP.NET Core Razor components.
     */
    #endregion
    #region NOTES Azure Function , CosmosDB
    /*1.To Run Locally a local.settings.json must be created with the following values
     {
        "IsEncrypted": false,
        "Values": {
                    "AzureWebJobsStorage": "ENDPOINTFORSTORAGEACCOUNT HERE ",
                    "FUNCTIONS_WORKER_RUNTIME": "dotnet",

    //This should be in azure app Settings so that the Function will know what is the Connection String for CosmosDB
    "DBConnectionString": "AccountEndpoint=https://bronze-cosmos-db.documents.azure.com:443/;AccountKey=DBFH0YgSo8WWhf8awfmhEdXPLKBHAjjWfnyQKrl6UfZ8eqIox8T6UumMFE8OWa3fb0efCcZrCm2MR9z68p84dw==;"
  },

  //So that the function can Run Locally!
  //These are not needed in the Azure AppSettings as Azure takes care of cors by itself
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*"
  }
}
     */
    #endregion
    public class Program
    {
        public const string AuthedUserClient = "AuthedClient";
        public const string AnonymousUserClient = "AnonymousClient";
        

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
            //When the ::after pseudo-selector is specified,
            //the contents of the root component are appended to the existing head contents
            //instead of replacing the content.(When using "HeadContent" Component)
            //This allows the app to retain static head content in wwwroot/index.html
            //without having to repeat the content in the app's Razor components.
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            //*** DEPRECATED DECLARATION *** With this way the Access Tokens are not Automatically being inserted in the http Headers ...
            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            if (builder.HostEnvironment.IsDevelopment())
            {
                Console.WriteLine($"Current Environment is {builder.HostEnvironment.Environment}");

                // Add the Http Client with a Custom Authorization Message Handler , this way 
                // During Development the Client Adds to the Http Headers the Bearer Property Containing the Access Token
                // Otherwise calls at a different address wont Have the Access token Included.
                builder.Services.AddTransient<CustomAuthorizationMessageHandler>();
                builder.Services.AddHttpClient(AuthedUserClient, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                                .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

                //********   Deprecated Declarations for Functions   ********//********   Deprecated Declarations for Functions   ********
                //For the AzureFunction to work locally we have to change base URI to http://localhost:7071/ ,
                //and Start Both Projects BronzeArtWebApplication & FunctionAPI
                //this way both Functions and App Work on the Same Machine for Testing
                //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7071/") });
                //Console.WriteLine($"Http Base Address Started in Port : http://localhost:7071/");
                //********   Deprecated Declarations for Functions   ********//********   Deprecated Declarations for Functions   ********
            }
            else
            {
                //Add HttpClient Factory for Named Http Clients , consumers of http clients need to inject HttpClineFactory and get the required Http Client
                //Attach the Base Authorization Message Handler which attached Tokens to the Headers of each Request made with the desginated client . ONLY for Requests made to the same BASE ADDRESS  (otherwise a custom handler is needed => see above in Development block)
                //Only when user is Authed Otherwise it redirects to Login as when it does not find a token there is no call to the API a MissingToken Exception is thrown.
                builder.Services.AddHttpClient(AuthedUserClient, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
                //****** not sure about the corectness of the below statements ******
                // Though for the Tokens to be Included an API Scope must be defined , AND The tokens will be ADDED ONLY TO CALLS on the BASE Adress otherwise we have to make a custom message handler
                // Otherwise the Library does not ADD any Access tokens , and will only get the Id Token from the Authentication.
            }
            //The Anonymous Client Used only for Anonymous Calls to the API
            builder.Services.AddHttpClient(AnonymousUserClient, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddScoped<JSCallService>();

            // Service for the Authentication through AAD B2C 
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                
                // as per MSDocs
                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                // this is the API Scope
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://bronzeapp.onmicrosoft.com/BronzeApi/Api.Read");
                
                // I am not sure what this was ... for some reason i used it in the past , but now it conflicts with the scopes needed for API Access
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");

                // Stores Auth State in A Cookie so User can Stay Logged in During Sessions
                options.ProviderOptions.Cache.CacheLocation = "sessionStorage";
                

                //The framework defaults to pop-up login mode and falls back to redirect login mode if a pop-up can't be opened.
                //Configure MSAL to use redirect login mode by setting the LoginMode property of MsalProviderOptions to redirect:
                options.ProviderOptions.LoginMode = "redirect";
                
                //Adds Roles to a User Through this Claim (YES IT WORKS , Treats a Certain Claim as being the Users Role)
                options.UserOptions.RoleClaim = "extension_BronzeRole";
            });
            builder.Services.AddCascadingAuthenticationState();

            //Local Storage Blazored NugetPack. Service
            builder.Services.AddBlazoredLocalStorage(config =>
                    config.JsonSerializerOptions.WriteIndented = true
            );

            // Download File Service -- to Download Clientside Generated Files
            builder.Services.AddBlazorDownloadFile();

            // Service for the Translations required YML files to Support multiple Languages
            builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly()); //Allows Localization (MultiLanguage Package with .yaml files)


            //The User Service (Needs to be first so it can get injected to the ViewModels Afterwards) -- Pass the authentication State Provider (The User Service Stores Claims -- Pass the URI Base Address to check for Client Localized Options)
            builder.Services.AddScoped(sp=> new BronzeUser(sp.GetService<AuthenticationStateProvider>(),builder.HostEnvironment.BaseAddress));

            //Add Cabins Repository and Factory
            builder.Services.AddScoped<ICabinMemoryRepository,CabinMemoryRepository>();
            builder.Services.AddScoped<CabinFactory>();
            builder.Services.AddScoped<GlassesBuilderDirector>();
            builder.Services.AddScoped<CabinValidator>();
            builder.Services.AddScoped<SynthesisDrawFactory>();

            builder.Services.AddScoped<CabinStringFactory>();

            // Add the Bronze Products Builder along with the Description/Photos Options
            
            builder.Services.AddScoped(typeof(BronzeItemsPriceBuilderOptions), (s) =>
            {
                return PriceBuilderOptionsInitilizer.InitilizeBuilderOptions();
            });
            builder.Services.AddScoped<BronzeItemsPriceBuilder>();

            // Adds the ViewModel for the Mirror Being Assembled (The Viewmodel has a Dependency on Language Container)
            builder.Services.AddScoped<AssembleMirrorViewModel>();
            // Add the MirrorDialogs Navigator Service
            builder.Services.AddScoped<MirrorDialogNavigator>();
            builder.Services.AddScoped<AssembleCabinViewModel>();

            //Add the Service which makes the Calls to the APIs
            builder.Services.AddApiCallService((options) =>
            {
                if (builder.HostEnvironment.IsDevelopment())
                {
                    // When in development calls to the API are made through the local port
                    options.BronzeApiBaseUrl = "https://localhost:7065/api";
                    options.IsInDevelopment = builder.HostEnvironment.IsDevelopment();
                }
                else
                {
                    // when in release the calls are made through the Base Adress directly to the /api
                    // WITHOUT ANY OTHER PREFIXES as the NAME Identifier etc . the System inserts it by itself
                    // though i guess routes between the Various APIs registered for the App should be unique otherwise there will certainly be a conflict
                    // or a policy needs to be added to change the calls to the needed.
                    options.BronzeApiBaseUrl = "/api";
                }
                options.BronzeApiGetAccessoriesStashCall = $"{options.BronzeApiBaseUrl}/Accessories/GetAccessoriesDtoStash";
                options.BronzeApiGetDownloadUri = $"{options.BronzeApiBaseUrl}/Blobs/GetBlobSasDownloadUri";
                options.LanguageParameterName = "lng";
                options.BlobNameParameterName = "blobName";
                options.RegisteredMachineParameterName = "rm";
            });
            // The Memory Repository for the Accessories
            builder.Services.AddScoped<IAccessoriesMemoryRepository, AccessoriesMemoryRepository>();
            builder.Services.AddAccessoriesUrlHelper((options) =>
            {
                options.ThumbnailSuffix = "-Thumb";
                options.SmallPhotoSuffix = "-Small";
                options.MediumPhotoSuffix = "-Medium";
                options.LargePhotoSuffix = "-Large";
                options.ContainerAddress = "https://storagebronze.blob.core.windows.net/accessories-images/";
            });

            //Mud Blazor Services for all the Components
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopLeft;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;//10sec
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                config.SnackbarConfiguration.MaxDisplayedSnackbars = 6;
            });

            builder.Services.AddScoped<AccessoriesPageViewModel>();
            builder.Services.AddScoped<BasketViewModel>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<BasketSaveService>();

            builder.Services.AddScoped<BasketItemWholesaleQuoteTemplate>();
            builder.Services.AddScoped<BasketItemRetailQuoteTemplate>();
            builder.Services.AddScoped<BasketItemGuestQuoteTemplate>();
            builder.Services.AddScoped<AccessoryPriceableReportTemplate>();

            //Weather the Application is Busy
            builder.Services.AddScoped<BusyStateService>();

            //Add the cache collection object for bathroom accessories
            builder.Services.AddScoped<ICacheCollection<BathroomAccessory>, AccessoriesCacheCollection>();

            //Add the Cache Service
            builder.Services.AddScoped<BronzeCacheService>(provider =>
            {
                var js = provider.GetRequiredService<IJSRuntime>();
                var localStorage = provider.GetRequiredService<ILocalStorageService>();
                List<ICacheCollection> cacheCollections = 
                [
                    provider.GetRequiredService<ICacheCollection<BathroomAccessory>>()
                ];
                return new BronzeCacheService(js, localStorage, cacheCollections);
            });


            //builder.Services.BuildServiceProvider()
            //    .GetRequiredService<ILanguageContainerService>()
            //    .SetLanguage(System.Globalization.CultureInfo.GetCultureInfo("en-US"));

            await builder.Build().RunAsync();
        }
    }
}
