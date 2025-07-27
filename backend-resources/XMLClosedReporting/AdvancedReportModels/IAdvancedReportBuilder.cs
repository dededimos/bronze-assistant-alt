namespace XMLClosedReporting.AdvancedReportModels
{
    public interface IAdvancedReportBuilder<T> : IAdvancedReportBuilderTablesState<T>, IAdvancedReportBuilderOptionsState<T>, IAdvancedReportBuilderFinishState<T> 
    {
        public IEnumerable<ReportTableDefinition<T>> BuiltTables { get; }
    }

    public interface IAdvancedReportBuilderOptionsState<T>
    {
        IAdvancedReportBuilderTablesState<T> ConfigureOptions(Action<AdvancedReportOptions<T>> optionsAction);
        IAdvancedReportBuilderTablesState<T> UsePreConfiguredOptions();
    }
    public interface IAdvancedReportBuilderTablesState<T>
    {
        IAdvancedReportBuilderFinishState<T> CreateTables(Action<ITablesBuilder<T>> tablesBuilder);
    }
    public interface IAdvancedReportBuilderFinishState<T>
    {
        void BuildReportTemplate();
    }

    public class AdvancedReportBuilder<T> : IAdvancedReportBuilder<T>
    {
        public AdvancedReportBuilder()
        {
            options = AdvancedReportOptions<T>.DefaultOptions();
            tablesBuilder = new TablesBuilder<T>(options.TablesOptions);
        }
        private readonly AdvancedReportOptions<T> options;
        public AdvancedReportOptions<T> Options { get => options; }

        private readonly TablesBuilder<T> tablesBuilder;

        private bool isBuildComplete = false;
        public bool IsBuildComplete { get => isBuildComplete; }

        public IEnumerable<ReportTableDefinition<T>> BuiltTables { get => tablesBuilder.BuiltTables.OrderBy(t=>t.TableOrderNo); }

        public IAdvancedReportBuilderTablesState<T> ConfigureOptions(Action<AdvancedReportOptions<T>> optionsAction)
        {
            optionsAction.Invoke(options);
            //Assign also the new Configured Table Options to the table Builder
            tablesBuilder.SetNewReportTableOptions(options.TablesOptions);
            return this;
        }
        public IAdvancedReportBuilderTablesState<T> UsePreConfiguredOptions()
        {
            //The Caller will have the preconfigured options set in the constructor
            return this;
        }
        public IAdvancedReportBuilderFinishState<T> CreateTables(Action<ITablesBuilder<T>> tablesBuilder)
        {
            tablesBuilder.Invoke(this.tablesBuilder);
            return this;
        }
        public void BuildReportTemplate()
        {
            isBuildComplete = true;
        }
    }
}
