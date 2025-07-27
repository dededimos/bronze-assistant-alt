using CommonHelpers.Comparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReportingHelperLibrary;

public class Report<T>
{
    private readonly IEnumerable<T> reportItems;
    private readonly ReportOptions options;

    /// <summary>
    /// This is a counter saving which Properties have had already their Values Read , to avoid recursive operations in objects that have nests of themselves 
    /// For Example GlassOrderRow has nested CabinOrderRow which in turn has again a GlassOrderRow Nested.
    /// Whenever a GetPropNames Recursion Starts this list should be cleared and filled during the iteration
    /// </summary>
    private readonly HashSet<PropertyInfo> currentProcessedProperties = new();

    public IEnumerable<ReportColumn> Columns { get; }
    public IEnumerable<string> AlteredColumnNames { get => Columns.Select(c => c.GetAlteredColumnName()); }

    public Report(IEnumerable<T> reportObjectsList, ReportOptions? options = null)
    {
        reportItems = reportObjectsList;

        // Create empty options if none provided
        this.options = options ?? new ReportOptions();

        //Get all The Property Names for the provided 'T'
        var propertyNames = GetReportPropertyNames();

        //Exclude any properties defined in the Excluded Properties
        if (this.options.ExcludedProperties.Any())
        {
            propertyNames = propertyNames.Where(name => this.options.ExcludedProperties.All(exclProp => name.Contains(exclProp) is false));
        }

        // Generate the Columns needed
        Columns = GetReportColumns(propertyNames);

        // Assign a global Name Manipulation Function to all of the Columns if there is one
        if (this.options.ColumnNamesTotalManipulationFunction is not null)
        {
            foreach (var column in Columns)
            {
                column.ManipulateColumnNameFunction = this.options.ColumnNamesTotalManipulationFunction;
            }
        }

        // Assign EnumManipulation Function if Present
        if (this.options.EnumsValueManipulationFunction is not null)
        {
            foreach (var column in Columns.Where(c => c.TypeOfOriginalValue is not null && c.TypeOfOriginalValue.IsEnum))
            {
                column.ManipulateValueFunction = this.options.EnumsValueManipulationFunction;
            }
        }

        // Assign Date Manipulation Function if Present
        if (this.options.DatesValueManipulationFunction is not null)
        {
            foreach (var column in Columns.Where(c => c.TypeOfOriginalValue is not null && typeof(DateTime).IsAssignableFrom(c.TypeOfOriginalValue)))
            {
                column.ManipulateValueFunction = this.options.DatesValueManipulationFunction;
            }
        }

        // Assign any Manipulation Functions to Specific Properties
        if (this.options.SpecificPropertiesManipulationValueFunctions.Any())
        {
            foreach (var prop in this.options.SpecificPropertiesManipulationValueFunctions.Keys)
            {
                // Find the Properties whoose name is equal and assign the manipulation function from the dictionary
                var columnsWithThisPropName = Columns.Where(c => (c.ColumnName.Split('.').LastOrDefault() ?? c.ColumnName) == prop);
                foreach (var column in columnsWithThisPropName)
                {
                    column.ManipulateValueFunction = this.options.SpecificPropertiesManipulationValueFunctions[prop];
                }
            }
        }

    }

    /// <summary>
    /// Returns all the Public Properties of the Object being Reported ,
    /// If Objects are Flattened Returns All Nested Properties Also
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">When there are no Properties that can be Reported</exception>
    private IEnumerable<string> GetReportPropertyNames()
    {
        List<string> propertyNames = new();
        // Find all 1st Level Props
        var allProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

        if (options.FlattenNestedObjectsProperties)
        {
            // Recursivly get all Nested Properties with the Dot Notation
            foreach (var property in allProps)
            {
                //Reset the Currently Processed Properties (used to avoid StackOverflow Exception)
                currentProcessedProperties.Clear();

                foreach (var name in GetNestedPropertyNames(property))
                {
                    propertyNames.Add(name);
                }
            }
        }
        else
        {
            propertyNames = allProps.Select(p => p.Name).ToList();
        }

        return propertyNames; ;
    }

