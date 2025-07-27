using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace XMLClosedReporting.AdvancedReportModels
{
    /// <summary>
    /// A Formula Creator Object that helps to build a Formula for a Column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFormulaCreator<T> : IFormulaValueState<T> , IFormulaOperatorState<T>{}

    /// <summary>
    /// The State of the Value Assignment on the Formula Creator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFormulaValueState<T>
    {
        /// <summary>
        /// Opens a Parenthesis
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> OpenParenthesis();
        /// <summary>
        /// Adds a specific Value to the Formula
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> Value(decimal value);
        /// <summary>
        /// Adds the Value of a Column in the Same Row to the Formula
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> ValueOfColumnInSameRow(string columnName);
        /// <summary>
        /// Adds the Value of a Column in the Same Row to the Formula
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> ValueOfColumnInSameRow(int columnNumber);
        /// <summary>
        /// Adds the Value of a Cell to the Formula
        /// </summary>
        /// <param name="cellAddress"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> ValueOfCell(AddressInfo cellAddress);
        /// <summary>
        /// Adds the Value of a Cell to the Formula
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> ValueOfCell(int row , int column);
        /// <summary>
        /// Adds the Value of a Relative Address to the Formula
        /// <para>Relative Row : -4  => means 4 Rows above current one</para>
        /// Relative Column : 2 => means 2 Columns to the right of the current one
        /// </summary>
        /// <param name="relativeRow"></param>
        /// <param name="relativeColumn"></param>
        /// <returns></returns>
        IFormulaOperatorState<T> ValueOfRelativeAddress(int relativeRow, int relativeColumn);
    }
    /// <summary>
    /// The State of the Operator Assignment on the Formula Creator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFormulaOperatorState<T>
    {
        /// <summary>
        /// Add the Multiply Operator to the Formula '*'
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> Multiply();
        /// <summary>
        /// Add the Divide Operator to the Formula '/'
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> Divide();
        /// <summary>
        /// Add the Add Operator to the Formula '+'
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> Add();
        /// <summary>
        /// Add the Deduct Operator to the Formula '-'
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> Deduct();
        /// <summary>
        /// Add the Power Operator to the Formula '^'
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> Power();
        /// <summary>
        /// Closes a Parenthesis
        /// </summary>
        /// <returns></returns>
        IFormulaValueState<T> CloseParenthesis();
    }

    public class FormulaCreator<T> : IFormulaCreator<T>
    {
        public FormulaInfo Formula { get; } = new();
        public IFormulaValueState<T> Add()
        {
            Formula.AddPart("+");
            return this;
        }
        public IFormulaValueState<T> CloseParenthesis()
        {
            Formula.AddPart(")");
            return this;
        }
        public IFormulaValueState<T> Deduct()
        {
            Formula.AddPart("-");
            return this;
        }
        public IFormulaValueState<T> Divide()
        {
            Formula.AddPart("/");
            return this;
        }
        public IFormulaValueState<T> Multiply()
        {
            Formula.AddPart("*");
            return this;
        }
        public IFormulaValueState<T> OpenParenthesis()
        {
            Formula.AddPart("(");
            return this;
        }
        public IFormulaValueState<T> Power()
        {
            Formula.AddPart("^");
            return this;
        }
        public IFormulaOperatorState<T> Value(decimal value)
        {
            Formula.AddPart(value.ToString());
            return this;
        }
        public IFormulaOperatorState<T> ValueOfCell(AddressInfo cellAddress)
        {
            Formula.AddPartFromCellAddress(cellAddress);
            return this;
        }
        public IFormulaOperatorState<T> ValueOfCell(int row, int column)
        {
            Formula.AddPartFromCellAddress(new AddressInfo(row, column));
            return this;
        }
        public IFormulaOperatorState<T> ValueOfColumnInSameRow(string columnName)
        {
            Formula.AddPartFromColumnName(columnName);
            return this;
        }
        public IFormulaOperatorState<T> ValueOfColumnInSameRow(int columnNumber)
        {
            Formula.AddPartFromColumnNumber(columnNumber);
            return this;
        }
        public IFormulaOperatorState<T> ValueOfRelativeAddress(int relativeRow, int relativeColumn)
        {
            Formula.AddPartFromRelativeCell(relativeRow, relativeColumn);
            return this;
        }
    }
}