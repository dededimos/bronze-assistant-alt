using ClosedXML.Excel;
using CommonHelpers;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XMLClosedReporting
{
    /// <summary>
    /// An Object Representing a Report Template , Inherit from this to Create a ReportTemplate for any Object (Similar to Fluent Validation)
    /// Build the Report Columns and options in the Constructor using 'builder' and 'options'
    /// </summary>
    /// <typeparam name="T">The Object being reported</typeparam>
    public abstract class AbstractReportTemplate<T>
    {
        protected readonly IColumnsCreator<T> builder;
        protected readonly ReportTemplateOptions options = new();
        

        public AbstractReportTemplate(IColumnsCreator<T> columnsCreator)
        {
            builder = columnsCreator;
        }
        public AbstractReportTemplate()
        {
            builder = new ColumnsBuilder<T>();
        }

        /// <summary>
        /// Generates a Report and saves it into a Stream
        /// </summary>
        /// <param name="itemsReported">The Items to Report</param>
        /// <param name="savedWbStream">The Stream to Save the report into</param>
        public async Task GenerateReport(List<T> itemsReported,
                                         XLWorkbook wb,
                                         string? notes = null,
                                         decimal? vatFactor = null,
                                         IProgress<TaskProgressReport>? progress = null,
                                         CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            progress?.Report(new(3, 0, $"{options.ProgressTranslations.GeneratingReportTranslation}"));
            await Task.Delay(50);

            var ws = wb.Worksheets.Add($"Report");
            ReportCursor cursor = new(ws,options.FirstColumn, options.FirstRow);

            //Add Title
            AddReportTitle(ws,cursor);

            //return if there are no Columns Present
            if (!builder.BuiltColumns.Any()) return; 

            //Add Headers
            AddColumnHeaders(ws, cursor);

            progress?.Report(new(3, 1, $"{options.ProgressTranslations.GeneratingReportTranslation}"));
            cancellationToken?.ThrowIfCancellationRequested();
            await Task.Delay(50);

            //Add Values
            await AddColumnValues(ws, cursor, itemsReported, progress,cancellationToken);

            progress?.Report(new(3, 2, $"{options.ProgressTranslations.CreatingTableFormatTranslation}"));
            cancellationToken?.ThrowIfCancellationRequested();
            await Task.Delay(50);

            //Create a Table out of the Data Range with Headers
            CreateTableFormat(ws, cursor);

            if (options.UseSumsTable)
            {
                // a table on the far right of the general Table
                AddSumsTable(ws, cursor, vatFactor);
            }
            else
            {
                // only sums below the value cells
                AddSumCells(ws, cursor);
            }

            AddNotes(ws,cursor,notes ?? string.Empty);

            //Adjust To Contents
            ws.RowsUsed().AdjustToContents();
            ws.ColumnsUsed().AdjustToContents();

            ApplyStyles(cursor);
            cancellationToken?.ThrowIfCancellationRequested();
            progress?.Report(new(3, 3, $"{options.ProgressTranslations.ReportGeneratedTranslation}"));
            await Task.Delay(50);
        }
        /// <summary>
        /// Configure the options of the Report
        /// </summary>
        /// <param name="optionsAction">The Action that manipulates the Default Options</param>
        public void ConfigureReportOptions(Action<ReportTemplateOptions> optionsAction)
        {
            optionsAction.Invoke(options);
        }

        /// <summary>
        /// Adds the Report Title and Saves its range to the Cursor
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        private void AddReportTitle(IXLWorksheet ws,ReportCursor cursor)
        {
            var numberOfColumns = builder.BuiltColumns.Count();
            cursor.CurrentCell.Value = options.ReportTitle;

            //the Range of the Title to Merge
            cursor.ReportTitleRange = ws.Range(cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, cursor.CurrentColumn + numberOfColumns - 1);
            cursor.ReportTitleRange.Merge();
        }
        /// <summary>
        /// Creates the Columns Headers and Saves the Range to the Cursor
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="currentColumn"></param>
        /// <param name="currentRow"></param>
        /// <returns></returns>
        private void AddColumnHeaders(IXLWorksheet ws, ReportCursor cursor)
        {
            cursor.MoveTo(cursor.ReportTitleRange.FirstCell()).MoveDown();
            cursor.SaveCurrentCell();

            foreach (var columnDefinition in builder.BuiltColumns)
            {
                cursor.CurrentCell.Value = columnDefinition.ColumnName;
                cursor.MoveRight();
            }

            //Negate Last Move
            cursor.MoveLeft();

            //Save the Range of the Headers
            cursor.HeadersRange = ws.Range(cursor.SavedCell,cursor.CurrentCell);
        }
        /// <summary>
        /// Adds all the Column Values and Returns the range of the Values
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <param name="itemsReported"></param>
        /// <returns></returns>
        private async Task AddColumnValues(IXLWorksheet ws,
                                           ReportCursor cursor,
                                           List<T> itemsReported,
                                           IProgress<TaskProgressReport>? progress = null,
                                           CancellationToken? cancellationToken = null)
        {
            cursor.MoveTo(cursor.HeadersRange.FirstCell()).MoveDown();
            var startingPosition = cursor.SaveCurrentCell().SavedCell;
            var currentColumn = 0;
            foreach (var column in builder.BuiltColumns)
            {
                //Delegate Progress
                cancellationToken?.ThrowIfCancellationRequested();
                progress?.Report(new(itemsReported.Count, 0, $"{options.ProgressTranslations.ColumnTranslation} : {column.ColumnName}"));
                await Task.Delay(10);

                // go back to first row and next column
                cursor.MoveTo(startingPosition.WorksheetRow().RowNumber(),startingPosition.WorksheetColumn().ColumnNumber() + currentColumn);
                cursor.SaveCurrentCell();

                //Produce the rows of the Column
                //For each item on the provided list that gets reported . Add a value and move to the next row .
                foreach (var item in itemsReported)
                {
                    //If The values in this column are from a formula
                    if (column.IsValueFromFormula)
                    {
                        cursor.CurrentCell.FormulaR1C1 = column.FormulaInfo.GetFormula(cursor.GetColumnNumbersOfColumns());

                    }
                    else //set value
                    {
                        //for creating a link out of the value (for pictures)
                        if (column.Options.ShouldCreateHyperlinkInValue)
                        {
                            cursor.CurrentCell.SetHyperlink(new(column.ValueRetriever(item).ToString()));
                        }
                        
                        cursor.CurrentCell.Value = XLCellValue.FromObject(column.ValueRetriever(item));
                    }

                    cursor.CurrentCell.Style.NumberFormat.Format = column.Options.NumberFormat;
                    // Move down in its value to write the next one
                    cursor.MoveDown();

                    //Delegate Progress
                    cancellationToken?.ThrowIfCancellationRequested();
                    progress?.Report(new(itemsReported.Count, itemsReported.IndexOf(item) + 1, $"{options.ProgressTranslations.ColumnTranslation} : {column.ColumnName}"));
                    await Task.Delay(1);
                }

                //After Finishing the Column , We must negate the last MoveDown
                //Negate the DownMovement of the last value insertion
                cursor.MoveUp();

                //Save the Column Range
                var columnRange = ws.Range(cursor.SavedCell, cursor.CurrentCell);
                cursor.SetColumnRange(column.ColumnName, columnRange);

                // next column
                currentColumn++;
            }
        }
        private void AddSumCells(IXLWorksheet ws, ReportCursor cursor)
        {
            foreach (var column in builder.BuiltColumns)
            {
                if (column.Options.ShouldSumColumnValues)
                {
                    //Move to the End of the Column and one Cell below to apply the Title
                    cursor.MoveTo(cursor.GetColumnRange(column.ColumnName).LastCell()).MoveDown();

                    var columnRange = cursor.GetColumnRange(column.ColumnName);
                    
                    cursor.CurrentCell.Value = column.Options.SumCellTitle;
                    options.ColumnHeadersStyleOptions.ApplyStyle(cursor.CurrentCell.Style);
                    options.ColumnHeadersStyleOptions.ApplyDimensions(cursor.CurrentCell);

                    // 109 is the function_num argument for SUBTOTAL to sum only visible rows in a table
                    cursor.MoveDown();
                    cursor.CurrentCell.FormulaR1C1 = $"=SUBTOTAL(109,R{columnRange.FirstRow().RowNumber()}C{columnRange.FirstColumn().ColumnNumber()}:R{columnRange.LastRow().RowNumber()}C{columnRange.LastColumn().ColumnNumber()})";
                    cursor.CurrentCell.Style.NumberFormat.Format = column.Options.NumberFormat;
                    column.Options.ValueCellsStyle.ApplyStyle(cursor.CurrentCell.Style);
                }
            }
        }
        private void AddNotes(IXLWorksheet ws, ReportCursor cursor, string notes)
        {
            //Return if there are no Notes
            if (string.IsNullOrEmpty(notes))
            {
                return;
            }

            //Find the Cells on the left of the SumsTable and merge them to a single Notes Cell
            if (options.UseSumsTable)
            {
                //Set the Range of the of the Notes Cell from the first column till one column before the sums Table
                //(Row of first Cell of Sums Table , First Column Of Workseet , Row of Last Cell of Sums Table , Previous Column of First Cell of Sums Table)
                cursor.NotesRange = ws.Range(cursor.SumsTableTitlesRange.FirstRow().RowNumber(), options.FirstColumn, cursor.SumsTableTitlesRange.LastRow().RowNumber(), cursor.SumsTableTitlesRange.FirstColumn().ColumnNumber() - 1);
            }
            else
            {
                //If there is no sums Table put the notes just below the Summed Columns
                var lastCell = ws.LastCellUsed();
                cursor.NotesRange = ws.Range(lastCell.CellBelow().WorksheetRow().RowNumber(), options.FirstColumn, lastCell.CellBelow().WorksheetRow().RowNumber() + 3, lastCell.WorksheetColumn().ColumnNumber());
            }

            cursor.NotesRange.Merge();
            cursor.NotesRange.Value = $"{options.NotesTitle}: {notes}";
            options.NotesStyleOptions.ApplyStyle(cursor.NotesRange.Style);
        }
        private void AddSumsTable(IXLWorksheet ws, ReportCursor cursor , decimal? vatFactor = null)
        {
            //TODO : Refactor To Move with Cursor and Save Seperately the Ranges of Titles and Value Cells
            //Then Remove the Styling Methods and Apply them on the end of the Report Generation
            //Do the Same for Notes

            //Move at the cell above of the first Title Cell (This way the iterations can save and move down)
            //The Sums table consists of 2 columns so the first cell above the titles of the sums table is the second to Last from the values Range.
            cursor.MoveTo(cursor.GetValueCellsRange().LastCell()).MoveLeft().SaveCurrentCell();
            
            // Save the First value and Title Cells (to use it when saving the ranges of the Sums Table)
            var firstTitleCell = cursor.CurrentCell.CellBelow();
            var firstValueCell = firstTitleCell.CellRight();

            foreach (var column in builder.BuiltColumns)
            {
                //Check if the Values of the Column Should be Summed
                if (column.Options.ShouldSumColumnValues)
                {
                    //Get the Range of the Column's values
                    var columnRange = cursor.GetColumnRange(column.ColumnName);

                    //Move the Cursor to the Next Row from the Last Saved Position , then Resave it for the next iteration
                    cursor.MoveToSavedCell().MoveDown().SaveCurrentCell();

                    //Write the Table on the Last Column and the One Before it
                    cursor.CurrentCell.Value = column.Options.SumCellTitle;
                    options.SumsTableTitlesOptions.ApplyStyle(cursor.CurrentCell.Style);
                    
                    //Move to the cell of the Value
                    cursor.MoveRight();

                    //Write the Formula that sums up the Visible Values of this specific Column
                    //If the Table is Filtered , Only the Visible Rows will be summed!!! with this formula
                    //The 109 is the function_num argument for SUBTOTAL to sum only visible rows in a table for excel
                    //From Chat GPT : 
                    /*
                     * The SUBTOTAL function in Excel allows you to apply various aggregate functions, such as SUM, AVERAGE, COUNT, etc., with the ability to include or exclude hidden rows.
                     *
                     *  The first argument in SUBTOTAL(function_num, range) determines which function is used. The function numbers are categorized as follows:
                     *  
                     *      1 - 11: Includes hidden rows.
                     *      101 - 111: Ignores hidden rows (i.e., only considers visible rows when using filters).
                     *  
                     *  In your case, 109 corresponds to:
                     *  
                     *      9 → SUM
                     *      100 + 9 = 109 → SUM that ignores hidden rows.
                     */
                    cursor.CurrentCell.FormulaR1C1 = $"=SUBTOTAL(109,R{columnRange.FirstRow().RowNumber()}C{columnRange.FirstColumn().ColumnNumber()}:R{columnRange.LastRow().RowNumber()}C{columnRange.LastColumn().ColumnNumber()})";
                    cursor.CurrentCell.Style.NumberFormat.Format = column.Options.NumberFormat;

                    //Add vat row if this Column is the TotalVat Column
                    if (column.ColumnName == options.VatColumnName && vatFactor != null)
                    {
                        cursor.MoveDown().MoveLeft();
                        cursor.CurrentCell.Value = $"{options.VatSumTitle}({(vatFactor - 1) * 100}%)";

                        cursor.MoveDown();
                        cursor.CurrentCell.Value = options.FinalTotalSumTitle;

                        cursor.MoveUp().MoveRight();
                        //calculate the vat sum from the cell above
                        cursor.CurrentCell.FormulaR1C1 = $"R[-1]C*({vatFactor}-1)";
                        cursor.CurrentCell.Style.NumberFormat.Format = column.Options.NumberFormat;

                        cursor.MoveDown();
                        //Calculate the Total sum from the above two cells (vat cell - totalNet cell)
                        cursor.CurrentCell.FormulaR1C1 = "R[-1]C + R[-2]C";
                        cursor.CurrentCell.Style.NumberFormat.Format = column.Options.NumberFormat;
                    }
                }
                //The Current Cell of the Cursor has finished writing the last value so its also the last value cell
                //The Last Title Cell is the Left Cell from the Last Value Cell
                var lastValueCell = cursor.CurrentCell;
                var lastTitleCell = lastValueCell.CellLeft();

                //Set the Ranges for the cursor
                cursor.SumsTableTitlesRange = ws.Range(firstTitleCell, lastTitleCell);
                cursor.SumsTableValuesRange = ws.Range(firstValueCell,lastValueCell);
            }
        }
        private static void CreateTableFormat(IXLWorksheet ws , ReportCursor cursor)
        {
            //Range of the Table => from first Cell of headers to last cell of Values
            var tableRange = ws.Range(cursor.HeadersRange.FirstCell(), cursor.GetValueCellsRange().LastCell());
            var tableName = "ReportTable";
            var table = tableRange.CreateTable(tableName);
            table.Theme = XLTableTheme.TableStyleMedium16;

            //Freeze the Panes of the headers of the Table (Headers do not scroll!)
            ws.SheetView.FreezeRows(cursor.HeadersRange.FirstRow().RowNumber());
        }
        private void ApplyStyles(ReportCursor cursor)
        {
            //Style Title
            options.ReportTitleStyleOptions.ApplyStyle(cursor.ReportTitleRange.Style);
            options.ReportTitleStyleOptions.ApplyDimensions(cursor.ReportTitleRange);

            //Style Headers
            options.ColumnHeadersStyleOptions.ApplyStyle(cursor.HeadersRange.Style);
            options.ColumnHeadersStyleOptions.ApplyDimensions(cursor.HeadersRange);

            //Style Columns
            foreach (var column in builder.BuiltColumns)
            {
                var colRange = cursor.GetColumnRange(column.ColumnName);
                var colStyle = colRange.Style;
                column.Options.ValueCellsStyle.ApplyStyle(colStyle);
                column.Options.ValueCellsStyle.ApplyDimensions(colRange);
            }

            //Style Sums Table
            if (options.UseSumsTable)
            {
                options.SumsTableTitlesOptions.ApplyStyle(cursor.SumsTableTitlesRange.Style);
                options.SumsTableValuesOptions.ApplyStyle(cursor.SumsTableValuesRange.Style);
            }
        }
    }
}
