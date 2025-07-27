using ClosedXML.Excel;
using CommonHelpers;
using static XMLClosedReporting.AdvancedReportModels.StylesProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;

namespace XMLClosedReporting.AdvancedReportModels
{
    public abstract class AdvancedReportTemplate<T>
    {
        /// <summary>
        /// The Column Name for incremental Line Numbering
        /// </summary>
        protected const string incrementalLineNumberColumnName = "IncrementalLineNumberColumn";

        /// <summary>
        /// Helper to track the last added item number for incremental numbering during the report generation
        /// </summary>
        private int lastAddedItemNumber = 0;

        protected readonly AdvancedReportBuilder<T> builder = new();

        /// <summary>
        /// The Tables that should be Reported according to the options (usually only the ones with items)
        /// </summary>
        private List<ReportTableDefinition<T>> tablesToReport = [];
        private int columnsUsed = 0;

        //Helpers
        private readonly string itemsWithoutSpecificTableKey = "ItemsWithoutTable";
        private Dictionary<object, List<T>> segregatedItems = [];
        private Dictionary<object, List<T>> itemsForTables = [];

        public AdvancedReportTemplate()
        {

        }

        /// <summary>
        /// Generates a Report from the Items
        /// </summary>
        /// <param name="items">The Items being reported</param>
        /// <param name="wb">The Workbook</param>
        /// <param name="progress">The Progress reporter</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Constructs the report into the workbook</returns>
        protected async Task GenerateReportFromRows(List<T> items,
                                                    XLWorkbook wb,
                                                    IProgress<TaskProgressReport>? progress = null,
                                                    CancellationToken? token = null)
        {
            if (!builder.IsBuildComplete) throw new Exception("The Report Template has not been Built or its Configuration is not Valid");
            if (items.Count == 0) throw new Exception("There are no Items to Report");

            //assign some variables here to avoid big names in the code
            var strings = builder.Options.Strings;
            var options = builder.Options;
            int completedSteps = 0;
            int steps = 1;

            //Progress Report , Report Started
            token?.ThrowIfCancellationRequested();
            progress?.Report(new(steps, completedSteps, strings.GeneratingReportString));

            //Perform Various Calculations on the data that will be written to the report
            await Task.Run(() => PerformVariousCalculations(items));
            completedSteps++;//add a step completed for the initial calculations
            if (tablesToReport.Count == 0) throw new Exception($"The Tables Configuration is such that there where no Items to Report{Environment.NewLine}Make sure that there are Tables with the Reported Items Segregation keys and/or that there is a Table with a Null segregation keys to include all Rest Items");

            //Create the worksheet and the Cursor to assign data to the report
            var ws = wb.Worksheets.Add(strings.WorksheetName);
            ReportCursorAdvanced cursor = new(ws);

            //Move the Cursor to the First Cell
            cursor.MoveTo(options.FirstRow, options.FirstColumn);

            //Style with White all inbetween Cells
            for (int i = 0; i < options.FirstColumn + 1; i++)
            {
                ws.Column(i + 1).Style.Fill.SetBackgroundColor(XLColor.White);
            }
            //Style with White at least 25 Columns after the Last
            var lastColumn = columnsUsed + options.FirstColumn - 1;
            for (int i = 1; i <= 25; i++)
            {
                ws.Column(i + lastColumn).Style.Fill.SetBackgroundColor(XLColor.White);
            }

            //Add the Report Title to the Worksheet
            AddReportTitle(ws, cursor);

            //If there are not Built Tables then return
            //Progress Report Steps 
            //1. A Step for the initial Calculations (starts without knowing about the rest steps )
            //2. A Step for each table , a step for the notes table , a step for applying Styles

            //renew the steps now that the tables are known
            steps = 1 + tablesToReport.Count + 1 + 1; //Initial Calculations + Tables + Notes + Styles
            progress?.Report(new(steps, completedSteps, strings.BuildingTablesString));

            //Yield to the UI Thread (or the sync context) , to allow updates 
            await Task.Yield();

            //Move the cursor to the rows where the first table will be placed

            //First move it to the last row of the Report Title and the first Column of the report title
            cursor.MoveTo(builder.Options.ReportTitleRange.LastAddress.RowNumber, builder.Options.ReportTitleRange.FirstAddress.ColumnNumber);
            //then move it down as many times as designated by the rows between tables
            for (int i = 0; i <= builder.Options.RowsBetweenTables; i++)
            {
                cursor.MoveDown();
                if (i != builder.Options.RowsBetweenTables)
                {
                    var emptyRowRange = ws.Range(cursor.CurrentCell, cursor.CurrentCell.CellRight(columnsUsed - 1));
                    emptyRowRange.Style.Fill.SetBackgroundColor(XLColor.White);
                }
            }


            foreach (var table in tablesToReport)
            {
                token?.ThrowIfCancellationRequested();

                List<T> itemsForTable;
                //Get the segregated items by specific key or the generic ones , otherwise an empty table
                if (table.SegregationKey is null) itemsForTable = itemsForTables.TryGetValue(itemsWithoutSpecificTableKey, out var segregatedItems) ? segregatedItems : [];
                else itemsForTable = itemsForTables.TryGetValue(table.SegregationKey, out var segregatedItems) ? segregatedItems : [];

                //Build the Table
                await BuildTable(ws, cursor, table, itemsForTable, token);

                var tableRange = table.TableRange.GetXLRange(ws);
                //After each table move the cursor to the next Writing Position for the next table
                cursor.MoveTo(tableRange.LastRow().WorksheetRow().RowNumber(), tableRange.FirstColumn().WorksheetColumn().ColumnNumber());
                for (int i = 0; i <= options.RowsBetweenTables; i++)
                {
                    cursor.MoveDown();
                    if (i != builder.Options.RowsBetweenTables)
                    {
                        var emptyRowRange = ws.Range(cursor.CurrentCell, cursor.CurrentCell.CellRight(columnsUsed - 1));
                        emptyRowRange.Style.Fill.SetBackgroundColor(XLColor.White);
                    }
                }

                //Give the UI a chance to update
                token?.ThrowIfCancellationRequested();
                completedSteps++;
                progress?.Report(new(steps, completedSteps, strings.BuildingTablesString));
                await Task.Yield();
            }

            AddNotesTable(options, ws, cursor);
            completedSteps++;
            progress?.Report(new(steps, completedSteps, strings.ApplyingStylesString));
            await Task.Yield();

            //Apply the Styles
            token?.ThrowIfCancellationRequested();
            ApplyStylesToReport(options, ws);
            progress?.Report(new(steps, steps, strings.FinishedReportGenerationString));
            await Task.Yield();

            //Style with white at least 50rows after the last
            for (int i = 1; i <= 50; i++)
            {
                ws.Row(cursor.CurrentCell.WorksheetRow().RowNumber() + i).Style.Fill.SetBackgroundColor(XLColor.White);
            }

            //reset the cursor to the first cell of the worksheet
            cursor.MoveTo(1, 1);
            //reset the last added item number
            lastAddedItemNumber = 0;
            //reset the Non Empty Tables
            tablesToReport.Clear();
        }

