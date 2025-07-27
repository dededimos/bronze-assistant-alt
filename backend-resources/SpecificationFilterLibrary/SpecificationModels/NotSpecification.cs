using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.SpecificationModels
{
    /// <summary>
    /// Reverses a Speccification , its satisfied when its not
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;

        public NotSpecification(Specification<T> left)
        {
            _left = left;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.GetExpression();

            var notExpression = Expression.Not(leftExpression.Body);
            return Expression.Lambda<Func<T, bool>>(notExpression, leftExpression.Parameters.Single());
        }
    }
}
