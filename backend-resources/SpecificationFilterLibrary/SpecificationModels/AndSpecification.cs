using SpecificationFilterLibrary.VisitorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.SpecificationModels
{
    /// <summary>
    /// Combines two Speccs with the AND Operator (Both must be Satisfied)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.GetExpression();
            Expression<Func<T, bool>> rightExpression = _right.GetExpression();

            //The Parameter Used in each of the Right or LEFT EXPRESSION IS NOT THE SAME OBJECT REFERENCE
            //So we Have to manually change the Parameter to that of the left Expression for both (right and Left Expressions)
            //To Do that we either invoke (meaning run) the right expression with the parameter of the Left as below example:

            //BinaryExpression andExpression = Expression.AndAlso(leftExpression.Body, Expression.Invoke(rightExpression, leftExpression.Parameters.Single()));
            //return Expression.Lambda<Func<T, bool>>(andExpression, leftExpression.Parameters.Single());

            //Or we use a SwapParameter Visitor , which will change the Parameter of the right expression with that of the Left , without invoking it
            //This way any Code reading this will not need to run any methods . Just the usual Linq expressions.
            BinaryExpression andExp = Expression.AndAlso(
                leftExpression.Body, //The Left Expression
                new SwapParameterVisitor(rightExpression.Parameters[0], leftExpression.Parameters[0]).Visit(rightExpression.Body)); //The Right Expression with its parameter changed with that of the leftExpression
            return Expression.Lambda<Func<T, bool>>(andExp, leftExpression.Parameters[0]); //Create the Lamdba with the Parameter of the left expression
        }
    }
}