        /// <summary>
        /// Apply the Styles to the Report
        /// </summary>
        /// <param name="options"></param>
        /// <param name="ws"></param>
        private void ApplyStylesToReport(AdvancedReportOptions<T> options, IXLWorksheet ws)
        {

            //apply the styles to the Report Title
            //apply the styles to All the Tables titles 
            //apply the styles to all The Tables Column Headers and Values
            //apply the styles to the Notes Title
            //apply the styles to the Notes Cell
            //apply the margins to the worksheet

            options.ReportTitleRange.GetXLRange(ws).Style.ApplyStyle(options.ReportTitleStyle);
            foreach (var table in tablesToReport)
            {
                //Apply the specific styles to the table if there are any otherwise the table-wide styles
                table.TitleRange.GetXLRange(ws).Style.ApplyStyle(table.Options?.TableTitleStyle ?? options.TablesOptions.TableTitleStyle);

                foreach (var column in table.Columns)
                {
                    //Apply the specific styles to the Column if there are any otherwise the column-wide styles
                    column.HeaderRange.GetXLRange(ws).Style.ApplyStyle(column.Options?.HeaderCellStyle ?? table.Options?.ColumnsOptions.HeaderCellStyle ?? options.TablesOptions.ColumnsOptions.HeaderCellStyle);
                    column.ValuesRange.GetXLRange(ws).Style.ApplyStyle(column.Options?.ValueCellsStyle ?? table.Options?.ColumnsOptions.ValueCellsStyle ?? options.TablesOptions.ColumnsOptions.ValueCellsStyle);
                }

                //Apply the table values border styles  , if there are any columns
                if (table.Columns.Count != 0)
                {
                    var tableValuesBorderStyle = table.Options?.TableValuesOutsideBorderStyle ?? options.TablesOptions.TableValuesOutsideBorderStyle;
                    //get the cells on the perimeter
                    var leftMostCells = table.Columns.First().ValuesRange.GetXLRange(ws);
                    var rightMostCells = table.Columns.Last().ValuesRange.GetXLRange(ws);
                    var topMostCells = ws.Range(
                        table.Columns.First().ValuesRange.FirstAddress.RowNumber,
                        table.Columns.First().ValuesRange.FirstAddress.ColumnNumber,
                        table.Columns.Last().ValuesRange.FirstAddress.RowNumber,
                        table.Columns.Last().ValuesRange.FirstAddress.ColumnNumber);
                    var bottomMostCells = ws.Range(
                        table.Columns.First().ValuesRange.LastAddress.RowNumber,
                        table.Columns.First().ValuesRange.LastAddress.ColumnNumber,
                        table.Columns.Last().ValuesRange.LastAddress.RowNumber,
                        table.Columns.Last().ValuesRange.LastAddress.ColumnNumber);

                    leftMostCells.Style.Border.LeftBorder = tableValuesBorderStyle.LeftBorder;
                    leftMostCells.Style.Border.LeftBorderColor = tableValuesBorderStyle.LeftBorderColor;
                    rightMostCells.Style.Border.RightBorder = tableValuesBorderStyle.RightBorder;
                    rightMostCells.Style.Border.RightBorderColor = tableValuesBorderStyle.RightBorderColor;
                    bottomMostCells.Style.Border.BottomBorder = tableValuesBorderStyle.BottomBorder;
                    bottomMostCells.Style.Border.BottomBorderColor = tableValuesBorderStyle.BottomBorderColor;
                    topMostCells.Style.Border.TopBorder = tableValuesBorderStyle.TopBorder;
                    topMostCells.Style.Border.TopBorderColor = tableValuesBorderStyle.TopBorderColor;
                }


            }
            options.ReportNotesTitleRange.GetXLRange(ws).Style.ApplyStyle(options.ReportNotesTitleStyle);
            options.ReportNotesRange.GetXLRange(ws).Style.ApplyStyle(options.ReportNotesStyle);

            //Adjust everything to Contents (min max is bugged if used must be done seperately)
            ws.Columns().AdjustToContents();

            //Add indentation to all adjusted Columns and Rows
            foreach (var column in ws.ColumnsUsed())
            {
                if (column.Width >= (options.MaximumColumnWidthAdjustment - options.ColumnsWidthAdjustmentLeftAndRight))
                {
                    column.Width = options.MaximumColumnWidthAdjustment - options.ColumnsWidthAdjustmentLeftAndRight;
                }
                column.Width += options.ColumnsWidthAdjustmentLeftAndRight;
            }

            ws.Rows().AdjustToContents();

            foreach (var row in ws.RowsUsed())
            {
                if (row.Height >= (options.MaximumRowHeightAdjustment - options.RowsHeightAdjustmentUpAndDown))
                {
                    row.Height = options.MaximumRowHeightAdjustment - options.RowsHeightAdjustmentUpAndDown;
                }
                row.Height += options.RowsHeightAdjustmentUpAndDown;
            }

            //After Applying Adjust To Contents apply the Row Heights (IF THEY ARE BIGGER from what the adjustments has done already)

            //General Report Heights
            options.ReportTitleRange.GetXLRange(ws).LastRow().WorksheetRow().Height = options.ReportTitleRowHeight;
            options.ReportNotesRange.GetXLRange(ws).LastRow().WorksheetRow().Height = options.ReportNotesRowHeight;

            //Height of Table Titles
            foreach (var table in tablesToReport)
            {
                var range = table.TitleRange.GetXLRange(ws);
                var row = range.LastRow().WorksheetRow();

                //Get the Row height either from the specific table options , or from the tablewide if there are no specific options
                var rowHeightInOptions = table.Options?.TableHeaderRowHeight ?? options.TablesOptions.TableHeaderRowHeight;
                //check if smaller and if it is apply the options one
                if (row.Height < rowHeightInOptions) row.Height = rowHeightInOptions;
            }

            //Apply the Margins
            if (options.FirstRow > 1)
            {
                //foreach each row in between the first row and the first row of the report title divide the margin by the number of rows and apply it to each row
                var topMargin = options.MarginFromTop / (options.FirstRow - 1);
                for (int i = 1; i < options.FirstRow; i++)
                {
                    ws.Row(i).Height = topMargin;
                }
            }
            if (options.FirstColumn > 1)
            {
                //foreach each column in between the first column and the first column of the report title divide the margin by the number of column and apply it to each column
                var leftMargin = options.MarginFromLeft / (options.FirstColumn - 1);
                for (int i = 1; i < options.FirstColumn; i++)
                {
                    ws.Column(i).Width = leftMargin;
                }
            }
        }
        /// <summary>
        /// Add the Notes Table to the Worksheet
        /// </summary>
        /// <param name="options"></param>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        private static void AddNotesTable(AdvancedReportOptions<T> options, IXLWorksheet ws, ReportCursorAdvanced cursor)
        {
            //Build the Notes Table
            //The Cursor should be at the correct position to insert the Notes
            cursor.CurrentCell.Value = options.Strings.NotesTitleString;
            //Save the Range of the Notes Title , The Columns of the Range will be equal to the columns of Report Title
            options.ReportNotesTitleRange = new(cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, options.ReportTitleRange.LastAddress.ColumnNumber);
            options.ReportNotesTitleRange.GetXLRange(ws).Merge();

            //Move the cursor down to the first cell of the Notes , write the Notes and merge the cell
            cursor.MoveDown();
            cursor.CurrentCell.Value = options.Strings.Notes;
            options.ReportNotesRange = new(cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, options.ReportTitleRange.LastAddress.ColumnNumber);
            options.ReportNotesRange.GetXLRange(ws).Merge();
        }
        /// <summary>
        /// Add the Report Title to the Worksheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        private void AddReportTitle(IXLWorksheet ws, ReportCursorAdvanced cursor)
        {
            cursor.CurrentCell.Value = builder.Options.Strings.ReportTitle;

            //Save the Range of the Title
            builder.Options.ReportTitleRange = new(cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, cursor.CurrentColumn + columnsUsed - 1);
            //Merge the Cells of the Title
            builder.Options.ReportTitleRange.GetXLRange(ws).Merge();
        }
        /// <summary>
        /// Performs various calculations on the data that will be written to the report
        /// </summary>
        private void PerformVariousCalculations(List<T> items)
        {
            segregatedItems.Clear();
            //Segregate the items according to the segregation function
            segregatedItems = items.SegregateBy(builder.Options.SegregateItemsBy, itemsWithoutSpecificTableKey);

            itemsForTables.Clear();
            //Check for which keys there is a matching table , copy those to a new dictionary 
            //For the keys that there is no matching table , put those items under the nullkey 
            //if there is a table with segregation key null it will take those items .
            foreach (var key in segregatedItems.Keys)
            {
                //have to use the Equals method otherwise with == operator values are boxed and only checked for reference equality not their actual equality
                if (builder.BuiltTables.Any(t => t.SegregationKey?.Equals(key) ?? false))
                {
                    var itemsForTable = segregatedItems[key];
                    itemsForTables.Add(key, itemsForTable);

                    //Add the table to the tablesToReport , The Segregation Key of the table will always be Not Null Here , as its always checked against the segreagatedItems.Keys , where the keys cannot be null anyways
                    //The table will always have items if this block is hit , because its checked against the keys of the segregated items
                    tablesToReport.Add(builder.BuiltTables.First(t => t.SegregationKey!.Equals(key)));
                }
                else //when there is no table with the specific key , add the items to the itemsWithoutSpecificTableKey
                {
                    //check if there is already opened key "itemwithoutSpecificTableKey"
                    //If there is add more items inside the List 
                    //Otherwise open a new one and Add the table of Null items to the List
                    if (itemsForTables.ContainsKey(itemsWithoutSpecificTableKey))
                    {
                        itemsForTables[itemsWithoutSpecificTableKey].AddRange(segregatedItems[key]);
                    }
                    else 
                    {
                        itemsForTables.Add(itemsWithoutSpecificTableKey, segregatedItems[key]);

                        //check if there is a null segregation Key table , add it to the tablesToReport
                        //if the Else block is hit , it means there are already items that are placed in the itemsWithoutSpecificTableKey
                        if (builder.BuiltTables.Any(t => t.SegregationKey is null))
                        {
                            var nullKeyTable = builder.BuiltTables.First(t => t.SegregationKey is null);
                            tablesToReport.Add(nullKeyTable);
                        }
                    } 
                }
            }

            //Check weather the tableToReport are only the ones with items or all of them
            //If they should be all then cancel out any previous filtering from the above calculations and set all the Built Tables
            if (builder.Options.SkipEmptyTables == false) tablesToReport = builder.BuiltTables.ToList();
            else tablesToReport = tablesToReport.OrderBy(t => t.TableOrderNo).ToList(); //Order them according to their Order Number as selected in configuration

            //Get the number of columns used in the report , the table with the maximum columns defines also the number of columns used
            //Use the non Empty tables to get the maximum columns used // if the option is set to skip empty tables

            columnsUsed = builder.Options.SkipEmptyTables
                    ? tablesToReport.Select(t => t.Columns.Sum(c => c.GetOccupiedColumns(t, builder))).Max() //if we use max at the beginign the compiler struggles to fix the expression
                    : builder.BuiltTables.Select(t => t.Columns.Sum(c => c.GetOccupiedColumns(t, builder))).Max();

            //The used Rows are
            // -the sum of the items (each item takes one row) ,
            // -plus the sum of the tables x2 (each table has headers + title) ,
            // -plus the sum of the tables x the rows between them (last table has rows between it and notes)
            // -plus 2 rows for the Notes (notes header and notes cell)
            // -plus 1 row for the Report Title
            // -plus another rows between tables for the Rows between the Report Title and Tables
        }

