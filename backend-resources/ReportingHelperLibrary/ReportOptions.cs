namespace ReportingHelperLibrary;

public class ReportOptions
{
    /// <summary>
    /// Weather to Flatten the Properties of any Nested Objects into the Whole Object
    /// </summary>
    public bool FlattenNestedObjectsProperties { get; set; }

    /// <summary>
    /// Properties that should be Excluded from the Report 
    /// Any Nested Properties should be metnioned with their path using the '.' Notation (ex. PropertyOfReportObject.NestedProperty)
    /// </summary>
    public List<string> ExcludedProperties { get; set; } = new();

    /// <summary>
    /// The Manipulation Fucntion for ALL the Column Names of the Report
    /// </summary>
    public Func<string, string>? ColumnNamesTotalManipulationFunction { get; set; }
    /// <summary>
    /// The Manipulation Function for All the Enum Values of the Report
    /// </summary>
    public Func<string, string>? EnumsValueManipulationFunction { get; set; }
    /// <summary>
    /// The Manipulation Function for All the DateValues of the Report
    /// </summary>
    public Func<string, string>? DatesValueManipulationFunction { get; set; }

    /// <summary>
    /// A Dictionary containing Manipulation Functions only for Specific Properties by Name of the Property
    /// </summary>
    public Dictionary<string, Func<string, string>> SpecificPropertiesManipulationValueFunctions { get; set; } = new();

    /// <summary>
    /// The Names of the Columns that should appear first from the Rest
    /// </summary>
    public List<string> ColumnSortPriorityList { get; set; } = new();

}