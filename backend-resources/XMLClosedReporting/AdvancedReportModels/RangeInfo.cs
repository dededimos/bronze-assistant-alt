using ClosedXML.Excel;

namespace XMLClosedReporting.AdvancedReportModels
{
    /// <summary>
    /// A Record showing the Range of Cells
    /// </summary>
    public record RangeInfo
    {
        /// <summary>
        /// Creates a new RangeInfo Record with the First and Last Addresses
        /// </summary>
        /// <param name="firstAddress"></param>
        /// <param name="lastAddress"></param>
        public RangeInfo(AddressInfo firstAddress, AddressInfo lastAddress)
        {
            FirstAddress = firstAddress;
            LastAddress = lastAddress;
        }
        /// <summary>
        /// Creates a new RangeInfo Record with the First and Last Addresses
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="firstColumn"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastColumn"></param>
        public RangeInfo(int firstRow, int firstColumn, int lastRow, int lastColumn)
            : this(new AddressInfo(firstRow, firstColumn), new AddressInfo(lastRow, lastColumn)) { }

        public RangeInfo(IXLRange range) :this(new AddressInfo(range.FirstCell()), new AddressInfo(range.LastCell())) { }
        public RangeInfo(IXLCell firstCell,IXLCell lastCell):this(new AddressInfo(firstCell), new AddressInfo(lastCell)) {}
        public RangeInfo(IXLCell firstCell,int lastRow,int lastColumn):this(new AddressInfo(firstCell), new AddressInfo(lastRow, lastColumn)) { }
        public RangeInfo(IXLCell firstCell, AddressInfo lastAddress) : this(new AddressInfo(firstCell), lastAddress) { }


        /// <summary>
        /// Creates a Range Record with Unset Addresses
        /// </summary>
        /// <returns></returns>
        public static RangeInfo UnsetRange() => new(AddressInfo.UnsetAddress(), AddressInfo.UnsetAddress());

        /// <summary>
        /// The First Address of the Range
        /// </summary>
        public AddressInfo FirstAddress { get; set; }
        /// <summary>
        /// The Last Address of the Range
        /// </summary>
        public AddressInfo LastAddress { get; set; }

        public override string ToString()
        {
            return $"{FirstAddress}:{LastAddress}";
        }

        /// <summary>
        /// Returns weather the Range is Valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (IsUnset()) return false;
            // Validate that the first address is top-left compared to the last address.
            if (IsFirstAddressRowGreaterThanLastAddressRow()) return false;
            if (IsFirstAddressColumnGreaterThanLastAddressColumn()) return false;

            return true;
        }
        /// <summary>
        /// Returns weather the Range is Unset
        /// </summary>
        /// <returns></returns>
        private bool IsUnset()
        {
            // Ensure none of the addresses are unset (-1)
            if (FirstAddress.IsUnset() || LastAddress.IsUnset()) return true;
            else return false;
        }
        /// <summary>
        /// Returns weather the First Address Row is Greater than the Last Address Row
        /// </summary>
        /// <returns></returns>
        private bool IsFirstAddressRowGreaterThanLastAddressRow()
        {
            return FirstAddress.RowNumber > LastAddress.RowNumber;
        }
        /// <summary>
        /// Returns weather the First Address Column is Greater than the Last Address Column
        /// </summary>
        /// <returns></returns>
        private bool IsFirstAddressColumnGreaterThanLastAddressColumn()
        {
            return FirstAddress.ColumnNumber > LastAddress.ColumnNumber;
        }

        /// <summary>
        /// Throws an exception if the Range is not valid
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ThrowIfNotValid()
        {
            if (IsUnset()) throw new Exception($"Unset Range '{this}' , One of the Rows and/or Columns has a negative Value");
            if (IsFirstAddressRowGreaterThanLastAddressRow()) throw new Exception($"Invalid Rows '{this}' , The First Address Row is bigger than the Last");
            if (IsFirstAddressColumnGreaterThanLastAddressColumn()) throw new Exception($"Invalid Columns '{this}' , The First Address Column is bigger than the Last");
        }

        public IXLRange GetXLRange(IXLWorksheet ws)
        {
            return ws.Range(FirstAddress.RowNumber, FirstAddress.ColumnNumber, LastAddress.RowNumber, LastAddress.ColumnNumber);
        }
    }

    /// <summary>
    /// A Record showing the Address of a Cell
    /// </summary>
    public record AddressInfo
    {
        /// <summary>
        /// Creates a new Address Record with the Row and Column Numbers
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        public AddressInfo(int rowNumber, int columnNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }
        public AddressInfo(IXLCell cell) : this(cell.Address.RowNumber, cell.Address.ColumnNumber) { }

        private AddressInfo()
        {

        }

        /// <summary>
        /// The Number of the Row
        /// </summary>
        public int RowNumber { get; set; } = 0;
        /// <summary>
        /// The Number of the Column
        /// </summary>
        public int ColumnNumber { get; set; } = 0;

        public IXLAddress GetXLAddress(IXLWorksheet ws)
        {
            return ws.Cell(RowNumber, ColumnNumber).Address;
        }

        public bool IsUnset()
        {
            return RowNumber <= 0 || ColumnNumber <= 0;
        }
        public override string ToString()
        {
            return $"R{RowNumber}C{ColumnNumber}";
        }
        /// <summary>
        /// Creates an Unset Address Record
        /// </summary>
        /// <returns></returns>
        public static AddressInfo UnsetAddress() => new();
    }

}