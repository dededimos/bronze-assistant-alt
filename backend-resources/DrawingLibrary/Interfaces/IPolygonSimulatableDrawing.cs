using ShapesLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Interfaces
{
    public interface IPolygonSimulatableDrawing
    {
        int MinSimulationSides { get; }

        /// <summary>
        /// Returns the Path Data for the Polygon Simulation
        /// </summary>
        /// <param name="sides">The Number of Sides of the Simulation Polygon</param>
        /// <returns></returns>
        string GetPolygonSimulationPathData(int sides);

        /// <summary>
        /// Returns the Combined Path Data for the Normal Shape and the Polygon Simulation
        /// </summary>
        /// <param name="sides"></param>
        /// <returns></returns>
        string GetNormalAndSimulatedPathData(int sides);
    }
}
