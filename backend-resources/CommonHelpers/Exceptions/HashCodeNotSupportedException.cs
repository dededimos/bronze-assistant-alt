using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Exceptions
{
    public class HashCodeNotSupportedException : Exception
    {
        public HashCodeNotSupportedException(object hashCodeImplementor) : base($"{hashCodeImplementor.GetType().Name} does not Support a Get Hash Code Implementation")
        {
        }
    }
}
