namespace XMLClosedReporting.AdvancedReportModels
{
    /// <summary>
    /// An Object representing a Report Table Builder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITablesBuilder<T> : ITablesBuilderCreationState<T>, ITableBuilderSgregationKeyState<T>, ITableBuilderOptionsConfigurationState<T>, ITablesBuilderColumnBuildingState<T>, ITablesBuilderFinishState<T> { }

    /// <summary>
    /// The State of the Creation of the Table , Leads to the next state of Column Building
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITablesBuilderCreationState<T>
    {
        /// <summary>
        /// Creates columns Shared by all tables after this method is called
        /// <para>For example : If Called before all tables it will add the shared Columns to all tables</para>
        /// <para>For example2 : If Called after the first table creation it will add the shared columns to all tables except the 1st one</para>
        /// </summary>
        /// <param name="columnsBuilder">the shared Columns Builder</param>
        /// <returns></returns>
        ITablesBuilderCreationState<T> BuildSharedColumns(Action<IColumnsBuilderAdvancedCreationState<T>> columnsBuilder);
        /// <summary>
        /// Creates a Table with the specified Name
        /// <para>The Header of the Table is also the Name of the Table</para>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        ITableBuilderSgregationKeyState<T> CreateTable(string tableName);
        /// <summary>
        /// Creates a Table with the specified Name and Header
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableHeader"></param>
        /// <returns></returns>
        ITableBuilderSgregationKeyState<T> CreateTable(string tableName, string tableHeader);
    }

    public interface ITableBuilderSgregationKeyState<T>
    {
        /// <summary>
        /// Sets the Segregation Key of the Table
        /// </summary>
        /// <param name="segregationKey">
        /// The Segregation Key of the Table , defining which items belong to the table
        /// <para>If the key is null and this is the first table with a null key , all items for which the segregation selector function returns null will go here</para>
        /// <para>The Same applies when the segregation Selector function is null</para>
        /// <para>The Segregation selector function resides resides in <see cref="AdvancedReportOptions{T}.SegregateItemsBy"/></para>
        /// <para>Setting the segregation key to null , will prevent any more table creation</para>
        /// </param>
        /// <returns></returns>
        ITableBuilderOptionsConfigurationState<T> SetTableSegregationKey(object? segregationKey);
        /// <summary>
        /// This will be the Last table of the Report
        /// </summary>
        /// <returns></returns>
        ITableBuilderOptionsConfigurationState<T> SetTableAsLastTable();
    }

    /// <summary>
    /// The State of the Options Assignment of the table or the Finish of the Table Creation , Leads to further Table Creation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITableBuilderOptionsConfigurationState<T>
    {
        /// <summary>
        /// Configures the Options of the Table
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        ITablesBuilderColumnBuildingState<T> ConfigureOptions(Action<ReportTableOptions<T>> optionsAction);
        /// <summary>
        /// Uses the Pre Configured Options
        /// </summary>
        /// <returns></returns>
        ITablesBuilderColumnBuildingState<T> UsePreConfiguredOptions();
    }
    /// <summary>
    /// The State of the Creation of the Columns of the Table , Leads to the next state of Options Assignment or Finish of the Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITablesBuilderColumnBuildingState<T>
    {
        ITablesBuilderFinishState<T> BuildColumns(Action<IColumnsBuilderAdvancedCreationState<T>> columnsBuilder);
    }
    public interface ITablesBuilderFinishState<T>
    {
        ITablesBuilderFinishState<T> SetTableOrderNo(int tableOrderNo);
        /// <summary>
        /// Builds the Table
        /// </summary>
        /// <returns></returns>
        ITablesBuilderCreationState<T> BuildTable();
    }

    public class TablesBuilder<T> : ITablesBuilder<T>
    {
        public TablesBuilder(ReportTableOptions<T> reportTableOptions)
        {
            //Save the reports table - wide options , use them whenever caller wants to configure further the options of a table
            this.reportTableOptions = reportTableOptions;
            //Create a new Columns builder and pass the columnwide options inside , so that caller can seperately configure options of columns
            columnsBuilder = new ColumnsBuilderAdvanced<T>(reportTableOptions.ColumnsOptions);
            sharedColumnsBuilder = new ColumnsBuilderAdvanced<T>(reportTableOptions.ColumnsOptions);
        }
        private ReportTableOptions<T> reportTableOptions;
        /// <summary>
        /// Changes the Table Options of the Builder , so that the options for configurement are based on the new options
        /// </summary>
        /// <param name="reportColumnOptions"></param>
        public void SetNewReportTableOptions(ReportTableOptions<T> reportTableOptions)
        {
            //change the options for the table builder
            this.reportTableOptions = reportTableOptions;
            //change the options for the columns builders also
            columnsBuilder.SetNewReportColumnOptions(reportTableOptions.ColumnsOptions);
            sharedColumnsBuilder.SetNewReportColumnOptions(reportTableOptions.ColumnsOptions);
        }
        private readonly ColumnsBuilderAdvanced<T> columnsBuilder;
        private readonly ColumnsBuilderAdvanced<T> sharedColumnsBuilder;

