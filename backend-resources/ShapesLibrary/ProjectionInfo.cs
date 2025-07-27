using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary
{
    /// <summary>
    /// The Information about the Projection of a Shape onto a Line (<see cref="Vector"/>)
    /// </summary>
    public class ProjectionInfo(double min, double max)
    {
        /// <summary>
        /// The Min Value of the Dot Product from the Dot Products of each Projected Point of the Shape
        /// </summary>
        public double Min { get; } = min;
        /// <summary>
        /// The Max Value of the Dot Product from the Dot Products of each Projected Point of the Shape
        /// </summary>
        public double Max { get; } = max;

        /// <summary>
        /// Calculates the Overlap between two Projections
        /// How much (magnitude) the Two lines Projected from the shapes overlap on the SAME axis(line)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double GetOverlap(ProjectionInfo other)
        {
            return Math.Min(this.Max,other.Max) - Math.Max(this.Min, other.Min);
        }
    }
}
