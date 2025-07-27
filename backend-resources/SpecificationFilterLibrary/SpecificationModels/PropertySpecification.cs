using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.SpecificationModels
{
    /// <summary>
    /// A Specification Comparison of a property against a certain value
    /// </summary>
    /// <typeparam name="TItem">The Type that contains the Property of the Specification (ex. 'Person')</typeparam>
    /// <typeparam name="TPropertyType">The Type of the Property of the Specification (ex 'string' for Property 'Person.Name')</typeparam>
    public class PropertySpecification<TItem, TPropertyType> : Specification<TItem>
        where TItem : class
    {
        private readonly string propertyName; //The Name of the Property of the T object being compared
        private readonly TPropertyType equalityValue; //The Value to compare with the Property Name
        private readonly ComparisonOperator comparisonOperator;

        /// <summary>
        /// Creates a Property Comparison for the Object being checked 
        /// </summary>
        /// <param name="propertyNamePath">The Name Path of the Property to Compare against a Constant Value (ex. for PersonModel : ('OMIT THIS' Person.)Address.City.Name)</param>
        /// <param name="equalityValue">The Constant Value against which to compare the Property</param>
        /// <param name="comparisonOperator">The Comparison Operator (ex Eq , Gt ,Lt e.t.c.)</param>
        public PropertySpecification(string propertyNamePath, TPropertyType equalityValue, ComparisonOperator comparisonOperator)
        {
            this.propertyName = propertyNamePath;
            //If Equality Value is a Date then Make it Local Time
            if (equalityValue is DateTime dt)
            {
                this.equalityValue = (TPropertyType)(object) dt.Date;
            }
            else
            {
                this.equalityValue = equalityValue;
            }
            this.comparisonOperator = comparisonOperator;
        }

        public override Expression<Func<TItem, bool>> GetExpression()
        {
            //Create the Expression of the Parameter (The item whose property is tested for equality is the actual parameter)
            var parameter = Expression.Parameter(typeof(TItem), nameof(TItem));

            //First Create the property Expression as the Whole Parameter 
            Expression property = parameter;

            // If the property is nested it will split the name and create a new Expression by iterating from the prev. Expression
            // Otherwise it will take only the non nested Property (will run only once)
            // Fantastic Solution https://stackoverflow.com/questions/16208214/construct-lambdaexpression-for-nested-property-from-string
            foreach (var member in propertyName.Split('.'))
            {
                property = Expression.Property(property, member);
            }

            //Create the expression for the constant value against which the equality comparing is done
            var constant = Expression.Constant(equalityValue, typeof(TPropertyType));

            //Create one of the Comparisons based on the Passed operator
            Expression comparison = comparisonOperator switch
            {
                ComparisonOperator.NotSet => throw new InvalidOperationException("Operator Not Set"),
                ComparisonOperator.Equal => Expression.Equal(property, constant),
                ComparisonOperator.NotEqual => Expression.NotEqual(property, constant),
                ComparisonOperator.GreaterThan => Expression.GreaterThan(property, constant),
                ComparisonOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
                ComparisonOperator.LessThan => Expression.LessThan(property, constant),
                ComparisonOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),

                //Call the String Methods accordingly to Create the Expression
                ComparisonOperator.Contains => typeof(TPropertyType) == typeof(string)
                ? Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant)
                : throw new NotSupportedException($"Comparison '{nameof(ComparisonOperator.Contains)}' is not Supported for non {nameof(String)} values"),
                //Call the String Methods accordingly to Create the Expression
                ComparisonOperator.StartsWith => typeof(TPropertyType) == typeof(string)
                ? Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant)
                : throw new NotSupportedException($"Comparison '{nameof(ComparisonOperator.StartsWith)}' is not Supported for non {nameof(String)} values"),
                //Call the String Methods accordingly to Create the Expression
                ComparisonOperator.EndsWith => typeof(TPropertyType) == typeof(string)
                ? Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constant)
                : throw new NotSupportedException($"Comparison '{nameof(ComparisonOperator.StartsWith)}' is not Supported for non {nameof(String)} values"),

                _ => throw new InvalidOperationException($"Unrecognized Operator - {comparisonOperator}"),
            };

            //Create the Lambda Expression (Containing the Expressions left(parameter) - right(constant) as well as the Operator for equality (==))
            //This is the final expression that will be compiled into the Func that produces the boolean result we need for the Equality
            return Expression.Lambda<Func<TItem, bool>>(comparison, parameter);
        }
    }
}
