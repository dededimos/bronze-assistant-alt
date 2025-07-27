using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Utilities.PipelineFilter
{
    /// <summary>
    /// A filter to be used in a Pipeline Class for Processing Objects
    /// </summary>
    public interface IFilter<T>
    {
        /// <summary>
        /// Executes Filtering , on the Input object
        /// </summary>
        /// <param name="input">The Input which will be processed from the filter</param>
        /// <returns></returns>
        public IEnumerable<T> Execute(IEnumerable<T> input);
    }
}
