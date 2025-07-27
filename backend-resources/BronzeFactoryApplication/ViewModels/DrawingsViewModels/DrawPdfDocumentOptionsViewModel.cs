using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Spreadsheet;
using DrawingLibrary.Models.PresentationOptions;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DrawingLibraryPDFTemplates.DrawPdfDocument;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DrawPdfDocumentOptionsViewModel : BaseViewModel, IEditorViewModel<DrawPdfDocumentOptions>
    {
        public DrawPdfDocumentOptionsViewModel()
        {
            SetModel(GetDefaultOptions());
        }
        private static readonly DrawInfoTableOptions.TableTitles defaultTitles = GetDefaultTableTitles();
        
        [ObservableProperty]
        private float pageWidth;
        [ObservableProperty]
        private float pageHeight;
        [ObservableProperty]
        private float pageMargin;
        [ObservableProperty]
        private bool scaleDrawToFit;
        [ObservableProperty]
        private string responsibleDepartment = string.Empty;
        [ObservableProperty]
        private string technicalReference = string.Empty;
        [ObservableProperty]
        private string createdBy = string.Empty;
        [ObservableProperty]
        private string approvedBy = string.Empty;
        [ObservableProperty]
        private string documentType = string.Empty;
        [ObservableProperty]
        private string documentStatus = string.Empty;
        [ObservableProperty]
        private string legalOwner = string.Empty;
        [ObservableProperty]
        private string logoPath = GetApplicationFolderFileFullPath("Resources/Images/Logos/BABigLogo.png");
        [ObservableProperty]
        private string drawItemTitle = string.Empty;
        [ObservableProperty]
        private string revision = string.Empty;
        [ObservableProperty]
        private string dateOfIssue = string.Empty;
        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        private string internalReferenceNotes = string.Empty;
        [ObservableProperty]
        private string code = string.Empty;

        /// <summary>
        /// Weather to stop changing automatically the Code according to the Drawn Item
        /// </summary>
        [ObservableProperty]
        private bool stopChangingCodeAutomatically = false;
        /// <summary>
        /// Weather to stop changing automatically the title according to the Drawn Item
        /// </summary>
        [ObservableProperty]
        private bool stopChangingTitleAutomatically = false;
        /// <summary>
        /// Weather to stop changing automatically the technical reference according to the Drawn Item
        /// </summary>
        [ObservableProperty]
        private bool stopChangingTechnicalReferenceAutomatically = false;

        public DrawPdfDocumentOptions CopyPropertiesToModel(DrawPdfDocumentOptions model)
        {
            throw new NotSupportedException($"{nameof(DrawPdfDocumentOptionsViewModel)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public void ChangeAutoCodeTitleTechRef(string code , string title,string techRef)
        {
            if (!StopChangingCodeAutomatically) Code = code;
            if (!StopChangingTitleAutomatically) DrawItemTitle = title;
            if (!StopChangingTechnicalReferenceAutomatically) TechnicalReference = techRef;
        }

        public DrawPdfDocumentOptions GetModel()
        {
            DrawPdfDocumentOptions options = new()
            {
                PageMargin = this.PageMargin,
                PageSize = new QuestPDF.Helpers.PageSize(this.PageWidth, this.PageHeight),
                ScaleDrawToFit = this.ScaleDrawToFit,
                TableInfo = new DrawInfoTableOptions()
                {
                    ResponsibleDepartment = this.ResponsibleDepartment,
                    TechnicalReference = this.TechnicalReference,
                    CreatedBy = this.CreatedBy,
                    ApprovedBy = this.ApprovedBy,
                    DocumentType = this.DocumentType,
                    DocumentStatus = this.DocumentStatus,
                    LegalOwner = this.LegalOwner,
                    LogoImageByteArray = GetImageAsByteArray(this.LogoPath),
                    DrawItemTitle = this.DrawItemTitle,
                    Revision = this.Revision,
                    DateOfIssue = this.DateOfIssue,
                    Notes = this.Notes,
                    InternalReferenceNotes = this.InternalReferenceNotes,
                    Code = this.Code,
                    Titles = defaultTitles,
                    LanguageTwoLetterISO = ((App)Application.Current).SelectedLanguage == "el-GR" ? "GR" : "EN"
                }
            };
            return options;
        }

        public void SetModel(DrawPdfDocumentOptions model)
        {
            this.PageWidth = model.PageSize.Width;
            this.PageHeight = model.PageSize.Height;
            this.PageMargin = model.PageMargin;
            this.ScaleDrawToFit = model.ScaleDrawToFit;
            this.ResponsibleDepartment = model.TableInfo.ResponsibleDepartment;
            this.TechnicalReference = model.TableInfo.TechnicalReference;
            this.CreatedBy = model.TableInfo.CreatedBy;
            this.ApprovedBy = model.TableInfo.ApprovedBy;
            this.DocumentType = model.TableInfo.DocumentType;
            this.DocumentStatus = model.TableInfo.DocumentStatus;
            this.LegalOwner = model.TableInfo.LegalOwner;
            this.LogoPath = GetApplicationFolderFileFullPath("Resources/Images/Logos/BABigLogo.png");
            this.DrawItemTitle = model.TableInfo.DrawItemTitle;
            this.Revision = model.TableInfo.Revision;
            this.DateOfIssue = model.TableInfo.DateOfIssue;
            this.Notes = model.TableInfo.Notes;
            this.InternalReferenceNotes = model.TableInfo.InternalReferenceNotes;
            this.Code = model.TableInfo.Code;
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.OK)
            {
                StopChangingCodeAutomatically = false;
                StopChangingTechnicalReferenceAutomatically = false;
                StopChangingTitleAutomatically = false;
                SetModel(GetDefaultOptions());
            }
        }
        protected static string GetApplicationFolderFileFullPath(string relativePath)
        {
            // Combine the base directory with the relative path
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            // Check if the file exists
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            throw new FileNotFoundException($"File not found: {fullPath}");
        }
        /// <summary>
        /// Returns an image CONTENT from the specified relative path of the project
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        protected static byte[] GetImageAsByteArray(string path)
        {
            // Check if the file exists
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path); // Read and return the byte array
            }

            throw new FileNotFoundException($"File not found: {path}");
        }

        /// <summary>
        /// Gets the Default Options of the DrawPdfDocumentOptions for this Application
        /// </summary>
        /// <returns></returns>
        public static DrawPdfDocumentOptions GetDefaultOptions()
        {
            DrawPdfDocumentOptions defaults = new()
            {
                FontSize = 12,
                PageMargin = 20,
                PageSize = PageSizes.A4.Landscape(),
                ScaleDrawToFit = true,
                TableInfo = new()
                {
                    DocumentType = "lngDraw".TryTranslateKeyWithoutError(),
                    ResponsibleDepartment = "lngProductionDepartment".TryTranslateKeyWithoutError(),
                    Titles = defaultTitles,
                }
            };
            return defaults;
        }
        private static DrawInfoTableOptions.TableTitles GetDefaultTableTitles()
        {
            return new()
            {
                ResponsibleDep = "lngResponsibleDepartment".TryTranslateKeyWithoutError(),
                TechnicalRef = "lngTechnicalReference".TryTranslateKeyWithoutError(),
                CreatedBy = "lngCreatedBy".TryTranslateKeyWithoutError(),
                ApprovedBy = "lngApprovedBy".TryTranslateKeyWithoutError(),
                DocumentType = "lngDocumentType".TryTranslateKeyWithoutError(),
                DocumentStatus = "lngDocumentStatus".TryTranslateKeyWithoutError(),
                LegalOwner = "lngLegalOwner".TryTranslateKeyWithoutError(),
                DrawTitle = "lngTitleSupplementaryTitle".TryTranslateKeyWithoutError(),
                Revision = "lngRev".TryTranslateKeyWithoutError(),
                Language = "lngLang".TryTranslateKeyWithoutError(),
                Sheet = "lngSheet".TryTranslateKeyWithoutError(),
                DateOfIssue = "lngDateOfIssue".TryTranslateKeyWithoutError(),
                Notes = "lngNotes".TryTranslateKeyWithoutError(),
                Code = "lngCode".TryTranslateKeyWithoutError(),
                InternalReference = "lngInternalReference".TryTranslateKeyWithoutError(),
            };
        }
    }
}
