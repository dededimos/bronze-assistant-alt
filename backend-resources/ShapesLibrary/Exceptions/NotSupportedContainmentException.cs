using ShapesLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Exceptions
{
    /// <summary>
    /// Throws a "BoundaryType does not support containment for the containedItemType" exception
    /// </summary>
    /// <param name="boundary"></param>
    /// <param name="containedItem"></param>
    public class NotSupportedContainmentException(ShapeInfo boundary, object containedItem,string? extraMsg = null) 
    : Exception($"{boundary.GetType().Name} does not support containment for {containedItem.GetType().Name}{(extraMsg is not null ? Environment.NewLine + extraMsg : string.Empty)}") {}

    public class SimulationSidesOutOfRangeException(IPolygonSimulatable simulatable)
    : Exception($"{simulatable.GetType().Name} needs at least {simulatable.MinSimulationSides} points to simulate as a Polygon");
}
