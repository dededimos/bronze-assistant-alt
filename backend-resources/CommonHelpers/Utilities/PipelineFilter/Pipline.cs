using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Utilities.PipelineFilter
{
    /// <summary>
    /// A Pipline to Filter Lists of Objects
    /// </summary>
    /// <typeparam name="T">The Item Type to Filter</typeparam>
    public abstract class Pipline<T>
    {
        protected readonly List<IFilter<T>> filters = new();

        /// <summary>
        /// Registers a Filter into the Pipline
        /// </summary>
        /// <param name="filter">The Filter to Register in the Pipeline</param>
        /// <returns>The Pipline - so this method can be chained</returns>
        public Pipline<T> RegisterFilter(IFilter<T> filter)
        {
            filters.Add(filter);
            return this;
        }

        /// <summary>
        /// Process the Input , executing all the Filters
        /// </summary>
        /// <param name="input">The input to process</param>
        /// <returns>The input after Processing</returns>
        public abstract IEnumerable<T> Process(IEnumerable<T> input);

    }
}
