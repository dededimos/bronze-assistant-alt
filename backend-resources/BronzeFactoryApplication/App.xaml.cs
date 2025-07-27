using AccessoriesRepoMongoDB;
using AccessoriesRepoMongoDB.Repositories;
using AzureBlobStorageLibrary;
using BronzeFactoryApplication.ApplicationServices.AuthenticationServices;
using BronzeFactoryApplication.ApplicationServices.JsonXmlSerilizers;
using BronzeFactoryApplication.ApplicationServices.LabelingAccessoriesConversions;
using BronzeFactoryApplication.ApplicationServices.NumberingServices;
using BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using BronzeFactoryApplication.ViewModels.DrawingsViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels;
using BronzeFactoryApplication.ViewModels.SettingsViewModels;
using DrawingLibrary;
using DrawingLibrary.Models.PresentationOptions;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using MirrorsLib.DrawingElements;
using MirrorsLib.Helpers;
using MirrorsLib.Services;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsRepositoryMongoDB.Repositories;
using MongoDbCommonLibrary;
using QuestPDF.Infrastructure;
using ShapesLibrary.Validators;
using System.Windows.Controls;
using static BathAccessoriesModelsLibrary.Services.ServiceCollectionExtensionHelpers;

namespace BronzeFactoryApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // From Nugets Microsoft.Extensions.DependencyInjection , Microsoft.Extensions.Hosting
        // This is the Host that implements the Dependency Injection , has to have these two nuget Packages .
        public static IHost? AppHost { get; private set; } 

#nullable disable
        public static ImagesMappings ImagesMap { get; private set; }
        public static BlobUrlHelper BlobHelper { get; private set; }
