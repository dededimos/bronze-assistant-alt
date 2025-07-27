using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.Properties;
using CommonHelpers;
using DataAccessLib;
using DataAccessLib.NoSQLModels;
using Microsoft.Extensions.Logging;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.DataService
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDataInitilizer
    {
        private readonly ICabinMemoryRepository memoryRepository;
        private readonly PartSetsApplicatorService partSetsApplicatorService;
        private readonly ILogger<AppDataInitilizer> logger;

        //[ObservableProperty]
        //private ObservableCollection<string> failureMessages = new();

        public bool IsMemoryRepoInitilized { get; private set; }
        public bool IsPartSetsApplicatorInitilized { get; private set; }
        public bool HasFullyInitilized
        {
            get => IsMemoryRepoInitilized && IsPartSetsApplicatorInitilized;
        }

        public AppDataInitilizer(
            ICabinMemoryRepository memoryRepository,
            PartSetsApplicatorService partSetsApplicatorService,
            ILogger<AppDataInitilizer> logger)
        {
            this.memoryRepository = memoryRepository;
            this.partSetsApplicatorService = partSetsApplicatorService;
            this.logger = logger;
        }

        /// <summary>
        /// Initilizes the Data For The Service
        /// </summary>
        /// <returns></returns>
        public async Task InitService()
        {
            await InitMemoryRepository();
            await InitPartSetsApplicatorService();
        }
        public async Task InitMemoryRepository()
        {
            string selectedLanguage = ((App)App.Current).SelectedLanguage;
            OperationResult operation = await memoryRepository.InitilizeRepo(selectedLanguage);
            if (operation.IsSuccessful is false)
            {
                throw new Exception($"{operation.FailureMessage},{Environment.NewLine}{operation.Exception?.Message}", operation.Exception);
            }
            else
            {
                logger.LogInformation("Intilized Memory Repo...");
                IsMemoryRepoInitilized = true;
            }
        }
        public async Task InitPartSetsApplicatorService()
        {
            try
            {
                await Task.Run(() => partSetsApplicatorService.InitilizeService());
                IsPartSetsApplicatorInitilized = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error Initialization of Parts Set Applicator Task");
                throw new Exception($"PartsSets Applicator Initilization Failed with message:{Environment.NewLine}{ex.Message}", ex);
            }
        }
    }
}
