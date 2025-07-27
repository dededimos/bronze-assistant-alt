using XMLClosedReporting.StylesModels;

namespace XMLClosedReporting
{
    public class ReportTemplateOptions
    {
        public int FirstColumn { get; set; } = 2;
        public int FirstRow { get; set; } = 2;
        public string ReportTitle { get; set; } = string.Empty;
        public StyleOptions ColumnHeadersStyleOptions { get; set; } = StyleOptions.ColumnHeadersDefaultStyles();
        public StyleOptions ReportTitleStyleOptions { get; set; } = StyleOptions.ReportTitleDefaultStyles();
        public StyleOptions NotesStyleOptions { get; set; } = StyleOptions.ReportNotesDefaultStyles();
        public string NotesTitle { get; set; } = "Notes";

        /// <summary>
        /// Weather to Use a sums table instead of Sums just below each Column
        /// </summary>
        public bool UseSumsTable { get; set; }
        /// <summary>
        /// The Column From which to calculate the Sum of Vat
        /// </summary>
        public string VatColumnName { get; set; } = string.Empty;
        public string VatSumTitle { get; set; } = string.Empty;
        public string FinalTotalSumTitle { get; set; } = string.Empty;

        /// <summary>
        /// The Sums Values Styles on the Table
        /// </summary>
        public StyleOptions SumsTableValuesOptions { get; set; } = StyleOptions.ReportSumsTableValuesDefaultStyles();
        /// <summary>
        /// The Sums Titles Styles on the Table
        /// </summary>
        public StyleOptions SumsTableTitlesOptions { get; set; } = StyleOptions.ReportSumsTableTitlesDefaultStyles();

        /// <summary>
        /// The informative strings of the Progress that is delegated back to listeners of the Report Generation Method
        /// </summary>
        public ProgressLocalizationValues ProgressTranslations { get; set; } = new();
    }

    public class ProgressLocalizationValues
    {
        public string GeneratingReportTranslation { get; set; } = "Generating Report...";
        public string ColumnTranslation { get; set; } = "Column";
        public string CreatingTableFormatTranslation { get; set; } = "Creating Table Format...";
        public string ReportGeneratedTranslation { get; set; } = "Report Generated";
    }

}
