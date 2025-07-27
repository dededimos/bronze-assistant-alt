using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.SpecificationModels
{
    /// <summary>
    /// A Speccification created from an Expression
    /// </summary>
    /// <typeparam name="T">The Type of the Object for the Specification</typeparam>
    public class ExpressionSpecification<T> : Specification<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return _expression;
        }
    }
}
