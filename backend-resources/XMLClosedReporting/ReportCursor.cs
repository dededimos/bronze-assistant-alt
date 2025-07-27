using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics.CodeAnalysis;

namespace XMLClosedReporting
{
    /// <summary>
    /// A Helper class to track the Current CursorPosition in the Report while writing it
    /// </summary>
    public class ReportCursor
    {
        private readonly IXLWorksheet ws;

        public int StartingColumn { get; }
        public int StartingRow { get; }

        public int CurrentColumn { get => CurrentCell.WorksheetColumn().ColumnNumber(); }
        public int CurrentRow { get => CurrentCell.WorksheetRow().RowNumber(); }
        
        public IXLCell SavedCell { get; set; }

        public IXLCell CurrentCell { get; set; }
        public IXLRange ReportTitleRange { get; set; }
        public IXLRange HeadersRange { get; set; }
        public IXLRange NotesRange { get; set; }
        public IXLRange SumsTableTitlesRange { get; set; }
        public IXLRange SumsTableValuesRange { get; set; }

        /// <summary>
        /// A dictionary containing the Column Names as Keys 
        /// The Ranges of VALUES of the Columns as Values
        /// </summary>

        private readonly Dictionary<string, IXLRange> columnValuesRanges = [];

        public ReportCursor(IXLWorksheet ws , int startingColumn = 1, int startingRow = 1)
        {
            this.ws = ws;
            this.StartingColumn = startingColumn;
            this.StartingRow = startingRow;
            CurrentCell = ws.Cell(StartingRow, StartingColumn);
            SavedCell = ws.Cell(1, 1);
            ReportTitleRange = ws.Range(1, 1, 1, 1);
            HeadersRange = ws.Range(1, 1, 1, 1);
            NotesRange = ws.Range(1, 1, 1, 1);
            SumsTableTitlesRange = ws.Range(1, 1, 1, 1);
            SumsTableValuesRange = ws.Range(1, 1, 1, 1);
        }

        public ReportCursor MoveRight()
        {
            CurrentCell = CurrentCell.CellRight();
            return this;
        }
        public ReportCursor MoveLeft()
        {
            CurrentCell = CurrentCell.CellLeft();
            return this;
        }
        public ReportCursor MoveUp()
        {
            CurrentCell = CurrentCell.CellAbove();
            return this;
        }
        public ReportCursor MoveDown() 
        {
            CurrentCell = CurrentCell.CellBelow();
            return this;
        }

        public ReportCursor MoveTo(int row,int column)
        {
            CurrentCell = ws.Cell(row, column);
            return this;
        }
        public ReportCursor MoveTo(IXLCell cell)
        {
            CurrentCell = cell;
            return this;
        }

        public ReportCursor MoveToRow(int row)
        {
            CurrentCell = ws.Cell(row, CurrentColumn);
            return this;
        }
        public ReportCursor MoveToColumn(int column)
        {
            CurrentCell = ws.Cell(CurrentRow, column);
            return this;
        }

        public ReportCursor ResetToStartingCell()
        {
            CurrentCell = ws.Cell(StartingRow, StartingColumn);
            return this;
        }

        public ReportCursor SaveCurrentCell()
        {
            SavedCell = ws.Cell(CurrentRow,CurrentColumn);
            return this;
        }
        public ReportCursor MoveToSavedCell()
        {
            CurrentCell = ws.Cell(SavedCell.Address);
            return this;
        }



        /// <summary>
        /// Returns the Range of the Specified Column Name
        /// </summary>
        /// <param name="columnName">The Name of the Column for which to get the Range</param>
        /// <returns>The Range of the Column</returns>
        /// <exception cref="Exception">When the name is Invalid</exception>
        public IXLRange GetColumnRange(string columnName)
        {
            columnValuesRanges.TryGetValue(columnName, out var range);
            if (range is not null)
            {
                return range;
            }
            else
            {
                throw new Exception($"Column : {columnName} is not a Valid Column");
            }
        }
        /// <summary>
        /// Set the Range of the Values for a specific Column
        /// </summary>
        /// <param name="columnName">The Name of the Column</param>
        /// <param name="range">The Range of the Column</param>
        public void SetColumnRange(string columnName, IXLRange range) => columnValuesRanges.Add(columnName, range);
        /// <summary>
        /// Gets the First and Last Address from the Cell Values (so to create a Range)
        /// </summary>
        /// <param name="firstColumnName"></param>
        /// <param name="lastColumnName"></param>
        /// <returns></returns>
        public IXLRange GetValueCellsRange()
        {
            var firstRow = columnValuesRanges.Values.Select(r => r.FirstRow().RowNumber()).Min();
            var lastRow = columnValuesRanges.Values.Select(r => r.LastRow().RowNumber()).Max();
            var firstColumn = columnValuesRanges.Values.Select(r => r.FirstColumn().ColumnNumber()).Min();
            var lastColumn = columnValuesRanges.Values.Select(r => r.LastColumn().ColumnNumber()).Max();

            return ws.Range(firstRow,firstColumn,lastRow,lastColumn);
        }

        /// <summary>
        /// Returns a Dictioanry with the Column names as keys and their Column Number as Value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,int> GetColumnNumbersOfColumns()
        {
            return columnValuesRanges.ToDictionary(kvp=> kvp.Key, kvp => kvp.Value.FirstColumn().ColumnNumber());
        }
    }

}
