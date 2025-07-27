using CommonInterfacesBronze;

namespace XMLClosedReporting.AdvancedReportModels
{
    /// <summary>
    /// An Object representing a Formula
    /// </summary>
    public class FormulaInfo : IDeepClonable<FormulaInfo>
    {
        public string? ExplicitlySetFormula { get; set; }
        public bool IsFormulaExplicitlySet { get => ExplicitlySetFormula != null; }
        public List<FormulaAffix> FormulaParts { get; set; } = [];

        /// <summary>
        /// Gets the Formula for the Value Cells of the Column
        /// </summary>
        /// <param name="columnsNumbers">A dictionary containing the Names of the Columns as Keys and the Number of each column as Value</param>
        /// <returns></returns>
        public string GetFormula(Dictionary<string, int>? columnsNumbers = null)
        {
            if (IsFormulaExplicitlySet)
            {
                return ExplicitlySetFormula!;
            }
            else
                return string.Join("", FormulaParts.Select(part => part.GetAffix(columnsNumbers)).ToArray());
        }

        /// <summary>
        /// Creates an Empty Formula Object
        /// </summary>
        public FormulaInfo() { }
        /// <summary>
        /// Creates a Formula Object with an Explicitly Set Formula
        /// </summary>
        /// <param name="formula"></param>
        public FormulaInfo(string formula) { ExplicitlySetFormula = formula; }

        /// <summary>
        /// Adds a Part/Affix to the Formula
        /// </summary>
        /// <param name="part"></param>
        public void AddPart(FormulaAffix part) => FormulaParts.Add(part);
        /// <summary>
        /// Adds a Part/Affix to the Formula with a specific Explicit Value (ex. an Operator , a constant value e.t.c.)
        /// </summary>
        /// <param name="affixExplicitValue"></param>
        public void AddPart(string affixExplicitValue) => FormulaParts.Add(new() { AffixExplicitValue = affixExplicitValue });
        /// <summary>
        /// Adds a Part/Affix to the Formula with the value of a specific Column
        /// </summary>
        /// <param name="columnName"></param>
        public void AddPartFromColumnName(string columnName) => FormulaParts.Add(new() { ColumnNameOfAffixValue = columnName });
        /// <summary>
        /// Adds a Part/Affix to the Formula with the value of a specific Column Number
        /// </summary>
        /// <param name="columnNumber"></param>
        public void AddPartFromColumnNumber(int columnNumber) => FormulaParts.Add(new() { ColumnNumberOfAffixValue = columnNumber });
        /// <summary>
        /// Adds a Part/Affix to the Formula with the value of a specific Cell Address
        /// </summary>
        /// <param name="cellAddress"></param>
        public void AddPartFromCellAddress(AddressInfo cellAddress) => FormulaParts.Add(new() { CellAddressOfAffixValue = cellAddress });
        /// <summary>
        /// Adds a Part/Affix to the Formula with the value of a specific Cell Address
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        public void AddPartFromCellAddress(int rowNumber, int columnNumber) => FormulaParts.Add(new() { CellAddressOfAffixValue = new(rowNumber, columnNumber) });
        /// <summary>
        /// Adds a Part/Affix to the Formula with the value of a specific Relative Cell
        /// </summary>
        /// <param name="relativeRow">-1 upper row , +1 bottom row</param>
        /// <param name="relativeColumn">-1 left column , +1 right column</param>
        public void AddPartFromRelativeCell(int relativeRow, int relativeColumn) => FormulaParts.Add(new() { RelativeCellRow = relativeRow, RelativeCellColumn = relativeColumn });

        public FormulaInfo GetDeepClone()
        {
            var clone = (FormulaInfo)MemberwiseClone();
            clone.FormulaParts = FormulaParts.Select(part => part.GetDeepClone()).ToList();
            return clone;
        }
    }

