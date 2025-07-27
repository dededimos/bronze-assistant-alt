using SpecificationFilterLibrary.SpecificationModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.Helpers
{
    public static class SpecificationFilterHelpers
    {
        /// <summary>
        /// Parses an Operator Value from a Text String and Returns it along with the Remaining string value
        /// </summary>
        /// <param name="text">The Text to Parse</param>
        /// <returns>The Operator and the remaining text</returns>
        public static (ComparisonOperator oper, string remainingText) ParseFilterOperator(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            {
                return (ComparisonOperator.Equal, text ?? string.Empty);
            }

            if (text.Length > 1
                && (text.StartsWith('%') || text.StartsWith('*'))
                && (text.EndsWith('%') || text.EndsWith('*')))
            {
                return (ComparisonOperator.Contains, text[1..^1]);
            }

            if (text.Length > 1
                && (!text.StartsWith('%') && !text.StartsWith('*'))
                && (text.EndsWith('%') || text.EndsWith('*')))
            {
                return (ComparisonOperator.StartsWith, text[0..^1]);
            }

            // Take only the first two charachters
            var oper = text[..2];

            ComparisonOperator resultOper = ComparisonOperator.NotSet;
            string remainingText;
            switch (oper.First())
            {
                case '%':
                case '*':
                    resultOper = ComparisonOperator.EndsWith;
                    remainingText = text[1..];
                    break;
                case '>':
                    // If two charachters trim the filterValue accordingly
                    resultOper = oper.Skip(1).First() == '=' ? ComparisonOperator.GreaterThanOrEqual : ComparisonOperator.GreaterThan;
                    remainingText = oper.Skip(1).First() == '=' ? text[2..] : text[1..];
                    break;
                case '<':
                    // If two charachters trim the filterValue accordingly
                    resultOper = oper.Skip(1).First() == '=' ? ComparisonOperator.LessThanOrEqual : ComparisonOperator.LessThan;
                    remainingText = oper.Skip(1).First() == '=' ? text[2..] : text[1..];
                    break;
                case '=':
                    // If two charachters trim the filterValue accordingly
                    resultOper = ComparisonOperator.Equal;
                    remainingText = oper.Skip(1).First() == '=' ? text[2..] : text[1..];
                    break;
                case '!':
                    // If two charachters trim the filterValue accordingly
                    resultOper = ComparisonOperator.NotEqual;
                    remainingText = oper.Skip(1).First() == '=' ? text[2..] : text[1..];
                    break;
                default:
                    resultOper = ComparisonOperator.Equal;
                    remainingText = text;
                    break;
            }

            return (resultOper, remainingText);
        }

        /// <summary>
        /// Parses a struct from a string 
        /// </summary>
        /// <typeparam name="TValue">The type of struct</typeparam>
        /// <param name="value">The Value</param>
        /// <returns>The Parsed value or the default value if the conversion failed</returns>
        public static TValue ParseValue<TValue>(string value)
            where TValue : struct
        {
            TValue result = default;
            var converter = TypeDescriptor.GetConverter(typeof(TValue));
            if (converter != null) result = (TValue?)converter.ConvertFrom(value) ?? default;
            return result;
        }

        /// <summary>
        /// Parse a ValueType
        /// </summary>
        /// <param name="valueInText">The String Value to Parse</param>
        /// <param name="parseToType">The Value Type to Parse</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">When the Type is not a Value Type</exception>
        public static object? ParseValue(string valueInText, Type parseToType,bool datesToUTC = true)
        {
            if (!parseToType.IsValueType)
            {
                throw new NotSupportedException($"{parseToType.Name} is not Supported  by {nameof(ParseValue)} Method , Only Value Types are supported");
            }

            if (parseToType == typeof(DateTime) && DateTime.TryParse(valueInText, out DateTime dt))
            {
                return dt.ToUniversalTime();
            }

            var converter = TypeDescriptor.GetConverter(parseToType);
            if (converter != null) return converter.ConvertFromString(valueInText);
            return default;
        }

        /// <summary>
        /// Creates a Property Filter Specification
        /// </summary>
        /// <typeparam name="T">The Type which owns the Property</typeparam>
        /// <param name="property">The Property</param>
        /// <param name="propertyFilterStringValue">The Value with which to constraint/Filter the Property</param>
        /// <param name="filterOperator">The Operator of the Constraint (filter)</param>
        /// <param name="propertyOwnerName">The Name of the Owner Property if this is a nested Property , if this is nested more than once then the names of the owners with a dot '.' (MyProperty.MyNestedProperty1.MyNestedProperty2 e.t.c.)</param>
        /// <returns>The Specification</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Specification<T> CreatePropertySpecification<T>(PropertyInfo property, string propertyFilterStringValue, ComparisonOperator filterOperator, string? propertyOwnerName = "")
            where T : class
        {
            // Get the PropertyValue Constraint from the Passed in String (usually a user Input)
            var propertyValue = property.PropertyType == typeof(string) ? propertyFilterStringValue : ParseValue(propertyFilterStringValue, property.PropertyType);

            // Get the Type of the Property Specification that must be Created
            Type propSepcifivcationType = typeof(PropertySpecification<,>).MakeGenericType(new Type[] { typeof(T), property.PropertyType });

            // Get the Name of the Property
            string propName = property.Name;
            if (propertyOwnerName != "")
            {
                propName = propertyOwnerName + "." + propName;
            }

            // Create the PropertySpecification
            var instance = Activator.CreateInstance(propSepcifivcationType, propName, propertyValue, filterOperator);

            if (instance is null) throw new InvalidOperationException("Activator Created a Null Instance without an Exception...");

            // Return it
            return (Specification<T>)instance;
        }
    }
}
