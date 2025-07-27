using QuestPdfLibrary;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class VariousAppHelpersViewModel : BaseViewModel
    {
        
        public VariousAppHelpersViewModel()
        {
            Title = "Application Helpers";
        }

        [RelayCommand]
        private void GenerateQuestPdf()
        {
            PdfBomList.CreateBomList();
        }

    }
}