    /// <summary>
    /// An Object representing a Formula Affix (part of the Formula)
    /// </summary>
    public class FormulaAffix : IDeepClonable<FormulaAffix>
    {
        /// <summary>
        /// The Value of the Affix , if explicitly set (a specific value, an operator , a parenthesis e.t.c.)
        /// </summary>
        public string AffixExplicitValue { get; set; } = string.Empty;
        /// <summary>
        /// The Column Name from which to retrieve a value for the Affix
        /// <para>If Null the Affix will take its value from the <see cref="ColumnNumberOfAffixValue"/> </para>
        /// </summary>
        public string? ColumnNameOfAffixValue { get; set; }
        /// <summary>
        /// The Number of the Column from which to retrieve a value for the Affix
        /// <para>If Column Number is '<=0' The Affix will take its value from the <see cref="CellAddressOfAffixValue"/> </para>
        /// </summary>
        public int ColumnNumberOfAffixValue { get; set; } = 0;
        /// <summary>
        /// The Address of the Cell from which to retrieve a value for the Affix
        /// <para>If Unset The Affix will take its value from the <see cref="AffixExplicitValue"/></para>
        /// </summary>
        public AddressInfo CellAddressOfAffixValue { get; set; } = AddressInfo.UnsetAddress();
        /// <summary>
        /// The Relative Row of the Cell from which to retrieve a value for the Affix
        /// <para>-1 means above , +1 means below</para>
        /// </summary>
        public int? RelativeCellRow { get; set; }
        /// <summary>
        /// The Relative Column of the Cell from which to retrieve a value for the Affix
        /// <para>-1 means left , +1 means right</para>
        /// </summary>
        public int? RelativeCellColumn { get; set; }

        /// <summary>
        /// Returns the Affix Value in the following order of priority:
        /// <para>1. If Column Name is set, use the column number from the columnsNumbers dictionary.</para>
        /// <para>2. If Column Number is greater than 0, use the column number directly.</para>
        /// <para>3. If Cell Address is set, use the cell address.</para>
        /// <para>4. If a Relative Row and Column are set , use the relative cell to retrieve the value</para>
        /// <para>5. Otherwise, use the Affix Explicit Value.</para>
        /// </summary>
        /// <param name="columnsNumbers">A Dictionary with keys the Column Names and Values the Number of each Column<para>Used to Find the number of a certain column</para></param>
        /// <returns></returns>
        public string GetAffix(Dictionary<string, int>? columnsNumbers = null)
        {
            //Check if the affix value should be taken by a column Name
            if (ColumnNameOfAffixValue != null)
            {
                if (columnsNumbers == null) throw new Exception("The Column Numbers Dictionary is not set , cannot find the Column Number");
                if (columnsNumbers.TryGetValue(ColumnNameOfAffixValue, out int columnNumber))
                    return $"RC{columnNumber}";
                else throw new Exception($"The Column Name '{ColumnNameOfAffixValue}' was not found in the Column Numbers Dictionary");
            }
            //Else Check if the affix value should be taken by a column Row
            else if (ColumnNumberOfAffixValue > 0)
            {
                return $"RC{ColumnNumberOfAffixValue}";
            }
            //Else check if the affix value should be taken by a Cell Address
            else if (!CellAddressOfAffixValue.IsUnset())
            {
                return $"R{CellAddressOfAffixValue.RowNumber}C{CellAddressOfAffixValue.ColumnNumber}";
            }
            else if(RelativeCellRow != null || RelativeCellColumn != null)
            {
                if (RelativeCellRow == null) throw new Exception($"{nameof(RelativeCellRow)} cannot be null when {nameof(RelativeCellColumn)} is not");
                if (RelativeCellColumn == null) throw new Exception($"{nameof(RelativeCellColumn)} cannot be null when {nameof(RelativeCellRow)} is not");
                return $"R[{RelativeCellRow}]C[{RelativeCellColumn}]";
            }
            //Else return the Explicit Value
            else return AffixExplicitValue;
        }

        public FormulaAffix GetDeepClone()
        {
            var clone = (FormulaAffix)MemberwiseClone();
            clone.CellAddressOfAffixValue = new(this.CellAddressOfAffixValue.RowNumber, this.CellAddressOfAffixValue.ColumnNumber);
            return clone;
        }
    }
}