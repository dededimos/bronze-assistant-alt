using SpecificationFilterLibrary.Helpers;
using SpecificationFilterLibrary.SpecificationModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary
{
    /// <summary>
    /// A Class Creating Property Specifications for <typeparamref name="T"/> with which it can be filtered within a Collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPropertyFilter<T>
        where T : class
    {
        private readonly PropertyInfo[] filterableMainProperties;
        private readonly (string ownerProperty, PropertyInfo nestedProperty)[] filterableNestedProperties;

        /// <summary>
        /// All the Available properties that can be Filtered Including Nested Properties up to 2 levels Down
        /// All Properties are Public and not Excluded in the Constructor
        /// </summary>
        public (string propOwnerName, PropertyInfo property)[] FilterableProperties
        {
            get => filterableMainProperties
                .Select(p => (string.Empty, p))
                .Concat(filterableNestedProperties)
                .ToArray();
        }

        /// <summary>
        /// Creates a Filter for the Properties of <typeparamref name="T"/> 
        /// The Properties of the Filter are all the Public Non Excluded Properties
        /// </summary>
        /// <param name="excludedProperties">The Names of the Excluded Properties . Names of the Nested Properties should Follow the '.' naming without including the base object 
        /// (ex. (<typeparamref name="T"/>Not Incvuded).SomeProperty.SomeNestedProperty.Some2ndLevelNestedProperty)</param>
        public ObjectPropertyFilter(params string[] excludedProperties)
        {
            filterableMainProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                //Do not include the Excluded Properties
                .Where(property => !excludedProperties.Any(excludedPropName => excludedPropName == property.Name))
                .ToArray();

            //1st level Nesting
            filterableNestedProperties = filterableMainProperties
                .Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string))
                .SelectMany(propertyClass =>
                    propertyClass.PropertyType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                    //Exclude Props (ex. Car.CarName)
                    .Where(nestedp => !excludedProperties.Any(excludedPropName => excludedPropName == $"{propertyClass.Name}.{nestedp.Name}"))
                    .Select(nestedp => (propertyClass.Name, nestedp)))
                .ToArray();

            //2nd level Nesting (Adding all names with extra '.' syntax containing all owners names)
            filterableNestedProperties = filterableNestedProperties
                 .Concat(filterableNestedProperties
                    .Where(p => p.nestedProperty.PropertyType.IsClass && p.nestedProperty.PropertyType != typeof(string))
                    .SelectMany(propertyClass => propertyClass.nestedProperty.PropertyType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                        //Exclude Props (ex. Car.Door.DoorName)
                        .Where(nestedp2 => !excludedProperties.Any(excludedPropName => excludedPropName == $"{propertyClass.ownerProperty}.{propertyClass.nestedProperty.Name}.{nestedp2.Name}"))
                        .Select(nestedp2 => ($"{propertyClass.ownerProperty}.{propertyClass.nestedProperty.Name}", nestedp2))))
                 .ToArray();

            //After Classification has ended - Exclude all Objects/Classes from the Filterable Properties
            filterableMainProperties = filterableMainProperties.Where(p => p.PropertyType.IsClass is false || p.PropertyType == typeof(string)).ToArray();
            filterableNestedProperties = filterableNestedProperties.Where(p => p.nestedProperty.PropertyType.IsClass is false || p.nestedProperty.PropertyType == typeof(string)).ToArray();
        }

        /// <summary>
        /// Returns the Specification Filter for the Currently Selected Property and Property Value
        /// </summary>
        /// <returns></returns>
        public Specification<T> GetFilter(PropertyInfo? filteredProperty, string propertyConstraintTextValue, ComparisonOperator filterOperator)
        {
            if (filterOperator is ComparisonOperator.NotSet) throw new InvalidEnumArgumentException("Comparison Operator was not Set");
            if (string.IsNullOrWhiteSpace(propertyConstraintTextValue) || filteredProperty is null)
            {
                return new SatisfiedSpecification<T>();
            }

            //Check if The passed Property Argument is one of the Available ones for this specific Object
            var (propOwnerName, property) = FilterableProperties.FirstOrDefault(fp => fp.property == filteredProperty);
            if (property is null)
            {
                throw new InvalidEnumArgumentException($"The Selected Property for Filtering was not found in the {nameof(FilterableProperties)} List");
            }

            return SpecificationFilterHelpers.CreatePropertySpecification<T>(
                filteredProperty,
                propertyConstraintTextValue,
                filterOperator,
                propOwnerName);
        }
    }
}
