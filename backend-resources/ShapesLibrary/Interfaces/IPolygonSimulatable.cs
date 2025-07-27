using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Interfaces
{
    public interface IPolygonSimulatable
    {
        /// <summary>
        /// The minimum Number of Sides for the Simulation
        /// </summary>
        public int MinSimulationSides { get; }
        /// <summary>
        /// The Optimal number of Sides of the Simulated Polygon
        /// </summary>
        public int OptimalSimulationSides { get; }
        /// <summary>
        /// Returns the Polygon that simulates this object
        /// </summary>
        /// <param name="sides">The Number of Sides of the Polygon</param>
        /// <returns></returns>
        PolygonInfo GetPolygonSimulation(int sides);
        /// <summary>
        /// Returns the Optimal Simulation for this PolygonSimulatable
        /// </summary>
        /// <returns></returns>
        PolygonInfo GetOptimalPolygonSimulation() => GetPolygonSimulation(OptimalSimulationSides);
    }
}
