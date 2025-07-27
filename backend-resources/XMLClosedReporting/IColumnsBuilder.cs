using System.Linq.Expressions;
using XMLClosedReporting.AdvancedReportModels;

namespace XMLClosedReporting
{
    /// <summary>
    /// Can assign a Property to a Column
    /// </summary>
    public interface IColumnsBuilder<T>
    {
        /// <summary>
        /// Assigns a value to the Column Cells with an Expression from the reported Object <typeparamref name="T"/>
        /// <para>reportedObject => reportedObject.SomeProperty * somethingElse ... e.t.c </para>
        /// </summary>
        /// <param name="valueFunctionExpression">The Expression defining the Property</param>
        IColumnsBuilder<T> AssignColumnValue(Expression<Func<T, object>> valueFunctionExpression);
        /// <summary>
        /// Starts the creation of a Formula
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> CreateFormula();
        /// <summary>
        /// Sets the Formula Explicitly
        /// </summary>
        /// <param name="formula">The Formula to Set</param>
        /// <returns>The Builder</returns>
        IColumnsBuilder<T> SetFormula(string formula);
        /// <summary>
        /// Sets a name to the Column
        /// </summary>
        /// <param name="name">The Name of the Column</param>
        /// <returns></returns>
        IColumnsBuilder<T> SetColumnName(string name);
        IColumnsBuilder<T> ConfigureOptions(Action<ColumnOptions> optionsAction);
        /// <summary>
        /// Builds the Column
        /// </summary>
        void BuildColumn();
    }

    public interface IFormulaOperator<T>
    {
        /// <summary>
        /// Multiplies the Last Value with the Next that will be declared
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> Multiply();
        /// <summary>
        /// Adds the Last Value to the Next that will be declared
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> Add();
        /// <summary>
        /// Divides the Last Value from the Next that will be declared
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> Divide();
        /// <summary>
        /// Deducts the Last Value from the Next that will be declared
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> Deduct();
        /// <summary>
        /// Opens a Parenthesis for the next Operation
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> OpenParenthesis();
        /// <summary>
        /// Close the Parenthesis for the Next Operation
        /// </summary>
        /// <returns></returns>
        IFormulaOperator<T> CloseParenthesis();

        /// <summary>
        /// Ends the Formula Creation
        /// </summary>
        /// <returns></returns>
        IColumnsBuilder<T> EndFormula();
    }
    public interface IFormulaValueChooser<T>
    {
        /// <summary>
        /// Sets the Column Name of which to take the Value for the formula
        /// </summary>
        /// <param name="columnName">The Name of the Column , of which the value will be retrieved</param>
        /// <returns></returns>
        IFormulaOperator<T> ValueInColumn(string columnName);
        /// <summary>
        /// Sets directly a value to the formula
        /// </summary>
        /// <param name="value">The value that is being set</param>
        /// <returns></returns>
        IFormulaOperator<T> Value(decimal value);
        /// <summary>
        /// Opens a Parenthesis for the next Operation
        /// </summary>
        /// <returns></returns>
        IFormulaValueChooser<T> OpenParenthesis();
        /// <summary>
        /// Close the Parenthesis for the Next Operation
        /// </summary>
        /// <returns></returns>
        IFormulaOperator<T> CloseParenthesis();
    }

    public interface IColumnsCreator<T>
    {
        IColumnsBuilder<T> CreateColumn();
        public IEnumerable<ReportColumnDefinition<T>> BuiltColumns { get; }
    }

    public class ColumnsBuilder<T> : IColumnsBuilder<T>, IColumnsCreator<T>, IFormulaValueChooser<T>, IFormulaOperator<T>
    {
        private readonly List<ReportColumnDefinition<T>> columnDefinitions = new();
        public IEnumerable<ReportColumnDefinition<T>> BuiltColumns { get => columnDefinitions; }

        private ReportColumnDefinition<T> columnUnderConstruction = new();

