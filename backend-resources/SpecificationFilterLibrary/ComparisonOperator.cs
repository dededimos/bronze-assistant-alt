using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary
{
    public enum ComparisonOperator
    {
        NotSet = 0,
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,

        //string Only
        StartsWith,
        Contains,
        EndsWith
    }
}
