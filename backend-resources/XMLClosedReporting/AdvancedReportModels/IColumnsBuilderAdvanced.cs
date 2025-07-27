using System.Linq.Expressions;

namespace XMLClosedReporting.AdvancedReportModels
{
    /// <summary>
    /// An Object representing a Report Column Builder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnsBuilderAdvanced<T> : IColumnsBuilderAdvancedCreationState<T>, IColumnsBuilderAdvancedValueState<T>, IColumnsBuilderAdvancedOptionsFinishState<T> 
    {
        /// <summary>
        /// The Columns Built by the Builder in the current Session
        /// </summary>
        IEnumerable<ReportColumnDefinitionAdvanced<T>> BuiltColumns { get; }
        /// <summary>
        /// Resets the Columns Builder
        /// </summary>
        void ResetBuilder();
    }
    /// <summary>
    /// The State of the Creation of the Columns , Leads to next state of Value Assignment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnsBuilderAdvancedCreationState<T>
    {
        /// <summary>
        /// Creates a Column with the specified Name 
        /// <para>The Header of the Column is also the Name of the Column</para>
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedValueState<T> CreateColumn(string columnName);
        /// <summary>
        /// Creates a Column with the specified Name and Header
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedValueState<T> CreateColumn(string columnName, string columnHeader);
    }
    /// <summary>
    /// The State of the Options Assignment of the Column or the Finish of the Column Creation , Leads to further Column Creation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnsBuilderAdvancedOptionsFinishState<T>
    {
        /// <summary>
        /// Configures the Options of the Column
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedOptionsFinishState<T> ConfigureOptions(Action<ReportColumnOptionsAdvanced> optionsAction);
        /// <summary>
        /// Sets the Column Number by overriding the automatic ColumnNumber Assignment
        /// </summary>
        /// <param name="columnOrderNumber"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedOptionsFinishState<T> SetColumnOrderNumber(int columnOrderNumber);
        /// <summary>
        /// Builds the Column and adds it to the Columns of the Builder
        /// </summary>
        /// <returns></returns>
        IColumnsBuilderAdvancedCreationState<T> BuildColumn();
    }
    /// <summary>
    /// The State of the Value Assignment of the Column , Leads to the next state of Options Assignment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnsBuilderAdvancedValueState<T>
    {
        /// <summary>
        /// Assigns a value to the Column Cells with a <see cref="Func{T, TResult}"/>
        /// </summary>
        /// <param name="valueFunction"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsValue(Func<T, object> valueFunction);
        /// <summary>
        /// Assigns a Formula to the Column Cells , explicitly set
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsFormula(string formula);
        /// <summary>
        /// Assigns a Formula to the Column Cells , by building it
        /// </summary>
        /// <param name="formulaCreator"></param>
        /// <returns></returns>
        IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsFormula(Action<IFormulaCreator<T>> formulaCreator);
    }

    public class ColumnsBuilderAdvanced<T> : IColumnsBuilderAdvanced<T>
    {
        public ColumnsBuilderAdvanced(ReportColumnOptionsAdvanced reportColumnOptions)
        {
            this.reportColumnOptions = reportColumnOptions;
        }

        private ReportColumnOptionsAdvanced reportColumnOptions;
        /// <summary>
        /// Changes the Column Options of the Builder , so that the options for configurement are based on the new options
        /// </summary>
        /// <param name="reportColumnOptions"></param>
        public void SetNewReportColumnOptions(ReportColumnOptionsAdvanced reportColumnOptions)
        {
            this.reportColumnOptions = reportColumnOptions;
        }

        private readonly List<ReportColumnDefinitionAdvanced<T>> columnDefinitions = [];
        /// <summary>
        /// The Columns Built by the Builder in the current Session
        /// </summary>
        public IEnumerable<ReportColumnDefinitionAdvanced<T>> BuiltColumns { get => columnDefinitions; }

        private ReportColumnDefinitionAdvanced<T> columnUnderConstruction = new();

        public IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsFormula(string formula)
        {
            columnUnderConstruction.Formula = new(formula);
            return this;
        }
        public IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsFormula(Action<IFormulaCreator<T>> formulaCreator)
        {
            // Create a concrete FormulaCreator<T> that wraps the FormulaInfo.
            // (Ensure that FormulaCreator<T> implements IFormulaCreator<T> and updates formulaInfo accordingly.)
            var creator = new FormulaCreator<T>();

            // Execute the action to allow the caller to build the formula.
            formulaCreator.Invoke(creator);

            // Assign the built formula to the column under construction.
            columnUnderConstruction.Formula = creator.Formula;

            return this;
        }
        public IColumnsBuilderAdvancedOptionsFinishState<T> AssignCellsValue(Func<T, object> valueFunction)
        {
            columnUnderConstruction.ValueRetriever = valueFunction;
            return this;
        }
        public IColumnsBuilderAdvancedCreationState<T> BuildColumn()
        {
            columnDefinitions.Add(columnUnderConstruction);
            columnUnderConstruction = new();
            return this;
        }
        public IColumnsBuilderAdvancedOptionsFinishState<T> ConfigureOptions(Action<ReportColumnOptionsAdvanced> optionsAction)
        {
            //For the Columns Builder only there is not harm configuring options at the end or at the beggining , there are not further builder consumers 
            //of options that should be passed before creating the columns

            //Create a copy of the default Options to allow the caller to configure them.
            var columnOptions = reportColumnOptions.GetDeepClone();
            //Invoke the action to allow the caller to configure the options.
            optionsAction.Invoke(columnOptions);
            //Assign the configured options to the column under construction.
            columnUnderConstruction.Options = columnOptions;
            return this;
        }
        public IColumnsBuilderAdvancedValueState<T> CreateColumn(string columnName)
        {
            columnUnderConstruction = new(columnName);
            return this;
        }
        public IColumnsBuilderAdvancedValueState<T> CreateColumn(string columnName, string columnHeader)
        {
            columnUnderConstruction = new(columnName, columnHeader);
            return this;
        }
        public IColumnsBuilderAdvancedOptionsFinishState<T> SetColumnOrderNumber(int columnOrderNumber)
        {
            columnUnderConstruction.ColumnOrderNumber = columnOrderNumber;
            return this;
        }

        public void ResetBuilder()
        {
            columnDefinitions.Clear();
        }
    }
}