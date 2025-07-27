using SpecificationFilterLibrary.SpecificationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary
{
    public abstract class Specification<T>
    {
        /// <summary>
        /// Returns the Expression Predicate of the Specification (a Func&lt;<see cref="T"/>,<see cref="bool"/>&gt;)
        /// </summary>
        /// <returns></returns>
        public abstract Expression<Func<T, bool>> GetExpression();

        private Func<T, bool>? specificationFunction;
        public Func<T, bool> SpecificationFunction { get => specificationFunction ??= GetExpression().Compile(); }

        public bool IsSatisfiedBy(T entity)
        {
            //Return the bool result of the Func
            return SpecificationFunction(entity);
        }

        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
    }
}
