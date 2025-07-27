using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class ReportCursorAdvanced
    {
        private readonly IXLWorksheet ws;
        public IXLWorksheet WS { get => ws; }

        public int CurrentColumn { get => CurrentCell.WorksheetColumn().ColumnNumber(); }
        public int CurrentRow { get => CurrentCell.WorksheetRow().RowNumber(); }

        public IXLCell CurrentCell { get; set; }
        public IXLCell SavedCell { get; set; }

        /// <summary>
        /// The Count of the Items that where Inserted to help with the Incremental Numbering
        /// </summary>
        public int InsertedItemsCount { get; set; }

        public ReportCursorAdvanced(IXLWorksheet ws)
        {
            this.ws = ws;
            CurrentCell = ws.Cell(1, 1);
            SavedCell = ws.Cell(100, 100);
        }

        public ReportCursorAdvanced MoveRight()
        {
            CurrentCell = CurrentCell.CellRight();
            return this;
        }
        public ReportCursorAdvanced MoveLeft()
        {
            CurrentCell = CurrentCell.CellLeft();
            return this;
        }
        public ReportCursorAdvanced MoveUp()
        {
            CurrentCell = CurrentCell.CellAbove();
            return this;
        }
        public ReportCursorAdvanced MoveDown()
        {
            CurrentCell = CurrentCell.CellBelow();
            return this;
        }

        public ReportCursorAdvanced MoveTo(int row, int column)
        {
            CurrentCell = ws.Cell(row, column);
            return this;
        }
        public ReportCursorAdvanced MoveTo(IXLCell cell)
        {
            CurrentCell = cell;
            return this;
        }
        public ReportCursorAdvanced MoveTo(AddressInfo address)
        {
            MoveTo(address.RowNumber, address.ColumnNumber);
            return this;
        }

        public ReportCursorAdvanced MoveToRow(int row)
        {
            CurrentCell = ws.Cell(row, CurrentColumn);
            return this;
        }
        public ReportCursorAdvanced MoveToColumn(int column)
        {
            CurrentCell = ws.Cell(CurrentRow, column);
            return this;
        }
        
        public ReportCursorAdvanced SaveCurrentCell()
        {
            SavedCell = ws.Cell(CurrentRow, CurrentColumn);
            return this;
        }
        public ReportCursorAdvanced MoveToSavedCell()
        {
            CurrentCell = ws.Cell(SavedCell.Address);
            return this;
        }
    }
}
