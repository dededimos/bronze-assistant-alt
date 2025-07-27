using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Linq.Expressions;

namespace XMLClosedReporting
{
    /// <summary>
    /// Represents a Generic Column of the Reported object : <typeparamref name="T"/> 
    /// </summary>
    /// <typeparam name="T">The Type of the reported Object</typeparam>
    public class ReportColumnDefinition<T>
    {
        /// <summary>
        /// The Name of the Column
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;
        /// <summary>
        /// The Options of the Column
        /// </summary>
        public ColumnOptions Options { get; set; } = new();
        public bool IsValueFromFormula { get; set; }
        public ColumnFormulaInfo FormulaInfo { get; set; } = new();

        private Expression<Func<T, object>> valueRetriverDefinition = t => default!; //returns default value that can be null or not depending on the type of the return value;

        /// <summary>
        /// The Expression that defines the function with which to get the Value of the Column from the <typeparamref name="T"/> 
        /// </summary>
        public Expression<Func<T, object>> ValueRetriverDefinition 
        { 
            get => valueRetriverDefinition;
            set
            {
                valueRetriverDefinition = value;
                
            }
        }

        private Func<T, object>? valueRetriever;
        /// <summary>
        /// The Function that Retrieves the Value for the Column from the designated object <typeparamref name="T"/>
        /// </summary>
        public Func<T, object> ValueRetriever { get => valueRetriever ?? CompileAndGetValueRetriever(); }

        /// <summary>
        /// Compiles the Expression if it has not already Compiled
        /// </summary>
        /// <returns></returns>
        private Func<T, object> CompileAndGetValueRetriever()
        {
            valueRetriever = ValueRetriverDefinition.Compile();
            return valueRetriever;
        }
    }

    public class ColumnFormulaInfo
    {
        /// <summary>
        /// The Steps to build the Formula
        /// </summary>
        public List<FormulaBuildingStep> BuildingSteps { get; set; } = new();
        /// <summary>
        /// The Formula (when it has been explicitly set)
        /// </summary>
        public string ExplicitlySetFormula { get; set; } = string.Empty;
        /// <summary>
        /// Weather the formula is not built but has been set Explicitly
        /// </summary>
        public bool IsFormulaExplicitlySet { get; set; } = false;

        /// <summary>
        /// Gets the Formula of the Column for each cell in the Column
        /// </summary>
        /// <param name="columnNumbers">A dictionary containing the Name of Each Column as Key , The Number of the Column as Value</param>
        /// <returns>The Formula</returns>
        public string GetFormula(Dictionary<string,int> columnNumbers)
        {
            string formula = string.Empty;

            //For each step in the Building steps , add the operation in the string of the Forumla , except if the Column Name on the Building step is not empty ,
            //then add also the reference RC'ColumnNumber'
            foreach (var step in BuildingSteps)
            {
                if (!string.IsNullOrWhiteSpace(step.ColumnNameOfValue))
                {
                    if(columnNumbers.TryGetValue(step.ColumnNameOfValue, out int columnNumber))
                        formula += $"RC{columnNumber}";
                }
                formula += step.OperationPart;
            }
            return formula;
        }
    }
    public class FormulaBuildingStep
    {
        public string ColumnNameOfValue { get; set; } = string.Empty;
        public string OperationPart { get; set; } = string.Empty;
    }

}