#nullable enable

        private readonly RichTextBox richTextBoxConsole = new(); //The Richtextbox that acts as a Console . Must be Declared here for the DI to Work
        
        public string SelectedLanguage { get; private set; } = "el-GR";
        public string SelectedTheme { get; private set; } = "Dark";
        public string ApplicationVersion { get; private set; } = "Version 4.04 - 060425(Wharehouse)";
        
        public static readonly string ApplicationDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create),"BronzeFactoryApplication");
        public static readonly string LocalDatabaseFolderPath = Path.Combine(ApplicationDataFolderPath, "BronzeSettings");
        public static readonly string LogsFolderPath = Path.Combine(ApplicationDataFolderPath, "Logs");
        public static readonly string CachedImagesPath = Path.Combine(ApplicationDataFolderPath, "CachedImages");

        public App()
        {
            //Cache Images here (NUGET Cached Image)
            CachedImage.Core.FileCache.AppCacheMode = CachedImage.Core.FileCache.CacheMode.Dedicated;
            CachedImage.Core.FileCache.AppCacheDirectory = CachedImagesPath;
            
            //t
            //ConfigHelper.Instance.SetLang("en"); //(For Handy Control)
            //Create the Default Builder where we can Ask for our Services
            AppHost = Host.CreateDefaultBuilder()
                //Serilog Configuration
                .UseSerilog((host, loggerConfiguration) =>
                {
                    loggerConfiguration
                    .WriteTo.Logger(lc => lc.WriteTo.File(
                        LogsFolderPath, 
                        rollingInterval: RollingInterval.Day, 
                        rollOnFileSizeLimit: true, 
                        fileSizeLimitBytes: 100000)
                                            .MinimumLevel.Error())
                    .WriteTo.Logger(lc => lc.WriteTo.RichTextBox(richTextBoxConsole, theme: RichTextBoxConsoleTheme.Colored, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                                            .MinimumLevel.Verbose())
                    .WriteTo.Debug()
                    .MinimumLevel.Verbose();
                })
                .ConfigureServices((hostContext, services) => 
                {
                    
                    // Stops the Host Auto Messages to Console (This way they are not Shown in the RichTextBox)
                    services.Configure<ConsoleLifetimeOptions>(o => o.SuppressStatusMessages = true);
                    //The Non Default implementation is NOT Thread Safe!!!
                    services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
                    //A Dialog Service with Result (Opens new Window and not InWindow Dialogs)
                    services.AddSingleton<IDialogService>(new DialogService());
                    //A Store Service Mapping Images to their Paths (Used in Converters , this service is passed as a shadow static service in the App Class)
                    services.AddSingleton<ImagesMappings>();
                    //The Service that Initilizes/Refreshes the Memory Repository and PartsSetsApplicator Service
                    services.AddSingleton<AppDataInitilizer>();
                    //The Main Window (Application container)
                    services.AddSingleton<MainWindow>();
                    //The ViewModel of the Main Window and the Main ViewModel of the Application
                    services.AddSingleton<MainViewModel>();
                    //The ViewModel of the Console Window
                    services.AddSingleton<ConsoleViewModel>();
                    //The Logging Console Window
                    services.AddSingleton(x=> new LoggingConsole(richTextBoxConsole,x.GetService<ConsoleViewModel>()!));
                    //The ViewModel for a Search Filter (Can Handle ANY Object and will apply filters based on its properties through expressions and Reflection)
                    services.AddTransientAndFactory<FilterViewModel>();
                    services.AddSingleton<OperationProgressViewModel>();

                    services.AddSQLiteSettingsServices(LocalDatabaseFolderPath);
                    services.AddAllCabinRelatedServices();
                    services.AddCabinsViewModels();
                    services.AddMongoDbRepos();
                    services.AddMainPagesViewModels();
                    services.AddNavigationServices();
                    services.AddInWindowModals();
                    services.AddAllAccessoriesServices();

                    //To Create the Labeling Database
                    services.AddSingleton(new AccessoriesLabelsDbContextFactory($"Data Source={LocalDatabaseFolderPath}\\Accessories.db"));
                    services.AddSingleton<LabelingAccessoriesBuilder>();

                    // The Repository of the Blob Storage Unit
                    services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();
                    // A Helper to Construct Urls for the Blob Files (all Paths saved to DB are relative , do not contain Main URL of the blob that can change)
                    services.AddSingleton<BlobUrlHelper>();

                    //The Service Responsible for Bringing Orders from the Galaxy API
                    services.AddSingleton<GalaxyOrdersImportService>();
                    
                    //Add the Converter Method for MIN classes used in JSON and XML Serilization
                    services.AddSingleton<AccessoriesMinConverter>();

                    //Add the Url Helperfor the Photos of AccessoryMin
                    services.AddAccessoriesUrlHelper((options) =>
                    {
                        options.ThumbnailSuffix = "-Thumb";
                        options.SmallPhotoSuffix = "-Small";
                        options.MediumPhotoSuffix = "-Medium";
                        options.LargePhotoSuffix = "-Large";
                        options.ContainerAddress = "https://storagebronze.blob.core.windows.net/accessories-images/";
                    }, ServiceLifetime.Singleton);

                    // Build the IPublicClientApplication for Authentication
                    services.AddSingleton<IPublicClientApplication>(provider =>
                         PublicClientApplicationBuilder.Create(hostContext.Configuration["Authentication:AzureADB2CClientId"])
                        .WithRedirectUri(hostContext.Configuration["Authentication:RedirectUri"])
                        .Build());

                    //Get The Default Scopes from Configuration
                    string[] defaultScopes = hostContext.Configuration.GetSection("Authentication:DefaultScopes").GetChildren().Select(c => c.Value ?? "User.Read").ToArray() ?? new[] {"User.Read" };
                    // Add the Authentication Service 
                    services.AddSingleton<IAuthenticationServiceCustom , AuthenticationServiceCustom>(s =>
                    new AuthenticationServiceCustom(s.GetRequiredService<IPublicClientApplication>(), defaultScopes));

                    // Add the GraphAuth Provider
                    services.AddSingleton<IAuthenticationProvider,GraphAuthProvider>();
                    // Add Graph Client
                    services.AddSingleton<GraphServiceClient>(s=> new(s.GetRequiredService<IAuthenticationProvider>()));
                    // Add the Users Repository
                    services.AddSingleton<GraphUsersRepository>();

                    //Adds the ViewModels of MirrorShapes
                    services.AddMirrorsViewModelFactories();
                    services.AddEditModelInWindowModals();
                    services.AddMirrorsDataSpecificServices();

                    //The Builder of Codes for Mirrors
                    services.AddSingleton<MirrorCodesBuilder>();

                    //Add the Mirrors Orders ViewModels e.t.c.
                    services.AddMirrorOrdersSepecificServices();

                    services.AddSingleton<ShapeInfoValidator>();

                    services.AddSingleton<MirrorsModuleViewModel>();
                    services.AddSingleton<MirrorsEntitiesManagmentViewModel>();
                    services.AddSingleton<MirrorCustomizationAnalyzer>();
                    services.AddSingleton<MirrorCodesParser>();
                    services.AddSingleton<MirrorCollisionsResolver>();

                    services.AddNewDrawingServices();

                    //Just a Wording for the Logging Console
                    IsDevelopment = hostContext.HostingEnvironment.IsDevelopment();
                }).Build();
            
            //Help with DI or NOT Configuration of Serilog Below 
            #region How to Configure Serilog to Work with Dependency Injection and Or Microsoft ILoggers
            ////WITHOUT DI -- Configure a static Application Wide Logger (The Logger must Log.CloseAndFlush(); when application Closes!)
            //LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            //    //Configure a Sub Logger that writes to a File ONLY Errors (The Sub Logger cannot have a minimum Level Greater than that of the Main Logger)
            //    .WriteTo.Logger(lc => lc.WriteTo.File("Logs/ErrorLog.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
            //                            .MinimumLevel.Error())
            //    //Configure the main Logger that Logs to Debug all Logs
            //    .WriteTo.Debug()
            //    .MinimumLevel.Verbose();

            ////Create the Logger
            //Log.Logger = loggerConfiguration.CreateLogger();

            //Serilog and Serilog.AspNetCore and Microsoft.Extensions.Logging Nugers , To Use with the IHost Interface
            //BELOW IS THE CONFIGURATION IF WE NEED TO USE Dependency Injection in Components (But we will use an ApplicationWide Logger)
            //.UseSerilog((host, loggerConfiguration) =>
            //{
            //    loggerConfiguration
            //    .WriteTo.Logger(lc=>
            //        lc.WriteTo.File("Logs/ErrorLog.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
            //        .MinimumLevel.Error())
            //    .WriteTo.Debug()
            //    .MinimumLevel.Verbose();
            //})
            //For the Application Instance we Have to Create another Logger because we cannot inject it as the injection happens here:
            //So we create one with the Logger Factory . Blessed Tutorial at : https://www.youtube.com/watch?v=SfTdUNuApYc
            //BELOW IS THE CONFIGURATION TO SET A LOGGER FOR THE APP instance that is actually a Microsoft.Extensions.ILogger instead of Serilog
            //ILoggerFactory loggerFactory = LoggerFactory.Create(builder => 
            //{
            //    LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            //    .WriteTo.Logger(lc =>
            //         lc.WriteTo.File("Logs/ErrorLog.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000)
            //         .MinimumLevel.Error())
            //     .WriteTo.Debug()
            //     .MinimumLevel.Verbose();
            //    builder.AddSerilog(loggerConfiguration.CreateLogger());
            //});
            //appLogger = loggerFactory.CreateLogger<App>();
            #endregion
        }
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// Start the Application from here
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnStartup(StartupEventArgs e)
        {

            //The Exclamation mark just states that the AppHost will never be null , so that we do not get a warning
            await AppHost!.StartAsync();

            //Create Settings Database if Does not Exist (MUST ALSO CREATE FOLDER IF NOT THERE)
            using (var context = AppHost.Services.GetRequiredService<SettingsDbContextFactory>().CreateDbContext())
            {
                if (!Directory.Exists(LocalDatabaseFolderPath))
                {
                    Directory.CreateDirectory(LocalDatabaseFolderPath);
                }
                context.Database.Migrate();
            }
            //Create Settings Database if Does not Exist (MUST ALSO CREATE FOLDER IF NOT THERE)
            using (var context = AppHost.Services.GetRequiredService<AccessoriesLabelsDbContextFactory>().CreateDbContext())
            {
                if (!Directory.Exists(LocalDatabaseFolderPath))
                {
                    Directory.CreateDirectory(LocalDatabaseFolderPath);
                }
                context.Database.Migrate();
            }
            
            Log.Information("Initilized Settings Database at :{extraLine}{path}", Environment.NewLine, LocalDatabaseFolderPath);
            Log.Information("Initilized Accessories Labels Database at :{extraLine}{path}", Environment.NewLine, LocalDatabaseFolderPath);
            Log.Information("Initilized Logs at :{exraLine}{path}", Environment.NewLine, LogsFolderPath);

            //Assign Quest Pdf License
            QuestPDF.Settings.License = LicenseType.Community;
            Log.Information("Quest PDF License Set...");
            QuestPDF.Settings.EnableCaching = true;

            //Set ImagesMappings
            ImagesMap = AppHost.Services.GetRequiredService<ImagesMappings>();
            Log.Information("Images Paths set...");
            BlobHelper = AppHost.Services.GetRequiredService<BlobUrlHelper>();
            Log.Information("Blob Url Helper set...");

            //Configure User Settings
            var settingsConfig = AppHost.Services.GetRequiredService<ISettingsConfigurator>();
            await settingsConfig.ConfigureUserSettings();
            Log.Information("User Settings Configured...");

            var setProvider = AppHost.Services.GetRequiredService<IGeneralSettingsProvider>();
            SelectedLanguage = await setProvider.GetSelectedLanguage();
            SelectedTheme = await setProvider.GetSelectedTheme();
            //Deprecated Setting Needs to be in App not Db...
            //ApplicationVersion = await setProvider.GetApplicationVersion();

            Log.Information("{appVersion} :",ApplicationVersion);
            Log.Information("Selected Language :{language} , Selected Theme :{theme}", SelectedLanguage, SelectedTheme);

            //Fix Global Drawing Presentation Options
            if (SelectedTheme == "Dark") DrawingPresentationOptionsGlobal.IsDarkTheme = true;
            else DrawingPresentationOptionsGlobal.IsDarkTheme = false;

            //Start the Stock Service
            var stockService = AppHost.Services.GetRequiredService<GlassesStockService>();
            await stockService.InitilizeService();
            Log.Information("Glasses Stock Service Initilized...");

            //Set Starting View to Menu
            var navigator = AppHost.Services.GetRequiredService<MainMenuNavigationService>();
            await navigator.NavigateAsync();

            //The Starting point is the first Form , The Main Window Here.
            MainWindow startupWindow = AppHost.Services.GetRequiredService<MainWindow>();

            startupWindow.Show();
            
            Log.Information("Application Started...");
            Log.Information("Application State : {mode}", IsDevelopment ? "DevelpomentMode" : "ReleaseMode");

            var progressVm = AppHost.Services.GetRequiredService<OperationProgressViewModel>();
            
            Progress<string> progress = new(msg => progressVm.SetNewOperation(msg));
            MainViewModel mainVm = AppHost.Services.GetRequiredService<MainViewModel>();
            mainVm.IsBusy = true;
            
            try
            {
                await Task.Run(()=>LongRunningInitilization(progress));
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally 
            {
                mainVm.IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }

            base.OnStartup(e);
        }

        private static async Task LongRunningInitilization(Progress<string> progress)
        {
            AppDataInitilizer appDataService = AppHost!.Services.GetRequiredService<AppDataInitilizer>();
            await appDataService.InitService();
            var mirrorsRepo = AppHost!.Services.GetRequiredService<IMongoMirrorsRepository>();
            await mirrorsRepo.BuildCachesAsync();
            Log.Information("Mirrors Repository Cache Built...");
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                Log.Information("Closing...");
                //Disposes the Static Logger Instance
                Log.CloseAndFlush();

                //To Dispose this !
                await AppHost!.StopAsync();
                AppHost?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An Error Occured on Application Close:{Environment.NewLine}{Environment.NewLine}{ex.Message}");
            }
            finally { base.OnExit(e); }
        }

        private int exceptionsCounter = 0;
        private bool isHandlingException = false;
        /// <summary>
        /// Handles all the Unhandled Exceptions of the Application and Logs the Errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (isHandlingException) return;
            isHandlingException = true;
#if DEBUG
            exceptionsCounter++;
            Log.Error(e.Exception, "Unhandled Exception - {message}",e.Exception.Message);
            MessageBox.Show("An Unhandled Exception has Occured - Check Logs For More", "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            if (exceptionsCounter < 20)
            {
                return;
            }
            else
            {
                Application.Current.Shutdown();
            }

#else
            MessageService.LogAndDisplayException(e.Exception);
            e.Handled = true;            
#endif
            isHandlingException = false;
        }
    }
}