        /// <summary>
        /// Builds a Table in the Worksheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <param name="table"></param>
        /// <param name="tableItems"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task BuildTable(IXLWorksheet ws,
                                             ReportCursorAdvanced cursor,
                                             ReportTableDefinition<T> table,
                                             List<T> tableItems,
                                             CancellationToken? token = null)
        {
            await Task.Yield();

            AddTableTitle(ws, cursor, table);

            //move the Cursor below the Title to Write Column Headers
            cursor.MoveTo(table.TitleRange.FirstAddress.RowNumber, table.TitleRange.FirstAddress.ColumnNumber)
                  .MoveDown();

            AddColumnsHeaders(cursor, table);

            token?.ThrowIfCancellationRequested();
            await Task.Yield();

            //when finished move the cursor to the first cell of the data
            cursor.MoveTo(cursor.CurrentRow + 1, table.TitleRange.FirstAddress.ColumnNumber);

            await WriteColumnsValues(cursor, table, tableItems, token);
            //cursor finishes at the right of the last column value of the table
        }
        /// <summary>
        /// Add the Table Title to the Worksheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <param name="table"></param>
        private void AddTableTitle(IXLWorksheet ws,
                                   ReportCursorAdvanced cursor,
                                   ReportTableDefinition<T> table)
        {
            //Add the Table Title
            cursor.CurrentCell.Value = table.TableHeader;
            //Save the Range of the Table Title , The Columns of the Range will be equal to the columns of the table
            table.TitleRange = new(cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, cursor.CurrentColumn + table.Columns.Sum(c=> c.GetOccupiedColumns(table,builder)) - 1);
            table.TitleRange.GetXLRange(ws).Merge();
        }
        /// <summary>
        /// Add the Columns Headers to the Worksheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <param name="table"></param>
        private void AddColumnsHeaders(ReportCursorAdvanced cursor,
                                       ReportTableDefinition<T> table)
        {
            //Add Columns Headers
            foreach (var column in table.Columns)
            {
                cursor.CurrentCell.Value = column.ColumnHeader;

                //save the current first Cell to use later in the range 
                cursor.SaveCurrentCell();

                //Get the number of ocuppied Columns so to properly merge the headers if needed
                int occupiedColumnsMoreThan1 = column.GetOccupiedColumns(table, builder) - 1;

                for (int i = 0; i < occupiedColumnsMoreThan1; i++)
                {
                    //Move the cursor to the right for each extra column that is occupied by the column Definition
                    cursor.MoveRight();
                }

                column.HeaderRange = new(cursor.SavedCell, cursor.CurrentCell);
                column.HeaderRange.GetXLRange(cursor.WS).Merge(); // if both saved and current are the same nothing will happen
                cursor.MoveRight();//in the end move again right so to write the next Header
            }
        }
        /// <summary>
        /// Write the Columns Values to the Worksheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cursor"></param>
        /// <param name="table"></param>
        /// <param name="tableItems"></param>
        /// <param name="token"></param>
        private async Task WriteColumnsValues(ReportCursorAdvanced cursor,
                                              ReportTableDefinition<T> table,
                                              List<T> tableItems,
                                              CancellationToken? token = null)
        {
            //Write the Data for each column for each item . by first iterating the columns and then the items 
            //save each rangeInfo for the written column values in the end of each column data write
            foreach (var column in table.Columns)
            {
                //Get the number of ocuppied Columns so to properly merge the ValueCells if needed
                int occupiedColumnsMoreThan1 = column.GetOccupiedColumns(table, builder) - 1;

                var firstValueCell = cursor.CurrentCell;//save the first cell of the column to use it to save the column range at the end of data writing

                //find the formula if there is one , it will be the same for every cell in the column
                string formula = column.Formula?.GetFormula(table.GetColumnNumbersDictionary()) ?? "";

                int itemCount = 0;
                foreach (var item in tableItems)
                {
                    if (column.ColumnName == incrementalLineNumberColumnName)
                    {
                        lastAddedItemNumber++;
                        cursor.CurrentCell.Value = lastAddedItemNumber;
                    }
                    else if (column.HasFormula)
                    {
                        cursor.CurrentCell.FormulaR1C1 = formula;
                    }
                    else
                    {
                        cursor.CurrentCell.Value = XLCellValue.FromObject(column.ValueRetriever(item));
                    }

                    //check how many are the occupied Columns of this Column Definition (so to merge if needed)
                    //Save the first cell of the value 
                    cursor.SaveCurrentCell();

                    for (int i = 0; i < occupiedColumnsMoreThan1; i++)
                    {
                        //Move the cursor to the right for each extra column that is occupied by the column Definition
                        cursor.MoveRight();
                    }
                    //Merge the range of the value
                    cursor.WS.Range(cursor.SavedCell, cursor.CurrentCell).Merge();

                    cursor.MoveTo(cursor.SavedCell); //move to the first cell and then down (so if something merged to properly move to the below cell , and not the right one and then below)
                    cursor.MoveDown();//for the next value insertion

                    //Check if the token is cancelled every 50 values
                    itemCount++;
                    if (itemCount % 50 == 0)
                    {
                        token?.ThrowIfCancellationRequested();
                        await Task.Yield();
                    }
                }

                //When out of the loop of the values insertion of the columns
                //the cursor should be at the below cell from the last inserted value . 
                //If the cell of the last inserted value is merged , the cursor will be at the first cell below the merge (first cell on the left from the below cells).

                //so according to the number of extra occupied cells above 1 , move the cursor right , in the end move it up to find the end of the range
                for (int i = 0; i < occupiedColumnsMoreThan1; i++)
                {
                    cursor.MoveRight();
                }
                cursor.MoveUp();

                //save the Value range
                column.ValuesRange = new(firstValueCell, cursor.CurrentCell);
                //In the end move the cursor to the next column first Value cell
                cursor.MoveTo(column.ValuesRange.FirstAddress.RowNumber,column.ValuesRange.LastAddress.ColumnNumber).MoveRight();
                token?.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

    }
}