        private readonly List<ReportTableDefinition<T>> tableDefinitions = [];
        public IEnumerable<ReportTableDefinition<T>> BuiltTables { get => tableDefinitions; }

        private ReportTableDefinition<T> tableUnderConstruction = new();

        /// <summary>
        /// Weather the builder can build more tables
        /// <para>If there is at least one table with a null key the Builder cannot make more tables</para>
        /// </summary>
        public bool CanBuildMoreTables { get => BuiltTables.Any(t => t.SegregationKey == null) == false; }

        public ITablesBuilderCreationState<T> BuildSharedColumns(Action<IColumnsBuilderAdvancedCreationState<T>> columnsBuilder)
        {
            columnsBuilder.Invoke(this.sharedColumnsBuilder);
            return this;
        }
        public ITableBuilderSgregationKeyState<T> CreateTable(string tableName)
        {
            if (!CanBuildMoreTables) throw new Exception("Cannot build more tables , there is at least one table with a null Segregation key");
            tableUnderConstruction = new(tableName);
            return this;
        }
        public ITableBuilderSgregationKeyState<T> CreateTable(string tableName, string tableHeader)
        {
            if (!CanBuildMoreTables) throw new Exception("Cannot build more tables , there is at least one table with a null Segregation key");
            tableUnderConstruction = new(tableName,tableHeader);
            return this;
        }
        public ITableBuilderOptionsConfigurationState<T> SetTableSegregationKey(object? segregationKey)
        {
            //Check that no other table has an equal segregation key
            if (BuiltTables.Any(t => t.SegregationKey == segregationKey)) throw new Exception($"Cannot have two tables with the same Segregation Key :{segregationKey?.ToString() ?? "NULLKEY"}");

            tableUnderConstruction.SegregationKey = segregationKey;
            return this;
        }
        public ITableBuilderOptionsConfigurationState<T> SetTableAsLastTable()
        {
            //Check that no other table has a null segregation key else throw
            if (BuiltTables.Any(t => t.SegregationKey == null)) throw new Exception("Cannot have more than one table with a null Segregation Key");
            //Leaves the table under construction with a null segregation key , no more tables can be build
            return this;
        }
        public ITablesBuilderColumnBuildingState<T> ConfigureOptions(Action<ReportTableOptions<T>> optionsAction)
        {
            //Clone the reports table-wide options and pass them to the table under construction so that caller can configure the options of the table
            var tableOptions = reportTableOptions.GetDeepClone();
            //Invoke the action to configure the options
            optionsAction.Invoke(tableOptions);
            //Assign the configured options to the table under construction
            tableUnderConstruction.Options = tableOptions;
            //Assign the configured column options to the Columns Builder so that the caller configures further in the columns the same options and not the default ones passed in the constructor
            columnsBuilder.SetNewReportColumnOptions(tableOptions.ColumnsOptions);
            return this;
        }
        public ITablesBuilderColumnBuildingState<T> UsePreConfiguredOptions()
        {
            //Does nothing , the caller will use the options passed in the constructor from the report
            return this;
        }
        public ITablesBuilderFinishState<T> BuildColumns(Action<IColumnsBuilderAdvancedCreationState<T>> columnsBuilder)
        {
            columnsBuilder.Invoke(this.columnsBuilder);
            tableUnderConstruction.Columns.AddRange(this.columnsBuilder.BuiltColumns.Concat(sharedColumnsBuilder.BuiltColumns.Select(c=> c.GetDeepClone())).OrderBy(c=> c.ColumnOrderNumber));
            return this;
        }
        public ITablesBuilderFinishState<T> SetTableOrderNo(int tableOrderNo)
        {
            tableUnderConstruction.TableOrderNo = tableOrderNo;
            return this;
        }
        public ITablesBuilderCreationState<T> BuildTable()
        {
            tableDefinitions.Add(tableUnderConstruction);
            tableUnderConstruction = new();
            
            columnsBuilder.ResetBuilder();
            //Re Assign the columns builder options to the default ones passed in the constructor , so that any previous configuration does not affect the next table
            columnsBuilder.SetNewReportColumnOptions(reportTableOptions.ColumnsOptions);
            return this;
        }

    }
}