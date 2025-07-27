using ClosedXML.Excel;
using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class ReportColumnDefinitionAdvanced<T> : IDeepClonable<ReportColumnDefinitionAdvanced<T>>
    {
        public ReportColumnDefinitionAdvanced(){}

        /// <summary>
        /// Creates a Column with the specified Name , the Header of the Column is also the Name of the Column
        /// </summary>
        /// <param name="columnName"></param>
        public ReportColumnDefinitionAdvanced(string columnName) : this()
        {
            ColumnName = columnName;
            ColumnHeader = columnName;
        }
        /// <summary>
        /// Creates a Column with the specified Name and Header
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="columnHeader"></param>
        public ReportColumnDefinitionAdvanced(string columnName, string columnHeader) : this()
        {
            ColumnName = columnName;
            ColumnHeader = columnHeader;
        }
        /// <summary>
        /// The Name of the Column , used to identify the Column
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;
        /// <summary>
        /// The Header of the Column , appearing in the Excel File
        /// </summary>
        public string ColumnHeader { get; set; } = string.Empty;
        /// <summary>
        /// The ORDER Number of the Column
        /// </summary>
        public int ColumnOrderNumber { get; set; } = -1; // -1 means that the Column Number has not been set
        /// <summary>
        /// The Column Number of the Column in the Excel File
        /// <para>If the Column has no headers Range then this will be '0' ...</para>
        /// </summary>
        public int ColumnNumber { get => HeaderRange.LastAddress.ColumnNumber; }
        /// <summary>
        /// The Range of the Values of the Column
        /// </summary>
        public RangeInfo ValuesRange { get; set; } = RangeInfo.UnsetRange();
        /// <summary>
        /// The Range of the Header of the Column
        /// </summary>
        public RangeInfo HeaderRange { get; set; } = RangeInfo.UnsetRange();
        /// <summary>
        /// The Range of the Column
        /// </summary>
        public RangeInfo ColumnRange { get => new(HeaderRange.FirstAddress, ValuesRange.LastAddress); }

        /// <summary>
        /// The Options of the Column
        /// <para>If Null the report builder will apply the default or Column-wide options</para>
        /// </summary>
        public ReportColumnOptionsAdvanced? Options { get; set; }

        /// <summary>
        /// The Formula of the Value Cells if set
        /// </summary>
        public FormulaInfo? Formula { get; set; }

        /// <summary>
        /// Indicates if the Column Values are set through a R1C1 Formula
        /// </summary>
        [MemberNotNullWhen(true, nameof(Formula))]
        public bool HasFormula => Formula is not null;

        /// <summary>
        /// The Function that Retrieves the Value for the Column from the designated object <typeparamref name="T"/>
        /// </summary>
        public Func<T, object> ValueRetriever { get; set; } = (t) => "ValueRetrieverNotSet";

        public ReportColumnDefinitionAdvanced<T> GetDeepClone()
        {
            var clone = (ReportColumnDefinitionAdvanced<T>)MemberwiseClone();
            clone.Formula = Formula?.GetDeepClone();
            clone.ValuesRange = new(new AddressInfo(ValuesRange.FirstAddress.RowNumber, ValuesRange.FirstAddress.ColumnNumber), new AddressInfo(ValuesRange.LastAddress.RowNumber, ValuesRange.LastAddress.ColumnNumber));
            clone.HeaderRange = new(new AddressInfo(HeaderRange.FirstAddress.RowNumber, HeaderRange.FirstAddress.ColumnNumber), new AddressInfo(HeaderRange.LastAddress.RowNumber, HeaderRange.LastAddress.ColumnNumber));
            clone.Options = Options?.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Returns the Actual Columns occupied by this ColumnDefinition
        /// <para>The Options of a column are not always its own (the column might take options from its predecessors if its own are null)</para>
        /// </summary>
        /// <param name="tableParent">The Table host of the Column</param>
        /// <param name="builderParent">The Builder of the Column</param>
        /// <returns></returns>
        public int GetOccupiedColumns(ReportTableDefinition<T> tableParent, AdvancedReportBuilder<T> builderParent)
        {
            return (this.Options?.NumberOfOccupiedColumns
                    ?? tableParent.Options?.ColumnsOptions.NumberOfOccupiedColumns
                    ?? builderParent.Options.TablesOptions.ColumnsOptions.NumberOfOccupiedColumns);
        }
    }
}