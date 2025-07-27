using XMLClosedReporting.StylesModels;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class ReportTableDefinition<T>
    {
        /// <summary>
        /// Creates a new Table Definition with no Name or Header
        /// </summary>
        public ReportTableDefinition()
        {

        }
        /// <summary>
        /// Creates a new Table Definition , with the specified Table Name , the Header of the Table is also the Name of the Table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="segregationKey">The Segregation Key of the Table , defining which items belong to the table</param>
        public ReportTableDefinition(string tableName) : this()
        {
            TableName = tableName;
            TableHeader = tableName;
        }
        /// <summary>
        /// Creates a new Table Definition , with the specified Table Name and Header
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableHeader"></param>
        /// <param name="segregationKey">The Segregation Key of the Table , defining which items belong to the table</param>
        public ReportTableDefinition(string tableName, string tableHeader) : this()
        {
            TableName = tableName;
            TableHeader = tableHeader;
        }

        /// <summary>
        /// The Segregation Key of the Table , defining which items belong to the table
        /// <para>If the key is null and this is the first table with a null key , all items for which the segregation selector function returns null will go here</para>
        /// <para>The Same applies when the segregation Selector function is null</para>
        /// <para>The Segregation selector function resides resides in <see cref="AdvancedReportOptions{T}.SegregateItemsBy"/></para>
        /// </summary>
        public object? SegregationKey { get; set; }
        /// <summary>
        /// The Number that defines the Order of the Table (weather its the first table e.t.c.)
        /// </summary>
        public int TableOrderNo { get; set; } = -1;
        public string TableName { get; set; } = string.Empty;
        public string TableHeader { get; set; } = string.Empty;
        public RangeInfo TitleRange { get; set; } = RangeInfo.UnsetRange();
        public RangeInfo TableValuesRange
        {
            get
            {
                if (Columns.Count == 0) return RangeInfo.UnsetRange();
                return new(Columns[0].ValuesRange.FirstAddress, Columns[^1].ValuesRange.LastAddress);
            }
        }
        public RangeInfo TableRange 
        {
            get 
            {
                if (Columns.Count == 0) return RangeInfo.UnsetRange();
                return new(TitleRange.FirstAddress, Columns[^1].ColumnRange.LastAddress);
            }
        }
        public List<ReportColumnDefinitionAdvanced<T>> Columns { get; set; } = [];
        /// <summary>
        /// The Options of the Table , if not set the Report Builder will apply the default or table-wide options
        /// </summary>
        public ReportTableOptions<T>? Options { get; set; }

        /// <summary>
        /// Returns the Column Numbers of the Table in a Dictionary with the Column Names as Keys
        /// <para>The Columns will not have numbers if there is no headers Range Set for each one of them</para>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetColumnNumbersDictionary()
        {
            Dictionary<string, int> columnNumbers = new();
            foreach (var column in Columns)
            {
                columnNumbers.Add(column.ColumnName, column.ColumnNumber);
            }
            return columnNumbers;
        }
    }
}