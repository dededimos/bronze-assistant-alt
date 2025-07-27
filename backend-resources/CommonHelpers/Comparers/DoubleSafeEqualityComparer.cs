using CommonHelpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Comparers
{
    /// <summary>
    /// A Comparer for doubles to safely compare them for smaller values
    /// </summary>
    /// <param name="epsilon">The tollerance of the comparer. If the values diff is this much they will appear equal</param>
    public class DoubleSafeEqualityComparer(double epsilon = DoubleSafeEqualityComparer.DefaultEpsilon) : IEqualityComparer<double>
    {
        public const double DefaultEpsilon = 1e-10;
        private readonly double epsilon = epsilon;

        public bool Equals(double x, double y)
        {
            if (double.IsNaN(x) && double.IsNaN(y)) return true;

            if (double.IsInfinity(x) && double.IsInfinity(y)) return x == y;
            return Math.Abs(x - y) < epsilon;
        }

        public int GetHashCode([DisallowNull] double obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