        public IColumnsBuilder<T> CreateColumn()
        {
            columnUnderConstruction = new();
            return this;
        }
        public IColumnsBuilder<T> AssignColumnValue(Expression<Func<T, object>> valueFunctionExpression)
        {
            columnUnderConstruction.ValueRetriverDefinition = valueFunctionExpression;
            return this;
        }
        public IColumnsBuilder<T> SetColumnName(string name)
        {
            columnUnderConstruction.ColumnName = name;
            return this;
        }
        public IColumnsBuilder<T> ConfigureOptions(Action<ColumnOptions> optionsAction)
        {
            optionsAction.Invoke(columnUnderConstruction.Options);
            return this;
        }
        public void BuildColumn()
        {
            columnDefinitions.Add(columnUnderConstruction);
        }

        /// <summary>
        /// Starts the Creation of the Formula
        /// </summary>
        /// <returns></returns>
        public IFormulaValueChooser<T> CreateFormula()
        {
            columnUnderConstruction.IsValueFromFormula = true;
            return this;
        }

        /// <summary>
        /// Sets the Formula Explicitly
        /// </summary>
        /// <param name="formula">The Formula to Set</param>
        /// <returns>The Builder</returns>
        public IColumnsBuilder<T> SetFormula(string formula)
        {
            columnUnderConstruction.IsValueFromFormula = true;
            columnUnderConstruction.FormulaInfo.IsFormulaExplicitlySet = true;
            columnUnderConstruction.FormulaInfo.ExplicitlySetFormula = formula;
            return this;
        }
        /// <summary>
        /// Sets the Column Name of which to take the Value for the formula
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public IFormulaOperator<T> ValueInColumn(string columnName)
        {
            columnUnderConstruction.FormulaInfo.BuildingSteps.Add(new() { ColumnNameOfValue = columnName });
            return this;
        }
        /// <summary>
        /// Sets directly a value to the formula
        /// </summary>
        /// <param name="value">The value that is being set</param>
        /// <returns></returns>
        public IFormulaOperator<T> Value(decimal value)
        {
            columnUnderConstruction.FormulaInfo.BuildingSteps.Add(new() { OperationPart = value.ToString() });
            return this;
        }

        public IFormulaValueChooser<T> Multiply()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault() 
                ?? throw new Exception("Building Steps have not yet been configured for this formula... Cannot set Formula Operator");
            lastStep.OperationPart += "*";
            return this;
        }

        public IFormulaValueChooser<T> Add()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault()
                ?? throw new Exception("Building Steps have not yet been configured for this formula... Cannot set Formula Operator");
            lastStep.OperationPart += "+";
            return this;
        }

        public IFormulaValueChooser<T> Divide()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault()
                ?? throw new Exception("Building Steps have not yet been configured for this formula... Cannot set Formula Operator");
            lastStep.OperationPart += "/";
            return this;
        }

        public IFormulaValueChooser<T> Deduct()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault()
                ?? throw new Exception("Building Steps have not yet been configured for this formula... Cannot set Formula Operator");
            lastStep.OperationPart += "-";
            return this;
        }

        public IColumnsBuilder<T> EndFormula()
        {
            return this;
        }

        public IFormulaValueChooser<T> OpenParenthesis()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault();
            if (lastStep is null)
            {
                //Open a new building step if there are not any
                columnUnderConstruction.FormulaInfo.BuildingSteps.Add(new() { OperationPart = "(" });
            }
            else
            {
                lastStep.OperationPart += "(";
            }
            return this;
        }

        public IFormulaOperator<T> CloseParenthesis()
        {
            var lastStep = columnUnderConstruction.FormulaInfo.BuildingSteps.LastOrDefault()
                ?? throw new Exception("Building Steps have not yet been configured for this formula... Cannot set Formula Operator");
            lastStep.OperationPart += ")";
            return this;
        }
    }
}
