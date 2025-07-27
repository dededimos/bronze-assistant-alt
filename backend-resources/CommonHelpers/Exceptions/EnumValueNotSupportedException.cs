using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Exceptions
{
    public class EnumValueNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueNotSupportedException"/> class
        /// with the specified unsupported enum value.
        /// </summary>
        /// <param name="enumValue">The unsupported enum value.</param>
        public EnumValueNotSupportedException(Enum enumValue) : base($"Value '{enumValue}' is not supported"){}
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueNotSupportedException"/> class
        /// with the specified unsupported enum value.
        /// </summary>
        /// <param name="enumValue">The unsupported enum value.</param>
        /// <param name="operation">The operation under which this exception is thrown</param>
        public EnumValueNotSupportedException(Enum enumValue,string? operation):base($"Value '{enumValue}' is not supported in the current operation {(operation ?? "UndefinedOperationName")}") {}
    }
}
