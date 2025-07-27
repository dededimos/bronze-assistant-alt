using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.SpecificationModels
{
    /// <summary>
    /// An always satisfied Specification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SatisfiedSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> GetExpression()
        {
            return (x) => true;
        }
    }
}
