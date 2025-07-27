using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers
{
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Returns a lamda Expression of a Property ( p => p.SomeProperty )
        /// </summary>
        /// <typeparam name="T">The Type of the Object containing the Property</typeparam>
        /// <typeparam name="TValue">The Type of the Property</typeparam>
        /// <param name="propertyName">The Name of the Property</param>
        /// <returns>The Expression (ex. p=> p.SomeProperty )</returns>
        /// <exception cref="ArgumentException">When the Property is Null or the Type of the Property is not correct</exception>
        public static Expression<Func<T,TValue>> CreatePropertyExpression<T, TValue>(string propertyName)
        {
            var propInfo = typeof(T).GetProperty(propertyName) ?? throw new ArgumentException($"Provided name is not a Valid property Name for the object of type {typeof(T).Name}", nameof(propertyName));

            if (propInfo.PropertyType != typeof(TValue))
            {
                throw new ArgumentException($"Property '{propertyName}' is not of type '{typeof(TValue)}'.");
            }

            //Create the parameter expression , (p=> ...)
            ParameterExpression paramExp = Expression.Parameter(typeof(T), "p");
            //Create the Member Expression ( p => p.SomeProperty)
            MemberExpression propExp = Expression.Property(paramExp, propInfo);
            return Expression.Lambda<Func<T, TValue>>(propExp, paramExp);

        }

        /// <summary>
        /// Returns the lamda Expression of a Nested(or not) Property by its dot '.' notation name "SomeClass.SomeProperty.SomeNestedProperty"
        /// </summary>
        /// <typeparam name="T">The Class fro mwhich the Property Expressionm is extracted</typeparam>
        /// <typeparam name="TValue">The Value Type of the nested Property</typeparam>
        /// <param name="propertyNamePath">The Path of the Property</param>
        /// <returns>the Lamda Expression of the Property</returns>
        public static Expression<Func<T,TValue>> CreateNestedPropertyExpression<T,TValue>(string propertyNamePath) 
        {
            var parameter = Expression.Parameter(typeof(T),"p");
            
            // Making the exprtession in steps , at the start we only have the parameter (p => ...)
            Expression propertyExp = parameter;

            //Foreach '.' recreate the expression by adding the previous one , thus creating the nested Expression
            foreach (var member in propertyNamePath.Split('.'))
            {
                propertyExp = Expression.Property(propertyExp, member);
            }
            return Expression.Lambda<Func<T, TValue>>(propertyExp, parameter);
        }
    }
}
