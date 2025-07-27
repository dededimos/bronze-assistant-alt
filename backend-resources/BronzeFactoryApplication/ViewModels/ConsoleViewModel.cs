using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class ConsoleViewModel : BaseViewModel
    {
        private readonly IDialogService dialogService;
        private readonly ICabinConstraintsRepository constraints;
        private readonly ICabinPartsListsRepository partsLists;
        private readonly IGlassOrderRepository glassOrders;
        private readonly CabinMemoryRepository repo;

        public override bool IsDisposable => false;

        public ConsoleViewModel(
            IDialogService dialogService,
            ICabinConstraintsRepository constraints,
            ICabinPartsListsRepository partsLists,
            IGlassOrderRepository glassOrders)
        {
            this.dialogService = dialogService;
            this.constraints = constraints;
            this.partsLists = partsLists;
            this.glassOrders = glassOrders;
            this.repo = new();
        }

        [RelayCommand]
        /// <summary>
        /// Logs The Names of the Currently Open/Instantiated Windows
        /// </summary>
        private void LogWindowsList()
        {
            List<string> windowsTypes = new();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType().Name is not "AdornerWindow")
                {
                    windowsTypes.Add(window.GetType().Name);
                }
            }
            Log.Information("Current Windows : {windowTypes}", windowsTypes);
        }

        [RelayCommand]
        private void OpenMessageDialog()
        {
            var result = dialogService.OpenDialog(new MessageDialogViewModel("Test Title", "Test Message"));
            Log.Information("My Result Was : {result}", result);
        }

        [RelayCommand]
        private async Task TestFunc()
        {
            await Task.Delay(1);
            MessageService.Info("This Test Button is Empty", "Empty Button");
        }

    }

}