    /// <summary>
    /// Gets the The Name of a Nested Property or The Names of all the Nested Properties if its a Class object with nested Properties
    /// </summary>
    /// <param name="property">The Property for which to get Nested Properties</param>
    /// <param name="prefix">The Prefix for the Dot notation - always empty on the 1st level property</param>
    /// <returns>The Enumerable of Names of the Properties . All nested Properties are returned with the '.' notation</returns>
    private IEnumerable<string> GetNestedPropertyNames(PropertyInfo property, string prefix = "")
    {
        if (currentProcessedProperties.Contains(property))
        {
            //Exit the iteration will return an empty enumerable i think?
            yield break;
        }
        var propertyType = property.PropertyType;
        currentProcessedProperties.Add(property);

        //Check if Class , but not string and not Enumerable ! this way any List properties will not be checked for nested properties
        if (propertyType.IsClass && propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType) is false)
        {
            var nestedProperties = propertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            foreach (var nestedProperty in nestedProperties)
            {
                // Add the '.' notation where prefix is not empty
                var nestedPrefix = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                foreach (var name in GetNestedPropertyNames(nestedProperty, nestedPrefix))
                {
                    yield return name;
                }
            }
        }
        else
        {
            yield return string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
        }
    }

    /// <summary>
    /// Creates the List of Values for each Property of the Reported Objects 
    /// Each Property is stored in a single Object for all the Items being reported
    /// </summary>
    private IEnumerable<ReportColumn> GetReportColumns(IEnumerable<string> propertiesNames)
    {
        // Generate all the Columns
        var columns = propertiesNames.Select(name => new ReportColumn(name)).ToList(); //to list otherwise the enumerable does not properly keep references below

        // Add values to each Column
        // Each Column Represents a Property that is Nested or Not Nested
        // Find this Property Value for each item in the List of Provided Report Items and convert its value into a string 
        foreach (var column in columns)
        {
            foreach (var item in reportItems)
            {
                if (item is null) throw new Exception("Object to Report was Null");

                var columnValueOfItem = Report<T>.GetValueOfPropertyWithDotNotation(column.ColumnName, item);
                column.SetTypeOfOriginalValue(columnValueOfItem?.GetType());

                // The String representation of the columnValue Of the Item
                string stringValueToAdd;
                //If the Value is of Type IEnyumerable<T> then get the String Value of each item and save it as a single String seperated by New Lines
                if (columnValueOfItem is null)
                {
                    stringValueToAdd = string.Empty;
                }
                else if (columnValueOfItem.GetType() != typeof(string) && columnValueOfItem is IEnumerable enumerable)
                {
                    StringBuilder builder = new();

                    // Dictionary Case
                    if (enumerable is IDictionary dictionary)
                    {
                        foreach (var dicItem in dictionary.Values)
                        {
                            builder.Append(dicItem.ToString()).Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        foreach (var enumeratedItem in enumerable)
                        {
                            builder.Append(enumeratedItem.ToString()).Append(Environment.NewLine);
                        }
                    }
                    //Set the String Values and Trim the Last New Line Charachter
                    stringValueToAdd = builder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                }
                else
                {
                    stringValueToAdd = columnValueOfItem?.ToString() ?? string.Empty;
                }

                // Add it to the List of unmanipulatedValues
                column.AddColumnValue(stringValueToAdd);
            }
        }
        //Return the Columns Sort based on the Priority List of the Options Sorting
        var stringComparer = new StringPriorityComparer(options.ColumnSortPriorityList);

        // Order by True Property Name , removing all dot notations preceding it
        return columns.OrderBy(c => c.ColumnName.Split('.').LastOrDefault() ?? c.ColumnName, stringComparer);
    }

    /// <summary>
    /// Returns the Value of a Nested Property , whatever the type of that nested Property is 
    /// Properties that are of IEnumerable<T> , List<T> e.t.c. will always be Last in nesting so the Whole enumerable will be returned as Value 
    /// </summary>
    /// <param name="propertyPath">The Path to the Property , seperating each one with a '.' ex. PrimaryModel.Constraints.MaximumLength</param>
    /// <param name="parentObjectContainingProperty">The Object for which we need the value of one of its nested Properties (ex. Synthesis)</param>
    /// <returns>The Value of the defined Property Path for the selected Object</returns>
    /// <exception cref="ArgumentException">When the Property Path does not match any property of the Type of the Provided object</exception>
    private static object? GetValueOfPropertyWithDotNotation(string propertyPath, object parentObjectContainingProperty)
    {
        // Split to all the Individual Properties (ex. PrimaryCabin.Parts.WallProfile => will get three props Main => Cabin then Parts then WallProfile)
        var propertiesNames = propertyPath.Split('.');
        // Set the Current object that is being evaluated (to start its always the Object which contains all the Above properties (for example a Synthesis))
        var currentObject = parentObjectContainingProperty;
        // The Type of the Passed Object
        var currentType = currentObject.GetType();

        foreach (var propertyName in propertiesNames)
        {
            // Get the Current Property (each one is retrieved in sequence from the Split names received at the begining of the Method)
            var property = currentType.GetProperty(propertyName)
                ?? throw new ArgumentException($"Property {propertyName} not found on type {currentType.Name}");

            // Get the Value of the current object and store it a new to current object for the next iteration
            currentObject = property.GetValue(currentObject, null);
            // Do the same for the Type
            currentType = property.PropertyType;
        }
        // Return the final stored Value , if there are no Nested Props on it , it will simply return the Value of the Property otherwise the Value of the Last nested Property
        return currentObject;
    }
}
