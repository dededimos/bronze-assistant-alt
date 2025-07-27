namespace ReportingHelperLibrary;

/// <summary>
/// A Column of a Report
/// </summary>
public class ReportColumn
{
    /// <summary>
    /// The Values of the Column as String
    /// </summary>
    protected readonly List<string> columnStringValues = new();

    public Type? TypeOfOriginalValue { get; private set; }

    /// <summary>
    /// The name of the Column
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// A Function to Manipulate or Localize the String Values of the Column
    /// </summary>
    public Func<string, string>? ManipulateValueFunction { get; set; }
    public Func<string, string>? ManipulateColumnNameFunction { get; set; }

    /// <summary>
    /// Creates a Reports Column
    /// </summary>
    /// <param name="columnName">The Name of the Column , AKA Header</param>
    /// <param name="typeOfValue">The Original Type of the Value of the Column</param>
    /// <param name="localizeValueFunction">A Function that manipulates the Values in a certain way</param>
    public ReportColumn(string columnName, Func<string, string>? localizeValueFunction = null, Func<string, string>? manipulateColumnNameFunction = null)
    {
        ColumnName = columnName;
        ManipulateValueFunction = localizeValueFunction;
        ManipulateColumnNameFunction = manipulateColumnNameFunction;
    }

    /// <summary>
    /// Returns the Values After they Have been Manipulated by the Manipulation Function
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetAlteredValues()
    {
        if (ManipulateValueFunction is null) return columnStringValues;
        List<string> manipulatedValues = new();
        foreach (var stringValue in columnStringValues)
        {
            manipulatedValues.Add(ManipulateValueFunction.Invoke(stringValue));
        }
        return manipulatedValues;
    }

    /// <summary>
    /// Returns the Column Name After it has been Manipulated by the Manipulation Function
    /// </summary>
    /// <returns></returns>
    public string GetAlteredColumnName()
    {
        // find the Actual name of the Column without any preceding names from the '.' notation
        var actualName = ColumnName.Split('.').LastOrDefault() ?? ColumnName;

        if (ManipulateColumnNameFunction is null) return actualName;
        return ManipulateColumnNameFunction.Invoke(actualName);
    }

    /// <summary>
    /// Adds a Value to the Column Values (Unmanipulated)
    /// </summary>
    /// <param name="unmanipulatedValue"></param>
    public void AddColumnValue(string unmanipulatedValue)
    {
        columnStringValues.Add(unmanipulatedValue);
    }

    public void SetTypeOfOriginalValue(Type? type)
    {
        TypeOfOriginalValue = type;
    }
}
