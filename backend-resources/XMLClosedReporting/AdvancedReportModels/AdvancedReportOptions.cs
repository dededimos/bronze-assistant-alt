namespace XMLClosedReporting.AdvancedReportModels
{
    public class AdvancedReportOptions<T>
    {

        /// <summary>
        /// The Function that selects the property of the items that will be used to segregate them in different tables
        /// <para>If null All items will go to the first table without a segregation key</para>
        /// </summary>
        public Func<T, object>? SegregateItemsBy { get; set; }
        /// <summary>
        /// Does not include Tables that have no Items
        /// </summary>
        public bool SkipEmptyTables { get; set; } = true;
        public ReportStringsOptions Strings { get; set; } = new();

        /// <summary>
        /// The Options for ALL the Tables of the Report
        /// <para>Each table has also a table Options of it self , that can be configured seperately</para>
        /// </summary>
        public ReportTableOptions<T> TablesOptions { get; set; } = ReportTableOptions<T>.DefaultOptions();
        /// <summary>
        /// The Style of the Cells that contain the Title of the Report
        /// </summary>
        public StylesInfo ReportTitleStyle { get; set; } = StylesProvider.ReportTitleDefaultStyles();
        public RangeInfo ReportTitleRange { get; set; } = RangeInfo.UnsetRange();

        /// <summary>
        /// The Style of the Cell that contains the Title of the Notes of the Report
        /// </summary>
        public StylesInfo ReportNotesTitleStyle { get; set; } = StylesProvider.DefaultNotesTitleStyle();
        /// <summary>
        /// The Style of the Cell/s that contain the Notes of the Report
        /// </summary>
        public StylesInfo ReportNotesStyle { get; set; } = StylesProvider.DefaultNotesTableStyle();
        /// <summary>
        /// The Range of the Notes of the Report
        /// </summary>
        public RangeInfo ReportNotesRange { get; set; } = RangeInfo.UnsetRange();
        /// <summary>
        /// The Range of the Title of the Notes of the Report
        /// </summary>
        public RangeInfo ReportNotesTitleRange { get; set; } = RangeInfo.UnsetRange();


        /// <summary>
        /// The Height of the Row where the Report Title will be placed
        /// </summary>
        public double ReportTitleRowHeight { get; set; } = 40d;
        /// <summary>
        /// The Height of the Row where the Report Notes will be placed
        /// </summary>
        public double ReportNotesRowHeight { get; set; } = 80d;
        /// <summary>
        /// The First Row where the Report will start
        /// </summary>
        public int FirstRow { get; set; } = 2;
        /// <summary>
        /// The First Column where the Report will start
        /// </summary>
        public int FirstColumn { get; set; } = 2;
        /// <summary>
        /// The Margin from the Top of the Excel Page
        /// </summary>
        public double MarginFromTop { get; set; } = 30;
        /// <summary>
        /// The Margin from the Left of the Excel Page
        /// </summary>
        public double MarginFromLeft { get; set; } = 20;

        public int RowsBetweenTables { get; set; } = 2;


        /// <summary>
        /// Weather to include incremental Line Numbers to the Reported Rows
        /// </summary>
        public bool IncludeIncrementalLineNumbers { get; set; } = true;

        /// <summary>
        /// The Maximum Width that a Column can have in the Report when Adjusted to Contents
        /// </summary>
        public double MaximumColumnWidthAdjustment { get; set; } = 40d;
        /// <summary>
        /// Adjusts all the Used Columns Width after Adjust to Contents has been Used , to add a little more space between them
        /// </summary>
        public double ColumnsWidthAdjustmentLeftAndRight { get; set; } = 5;

        /// <summary>
        /// The Maximum Height that a Row can have in the Report when Adjusted to Contents
        /// </summary>
        public double MaximumRowHeightAdjustment { get; set; } = 200d;
        /// <summary>
        /// Adjusts all the Used Rows Height after Adjust to Contents has been Used , to add a little more space between them
        /// </summary>
        public double RowsHeightAdjustmentUpAndDown { get; set; } = 5;
        public static AdvancedReportOptions<T> DefaultOptions() => new();
    }

    public class ReportStringsOptions
    {
        public string ReportTitle { get; set; } = "Report Title";
        public string NotesTitleString { get; set; } = "Notes";
        public string Notes { get; set; } = "-";
        public string WorksheetName { get; set; } = "Report";

        public string GeneratingReportString { get; set; } = "Generating Report";
        public string BuildingTablesString { get; set; } = "Building Tables";
        public string ApplyingStylesString { get; set; } = "Applying Styles";
        public string FinishedReportGenerationString { get; set; } = "Finished Report Generation";
    }
}
